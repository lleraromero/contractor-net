using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public abstract class QueryDecorator
    {
        protected QueryDecorator decorateer;

        public virtual Action Decorate(Action query)
        {
            return decorateer.Decorate(this.InstrumentQuery(query));
        }

        public abstract Action InstrumentQuery(Action query);
    }
}
