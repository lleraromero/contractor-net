using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    public class MyLogger
    {
        static string logFile= "Mylog.txt";

        public static void LogStartAnalysis(string pathToDll)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(logFile, true))
            {
                file.WriteLine("***** Starting new analysis *****");
                file.WriteLine("date: " + System.DateTime.Now);
                file.WriteLine("dll to analyze: " + pathToDll + System.Environment.NewLine);

            }
        }

        public static void LogAction(string action, string exitCode, string sourceState)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(logFile, true))
            {
                file.WriteLine("in source: " + sourceState + " action: " + action + " exitCode: " + exitCode + System.Environment.NewLine);
            }
        }

        public static void LogBCT(string args)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(logFile, true))
            {
                file.WriteLine("BCT args: " + args + System.Environment.NewLine);
            }
        }

        public static void LogBCTBreakingQuery(string pathToDll)
        {
            System.IO.File.WriteAllText(@"LastBreakingQuery.txt", pathToDll);
        }

        public static void LogCorral(string args)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(logFile, true))
            {
                file.WriteLine("Corral args: " + args + System.Environment.NewLine);
            }
        }



        public static void LogMsg(string msg)
        {
            using (System.IO.StreamWriter file =
             new System.IO.StreamWriter(logFile, true))
            {
                file.WriteLine("contractor msg: " + msg + System.Environment.NewLine);
            }
        }
    }
}
