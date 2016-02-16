using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Instrumenter
{
    public class SwitchGenerator
    {
        public ISwitchCase GenerateSwitchCase(FieldDefinition field, string fromId, List<string> to)
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
                stmt = new AssignmentGenerator().GenerateAssign(field, to.First());
            else
                stmt = new IfGenerator().GenerateIf(field, to);

            caseStmt.Body.Add(stmt);
            caseStmt.Body.Add(new BreakStatement());
            return caseStmt;
        }

        public IStatement GenerateSwitch(FieldDefinition field, Dictionary<string, List<string>> transitions)
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
                var caseStmt = GenerateSwitchCase(field, t.Key, t.Value);
                switchStmt.Cases.Add(caseStmt);
            }

            return switchStmt;
        }
    }
}