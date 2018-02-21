using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.IO;
using Contractor.Core.Properties;
using System.Collections;

using Contractor.Core;
using Contractor.Core.Model;
namespace Analyzer.CodeContracts
{
    public class CSGenerator
    {
        private static CSGenerator instance;
        private AnalyzerWithCondition checker;

        private CSGenerator(AnalyzerWithCondition checker)
        {
            this.checker = checker;           
        }

        public static CSGenerator Instance(AnalyzerWithCondition checker)
        {
                if (instance == null)
                {
                    instance = new CSGenerator(checker);
                }
                return instance;
        }


        public IReadOnlyCollection<Transition> generateConditions(IReadOnlyCollection<Transition> transitions)
        {
            return checker.GenerateConditions(transitions);
        }

        
        //public static string generateCS(Dictionary<string, List<string>> result, string query)
        public static string generateCS(List<string>list)
        {
            if (list == null)
            {
                return "True";
            }
                if (list.Count == 0)
                {
                    return "True";
                }
                HashSet<string> set = new HashSet<string>();

                string condition = list.ElementAt(0);
                set.Add(condition);
                for (int i = 1; i < list.Count; i++)
                {
                    if (!set.Contains(list.ElementAt(i)))
                        condition += " && " + list.ElementAt(i);
                }
                return condition;
        }

        public static string parseCondition(string message)
        {
            if (message.Contains("Suggested requires: "))
            {
                string condition = message.Substring(message.IndexOf('('));
                condition = condition.Substring(0, condition.Length - 1);
                return condition;
            }
            else if (message.Contains("Suggested assume: "))
            {
                string condition = message.Substring(message.LastIndexOf(':')+3);
                //condition = condition.Substring(0, condition.Length - 1);
                return condition;
            }
            else if (message.Contains("Missing precondition in an externally visible method"))
            {
                string condition = message.Substring(message.IndexOf('('));
                condition = condition.Substring(0, condition.IndexOf(';'));
                return condition;
            }
            else
            {
                return "";
            }
        }

    }
}
