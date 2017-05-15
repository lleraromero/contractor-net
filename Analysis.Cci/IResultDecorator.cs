using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public abstract class IResultDecorator: QueryDecorator
    {
        public abstract void DefineResultValueToCompare(IExpression value);
    }
}
