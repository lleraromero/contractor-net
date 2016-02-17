using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Instrumenter
{
    public class AssignmentGenerator
    {
        public IStatement GenerateAssign(FieldDefinition field, int toStateId)
        {
            var assignStmt = new ExpressionStatement
            {
                Expression = new Assignment
                {
                    Type = field.Type,
                    Target = new TargetExpression
                    {
                        Definition = field,
                        Instance = new ThisReference(),
                        Type = field.Type
                    },
                    Source = new CompileTimeConstant
                    {
                        Type = field.Type,
                        Value = toStateId
                    }
                }
            };

            return assignStmt;
        }
    }
}