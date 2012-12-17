using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableContracts;

namespace Contractor.Core
{
	static class Helper
	{
		// A || B = A ? true : B
		public static IExpression JoinWithLogicalOr(IMetadataHost host, List<IExpression> expressions, bool defaultValue)
		{
			if (expressions.Count == 0)
				return new CompileTimeConstant()
				{
					Type = host.PlatformType.SystemBoolean,
					Value = defaultValue
				};

			IExpression result = expressions[0];

			for (int i = 1; i < expressions.Count; ++i)
			{
				result = new Conditional()
				{
					Type = host.PlatformType.SystemBoolean,
					Condition = result,
					ResultIfTrue = new CompileTimeConstant()
					{
						Type = host.PlatformType.SystemBoolean,
						Value = true
					},
					ResultIfFalse = expressions[i]
				};
			}

			return result;
		}

		// A && B = A ? B : false
		public static IExpression JoinWithLogicalAnd(IMetadataHost host, List<IExpression> expressions, bool defaultValue)
		{
			if (expressions.Count == 0)
				return new CompileTimeConstant()
				{
					Type = host.PlatformType.SystemBoolean,
					Value = defaultValue
				};

			IExpression result = expressions[0];

			for (int i = 1; i < expressions.Count; ++i)
			{
				result = new Conditional()
				{
					Type = host.PlatformType.SystemBoolean,
					Condition = result,
					ResultIfTrue = expressions[i],
					ResultIfFalse = new CompileTimeConstant()
					{
						Type = host.PlatformType.SystemBoolean,
						Value = false
					}
				};
			}

			return result;
		}

		public static List<IExpression> GenerateTypeInvariant(IMetadataHost host, ContractProvider cp, NamespaceTypeDefinition type)
		{
			var exprs = new List<IExpression>();
			ITypeContract mci = cp.GetTypeContractFor(type);

			if (mci != null && mci.Invariants.Count() > 0)
			{
				var conditions = from inv in mci.Invariants
								 select inv.Condition;

				exprs.AddRange(conditions);
			};

			return exprs;
		}

		public static List<IExpression> GenerateStatesConditions(IMetadataHost host, Dictionary<int, List<IPrecondition>> preconditions,
			NamespaceTypeDefinition type, IEnumerable<EpaState> states)
		{
			//Optimizacion: calculamos la interseccion de todas las acciones habilitadas
			//y desabilitadas de todos los estados y se la restamos a todos
			var firstState = states.First();
			var enabledIntersection = states.Aggregate(firstState.EnabledActions, (IEnumerable<int> a, EpaState s) => a.Intersect(s.EnabledActions));
			var disabledIntersection = states.Aggregate(firstState.DisabledActions, (IEnumerable<int> a, EpaState s) => a.Intersect(s.DisabledActions));
			var conditions = new List<IExpression>();

			foreach (var state in states)
			{
				var enabledActionsId = state.EnabledActions.Except(enabledIntersection);
				var disabledActionsId = state.DisabledActions.Except(disabledIntersection);

				var exprs = generateStateInvariant(host, preconditions, type, enabledActionsId, disabledActionsId);
				var condition = Helper.JoinWithLogicalAnd(host, exprs, true);
				conditions.Add(condition);
			}

			return conditions;
		}

		// Do not include the type invariant
		public static List<IExpression> GenerateStateInvariant(IMetadataHost host, ContractProvider cp, NamespaceTypeDefinition type, State state)
		{
			var preconditions = new Dictionary<int, List<IPrecondition>>();

			foreach (var action in state.EnabledActions)
			{
				var mc = cp.GetMethodContractFor(action);
				if (mc == null) continue;

				preconditions.Add(action.Name.UniqueKey, mc.Preconditions.ToList());
			}

			foreach (var action in state.DisabledActions)
			{
				var mc = cp.GetMethodContractFor(action);
				if (mc == null) continue;

				preconditions.Add(action.Name.UniqueKey, mc.Preconditions.ToList());
			}

			return generateStateInvariant(host, preconditions, type, state.EnabledActions, state.DisabledActions);
		}

		// Do not include the type invariant
		private static List<IExpression> generateStateInvariant(IMetadataHost host, Dictionary<int, List<IPrecondition>> preconditions,
			NamespaceTypeDefinition type, IEnumerable<int> enabledActionsId, IEnumerable<int> disabledActionsId)
		{
			var exprs = new List<IExpression>();

			var enabledActions = from actionId in enabledActionsId
								 join action in type.Methods on actionId equals action.Name.UniqueKey
								 select action;

			var disabledActions = from actionId in disabledActionsId
								  join action in type.Methods on actionId equals action.Name.UniqueKey
								  select action;

			return generateStateInvariant(host, preconditions, type, enabledActions, disabledActions);
		}

		// Do not include the type invariant
		private static List<IExpression> generateStateInvariant(IMetadataHost host, Dictionary<int, List<IPrecondition>> preconditions, NamespaceTypeDefinition type,
			IEnumerable<IMethodDefinition> enabledActions, IEnumerable<IMethodDefinition> disabledActions)
		{
			var exprs = new List<IExpression>();

			foreach (var action in enabledActions)
			{
				if (!preconditions.ContainsKey(action.Name.UniqueKey)) continue;
				var actionPreconditions = preconditions[action.Name.UniqueKey];

				var conditions = from pre in actionPreconditions
								 select pre.Condition;

				exprs.AddRange(conditions);
			}

			foreach (var action in disabledActions)
			{
				List<IPrecondition> actionPreconditions = null;

				if (preconditions.ContainsKey(action.Name.UniqueKey))
					actionPreconditions = preconditions[action.Name.UniqueKey];

				if (actionPreconditions == null || actionPreconditions.Count == 0)
				{
					var literal = new CompileTimeConstant()
					{
						Type = host.PlatformType.SystemBoolean,
						Value = false
					};

					exprs.Add(literal);
				}
				else
				{
					var conditions = (from pre in actionPreconditions
									  select pre.Condition).ToList();

					var condition = new LogicalNot()
					{
						Type = host.PlatformType.SystemBoolean,
						Operand = Helper.JoinWithLogicalAnd(host, conditions, true)
					};

					exprs.Add(condition);
				}
			}

			return exprs;
		}
	}
}
