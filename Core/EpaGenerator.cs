using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Cci;
using Microsoft.Cci.MutableContracts;
using Microsoft.Cci.MutableCodeModel;

namespace Contractor.Core
{
	#region EPAGenerator EventArgs

	public abstract class TypeEventArgs : EventArgs
	{
		public string TypeFullName { get; private set; }

		public TypeEventArgs(string typeFullName)
		{
			this.TypeFullName = typeFullName;
		}
	}

	public class TypeAnalysisStartedEventArgs : TypeEventArgs
	{
		public TypeAnalysisStartedEventArgs(string typeFullName)
			: base(typeFullName)
		{
		}
	}

	public class TypeAnalysisDoneEventArgs : TypeEventArgs
	{
		public TypeAnalysisResult AnalysisResult { get; private set; }

		public TypeAnalysisDoneEventArgs(string typeFullName, TypeAnalysisResult analysisResult)
			: base(typeFullName)
		{
			this.AnalysisResult = analysisResult;
		}
	}

	public class StateAddedEventArgs : TypeEventArgs
	{
		public IState State { get; private set; }

		public StateAddedEventArgs(string typeFullName, IState state)
			: base(typeFullName)
		{
			this.State = state;
		}
	}

	public class TransitionAddedEventArgs : TypeEventArgs
	{
		public ITransition Transition { get; private set; }

		public TransitionAddedEventArgs(string typeFullName, ITransition transition)
			: base(typeFullName)
		{
			this.Transition = transition;
		}
	}

	#endregion

	public class EpaGenerator : IDisposable
	{
		private bool assemblyLoaded;
		private string assemblyFileName;
		private Dictionary<int, Epa> epas;
		private CodeContractAwareHostEnvironment host;
		private Module module;
		private PdbReader pdbReader;

		public event EventHandler<TypeAnalysisStartedEventArgs> TypeAnalysisStarted;
		public event EventHandler<TypeAnalysisDoneEventArgs> TypeAnalysisDone;
		public event EventHandler<StateAddedEventArgs> StateAdded;
		public event EventHandler<TransitionAddedEventArgs> TransitionAdded;

		public EpaGenerator(string inputFileName)
		{
			assemblyLoaded = false;
			assemblyFileName = inputFileName;
			epas = new Dictionary<int, Epa>();
		}

		public void Dispose()
		{
			this.UnloadAssembly();
			GC.SuppressFinalize(this);
		}

		public void LoadAssembly()
		{
			if (assemblyLoaded) return;
			host = new CodeContractAwareHostEnvironment(true);
			var staticModule = host.LoadUnitFrom(assemblyFileName) as IModule;

			if (staticModule == null || staticModule == Dummy.Module || staticModule == Dummy.Assembly)
				throw new Exception("The input is not a valid CLR module or assembly.");

			string pdbFileName = Path.ChangeExtension(assemblyFileName, "pdb");

			if (File.Exists(pdbFileName))
			{
				using (var pdbStream = File.OpenRead(pdbFileName))
					pdbReader = new PdbReader(pdbStream, host);
			}

			module = Microsoft.Cci.ILToCodeModel.Decompiler.GetCodeModelFromMetadataModel(host, staticModule, pdbReader);
			assemblyLoaded = true;
		}

		public void UnloadAssembly()
		{
			if (pdbReader != null)
				pdbReader.Dispose();

			if (host != null)
				host.Dispose();

			foreach (var epa in epas.Values)
				epa.Instrumented = false;

			assemblyLoaded = false;
		}

		public Dictionary<string, TypeAnalysisResult> GenerateEpas()
		{
			if (!assemblyLoaded)
				this.LoadAssembly();

			var analysisResults = new Dictionary<string, TypeAnalysisResult>();

			foreach (var staticType in module.AllTypes)
			{
				var type = staticType as NamespaceTypeDefinition;
				var typeNameUniqueKey = type.Name.UniqueKey;

				if (type.Name.Value == "<Module>")
					continue;

				if (type.ContainingUnitNamespace.ToString() == "System.Diagnostics.Contracts")
					continue;

				if (!type.IsClass && !type.IsStruct)
					continue;

				if (!epas.ContainsKey(typeNameUniqueKey))
					epas.Add(typeNameUniqueKey, new Epa());

				if (!epas[typeNameUniqueKey].GenerationCompleted)
					generateEpa(type);

				var epa = epas[typeNameUniqueKey];
				analysisResults.Add(type.ToString(), epa.AnalysisResult);
			}

			return analysisResults;
		}

		public TypeAnalysisResult GenerateEpa(string typeFullName)
		{
			if (!assemblyLoaded)
				this.LoadAssembly();

			//Borramos del nombre los parametros de generics
			int start = typeFullName.IndexOf('<');

			if (start != -1)
				typeFullName = typeFullName.Remove(start);

			var type = module.AllTypes.Find(t => t.ToString() == typeFullName) as NamespaceTypeDefinition;
			var typeNameUniqueKey = type.Name.UniqueKey;

			if (!epas.ContainsKey(typeNameUniqueKey))
				epas.Add(typeNameUniqueKey, new Epa());

			if (!epas[typeNameUniqueKey].GenerationCompleted)
				generateEpa(type);

			var epa = epas[typeNameUniqueKey];
			return epa.AnalysisResult;
		}

		private void generateEpa(NamespaceTypeDefinition type)
		{
			var typeFullName = type.ToString();
			var analysisStart = DateTime.Now;

			if (this.TypeAnalysisStarted != null)
				this.TypeAnalysisStarted(this, new TypeAnalysisStartedEventArgs(typeFullName));

			var constructors = from m in type.Methods
							   where !m.IsStaticConstructor && m.IsConstructor && m.Visibility == TypeMemberVisibility.Public
							   select m;

			var actions = (from m in type.Methods
						   where !m.IsStaticConstructor && !m.IsConstructor && m.Visibility == TypeMemberVisibility.Public
						   select m)
						   .ToList();

			//MyCodeMutator dd = new MyCodeMutator(host);
			//dd.Visit(type);

			var epa = epas[type.Name.UniqueKey];
			epa.Clear();

			IAnalyzer checker = new CodeContractsAnalyzer(host, module, pdbReader, type);
			var states = new Dictionary<string, State>();

			var dummy = new State();
			dummy.EnabledActions.AddRange(constructors);
			dummy.Sort();
			dummy.Name = "dummy";

			var newStates = new Queue<State>();
			newStates.Enqueue(dummy);

			while (newStates.Count > 0)
			{
				State source = newStates.Dequeue();
				bool isDummySource = (source == dummy);

				foreach (var action in source.EnabledActions)
				{
					ActionAnalysisResults actionsResult = checker.AnalyzeActions(source, action, actions);
					var inconsistentActions = actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).ToList();

					if (inconsistentActions.Any())
					{
						foreach (var act in inconsistentActions)
						{
							actionsResult.EnabledActions.Remove(act);
							actionsResult.DisabledActions.Remove(act);
						}
					}

					List<State> possibleTargets = generatePossibleStates(actions, actionsResult);
					TransitionAnalysisResult transitionsResults = checker.AnalyzeTransitions(source, action, possibleTargets);

					foreach (var transition in transitionsResults.Transitions)
					{
						var target = transition.TargetState;

						if (states.ContainsKey(target.Name))
						{
							target = states[target.Name];
						}
						else
						{
							target.Id = (uint)(states.Keys.Count + 1);
							target.IsInitial = isDummySource;
							states.Add(target.Name, target);
							newStates.Enqueue(target);
							epa.States.Add(target.Id, target.EpaState);

							epa.AnalysisResult.States.Add(target);

							if (this.StateAdded != null)
							{
								var eventArgs = new StateAddedEventArgs(typeFullName, target);
								this.StateAdded(this, eventArgs);
							}
						}

						var sourceId = isDummySource ? 0 : source.Id;

						if (!epa.ContainsKey(action.Name.UniqueKey))
							epa.Add(action.Name.UniqueKey, new EpaTransitions());

						var actionTransitions = epa[action.Name.UniqueKey];

						if (!actionTransitions.ContainsKey(sourceId))
							actionTransitions.Add(sourceId, new List<uint>());

						actionTransitions[sourceId].Add(target.Id);

						if (!isDummySource)
						{
							epa.AnalysisResult.Transitions.Add(transition);

							if (this.TransitionAdded != null)
							{
								var eventArgs = new TransitionAddedEventArgs(typeFullName, transition);
								this.TransitionAdded(this, eventArgs);
							}
						}
					}
				}
			}

			epa.GenerationCompleted = true;
			epa.AnalysisResult.TotalDuration = DateTime.Now - analysisStart;
			epa.AnalysisResult.TotalAnalyzerDuration = checker.TotalAnalysisDuration;
			epa.AnalysisResult.ExecutionsCount = checker.ExecutionsCount;
			epa.AnalysisResult.TotalGeneratedQueriesCount = checker.TotalGeneratedQueriesCount;
			epa.AnalysisResult.UnprovenQueriesCount = checker.UnprovenQueriesCount;

			if (this.TypeAnalysisDone != null)
			{
				var eventArgs = new TypeAnalysisDoneEventArgs(typeFullName, epa.AnalysisResult);
				this.TypeAnalysisDone(this, eventArgs);
			}
		}

		private List<State> generatePossibleStates(List<IMethodDefinition> actions, ActionAnalysisResults actionsResult)
		{
			var unknownActions = new HashSet<IMethodDefinition>(actions);

			unknownActions.ExceptWith(actionsResult.EnabledActions);
			unknownActions.ExceptWith(actionsResult.DisabledActions);

			var states = new List<State>();

			var v = new State();
			v.EnabledActions.AddRange(actionsResult.EnabledActions);
			v.DisabledActions.AddRange(actionsResult.DisabledActions);
			v.DisabledActions.AddRange(unknownActions);
			v.Sort();
			states.Add(v);

			while (unknownActions.Count > 0)
			{
				var m = unknownActions.First();
				unknownActions.Remove(m);

				int count = states.Count;

				for (int i = 0; i < count; ++i)
				{
					var w = new State();

					w.EnabledActions.Add(m);
					w.EnabledActions.AddRange(states[i].EnabledActions);
					w.DisabledActions.AddRange(states[i].DisabledActions);
					w.DisabledActions.Remove(m);
					w.Sort();
					
					states.Add(w);
				}
			}

			return states;
		}

		public void GenerateOutputAssembly(string outputFileName)
		{
			if (!assemblyLoaded)
				this.LoadAssembly();

			var contractProvider = ContractHelper.ExtractContracts(host, module, pdbReader, pdbReader);
			var instrumenter = new Instrumenter(host, contractProvider);

			foreach (var typeId in epas.Keys)
			{
				var epa = epas[typeId];

				if (!epa.Instrumented)
				{
					NamespaceTypeDefinition type = (from t in module.AllTypes
													where t.Name.UniqueKey == typeId
													select t as NamespaceTypeDefinition)
												   .First();

					instrumenter.InstrumentType(type, epa);
					epa.Instrumented = true;
				}
			}

			ContractHelper.InjectContractCalls(host, module, contractProvider, pdbReader);
			saveAssembly(outputFileName);
		}

		private void saveAssembly(string assemblyName)
		{
			foreach (var staticType in module.AllTypes)
			{
				var type = staticType as NamespaceTypeDefinition;
				var invariantMethod = type.Methods.Find(m => m.Name.Value == "$InvariantMethod$");
				type.Methods.Remove(invariantMethod);
			}

			var pdbName = Path.ChangeExtension(assemblyName, "pdb");

			using (Stream peStream = File.Create(assemblyName))
			{
				if (pdbReader == null)
				{
					PeWriter.WritePeToStream(module, host, peStream);
				}
				else
				{
					using (var pdbWriter = new PdbWriter(pdbName, pdbReader))
						PeWriter.WritePeToStream(module, host, peStream, pdbReader, pdbReader, pdbWriter);
				}
			}
		}
	}
}
