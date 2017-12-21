using Backend.Analyses;
using Backend.Model;
using Backend.Serialization;
using Backend.ThreeAddressCode.Instructions;
using Backend.Transformations;
using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public class ExceptionExtractor
    {
        private IMetadataHost host;
		private ISourceLocationProvider sourceLocationProvider;
        private ISet<string> allExceptions;
        private Dictionary<IMethodDefinition, List<string>> exceptionsByMethod;

        public ExceptionExtractor(string fileName)
		{
            
            this.host = CciHostEnvironment.GetInstance();
            Backend.Types.Initialize(host);
            var pdbFileName = Path.ChangeExtension(fileName, "pdb");

            if (File.Exists(pdbFileName))
            {
                using (var pdbStream = File.OpenRead(pdbFileName))
                    this.sourceLocationProvider = new PdbReader(pdbStream, this.host);
            }

            this.allExceptions= new HashSet<string>();
            this.exceptionsByMethod = new Dictionary<IMethodDefinition, List<string>>();
		}

        public void Process(IEnumerable<IMethodDefinition> rootMethods)
        {
            //var className = methodDefinition.ContainingTypeDefinition.ToString();
            //if (methodDefinition.IsExternal) return;

            //if (!className.Contains("NamedPipeServerStream") || methodDefinition.Name.Value != "PipeStream_Read") return;

            //var signature = MemberHelper.GetMethodSignature(methodDefinition, NameFormattingOptions.Signature | NameFormattingOptions.ParameterName);
            //System.Console.WriteLine(signature);

            var ch = new ClassHierarchyAnalysis(host);
            var cha = new ClassHierarchyCallGraphAnalysis(host, ch);
            cha.OnNewMethodFound = OnNewMethodFound;
            var cg = cha.Analyze(rootMethods);

            //var dgmlCG = DGMLSerializer.Serialize(cg);

            //var methodBody = MethodBodyProvider.Instance.GetBody(methodDefinition);
            //var methodBodyTAC = methodBody.ToString();

            var exceptionType = host.PlatformType.SystemException;

            foreach (var root in rootMethods)
            {
                var methodExceptions = new HashSet<string>();
                var reachableMethods = GetRechableMethods(cg, root);

                foreach (var method in reachableMethods)
                {
                    var exists = MethodBodyProvider.Instance.ContainsBody(method);
                    if (!exists || exceptionsByMethod.ContainsKey(method)) continue;

                    var body = MethodBodyProvider.Instance.GetBody(method);
                    //var throws = body.Instructions.OfType<ThrowInstruction>().ToList();
                    //var exceptions = throws.Select(t => t.Operand.Type).ToList();

                    var news = body.Instructions.OfType<CreateObjectInstruction>().ToList();
                    var exceptions = news.Select(t => t.AllocationType).Where(t => IsSubTypeOf(t, exceptionType)).ToList();
                    var exceptionsNames = exceptions.Select(t => t.ToString()).ToList();

                    methodExceptions.UnionWith(exceptionsNames);
                }

                this.exceptionsByMethod.Add(root, methodExceptions.ToList());
                this.allExceptions.UnionWith(methodExceptions);
            }

            ////var dot = DOTSerializer.Serialize(cfg);
            //var dgmlCFG = DGMLSerializer.Serialize(cfg);
            //var dgmlCH = DGMLSerializer.Serialize(ch);
        }

        private ISet<IMethodDefinition> GetRechableMethods(CallGraph cg, IMethodDefinition root)
        {
            var result = new HashSet<IMethodDefinition>();
            var worklist = new HashSet<IMethodReference>();
            worklist.Add(root);

            while (worklist.Count > 0)
            {
                var method = worklist.First();
                worklist.Remove(method);

                if (method.ResolvedMethod == null) continue;
                result.Add(method.ResolvedMethod);

                var invocations = cg.GetInvocations(method.ResolvedMethod);
                worklist.UnionWith(invocations.SelectMany(inv => inv.PossibleCallees));
            }

            return result;
        }

        //private bool IsExceptionType(ITypeReference type, ITypeReference baseType)
        //{
        //    var namedType = type as INamedTypeReference;
        //    var result = namedType.ResolvedType.BaseClasses.Any(t => t.ToString() == baseType.ToString());

        //    if (!result)
        //    {
        //        foreach (var t in namedType.ResolvedType.BaseClasses)
        //        {
        //            result = IsExceptionType(t, baseType);
        //            if (result) break;
        //        }
        //    }

        //    return result;
        //}

        private bool IsSubTypeOf(ITypeReference type, ITypeReference baseType)
        {
            var baseClass = TypeHelper.BaseClass(type.ResolvedType);
            if (baseClass == null) return false;

            var result = baseClass.ToString() == baseType.ToString();

            if (!result)
            {
                result = IsSubTypeOf(baseClass, baseType);
            }

            return result;
        }

        private bool OnNewMethodFound(IMethodDefinition methodDefinition)
        {
            if (methodDefinition.IsExternal) return false;

            var unit = TypeHelper.GetDefiningUnit(methodDefinition.ContainingTypeDefinition);

            if (unit.UnitIdentity.Equals(host.CoreAssemblySymbolicIdentity) ||
                unit.UnitIdentity.Equals(host.SystemCoreAssemblySymbolicIdentity))
                return false;

            var disassembler = new Disassembler(host, methodDefinition, sourceLocationProvider);
            var methodBody = disassembler.Execute();
            //var methodBodyTAC = methodBody.ToString();

            //System.Console.WriteLine(methodBody);
            //System.Console.WriteLine();

            var cfAnalysis = new ControlFlowAnalysis(methodBody);
            //var cfg = cfAnalysis.GenerateNormalControlFlow();
            var cfg = cfAnalysis.GenerateExceptionalControlFlow();

            //var domAnalysis = new DominanceAnalysis(cfg);
            //domAnalysis.Analyze();
            //domAnalysis.GenerateDominanceTree();

            //var loopAnalysis = new NaturalLoopAnalysis(cfg);
            //loopAnalysis.Analyze();

            //var domFrontierAnalysis = new DominanceFrontierAnalysis(cfg);
            //domFrontierAnalysis.Analyze();

            var splitter = new WebAnalysis(cfg);
            splitter.Analyze();
            splitter.Transform();

            methodBody.UpdateVariables();
            var methodBodyTAC = methodBody.ToString();

            var typeAnalysis = new TypeInferenceAnalysis(cfg);
            typeAnalysis.Analyze();
            //var forwardCopyAnalysis = new ForwardCopyPropagationAnalysis(cfg);
            //forwardCopyAnalysis.Analyze();
            //forwardCopyAnalysis.Transform(methodBody);

            //var backwardCopyAnalysis = new BackwardCopyPropagationAnalysis(cfg);
            //backwardCopyAnalysis.Analyze();
            //backwardCopyAnalysis.Transform(methodBody);

            //var pointsTo = new PointsToAnalysis(cfg);
            //var result = pointsTo.Analyze();

            //var liveVariables = new LiveVariablesAnalysis(cfg);
            //liveVariables.Analyze();

            //var ssa = new StaticSingleAssignment(methodBody, cfg);
            //ssa.Transform();
            //ssa.Prune(liveVariables);

            //methodBody.UpdateVariables();

            MethodBodyProvider.Instance.AddBody(methodDefinition, methodBody);
            return true;
        }

        public List<string> GetAllExceptions {
            get { return allExceptions.ToList(); }
        }

        public Dictionary<IMethodDefinition,List<string>> GetExceptionsByMethods
        {
            get { return exceptionsByMethod; }
        }


        public void Process(IEnumerable<Contractor.Core.Model.Action> methods)
        {
            var rootMethods = methods.Select(m => m.Method);
            this.Process(rootMethods);
        }
    }
}
