using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{

    public abstract class ResultDecorator: QueryDecorator
    {
        protected IExpression comparisonExpression; //this expression will be used to instrument the query.

        public void DefineResultValueToCompare(IExpression comparisonExpression)
        {
            this.comparisonExpression = comparisonExpression;
        }
    }
}
