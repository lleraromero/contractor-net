using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Analysis.Cci;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Instrumenter
{
    public class IfGenerator
    {
        public IStatement GenerateIf(FieldDefinition field, List<string> to)
        {
            throw new NotImplementedException();
            //Contract.Requires(field != null && to != null && to.Count > 0);
            //var host = CciHostEnvironment.GetInstance();

            //var toStates = from id in to
            //               join state in _instrumenter.epa.States on id equals state.Name
            //               select state;
            //var conditions = Helper.GenerateStatesConditions(host, _instrumenter.preconditions, toStates);

            //IStatement stmt = new AssignmentGenerator().GenerateAssign(field, to[0]);

            //for (int i = 1; i < to.Count; ++i)
            //    stmt = new ConditionalStatement()
            //    {
            //        Condition = conditions[i],
            //        TrueBranch = new AssignmentGenerator().GenerateAssign(field, to[i]),
            //        FalseBranch = stmt
            //    };

            //return stmt;
        }
    }
}