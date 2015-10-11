using Contractor.Core.Model;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core
{
    class Instrumenter
    {
        private IContractAwareHost host;
        private ContractProvider cp;
        private NamespaceTypeDefinition type;
        private Dictionary<string, List<IPrecondition>> preconditions;
        private Epa epa;

        public Instrumenter(IContractAwareHost host, ContractProvider cp)
        {
            Contract.Requires(host != null && cp != null);

            preconditions = new Dictionary<string, List<IPrecondition>>();
            this.host = host;
            this.cp = cp;
        }

        public void InstrumentType(NamespaceTypeDefinition type, Epa epa)
        {
            Contract.Requires(type != null && epa != null);
            Contract.Requires(epa.Type == type.GetUniqueName());

            this.type = type;
            this.epa = epa;

            var actions = from transition in epa.Transitions.GroupBy(t => (t as Transition).Action) select transition.Key;

            foreach (var action in actions)
            {
                var mc = cp.GetMethodContractFor(action) as MethodContract;
                if (mc == null) continue;

                var actionUniqueName = action.Name;
                preconditions.Add(actionUniqueName, mc.Preconditions.ToList());
            }

            //Borramos los metodos que continen los invariantes de tipo
            var tc = cp.GetTypeContractFor(type) as TypeContract;

            if (tc != null)
            {
                tc.Invariants.Clear();
                var methods = ContractHelper.GetInvariantMethods(type).ToList();

                foreach (var m in methods)
                    type.Methods.Remove(m);
            }

            //Agregamos un nuevo campo privado para codificar el estado
            var field = new FieldDefinition()
            {
                Name = host.NameTable.GetNameFor("$state"),
                Type = host.PlatformType.SystemInt32,
                Visibility = TypeMemberVisibility.Private,
                ContainingTypeDefinition = type,
                InternFactory = type.InternFactory,
                CompileTimeValue = new CompileTimeConstant()
                {
                    Type = host.PlatformType.SystemInt32,
                    Value = 0
                }
            };

            type.Fields.Add(field);

            // Como el $state es int, necesito el invariante ya que no puede ser negativo.
            // Se usa int en vez de uint, para que no haya problemas con la traduccion de BCT
            tc.Invariants.Add(new TypeInvariant()
            {
                Condition = new GreaterThanOrEqual()
                {
                    LeftOperand = new BoundExpression() { Definition = field, Instance = new ThisReference(), Type = field.Type},
                    RightOperand = new CompileTimeConstant() { Type = host.PlatformType.SystemInt32, Value = 0 }
                },
            });

            foreach (var action in actions)
            {
                var actionUniqueName = action.Name;
                // voy a agrupar las transiciones que usan esta accion por sourceState.Id
                // transitions = Dicc<uint, List<uint>> o sea: "Dicc<from, List<to>>"
                var transUsingAction = from t in epa.Transitions where ((Transition)t).Action.Equals(action) select t as Transition;
                var transSourceIds = (from t in transUsingAction select t.SourceState.Name).Distinct();
                var transitions = transUsingAction.GroupBy(t => t.SourceState.Name).ToDictionary(t => t.Key, t => (from tran in t select tran.TargetState.Name).ToList());

                var mc = cp.GetMethodContractFor(action) as MethodContract;

                if (mc == null)
                {
                    mc = new MethodContract();
                    cp.AssociateMethodWithContract(action, mc);
                }
                else
                {
                    mc.Preconditions.Clear();
                    mc.Postconditions.Clear();
                }

                BlockStatement actionBodyBlock = null;

                if (action.Method.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
                {
                    var actionBody = action.Method.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
                    actionBodyBlock = actionBody.Block as BlockStatement;
                }
                else if (action.Method.Body is SourceMethodBody)
                {
                    var actionBody = action.Method.Body as SourceMethodBody;
                    actionBodyBlock = actionBody.Block as BlockStatement;
                }

                //Por tratarse de un constructor insertamos
                //en 1 porque en 0 esta base..ctor();
                var insertAtIndex = (action.Method.IsConstructor ? 1 : 0);

                // CodeContracts no permite utilizar this
                // en los requires de los constructores
                if (!action.Method.IsConstructor)
                {
                    var pre = generatePrecondition(field, transitions.Keys);
                    mc.Preconditions.Add(pre);
                }

                var posts = generatePostconditions(field, transitions);
                mc.Postconditions.AddRange(posts);

                var stmt = generateSwitch(field, transitions);

                // Se actualiza el $state en un finally porque los if de adentro
                // del switch tienen que ser ejecutados despues del cuerpo de este metodo 

                var stmtsCount = actionBodyBlock.Statements.Count - insertAtIndex;
                var tryBlock = new BlockStatement();
                var bodyStmts = new List<IStatement>(actionBodyBlock.Statements.GetRange(insertAtIndex, stmtsCount));
                tryBlock.Statements.AddRange(bodyStmts);

                var finallyBlock = new BlockStatement();
                finallyBlock.Statements.Add(stmt);

                var tryStmt = new TryCatchFinallyStatement()
                {
                    TryBody = tryBlock,
                    FinallyBody = finallyBlock
                };

                actionBodyBlock.Statements.RemoveRange(insertAtIndex, stmtsCount);
                actionBodyBlock.Statements.Insert(insertAtIndex, tryStmt);
            }

            preconditions.Clear();
            this.type = null;
            this.epa = null;
        }

        private Precondition generatePrecondition(FieldDefinition field, IEnumerable<string> from)
        {
            var conditions = new List<IExpression>();

            foreach (var fromId in from)
            {
                var cond = new Equality()
                {
                    Type = host.PlatformType.SystemBoolean,
                    LeftOperand = new BoundExpression()
                    {
                        Definition = field,
                        Instance = new ThisReference(),
                        Type = field.Type
                    },
                    RightOperand = new CompileTimeConstant()
                    {
                        Type = field.Type,
                        Value = fromId
                    }
                };

                conditions.Add(cond);
            }

            return new Precondition()
            {
                Condition = Helper.JoinWithLogicalOr(host, conditions, false),
                OriginalSource = Helper.PrintExpression(Helper.JoinWithLogicalOr(host, conditions, false))
            };
        }

        private List<Postcondition> generatePostconditions(FieldDefinition field, Dictionary<string, List<string>> transitions)
        {
            var posts = new List<Postcondition>();

            foreach (var t in transitions)
            {
                var post = generatePostcondition(field, t.Key, t.Value);
                posts.Add(post);
            }

            return posts;
        }

        private Postcondition generatePostcondition(FieldDefinition field, string fromId, IEnumerable<string> to)
        {
            IExpression conditional;

            if (this.epa.Initial.Name == fromId)
            {
                //Initial state
                conditional = generateConditionPartTo(field, to);
            }
            else
            {
                conditional = new Conditional()
                {
                    Type = host.PlatformType.SystemBoolean,
                    Condition = generateConditionPartFrom(field, fromId),
                    ResultIfTrue = generateConditionPartTo(field, to),
                    ResultIfFalse = new CompileTimeConstant()
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = true
                    }
                };
            }

            return new Postcondition()
            {
                Condition = conditional,
                OriginalSource = Helper.PrintExpression(conditional)
            };
        }

        private IExpression generateConditionPartFrom(FieldDefinition field, string fromId)
        {
            return new Equality()
            {
                Type = host.PlatformType.SystemBoolean,
                LeftOperand = new OldValue()
                {
                    Type = field.Type,
                    Expression = new BoundExpression()
                    {
                        Definition = field,
                        Instance = new ThisReference(),
                        Type = field.Type
                    }
                },
                RightOperand = new CompileTimeConstant()
                {
                    Type = field.Type,
                    Value = fromId
                }
            };
        }

        private IExpression generateConditionPartTo(FieldDefinition field, IEnumerable<string> to)
        {
            var conditions = new List<IExpression>();

            foreach (var toId in to)
            {
                var cond = new Equality()
                {
                    Type = host.PlatformType.SystemBoolean,
                    LeftOperand = new BoundExpression()
                    {
                        Definition = field,
                        Instance = new ThisReference(),
                        Type = field.Type
                    },
                    RightOperand = new CompileTimeConstant()
                    {
                        Type = field.Type,
                        Value = toId
                    }
                };

                conditions.Add(cond);
            }

            return Helper.JoinWithLogicalOr(host, conditions, false);
        }

        private IStatement generateSwitch(FieldDefinition field, Dictionary<string, List<string>> transitions)
        {
            var switchStmt = new SwitchStatement()
            {
                Expression = new BoundExpression()
                {
                    Definition = field,
                    Instance = new ThisReference(),
                    Type = field.Type,
                }
            };

            foreach (var t in transitions)
            {
                //Estado deadlock o trampa
                if (t.Value.Count == 0) continue;
                var caseStmt = generateSwitchCase(field, t.Key, t.Value);
                switchStmt.Cases.Add(caseStmt);
            }

            return switchStmt;
        }

        private ISwitchCase generateSwitchCase(FieldDefinition field, string fromId, List<string> to)
        {
            Contract.Requires(field != null && to != null && to.Count > 0);

            var caseStmt = new SwitchCase()
            {
                Expression = new CompileTimeConstant()
                {
                    Type = field.Type,
                    Value = fromId
                }
            };

            IStatement stmt = null;

            //Determinismo y no determinismo
            if (to.Count == 1)
                stmt = generateAssign(field, to.First());
            else
                stmt = generateIf(field, to);

            caseStmt.Body.Add(stmt);
            caseStmt.Body.Add(new BreakStatement());
            return caseStmt;
        }

        private IStatement generateIf(FieldDefinition field, List<string> to)
        {
            Contract.Requires(field != null && to != null && to.Count > 0);

            var toStates = from id in to
                           join state in epa.States on id equals state.Name
                           select state;
            var conditions = Helper.GenerateStatesConditions(host, preconditions, type, toStates);

            IStatement stmt = generateAssign(field, to[0]);

            for (int i = 1; i < to.Count; ++i)
                stmt = new ConditionalStatement()
                {
                    Condition = conditions[i],
                    TrueBranch = generateAssign(field, to[i]),
                    FalseBranch = stmt
                };

            return stmt;
        }

        private IStatement generateAssign(FieldDefinition field, string toId)
        {
            var assignStmt = new ExpressionStatement()
            {
                Expression = new Assignment()
                {
                    Type = field.Type,
                    Target = new TargetExpression()
                    {
                        Definition = field,
                        Instance = new ThisReference(),
                        Type = field.Type
                    },
                    Source = new CompileTimeConstant()
                    {
                        Type = field.Type,
                        Value = toId
                    }
                }
            };

            return assignStmt;
        }
    }
}
