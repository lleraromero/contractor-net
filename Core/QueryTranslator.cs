using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Contractor.Core
{
    public class QueryTranslator
    {
        private string sourceINV;
        private string actionName;
        private string targetINV;
        private string exitCode;
        private string condition;
        private ITypeDefinition typeToAnalyze;
        private string[] selectedMethods;
        public QueryTranslator(string query, ITypeDefinition typeToAnalyze, string[] selectedMethods)
        {
            query = query.Replace(" ", string.Empty);

            string pattern = @"STATE\$((.|s|\n)*)ACTION\$((.|s|\n)*)STATE\$((.|s|\n)*)EXITCODE\$((.|s|\n)*)CONDITION\$((.|s|\n)*)";
            //Regex rgx = new Regex(Regex.Escape(pattern));
            Regex rgx = new Regex(pattern, RegexOptions.Multiline);
            this.sourceINV = rgx.Replace(query, "$1");
            this.actionName = rgx.Replace(query, "$3");
            this.targetINV = rgx.Replace(query, "$5");
            this.exitCode = rgx.Replace(query, "$7");
            this.condition = rgx.Replace(query, "$9");
            this.typeToAnalyze = typeToAnalyze;
            this.selectedMethods = selectedMethods;
        }

        public State getSource()
        {
            var enabled = this.sourceINV.Split(';');
            return CreateState(enabled);
        }

        private State CreateState(string[] enabled)
        {
            ISet<Contractor.Core.Model.Action> actions = typeToAnalyze.Actions();
            ISet<Contractor.Core.Model.Action> constructors = typeToAnalyze.Constructors();
            actions.UnionWith(constructors);
            var selected = actions.Where(x => this.selectedMethods.Contains(x.ToString()));
            var enabledActions = new HashSet<Contractor.Core.Model.Action>();
            foreach (var enabledAction in enabled)
            {
                foreach (var action in selected)
                {
                    var actionName = action.ToString().Replace(" ", string.Empty);
                    if (actionName.Equals(enabledAction))
                    {
                        enabledActions.Add(action);
                    }
                }
            }
            actions.ExceptWith(enabledActions);
            actions.ExceptWith(constructors);
            actions.IntersectWith(selected);
            return new State(enabledActions, actions);
        }

        public Contractor.Core.Model.Action getAction()
        {
            ISet<Contractor.Core.Model.Action> actions = typeToAnalyze.Actions();
            ISet<Contractor.Core.Model.Action> constructors = typeToAnalyze.Constructors();
            actions.UnionWith(constructors);
            foreach (var action in actions)
            {
                var actionName = action.ToString().Replace(" ", string.Empty);
                if (actionName.Equals(this.actionName))
                {
                    return action;
                }
            }
            throw new Exception("Invalid ActionName for the given list:"+this.actionName);
        }

        public State getTarget()
        {
            var enabled = this.targetINV.Split(';');
            return CreateState(enabled);
        }

        public string getExitCode()
        {
            if (this.exitCode == "")
            {
                return null;
            }
            return this.exitCode;
        }

        public string getCondition()
        {
            if (this.condition == "")
            {
                return null;
            }
            return this.condition;
        }
    }
}
