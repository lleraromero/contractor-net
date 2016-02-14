using CSharpSourceEmitter;
using Microsoft.Cci;
using SourceEmitter = CSharpSourceEmitter.SourceEmitter;

namespace Analysis.Cci
{
    public class CciExpressionPrettyPrinter
    {
        public static string PrintExpression(IExpression expression)
        {
            var sourceEmitterOutput = new SourceEmitterOutputString();
            var csSourceEmitter = new SourceEmitter(sourceEmitterOutput);
            csSourceEmitter.Traverse(expression);
            return sourceEmitterOutput.Data;
        }
    }
}