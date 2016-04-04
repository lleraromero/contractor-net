using Contractor.Core.Model;
using Contractor.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Analyzer.CodeContracts
{
    class ConditionQueryEvaluator
    {
        private CodeContractsRunner solver;
        private System.IO.FileInfo queryAssembly;

        public ConditionQueryEvaluator(CodeContractsRunner codeContractsRunner, System.IO.FileInfo queryAssembly)
        {
            // TODO: Complete member initialization
            this.solver = codeContractsRunner;
            this.queryAssembly = queryAssembly;
        }

        internal IReadOnlyCollection<Transition> GetTransitionsWithConditions(IReadOnlyCollection<Transition> transitions,IReadOnlyCollection<Contractor.Core.TransitionQuery> transitionQueries)
        {
            Contract.Requires(transitionQueries != null);

            var resultTransitions = new List<Transition>();

            var results = solver.ExecuteWithConditions(queryAssembly, transitionQueries);
            foreach (var query in transitionQueries)
            {
                List<string> result;
                try
                {
                    result = results[query];
                }
                catch(Exception e){
                    result = null;
                }
                resultTransitions.Add(new Transition(query.Action, query.SourceState, query.TargetState, false, CSGenerator.generateCS(result)));
            }

            return resultTransitions;
        }
    }
}
