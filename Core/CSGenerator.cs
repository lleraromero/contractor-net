using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contractor.Utils;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System.Threading;
using System.IO;
using Contractor.Core.Properties;
using Microsoft.Research.CodeAnalysis;
using System.Compiler.Analysis;
using System.Collections;

namespace Contractor.Core
{
    class CSGenerator
    {
        private static CSGenerator instance;
        private CodeContractsAnalyzer checker;

        private CSGenerator(CodeContractsAnalyzer checker)
        {
            this.checker = checker;
            //checker = new CodeContractsAnalyzer(host, inputAssembly, type, token);
            
        
        }

        public static CSGenerator Instance(CodeContractsAnalyzer checker)
        {
                if (instance == null)
                {
                    instance = new CSGenerator(checker);
                }
                return instance;
        }


        public Dictionary<string, List<string>> executeCheckerConditions(string queryAssemblyName)
        {
            return checker.executeCheckerConditions(queryAssemblyName);
        }

        
        public static string generateCS(Dictionary<string, List<string>> result, string query)
        {

            //var result = executeCheckerConditions(queryAssemblyName);
            //if (result.Keys.Contains(query))

            try
            {
                //
                foreach (string key in result.Keys)
                {
                    if (key.Equals(query))
                        break;
                    string pattern = System.Text.RegularExpressions.Regex.Escape(query);
                    int i = pattern.IndexOf('<');
                    if (i != -1)
                    {
                        int j = pattern.IndexOf('>');
                        pattern = pattern.Substring(0, i) + "(.*)" + pattern.Substring(j + 1);
                    }
                    i = pattern.LastIndexOf('(');
                    if (i != -1)
                    {
                        int j = pattern.LastIndexOf(')');
                        pattern = pattern.Substring(0, i + 1) + "(.*)" + pattern.Substring(j - 1);
                    }

                    System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    //string result = rgx.Replace(query, key);
                    //rgx.Replace()

                    if (rgx.IsMatch(key))
                    {
                        query = key;
                        break;
                    }
                }

                var list = result[query];
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
            catch
            {
                return "";
            }
        }

        public static string parseCondition(string message)
        {
            if (message.Contains("Suggested requires: "))
            {
                string condition = message.Substring(message.IndexOf('('));
                condition = condition.Substring(0, condition.Length - 1);
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
