﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using Microsoft.Cci.Contracts;
using Microsoft.Cci;
using Contractor.Utils;

namespace Contractor.Core
{
	class Instrumenter
	{
		private IMetadataHost host;
		private ContractProvider cp;
		private NamespaceTypeDefinition type;
		private Dictionary<int, List<IPrecondition>> preconditions;
		private Epa epa;

		public Instrumenter(IMetadataHost host, ContractProvider cp)
		{
			preconditions = new Dictionary<int, List<IPrecondition>>();
			this.host = host;
			this.cp = cp;
		}

		public void InstrumentType(NamespaceTypeDefinition type, Epa epa)
		{
			this.type = type;
			this.epa = epa;

			var actions = from actionId in epa.Keys
						  join action in type.Methods on actionId equals action.Name.UniqueKey
						  select action;

			foreach (var action in actions)
			{
				var mc = cp.GetMethodContractFor(action) as MethodContract;
				if (mc == null) continue;
				preconditions.Add(action.Name.UniqueKey, mc.Preconditions.ToList());
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
				Type = host.PlatformType.SystemUInt32,
				Visibility = TypeMemberVisibility.Private,
				ContainingTypeDefinition = type,
				InternFactory = type.InternFactory,
				CompileTimeValue = new CompileTimeConstant()
				{
					Type = host.PlatformType.SystemUInt32,
					Value = 0
				}
			};

			type.Fields.Add(field);

			foreach (var action in actions)
			{
				var transitions = epa[action.Name.UniqueKey];
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

				if (action.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
				{
					var actionBody = action.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
					actionBodyBlock = actionBody.Block as BlockStatement;
				}
				else if (action.Body is SourceMethodBody)
				{
					var actionBody = action.Body as SourceMethodBody;
					actionBodyBlock = actionBody.Block as BlockStatement;
				}

				//Por tratarse de un constructor insertamos
				//en 1 porque en 0 esta base..ctor();
				var insertAtIndex = (action.IsConstructor ? 1 : 0);

				// CodeContracts no permite utilizar this
				// en los requires de los constructores
				if (!action.IsConstructor)
				{
					var pre = generatePrecondition(field, transitions.Keys);
					mc.Preconditions.Add(pre);
				}

				var posts = generatePostconditions(field, transitions);
				mc.Postconditions.AddRange(posts);

				var stmt = generateSwitch(field, transitions);
				//actionBodyBlock.Statements.Insert(insertAtIndex, stmt);

				// Se actualiza el $state en un finally porque los if de adentro
				// del switch tienen que ser ejecutados despues del cuerpo de este metodo 

				////El -1 es para no borrar el ultimo return implicito
				//var stmtsCount = actionBodyBlock.Statements.Count - insertAtIndex - 1;

				var stmtsCount = actionBodyBlock.Statements.Count - insertAtIndex; 
				var tryBlock = new BlockStatement();
				var bodyStmts = actionBodyBlock.Statements.GetRange(insertAtIndex, stmtsCount);
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

		private Precondition generatePrecondition(FieldDefinition field, IEnumerable<uint> from)
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
				Condition = Helper.JoinWithLogicalOr(host, conditions, false)
			};
		}

		private List<Postcondition> generatePostconditions(FieldDefinition field, Dictionary<uint, List<uint>> transitions)
		{
			var posts = new List<Postcondition>();

			foreach (var t in transitions)
			{
				var post = generatePostcondition(field, t.Key, t.Value);
				posts.Add(post);
			}

			return posts;
		}

		private Postcondition generatePostcondition(FieldDefinition field, uint fromId, IEnumerable<uint> to)
		{
			IExpression conditional;

			if (fromId == 0)
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
				Condition = conditional
			};
		}

		private IExpression generateConditionPartFrom(FieldDefinition field, uint fromId)
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

		private IExpression generateConditionPartTo(FieldDefinition field, IEnumerable<uint> to)
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

		private IStatement generateSwitch(FieldDefinition field, Dictionary<uint, List<uint>> transitions)
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

		private ISwitchCase generateSwitchCase(FieldDefinition field, uint fromId, IList<uint> to)
		{
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

		private IStatement generateIf(FieldDefinition field, IList<uint> to)
		{
			var toStates = from id in to select epa.States[id];
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

		private IStatement generateAssign(FieldDefinition field, uint toId)
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
