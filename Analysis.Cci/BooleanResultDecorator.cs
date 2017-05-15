using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public class BooleanResultDecorator: IResultDecorator
    {
        private IExpression value; 

        public BooleanResultDecorator()
        {
            decorateer = new DummyQueryDecorator();
        }

        public BooleanResultDecorator(QueryDecorator decorateer)
        {
            this.decorateer = decorateer;
        }

        public override Action InstrumentQuery(Action query)
        {
            throw new NotImplementedException();
        }

        public override void DefineResultValueToCompare(IExpression value)
        {
            this.value = value;
        }
    }
}
