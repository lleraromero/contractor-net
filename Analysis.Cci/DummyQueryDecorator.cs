using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public class DummyQueryDecorator: QueryDecorator
    {
        public override Action InstrumentQuery(Action query)
        {
            //do nothing
            return query;
        }

        public override Action Decorate(Action query)
        {
            //do not call decorateer.Decorate(query)
            return query;
        }
    }
}
