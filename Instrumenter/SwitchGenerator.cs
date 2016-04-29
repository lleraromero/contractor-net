using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Instrumenter
{
    public class SwitchGenerator
    {
        protected Epa epa;
        protected Dictionary<State, int> stateNumberMap;

        public SwitchGenerator(Epa epa, Dictionary<State, int> stateNumberMap)
        {
            this.epa = epa;
            this.stateNumberMap = stateNumberMap;
        }

        public IStatement GenerateSwitch(FieldDefinition field, Dictionary<int, List<int>> transitions)
        {
            var switchStmt = new SwitchStatement
            {
                Expression = new BoundExpression
                {
                    Definition = field,
                    Instance = new ThisReference(),
                    Type = field.Type
                }
            };

            foreach (var t in transitions)
            {
                //Estado deadlock o trampa
                if (t.Value.Count == 0)
                {
                    continue;
                }
                var caseStmt = GenerateSwitchCase(field, t.Key, t.Value);
                switchStmt.Cases.Add(caseStmt);
            }

            return switchStmt;
        }

        protected ISwitchCase GenerateSwitchCase(FieldDefinition field, int fromStateId, List<int> toStateIds)
        {
            Contract.Requires(field != null && toStateIds != null && toStateIds.Count > 0);

            var caseStmt = new SwitchCase
            {
                Expression = new CompileTimeConstant
                {
                    Type = field.Type,
                    Value = fromStateId
                }
            };

            IStatement stmt;
            //Determinismo y no determinismo
            if (toStateIds.Count == 1)
            {
                stmt = new AssignmentGenerator().GenerateAssign(field, toStateIds.First());
            }
            else
            {
                stmt = new IfGenerator().GenerateIf(field, toStateIds, epa, stateNumberMap);
            }

            caseStmt.Body.Add(stmt);
            caseStmt.Body.Add(new BreakStatement());
            return caseStmt;
        }
    }
}