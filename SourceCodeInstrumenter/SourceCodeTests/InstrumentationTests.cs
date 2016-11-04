using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;

namespace SourceCodeTests
{
    [TestClass]
    public class InstrumentationTests
    {
        [ClassInitialize()]
        public static void CallInstrumenter(TestContext context)
        {
            //throw new NotImplementedException("Calling Instrumenter");

            //string path_to_experiment="C:\\Users\\Fernan\\Documents\\epa-i-o-experiments\\ListItr\\outputCream\\OriginalCode\\ListItr2.sln";
            //string path_to_sln=path_to_experiment+"\\ListItr2.sln";
            //string proj_to_analize="ListItr2";
            //string output_dir=path_to_experiment+"\\EPA-O";
            //string file2 = "ListItr";

            string path_to_experiment="C:\\Users\\Fernan\\Documents\\contractor-debug-log\\RegressionTests\\Tests";
            string path_to_sln=path_to_experiment+"\\Tests.sln";
            string proj_to_analize="Tests";
            string output_dir=path_to_experiment+"\\EPA-O";
            string file2 = " ";

            

            var args=new string[4];
            args[0]=path_to_sln;
            args[1]=proj_to_analize;
            args[2]=output_dir;
            args[3]=file2+".cs";

            var tmpDir = ".";
            using (var instrumenter = new Process())
            {
                instrumenter.StartInfo = new ProcessStartInfo
                {
                    FileName = @"..\..\..\Slicer\bin\Debug\PplSlicer.exe",
                    Arguments = string.Join(" ", args),
                    WorkingDirectory = tmpDir,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                
                instrumenter.Start();
                instrumenter.BeginErrorReadLine();
                instrumenter.BeginOutputReadLine();
                instrumenter.WaitForExit();
                if (instrumenter.ExitCode != 0)
                {
                    throw new Exception("Error instrumenting source code");
                }
            }
        }

        public bool HasDiff(string file)
        {
            //throw new NotImplementedException("Calling diff");

            string path_to_tests = @"C:\Users\Fernan\Documents\contractor-debug-log\RegressionTests\Tests\Tests\";
            string path_to_test_results = @"C:\Users\Fernan\Documents\contractor-debug-log\RegressionTests\Tests\EPA-O\Tests\";
            var args = new string[3];
            args[0] = "/c";
            args[1] = path_to_tests + file + ".cs";
            args[2] = path_to_test_results + file + ".cs";

            var tmpDir = ".";
            var output = new StringBuilder();
            using (var diff = new Process())
            {
                diff.StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Windows\System32\fc.exe",
                    Arguments = string.Join(" ", args),
                    WorkingDirectory = tmpDir,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                
                diff.OutputDataReceived += (sender, e) =>
                {
                    output.AppendLine(e.Data);
                };
                //diff.ErrorDataReceived += (sender, e) => { Logger.Log(LogLevel.Fatal, e.Data); };
                diff.Start();
                diff.BeginErrorReadLine();
                diff.BeginOutputReadLine();
                diff.WaitForExit();

                if (diff.ExitCode == 2)
                {
                    throw new Exception("Error executing diff");
                }
            }
            return output.ToString().Contains("FC: no differences encountered");
        }

        [TestMethod]
        public void MethodCallArrayElementAccess()
        {
            string file = "MethodCallElementAccessExpression";
            Debug.Assert(HasDiff(file), "Error instrumenting array element access with method call");
        }

        [TestMethod]
        public void MethodCallArrayElementAccess2()
        {
            string file = "MethodCallElementAccessExpression";
            Debug.Assert(HasDiff(file), "Error instrumenting array element access with method call");
        }

        
    }
}
