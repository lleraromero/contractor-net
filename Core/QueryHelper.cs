using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contractor.Core
{
    public class QueryHelper
    {
        protected IAnalyzer analyzer;
        protected int cutter;
        protected List<string> errorList;
        private ITypeDefinition typeToAnalyze;

        public QueryHelper(IAnalyzer analyzer, ITypeDefinition typeToAnalyze, List<string> errorList)
        {
            Contract.Requires(analyzer != null);
            this.analyzer = analyzer;
            this.errorList = errorList;
            this.typeToAnalyze = typeToAnalyze;
        }

        public Transition computeQuery(string query, string[] selectedMethods)
        {
            var translator = new QueryTranslator(query,typeToAnalyze,selectedMethods);
            var source = translator.getSource();
            var action = translator.getAction();
            var target = translator.getTarget();
            var exitCode = translator.getExitCode();
            var condition = translator.getCondition();
            return computeQuery(source, action, target,exitCode, condition);
        }

        public Transition computeQuery(State source, Contractor.Core.Model.Action action, State target)
        {
            return computeQuery(source, action, target, null);
        }

        public Transition computeQuery(State source, Contractor.Core.Model.Action action, State target, string exitCode)
        {
            return computeQuery(source, action, target,exitCode, (string)null);
        }

        public Transition computeQuery(State source,Contractor.Core.Model.Action action, State target, string exitCode, string condition)
        {
            var targets=new List<State>();
            targets.Add(target);
            var result= analyzer.AnalyzeTransitions(source, action, targets,exitCode);
            if (result.Count != 0)
            {
                return result.ElementAt(0);
            }
            return null;
        }

        //if all conditions are satisfactible then the condition will be true.
        public Transition computeQuery(State source, Contractor.Core.Model.Action action, State target, string exitCode, List<string> conditions)
        {
            bool allTrue = true;
            var transitions = new List<Transition>();
            foreach (var condition in conditions)
            {
                var transition= computeQuery(source, action, target, exitCode, condition);
                allTrue = allTrue && transition != null;
                if (transition!=null){
                    transitions.Add(transition);
                }
            }
            if (allTrue){
                //create transition with true
            }
            else
            {
                //unify true conditions
            }
            throw new NotImplementedException();
        }
    }
}
