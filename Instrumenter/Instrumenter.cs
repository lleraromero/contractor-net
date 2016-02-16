//using System;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//using System.Linq;
//using Analysis.Cci;
//using Contractor.Core.Model;
//using Contractor.Utils;
//using Microsoft.Cci;
//using Microsoft.Cci.Contracts;
//using Microsoft.Cci.MutableCodeModel;
//using Microsoft.Cci.MutableContracts;

// TODO (lleraromero): arreglar
//namespace Instrumenter
//{
//    public class Instrumenter
//    {
//        private IContractAwareHost host;
//        private ContractProvider cp;
//        private NamespaceTypeDefinition type;
//        private Dictionary<string, List<IPrecondition>> preconditions;
//        private Epa epa;

//        public Instrumenter(IContractAwareHost host, ContractProvider cp)
//        {
//            Contract.Requires(host != null && cp != null);

//            preconditions = new Dictionary<string, List<IPrecondition>>();
//            this.host = host;
//            this.cp = cp;
//        }

//        public void GenerateOutputAssembly(string outputFileName, Epa epa)
//        {
//            Contract.Requires(!string.IsNullOrEmpty(outputFileName));

//            var contractProvider = inputAssembly.ExtractContracts();
//            var instrumenter = new Instrumenter(host, contractProvider);

//            foreach (var typeUniqueName in epas.Keys)
//            {
//                var typeAnalysis = epas[typeUniqueName];

//                if (!instrumentedEpas.Contains(typeUniqueName))
//                {
//                    var type = (from t in inputAssembly.DecompiledModule.AllTypes
//                                where typeUniqueName == t.GetUniqueName()
//                                select t as NamespaceTypeDefinition)
//                                .First();

//                    instrumenter.InstrumentType(type, typeAnalysis.EPA);
//                    instrumentedEpas.Add(typeUniqueName);
//                }
//            }

//            inputAssembly.InjectContracts(contractProvider);
//            inputAssembly.Save(outputFileName);
//        }
//        public void InstrumentType(NamespaceTypeDefinition type, Epa epa)
//        {
//            Contract.Requires(type != null && epa != null);
//            Contract.Requires(epa.Type == type.GetUniqueName());

//            this.type = type;
//            this.epa = epa;

//            var actions = from transition in epa.Transitions.GroupBy(t => (t as Transition).Action) select transition.Key;

//            foreach (var action in actions)
//            {
//                var mc = cp.GetMethodContractFor(action) as MethodContract;
//                if (mc == null) continue;

//                var actionUniqueName = action.Name;
//                preconditions.Add(actionUniqueName, mc.Preconditions.ToList());
//            }

//            //Borramos los metodos que continen los invariantes de tipo
//            var tc = cp.GetTypeContractFor(type) as TypeContract;

//            if (tc != null)
//            {
//                tc.Invariants.Clear();
//                var methods = ContractHelper.GetInvariantMethods(type).ToList();

//                foreach (var m in methods)
//                    type.Methods.Remove(m);
//            }

//            //Agregamos un nuevo campo privado para codificar el estado
//            var field = new FieldDefinition()
//            {
//                Name = host.NameTable.GetNameFor("$state"),
//                Type = host.PlatformType.SystemInt32,
//                Visibility = TypeMemberVisibility.Private,
//                ContainingTypeDefinition = type,
//                InternFactory = type.InternFactory,
//                CompileTimeValue = new CompileTimeConstant()
//                {
//                    Type = host.PlatformType.SystemInt32,
//                    Value = 0
//                }
//            };

//            type.Fields.Add(field);

//            // Como el $state es int, necesito el invariante ya que no puede ser negativo.
//            // Se usa int en vez de uint, para que no haya problemas con la traduccion de BCT
//            tc.Invariants.Add(new TypeInvariant()
//            {
//                Condition = new GreaterThanOrEqual()
//                {
//                    LeftOperand = new BoundExpression() { Definition = field, Instance = new ThisReference(), Type = field.Type },
//                    RightOperand = new CompileTimeConstant() { Type = host.PlatformType.SystemInt32, Value = 0 }
//                },
//            });

//            foreach (var action in actions)
//            {
//                var actionUniqueName = action.Name;
//                // voy a agrupar las transiciones que usan esta accion por sourceState.Id
//                // transitions = Dicc<uint, List<uint>> o sea: "Dicc<from, List<to>>"
//                var transUsingAction = from t in epa.Transitions where ((Transition)t).Action.Equals(action) select t as Transition;
//                var transSourceIds = (from t in transUsingAction select t.SourceState.Name).Distinct();
//                var transitions = transUsingAction.GroupBy(t => t.SourceState.Name).ToDictionary(t => t.Key, t => (from tran in t select tran.TargetState.Name).ToList());

//                var mc = cp.GetMethodContractFor(action) as MethodContract;

//                if (mc == null)
//                {
//                    mc = new MethodContract();
//                    cp.AssociateMethodWithContract(action, mc);
//                }
//                else
//                {
//                    mc.Preconditions.Clear();
//                    mc.Postconditions.Clear();
//                }

//                BlockStatement actionBodyBlock = null;

//                if (action.Method.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
//                {
//                    var actionBody = action.Method.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
//                    actionBodyBlock = actionBody.Block as BlockStatement;
//                }
//                else if (action.Method.Body is SourceMethodBody)
//                {
//                    var actionBody = action.Method.Body as SourceMethodBody;
//                    actionBodyBlock = actionBody.Block as BlockStatement;
//                }

//                //Por tratarse de un constructor insertamos
//                //en 1 porque en 0 esta base..ctor();
//                var insertAtIndex = (action.Method.IsConstructor ? 1 : 0);

//                // CodeContracts no permite utilizar this
//                // en los requires de los constructores
//                if (!action.Method.IsConstructor)
//                {
//                    var pre = new PreconditionGenerator().GeneratePrecondition(field, transitions.Keys);
//                    mc.Preconditions.Add(pre);
//                }

//                var posts = new PostconditionGenerator().GeneratePostconditions(field, transitions);
//                mc.Postconditions.AddRange(posts);

//                var stmt = new SwitchGenerator(this).GenerateSwitch(field, transitions);

//                // Se actualiza el $state en un finally porque los if de adentro
//                // del switch tienen que ser ejecutados despues del cuerpo de este metodo 

//                var stmtsCount = actionBodyBlock.Statements.Count - insertAtIndex;
//                var tryBlock = new BlockStatement();
//                var bodyStmts = new List<IStatement>(actionBodyBlock.Statements.GetRange(insertAtIndex, stmtsCount));
//                tryBlock.Statements.AddRange(bodyStmts);

//                var finallyBlock = new BlockStatement();
//                finallyBlock.Statements.Add(stmt);

//                var tryStmt = new TryCatchFinallyStatement()
//                {
//                    TryBody = tryBlock,
//                    FinallyBody = finallyBlock
//                };

//                actionBodyBlock.Statements.RemoveRange(insertAtIndex, stmtsCount);
//                actionBodyBlock.Statements.Insert(insertAtIndex, tryStmt);
//            }

//            preconditions.Clear();
//            this.type = null;
//            this.epa = null;
//        }
//    }
//}
