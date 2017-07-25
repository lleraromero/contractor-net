using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.RegularExpressions;

namespace Console.Tests
{
    /// <summary>
    /// Tests_Stack_mscorlib contains tests for EPA generation with Stack.cs from mscorlib.dll 
    /// </summary>
    [TestClass]
    public class Tests_Stack_mscorlib
    {
        [TestMethod]
        public void Test_EPA_Stack_mscorlib()
        {
            string outputDir = @"C:\Users\Fernan\Documents\epa-i-o-results\Stack-mscorlib\StackMscorlib\StackMscorlib";
            string[] args = @"-i C:\Users\Fernan\Documents\epa-i-o-results\Stack-mscorlib\StackMscorlib\StackMscorlib\obj\Debug\Decl\StackMscorlib.dll -g C:\Users\Fernan\Documents\epa-i-o-results\Stack-mscorlib\StackMscorlib\StackMscorlib\EPA -tmp C:\Users\Fernan\Desktop\temp --il true -t System.Collections2.Stack -b Corral --xml".Split(' ') ;
            int r = Contractor.Console.Program.Main(args);
            Contract.Assert(r == 0);
            //xml exists
            Contract.Assert(File.Exists(outputDir + "\\Stack.xml"));
            //png exists
            Contract.Assert(File.Exists(outputDir + "\\Stack.png"));

            int number_of_transitions = Utils.number_of_transitions(outputDir + "\\Stack.xml");
            Contract.Assert(number_of_transitions == 30);
        }

        [TestMethod]
        public void Test_EPA_O_Exc_Stack_mscorlib()
        {
            string outputDir = @"C:\Users\Fernan\Documents\epa-i-o-results\Stack-mscorlib\StackMscorlib\StackMscorlib\EPA-O-Exc";
            string[] args = { };
            int r = Contractor.Console.Program.Main(args);
            Contract.Assert(r == 0);
            //xml exists
            Contract.Assert(File.Exists(outputDir + "\\Stack.xml"));
            //png exists
            Contract.Assert(File.Exists(outputDir + "\\Stack.png"));

            int number_of_transitions = Utils.number_of_transitions(outputDir + "\\Stack.xml");
            Contract.Assert(number_of_transitions == 30);
        }

        [TestMethod]
        public void Test_EPA_O_Customized_Stack_mscorlib()
        {
            string outputDir = @"C:\Users\Fernan\Documents\epa-i-o-results\Stack-mscorlib\StackMscorlib\StackMscorlib\EPA-O-Customized";
            string[] args = { };
            int r = Contractor.Console.Program.Main(args);
            Contract.Assert(r == 0);
            //xml exists
            Contract.Assert(File.Exists(outputDir + "\\Stack.xml"));
            //png exists
            Contract.Assert(File.Exists(outputDir + "\\Stack.png"));

            int number_of_transitions = Utils.number_of_transitions(outputDir + "\\Stack.xml");
            Contract.Assert(number_of_transitions==30);
        }

        [TestMethod]
        public void Test_EPA_O_All_Stack_mscorlib()
        {
            string outputDir = @"C:\Users\Fernan\Documents\epa-i-o-results\Stack-mscorlib\StackMscorlib\StackMscorlib\EPA-O-All";
            string[] args = { };
            int r = Contractor.Console.Program.Main(args);
            Contract.Assert(r == 0);
            //xml exists
            Contract.Assert(File.Exists(outputDir + "\\Stack.xml"));
            //png exists
            Contract.Assert(File.Exists(outputDir + "\\Stack.png"));

            int number_of_transitions = Utils.number_of_transitions(outputDir + "\\Stack.xml");
            Contract.Assert(number_of_transitions == 30);
        }
    }
    public class Utils
    {
        public static int number_of_transitions(string xml_file)
        {
            //Count the number of occurrences of the word "transition"
            return (new Regex(@"(?i)\btransition\b")).Matches(File.ReadAllText(xml_file)).Count;
        }
    }
}
