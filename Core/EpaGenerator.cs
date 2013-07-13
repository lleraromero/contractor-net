using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Cci;
using Microsoft.Cci.MutableContracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.ILToCodeModel;
using Contractor.Utils;

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
		private AssemblyInfo inputAssembly;
		private Dictionary<int, Epa> epas;
		private CodeContractAwareHostEnvironment host;

		public event EventHandler<TypeAnalysisStartedEventArgs> TypeAnalysisStarted;
		public event EventHandler<TypeAnalysisDoneEventArgs> TypeAnalysisDone;
		public event EventHandler<StateAddedEventArgs> StateAdded;
		public event EventHandler<TransitionAddedEventArgs> TransitionAdded;

		public EpaGenerator()
		{
			host = new CodeContractAwareHostEnvironment(true);
			inputAssembly = new AssemblyInfo(host);
			epas = new Dictionary<int, Epa>();
		}

		public void Dispose()
		{
			inputAssembly.Dispose();
		}

		public void LoadAssembly(string inputFileName)
		{
			inputAssembly.Load(inputFileName);
			inputAssembly.Decompile();
		}

		public void LoadContractReferenceAssembly(string inputFileName)
		{
			var contractsHost = new CodeContractAwareHostEnvironment(true);
			var contractsAssembly = new AssemblyInfo(contractsHost);
			contractsAssembly.Load(inputFileName);
			contractsAssembly.Decompile();
			AssemblyInfo.MergeContractsWithCode(inputAssembly, contractsAssembly);
			contractsAssembly.Unload();
		}

		public void UnloadAssembly()
		{
			inputAssembly.Unload();

			foreach (var epa in epas.Values)
				epa.Instrumented = false;
		}

		public Dictionary<string, TypeAnalysisResult> GenerateEpas()
		{
			var analysisResults = new Dictionary<string, TypeAnalysisResult>();

			foreach (var staticType in inputAssembly.DecompiledModule.AllTypes)
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
				{
					var methods = type.GetPublicInstanceMethods();
					generateEpa(type, methods);
				}

				var epa = epas[typeNameUniqueKey];
				analysisResults.Add(type.ToString(), epa.AnalysisResult);
			}

			return analysisResults;
		}

		public TypeAnalysisResult GenerateEpa(string typeFullName)
		{
			//Borramos del nombre los parametros de generics
			int start = typeFullName.IndexOf('<');

			if (start != -1)
				typeFullName = typeFullName.Remove(start);

			var type = inputAssembly.DecompiledModule.AllTypes.Find(t => t.ToString() == typeFullName) as NamespaceTypeDefinition;
			var typeNameUniqueKey = type.Name.UniqueKey;

			if (!epas.ContainsKey(typeNameUniqueKey))
				epas.Add(typeNameUniqueKey, new Epa());

			if (!epas[typeNameUniqueKey].GenerationCompleted)
			{
				var methods = type.GetPublicInstanceMethods();
				generateEpa(type, methods);
			}

			var epa = epas[typeNameUniqueKey];
			return epa.AnalysisResult;
		}

		public TypeAnalysisResult GenerateEpa(string typeFullName, IEnumerable<string> selectedMethods)
		{
			//Borramos del nombre los parametros de generics
			int start = typeFullName.IndexOf('<');

			if (start != -1)
				typeFullName = typeFullName.Remove(start);

			var type = inputAssembly.DecompiledModule.AllTypes.Find(t => t.ToString() == typeFullName) as NamespaceTypeDefinition;
			var typeNameUniqueKey = type.Name.UniqueKey;

			if (!epas.ContainsKey(typeNameUniqueKey))
				epas.Add(typeNameUniqueKey, new Epa());

			if (!epas[typeNameUniqueKey].GenerationCompleted)
			{
				var methods = from name in selectedMethods
							  join m in type.Methods
							  on name equals m.GetDisplayName()
							  select m;

				generateEpa(type, methods);
			}

			var epa = epas[typeNameUniqueKey];
			return epa.AnalysisResult;
		}

		private void generateEpa(NamespaceTypeDefinition type, IEnumerable<IMethodDefinition> methods)
		{
			var typeFullName = type.ToString();
			var analysisStart = DateTime.Now;

			if (this.TypeAnalysisStarted != null)
				this.TypeAnalysisStarted(this, new TypeAnalysisStartedEventArgs(typeFullName));

			var constructors = (from m in methods
								where m.IsConstructor
								select m)
								.ToList();

			var actions = (from m in methods
						   where !m.IsConstructor
						   select m)
						   .ToList();

			var epa = epas[type.Name.UniqueKey];
			epa.Clear();

			IAnalyzer checker = new CodeContractsAnalyzer(host, inputAssembly, type);
			var states = new Dictionary<string, State>();

			var dummy = new State();
			dummy.EnabledActions.AddRange(constructors);
			dummy.Sort();
			dummy.UniqueName = "dummy";

			var newStates = new Queue<State>();
			newStates.Enqueue(dummy);

			while (newStates.Count > 0)
			{
				var source = newStates.Dequeue();
				var isDummySource = (source == dummy);

				foreach (var action in source.EnabledActions)
				{
					var actionsResult = checker.AnalyzeActions(source, action, actions);
					var inconsistentActions = actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).ToList();

					foreach (var act in inconsistentActions)
					{
						actionsResult.EnabledActions.Remove(act);
						actionsResult.DisabledActions.Remove(act);
					}

					var possibleTargets = generatePossibleStates(actions, actionsResult);
					var transitionsResults = checker.AnalyzeTransitions(source, action, possibleTargets);

					foreach (var transition in transitionsResults.Transitions)
					{
						var target = transition.TargetState;

						if (states.ContainsKey(target.UniqueName))
						{
							target = states[target.UniqueName];
						}
						else
						{
							target.Id = (uint)(states.Keys.Count + 1);
							target.IsInitial = isDummySource;
							states.Add(target.UniqueName, target);
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

				var count = states.Count;

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
			var contractProvider = inputAssembly.ExtractContracts();
			var instrumenter = new Instrumenter(host, contractProvider);

			foreach (var typeId in epas.Keys)
			{
				var epa = epas[typeId];

				if (!epa.Instrumented)
				{
					var type = (from t in inputAssembly.DecompiledModule.AllTypes
								where t.Name.UniqueKey == typeId
								select t as NamespaceTypeDefinition)
								.First();

					instrumenter.InstrumentType(type, epa);
					epa.Instrumented = true;
				}
			}

			inputAssembly.InjectContracts(contractProvider);
			inputAssembly.Save(outputFileName);
		}
	}
}
