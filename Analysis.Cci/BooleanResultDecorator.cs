using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public class BooleanResultDecorator: ResultDecorator
    {

        public BooleanResultDecorator()
        {
            decorateer = new DummyQueryDecorator();
            InitializeDefaultExpressionToCompare();
        }

        public BooleanResultDecorator(QueryDecorator decorateer)
        {
            this.decorateer = decorateer;
            InitializeDefaultExpressionToCompare();
        }

        private void InitializeDefaultExpressionToCompare()
        {
            //comparisonExpression= result == value
          
            comparisonExpression = new Equality
            {
                //Type = host.PlatformType.SystemBoolean,

                LeftOperand = null,

                RightOperand = null
            };
        }

    
        public override Action InstrumentQuery(Action query)
        {
            throw new NotImplementedException();
        }

        public List<Action> GetDefaultQueries(Action query)
        {
            var result = new List<Action>();
            //saving old value to restore
            var copy = comparisonExpression;

            InitializeDefaultExpressionToCompare();
            //((Equality)comparisonExpression).Type = 
            //((Equality)comparisonExpression).RightOperand = new 
            result.Add(InstrumentQuery(query));
            
            result.Add(InstrumentQuery(query));

            //restoring
            DefineResultValueToCompare(copy);
            
            return result;
        }
    }
}
