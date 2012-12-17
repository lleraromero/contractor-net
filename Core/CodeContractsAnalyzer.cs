using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Cci.Contracts;
using Contractor.Core.Properties;

namespace Contractor.Core
{
	class CodeContractsAnalyzer : IAnalyzer
	{
		private enum ResultKind
		{
			None,
			UnsatisfiableRequires,
			FalseRequires,
			UnprovenEnsures,
			FalseEnsures
		}

		private const string notPrefix = ".Not.";
		private const string pattern = @"^ Method \W* \d+ \W* : \W* (?<MethodName> [^(]+) \( [^)]* \) |" +
							   @"^ [^(]* \( [^)]* \) \W* (\[ [^]]* \])? \W* : \W* ([^:]+ :)? \W* (?<Message> [^\r]+)";

		private IContractAwareHost host;
		private Module module;
		private PdbReader pdbReader;
		private NamespaceTypeDefinition type;
		private ContractProvider contractProvider;
		private StringBuilder output;
		private Regex outputParser;

		private List<IMethodDefinition> queries;

		public CodeContractsAnalyzer(IContractAwareHost host, Module module, PdbReader pdbReader, NamespaceTypeDefinition type)
		{
			this.queries = new List<IMethodDefinition>();
			this.outputParser = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);
			this.host = host;
			this.pdbReader = pdbReader;
			this.module = module;
			this.type = type;
		}

		public TimeSpan TotalAnalysisDuration { get; private set; }
		public int ExecutionsCount { get; private set; }
		public int TotalGeneratedQueriesCount { get; private set; }
		public int UnprovenQueriesCount { get; private set; }

		public ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions)
		{
			contractProvider = ContractHelper.ExtractContracts(host, module, pdbReader, pdbReader);
			string queryAssemblyName = generateQueries(source, action, actions);
			ContractHelper.InjectContractCalls(host, module, contractProvider, pdbReader);
			saveAssembly(queryAssemblyName);
			var result = executeChecker(queryAssemblyName);
			var evalResult = evaluateQueries(actions, result);

			removeQueries();
			return evalResult;
		}

		private string generateQueries(State state, IMethodDefinition action, List<IMethodDefinition> actions)
		{
			foreach (var target in actions)
			{
				var positiveQuery = generatePositiveNegativeQuery(state, action, target, false);
				var negativeQuery = generatePositiveNegativeQuery(state, action, target, true);

				positiveQuery.ContainingTypeDefinition = type;
				negativeQuery.ContainingTypeDefinition = type;

				type.Methods.Add(positiveQuery);
				type.Methods.Add(negativeQuery);

				queries.Add(positiveQuery);
				queries.Add(negativeQuery);

				this.TotalGeneratedQueriesCount += 2;
			}

			string queryAssemblyName = string.Format("query{0}.dll", type.Name.Value);
			return Path.Combine(Configuration.TempPath, queryAssemblyName);
		}

		private MethodDefinition generatePositiveNegativeQuery(State state, IMethodDefinition action, IMethodDefinition target, bool negative)
		{
			var contracts = new MethodContract();
			ITypeContract mci = contractProvider.GetTypeContractFor(type);
			IMethodContract mct = contractProvider.GetMethodContractFor(target);

			if (mci != null && mci.Invariants.Count() > 0)
			{
				var pres = from inv in mci.Invariants
						   select new Precondition()
						   {
							   Condition = inv.Condition
						   };

				contracts.Preconditions.AddRange(pres);
			}

			foreach (var c in state.EnabledActions)
			{
				IMethodContract mc = contractProvider.GetMethodContractFor(c);
				if (mc == null) continue;

				contracts.Preconditions.AddRange(mc.Preconditions);
			}

			foreach (var c in state.DisabledActions)
			{
				IMethodContract mc = contractProvider.GetMethodContractFor(c);

				if (mc == null || mc.Preconditions.Count() == 0)
				{
					var pre = new Precondition()
					{
						Condition = new CompileTimeConstant()
						{
							Type = host.PlatformType.SystemBoolean,
							Value = false
						}
					};

					contracts.Preconditions.Add(pre);
				}
				else
				{
					var pres = from pre in mc.Preconditions
							   select new Precondition()
							   {
								   Condition = new LogicalNot()
								   {
									   Type = host.PlatformType.SystemBoolean,
									   Operand = pre.Condition
								   }
							   };

					contracts.Preconditions.AddRange(pres);
				}
			}

			if (mct == null || mct.Preconditions.Count() == 0)
			{
				if (negative)
				{
					var post = new Postcondition()
					{
						Condition = new CompileTimeConstant()
						{
							Type = host.PlatformType.SystemBoolean,
							Value = false
						}
					};

					contracts.Postconditions.Add(post);
				}
			}
			else
			{
				if (negative)
				{
					var exprs = (from pre in mct.Preconditions
								select pre.Condition).ToList();

					var post = new Postcondition()
					{
						Condition = new LogicalNot()
						{
							Type = host.PlatformType.SystemBoolean,
							Operand = Helper.JoinWithLogicalAnd(host, exprs, true)
						}
					};

					contracts.Postconditions.Add(post);
				}
				else
				{
					var posts = from pre in mct.Preconditions
								select new Postcondition()
								{
									Condition = pre.Condition
								};

					contracts.Postconditions.AddRange(posts);
				}
			}

			string prefix = negative ? notPrefix : string.Empty;
			var methodName = string.Format("{0}_{1}_{2}{3}", state.Name, action.Name, prefix, target.Name);
			MethodDefinition method = generateQuery(methodName, action);

			contractProvider.AssociateMethodWithContract(method, contracts);
			return method;
		}

		private MethodDefinition generateQuery(string name, IMethodDefinition action)
		{
			var method = new MethodDefinition()
			{
				InternFactory = host.InternFactory,
				IsCil = true,
				Name = host.NameTable.GetNameFor(name),
				Type = action.Type,
				Visibility = TypeMemberVisibility.Private,
				GenericParameters = action.GenericParameters.ToList()
			};

			BlockStatement block = null;

			if (Configuration.InlineMethodsBody)
			{
				block = inlineMethodBody(action, method);
			}
			else
			{
				block = callMethod(action);
			}

			method.Body = new SourceMethodBody(host, pdbReader)
			{
				MethodDefinition = method,
				Block = block,
			};

			return method;
		}

		private BlockStatement callMethod(IMethodDefinition action)
		{
			var block = new BlockStatement();
			var args = new List<IExpression>();

			foreach (var arg in action.Parameters)
			{
				var defaultValue = new DefaultValue()
				{
					DefaultValueType = arg.Type,
					Type = arg.Type
				};

				args.Add(defaultValue);
			}

			var callExpr = new MethodCall()
			{
				IsStaticCall = false,
				MethodToCall = action,
				Type = action.Type,
				ThisArgument = new ThisReference(),
				Arguments = args
			};

			if (action.Type.TypeCode == PrimitiveTypeCode.Void)
			{
				var call = new ExpressionStatement()
				{
					Expression = callExpr
				};

				block.Statements.Add(call);
				block.Statements.Add(new ReturnStatement());
			}
			else
			{
				var ret = new ReturnStatement()
				{
					Expression = callExpr
				};

				block.Statements.Add(ret);
			}

			return block;
		}

		private BlockStatement inlineMethodBody(IMethodDefinition action, MethodDefinition method)
		{
			var block = new BlockStatement();
			method.Parameters.AddRange(action.Parameters);
			IMethodContract mc = contractProvider.GetMethodContractFor(action);

			if (mc != null && mc.Preconditions.Count() > 0)
			{
				var asserts = from pre in mc.Preconditions
							  select new AssertStatement()
							  {
								  Condition = pre.Condition
							  };

				block.Statements.AddRange(asserts);
			}

			IBlockStatement actionBodyBlock = null;

			if (action.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
			{
				var actionBody = action.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
				actionBodyBlock = actionBody.Block;
			}
			else if (action.Body is SourceMethodBody)
			{
				var actionBody = action.Body as SourceMethodBody;
				actionBodyBlock = actionBody.Block;
			}

			//Por tratarse de un constructor skipeamos
			//el primer statement porque es base..ctor();
			var skipCount = (action.IsConstructor ? 1 : 0);
			block.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));

			if (mc != null && mc.Postconditions.Count() > 0)
			{
				var assumes = from post in mc.Postconditions
							  select new AssumeStatement()
							  {
								  Condition = post.Condition
							  };

				//Ponemos los assume antes del return
				block.Statements.InsertRange(block.Statements.Count - 1, assumes);
			}

			return block;
		}

		private ActionAnalysisResults evaluateQueries(List<IMethodDefinition> actions, Dictionary<string, List<ResultKind>> result)
		{
			var analysisResult = new ActionAnalysisResults();
			analysisResult.EnabledActions.AddRange(actions);
			analysisResult.DisabledActions.AddRange(actions);

			foreach (var entry in result)
			{
				if (entry.Value.Contains(ResultKind.FalseEnsures) ||
					entry.Value.Contains(ResultKind.FalseRequires) ||
					entry.Value.Contains(ResultKind.UnsatisfiableRequires) ||
					entry.Value.Contains(ResultKind.UnprovenEnsures))
				{
					var query = entry.Key;
					var actionName = query.Substring(query.LastIndexOf('_') + 1);
					var isNegative = actionName.StartsWith(notPrefix);

					if (isNegative)
					{
						actionName = actionName.Remove(0, notPrefix.Length);
						var method = type.Methods.Find(m => m.Name.Value == actionName);
						analysisResult.DisabledActions.Remove(method);
					}
					else
					{
						var method = type.Methods.Find(m => m.Name.Value == actionName);
						analysisResult.EnabledActions.Remove(method);
					}

					if (entry.Value.Contains(ResultKind.UnprovenEnsures))
						this.UnprovenQueriesCount++;
				}
			}

			return analysisResult;
		}

		private Dictionary<string, List<ResultKind>> executeChecker(string queryAssemblyName)
		{
			string libPaths;

			if (module.TargetRuntimeVersion.StartsWith("v4.0"))
				libPaths = Configuration.ExpandVariables(Resources.Netv40);
			else
				libPaths = Configuration.ExpandVariables(Resources.Netv35);

			var typeName = type.ToString();

			if (type.IsGeneric)
				typeName = string.Format("{0}`{1}", typeName, type.GenericParameterCount);

			output = new StringBuilder();
			string cccheckArgs = Configuration.CheckerArguments;
			cccheckArgs = cccheckArgs.Replace("@assemblyName", queryAssemblyName);
			cccheckArgs = cccheckArgs.Replace("@fullTypeName", typeName);
			cccheckArgs = cccheckArgs.Replace("@libPaths", libPaths);

			using (Process cccheck = new Process())
			{
				cccheck.StartInfo = new ProcessStartInfo()
				{
					FileName = Configuration.CheckerFileName,
					Arguments = cccheckArgs,
					WorkingDirectory = Directory.GetCurrentDirectory(),
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false
				};

				cccheck.OutputDataReceived += cccheck_DataReceived;
				cccheck.ErrorDataReceived += cccheck_DataReceived;
				cccheck.Start();
				cccheck.BeginErrorReadLine();
				cccheck.BeginOutputReadLine();
				cccheck.WaitForExit();

				var analysisDuration = cccheck.ExitTime - cccheck.StartTime;
				this.TotalAnalysisDuration += analysisDuration;
				this.ExecutionsCount++;

//#if DEBUG
//                System.Console.WriteLine("\tCode Contracts analysis duration: {0}", analysisDuration);
//#endif
			}

			MatchCollection matches = outputParser.Matches(output.ToString());
			output = null;

			Dictionary<string, List<ResultKind>> result = new Dictionary<string, List<ResultKind>>();
			string currentMethod = null;

			foreach (Match m in matches)
			{
				if (m.Groups["MethodName"].Success)
				{
					currentMethod = m.Groups["MethodName"].Value;
					//Para el caso de .#ctor
					currentMethod = currentMethod.Replace("#", string.Empty);

					result.Add(currentMethod, new List<ResultKind>());
				}
				else if (m.Groups["Message"].Success)
				{
					var message = m.Groups["Message"].Value;
					var resultKind = parseResultKind(message);
					result[currentMethod].Add(resultKind);
				}
			}

			return result;
		}

		private ResultKind parseResultKind(string message)
		{
			if (message.Contains("Requires (including invariants) are unsatisfiable"))
				return ResultKind.UnsatisfiableRequires;

			else if (message.Contains("ensures unproven"))
				return ResultKind.UnprovenEnsures;

			else if (message.Contains("ensures is false"))
				return ResultKind.FalseEnsures;

			else if (message.Contains("ensures (always false) may be reachable"))
				return ResultKind.FalseEnsures;

			else if (message.Contains("requires is false"))
				return ResultKind.FalseRequires;

			else return ResultKind.None;
		}

		private void cccheck_DataReceived(object sender, DataReceivedEventArgs e)
		{
			//Console.WriteLine(e.Data);
			output.AppendLine(e.Data);
		}

		public TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets)
		{
			contractProvider = ContractHelper.ExtractContracts(host, module, pdbReader, pdbReader);
			string queryAssemblyName = generateQueries(source, action, targets);
			ContractHelper.InjectContractCalls(host, module, contractProvider, pdbReader);
			saveAssembly(queryAssemblyName);
			var result = executeChecker(queryAssemblyName);
			var evalResult = evaluateQueries(source, action,targets, result);

			removeQueries();
			return evalResult;
		}

		private string generateQueries(State state, IMethodDefinition action, List<State> states)
		{
			foreach (var target in states)
			{
				var query = generateQuery(state, action, target);

				query.ContainingTypeDefinition = type;
				type.Methods.Add(query);
				queries.Add(query);

				this.TotalGeneratedQueriesCount++;
			}

			string queryAssemblyName = string.Format("query{0}.dll", type.Name.Value);
			return Path.Combine(Configuration.TempPath, queryAssemblyName);
		}

		private MethodDefinition generateQuery(State state, IMethodDefinition action, State target)
		{
			var contracts = new MethodContract();
			var typeInv = Helper.GenerateTypeInvariant(host, contractProvider, type);

			var typeInvPre = from expr in typeInv
							  select new Precondition()
							  {
								  Condition = expr
							  };

			var stateInv = Helper.GenerateStateInvariant(host, contractProvider, type, state);

			var precondition = from expr in stateInv
							   select new Precondition()
							   {
								   Condition = expr
							   };

			contracts.Preconditions.AddRange(typeInvPre);
			contracts.Preconditions.AddRange(precondition);

			var typeInvPost = from expr in typeInv
							  select new Postcondition()
							  {
								  Condition = expr
							  };

			var targetInv = Helper.GenerateStateInvariant(host, contractProvider, type, target);

			var postcondition = new Postcondition()
			{
				Condition = new LogicalNot()
				{
					Type = host.PlatformType.SystemBoolean,
					Operand = Helper.JoinWithLogicalAnd(host, targetInv, true)
				},
			};

			contracts.Postconditions.AddRange(typeInvPost);
			contracts.Postconditions.Add(postcondition);

			var methodName = string.Format("{0}_{1}_{2}", state.Name, action.Name, target.Name);
			MethodDefinition method = generateQuery(methodName, action);

			contractProvider.AssociateMethodWithContract(method, contracts);
			return method;
		}

		private TransitionAnalysisResult evaluateQueries(State source, IMethodDefinition action, List<State> targets, Dictionary<string, List<ResultKind>> result)
		{
			var analysisResult = new TransitionAnalysisResult();

			foreach (var entry in result)
			{
				if (entry.Value.Contains(ResultKind.FalseEnsures) ||
					entry.Value.Contains(ResultKind.UnprovenEnsures))
				{
					var query = entry.Key;
					var actionName = query.Substring(query.LastIndexOf('_') + 1);
					var target = targets.Find(s => s.Name == actionName);
					var isUnproven = entry.Value.Contains(ResultKind.UnprovenEnsures);

					if (target != null)
					{
						var transition = new Transition(source, action, target, isUnproven);
						analysisResult.Transitions.Add(transition);
					}

					if (isUnproven)
						this.UnprovenQueriesCount++;
				}
			}

			return analysisResult;
		}

		private void saveAssembly(string assemblyName)
		{
			foreach (var staticType in module.AllTypes)
			{
				var type = staticType as NamespaceTypeDefinition;
				var invariantMethod = type.Methods.Find(m => m.Name.Value == "$InvariantMethod$");
				type.Methods.Remove(invariantMethod);
			}

			string pdbName = Path.ChangeExtension(assemblyName, "pdb");

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

		private void removeQueries()
		{
			foreach (var m in queries)
				type.Methods.Remove(m);

			queries.Clear();
		}
	}
}
