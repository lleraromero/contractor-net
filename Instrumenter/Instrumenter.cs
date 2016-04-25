using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Analysis.Cci;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;
using IAssembly = Contractor.Core.Model.IAssembly;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;
using SourceMethodBody = Microsoft.Cci.ILToCodeModel.SourceMethodBody;

namespace Instrumenter
{
    public class Instrumenter
    {
        public IAssembly InstrumentType(CciAssembly assembly, Epa epa)
        {
            Contract.Requires(assembly != null);
            Contract.Requires(epa != null);
            Contract.Requires(assembly.Types().Contains(epa.Type));

            // Clone assembly
            var host = CciHostEnvironment.GetInstance();
            var module = new CodeAndContractDeepCopier(host).Copy(assembly.Module);

            var contractExtractor = host.GetContractExtractor(module.UnitIdentity);
            var contractProvider = new AggregatingContractProvider(contractExtractor);

            var preconditions = new Dictionary<string, List<IPrecondition>>();

            var actions = new List<Action>(from transition in epa.Transitions.GroupBy(t => t.Action) select transition.Key);
            Contract.Assert(actions.Any());

            foreach (var action in actions)
            {
                // TODO (lleraromero): Es necesario? NullObject Pattern?
                var mc = action.Contract as MethodContract;
                if (mc == null) continue;

                preconditions.Add(action.Name, action.Contract.Preconditions.ToList());
            }

            var typeDefinition = epa.Type;
            var cciTypeDefinition =
                module.AllTypes.First(
                    t => TypeHelper.GetTypeName(t, NameFormattingOptions.UseGenericTypeNameSuffix).Equals(typeDefinition.Name)) as NamedTypeDefinition;
            Contract.Assert(cciTypeDefinition != null);

            var typeContract = RemoveInvariantMethods(typeDefinition, cciTypeDefinition);

            // Add a field to represent the state
            var field = CreateStateField(cciTypeDefinition, typeContract);
            cciTypeDefinition.Fields.Add(field);

            // Associate type contract to the contract provider
            contractProvider.AssociateTypeWithContract(cciTypeDefinition, typeContract);

            // Create Ids
            var stateNumberMap = new Dictionary<State, int>();
            foreach (var state in epa.States)
            {
                stateNumberMap.Add(state, stateNumberMap.Keys.Count);
            }

            foreach (var action in actions)
            {
                // voy a agrupar las transiciones que usan esta accion por sourceState.Id
                // transitions = Dicc<int, List<int>> o sea: "Dicc<from, List<to>>"
                var transitionsUsingAction = new List<Transition>(from t in epa.Transitions where t.Action.Equals(action) select t);
                var transitionsSourceIds = new HashSet<int>(from t in transitionsUsingAction select stateNumberMap[t.SourceState]).Distinct();

                var transitions = new Dictionary<int, List<int>>();
                foreach (var t in transitionsUsingAction)
                {
                    var sourceStateId = stateNumberMap[t.SourceState];
                    if (!transitions.ContainsKey(sourceStateId))
                    {
                        transitions.Add(sourceStateId, new List<int>());
                    }

                    var targetStateId = stateNumberMap[t.TargetState];
                    transitions[sourceStateId].Add(targetStateId);
                }

                var methodContract = action.Contract as MethodContract ?? new MethodContract();
                var methodDefinition = cciTypeDefinition.Methods.First(m => m.GetUniqueName().Equals(action.Method.GetUniqueName()));

                BlockStatement actionBodyBlock = null;
                if (methodDefinition.Body is SourceMethodBody)
                {
                    var actionBody = (SourceMethodBody) methodDefinition.Body;
                    actionBodyBlock = actionBody.Block as BlockStatement;
                }
                else if (methodDefinition.Body is Microsoft.Cci.MutableCodeModel.SourceMethodBody)
                {
                    var actionBody = (Microsoft.Cci.MutableCodeModel.SourceMethodBody) methodDefinition.Body;
                    actionBodyBlock = actionBody.Block as BlockStatement;
                }
                Contract.Assert(actionBodyBlock != null);

                //Por tratarse de un constructor insertamos en 1 porque en 0 esta base..ctor()
                var insertAtIndex = methodDefinition.IsConstructor ? 1 : 0;

                // CodeContracts no permite utilizar 'this' en los requires de los constructores
                if (!methodDefinition.IsConstructor)
                {
                    var pre = new PreconditionGenerator().GeneratePrecondition(field, transitions.Keys.ToList());
                    methodContract.Preconditions.Add(pre);
                }

                var posts = new PostconditionGenerator(stateNumberMap[epa.Initial]).GeneratePostconditions(field, transitions);
                methodContract.Postconditions.AddRange(posts);

                // Associate contract
                contractProvider.AssociateMethodWithContract(methodDefinition, methodContract);

                var stmt = new SwitchGenerator(epa, stateNumberMap).GenerateSwitch(field, transitions);

                // Se actualiza el $state en un finally porque los if de adentro del switch tienen que ser ejecutados despues del cuerpo de este metodo 
                var stmtsCount = actionBodyBlock.Statements.Count - insertAtIndex;
                var tryBlock = new BlockStatement();
                var bodyStmts = new List<IStatement>(actionBodyBlock.Statements.GetRange(insertAtIndex, stmtsCount));
                tryBlock.Statements.AddRange(bodyStmts);

                var finallyBlock = new BlockStatement();
                finallyBlock.Statements.Add(stmt);

                var tryStmt = new TryCatchFinallyStatement
                {
                    TryBody = tryBlock,
                    FinallyBody = finallyBlock
                };

                actionBodyBlock.Statements.RemoveRange(insertAtIndex, stmtsCount);
                actionBodyBlock.Statements.Insert(insertAtIndex, tryStmt);
            }

            return new CciAssembly(module, contractProvider);
        }

        protected FieldDefinition CreateStateField(NamedTypeDefinition typeDefinition, TypeContract typeContract)
        {
            var host = CciHostEnvironment.GetInstance();

            var field = new FieldDefinition
            {
                Name = host.NameTable.GetNameFor("$state"),
                Type = host.PlatformType.SystemInt32,
                Visibility = TypeMemberVisibility.Private,
                ContainingTypeDefinition = typeDefinition,
                InternFactory = typeDefinition.InternFactory,
                CompileTimeValue = new CompileTimeConstant
                {
                    Type = host.PlatformType.SystemInt32,
                    Value = 0
                }
            };

            // Como el $state es int, necesito el invariante ya que no puede ser negativo.
            // Se usa int en vez de uint, para que no haya problemas con la traduccion de BCT
            if (typeContract == null)
            {
                typeContract = new TypeContract();
            }

            typeContract.Invariants.Add(new TypeInvariant
            {
                Condition = new GreaterThanOrEqual
                {
                    LeftOperand = new BoundExpression { Definition = field, Instance = new ThisReference(), Type = field.Type },
                    RightOperand = new CompileTimeConstant { Type = host.PlatformType.SystemInt32, Value = 0 }
                }
            });

            return field;
        }

        protected TypeContract RemoveInvariantMethods(ITypeDefinition typeDefinition, NamedTypeDefinition cciTypeDefinition)
        {
            var tc = typeDefinition.TypeContract() as TypeContract;
            if (tc != null)
            {
                tc.Invariants.Clear();
                var methods = ContractHelper.GetInvariantMethods(cciTypeDefinition).ToList();

                foreach (var m in methods)
                {
                    cciTypeDefinition.Methods.Remove(m);
                }
            }
            return tc;
        }
    }
}