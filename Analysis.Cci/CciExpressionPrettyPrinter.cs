using CSharpSourceEmitter;
using Microsoft.Cci;
using SourceEmitter = CSharpSourceEmitter.SourceEmitter;

namespace Analysis.Cci
{
    public class CciExpressionPrettyPrinter
    {
        public string PrintExpression(IExpression expression)
        {
            //TODO (lleraromero): CCI no se comporta bien con paralelismo
            //var sourceEmitterOutput = new SourceEmitterOutputString();
            //var csSourceEmitter = new SourceEmitter(sourceEmitterOutput);
            //csSourceEmitter.Traverse(expression);
            //return sourceEmitterOutput.Data;
            
            return string.Empty;
        }
    }
}