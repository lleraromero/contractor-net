﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Analyzer.Corral
{
    public class BoggieHardcoderForExceptionSupport
    {
        private List<string> errorList;
        private string full_path_to_boogie_file;
        public BoggieHardcoderForExceptionSupport() {
            errorList = new List<string>();
            string[] lines = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\Analyzer.Corral\Exceptions.txt");
            foreach (string line in lines)
            {
                if(!line.Equals("") && !line.Contains("//"))
                    errorList.Add(line);
            }
        }

        public BoggieHardcoderForExceptionSupport(List<string> errorList)
        {
            this.errorList = errorList;
        }

        public void hardcodeExceptionsToFile(string file){
            if (file.Equals(this.full_path_to_boogie_file)) return;
            lock (file)
            {
                this.full_path_to_boogie_file = file;
                hardcodeExceptionEqualsToNullToQueries();
                hardcodeSubtypeAxioms();
                SolveConstUniqueProblem();
            }
        }
        
        public void SolveConstUniqueProblem(){
            var input = File.ReadAllText(this.full_path_to_boogie_file);
            string pattern = "const unique System.Collections.Generic.IArraySortHelper: Ref;";
            string replacement = "//const unique System.Collections.Generic.IArraySortHelper: Ref;";
            Regex rgx = new Regex(Regex.Escape(pattern));
            string result = rgx.Replace(input, replacement, rgx.Matches(input).Count - 1);
            File.WriteAllText(this.full_path_to_boogie_file, result);    
        }
        

        private void hardcodeSubtypeAxioms()
        {
            StringBuilder stringBuilder = new StringBuilder();
            //hardcodeExceptionNotSubtypeOf(stringBuilder);
            hardcodeAllNotSubtypeOf(stringBuilder);
            harcodeAllSubtypeOfException(stringBuilder);
            harcodeSubtypeOfThemSelves(stringBuilder);
            writeAxiomsToFile(stringBuilder,this.full_path_to_boogie_file);
        }

        private void writeAxiomsToFile(StringBuilder stringBuilder, string p)
        {

            var input = File.ReadAllText(this.full_path_to_boogie_file);
            string pattern = "function $Subtype(Type, Type) : bool;";
            string replacement = pattern+System.Environment.NewLine+stringBuilder.ToString();
            Regex rgx = new Regex(Regex.Escape(pattern));
            string result = rgx.Replace(input, replacement);
            File.WriteAllText(this.full_path_to_boogie_file, result);
        }

        private void harcodeSubtypeOfThemSelves(StringBuilder stringBuilder)
        {
            foreach (var exception in errorList)
            {
                if (!exception.Equals("Ok"))
                {
                    stringBuilder.AppendLine("axiom $Subtype(T$" + exception + "(), T$" + exception + "());");
                }
            }
            //stringBuilder.AppendLine("axiom $Subtype(T$System.DivideByZeroException(), T$System.DivideByZeroException());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.Exception(), T$System.Exception());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.NullReferenceException(), T$System.NullReferenceException());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.IndexOutOfRangeException(), T$System.IndexOutOfRangeException());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.OverflowException(), T$System.OverflowException());");

            //stringBuilder.AppendLine("axiom $Subtype(T$System.IllegalStateException(), T$System.IllegalStateException());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.ConcurrentModificationException(), T$System.ConcurrentModificationException());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.NoSuchElementException(), T$System.NoSuchElementException());");
        }

        private void harcodeAllSubtypeOfException(StringBuilder stringBuilder)
        {
            /*
            foreach (var exception in errorList)
            {
                if (!exception.Equals("Ok") && !exception.Equals("System.Exception"))
                {
                    stringBuilder.AppendLine("axiom $Subtype(T$" + exception + "(), T$System.Exception());");
                }
            }
            */
            //stringBuilder.AppendLine("axiom $Subtype(T$System.DivideByZeroException(), T$System.Exception());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.NullReferenceException(), T$System.Exception());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.IndexOutOfRangeException(), T$System.Exception());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.OverflowException(), T$System.Exception());");

            //stringBuilder.AppendLine("axiom $Subtype(T$System.IllegalStateException(), T$System.Exception());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.ConcurrentModificationException(), T$System.Exception());");
            //stringBuilder.AppendLine("axiom $Subtype(T$System.NoSuchElementException(), T$System.Exception());");

            //All declared as extern functions
            foreach (var exception in errorList)
            {
                if (!exception.Equals("Ok"))
                {
                    stringBuilder.AppendLine("function {:extern} T$" + exception + "() : Ref;");
                    stringBuilder.AppendLine("const {:extern} unique T$" + exception + ": int;");
                }
            }
            //stringBuilder.AppendLine("function {:extern} T$System.NullReferenceException() : Ref;");
            //stringBuilder.AppendLine("const {:extern} unique T$System.NullReferenceException: int;");

            //stringBuilder.AppendLine("function {:extern} T$System.IndexOutOfRangeException() : Ref;");
            //stringBuilder.AppendLine("const {:extern} unique T$System.IndexOutOfRangeException: int;");
            
            //stringBuilder.AppendLine("function {:extern} T$System.DivideByZeroException() : Ref;");
            //stringBuilder.AppendLine("const {:extern} unique T$System.DivideByZeroException: int;");
            
            //stringBuilder.AppendLine("function {:extern} T$System.OverflowException() : Ref;");
            //stringBuilder.AppendLine("const {:extern} unique T$System.OverflowException: int;");
            
            //stringBuilder.AppendLine("function {:extern} T$System.Exception() : Ref;");
            //stringBuilder.AppendLine("const {:extern} unique T$System.Exception: int;");

            
            //stringBuilder.AppendLine("function {:extern} T$System.IllegalStateException() : Ref;");
            //stringBuilder.AppendLine("const {:extern} unique T$System.IllegalStateException: int;");

            //stringBuilder.AppendLine("function {:extern} T$System.ConcurrentModificationException() : Ref;");
            //stringBuilder.AppendLine("const {:extern} unique T$System.ConcurrentModificationException: int;");

            //stringBuilder.AppendLine("function {:extern} T$System.NoSuchElementException() : Ref;");
            //stringBuilder.AppendLine("const {:extern} unique T$System.NoSuchElementException: int;");
        }

        private void hardcodeExceptionNotSubtypeOf(StringBuilder stringBuilder)
        {
            foreach (var exception in errorList)
            {
                if (!exception.Equals("Ok") && !exception.Equals("System.Exception"))
                {
                    stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$"+exception+"());");
                }
            }
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.DivideByZeroException());");
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.NullReferenceException());");
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.OverflowException());");
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.IndexOutOfRangeException());");

            //stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.IllegalStateException());");
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.ConcurrentModificationException());");
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.NoSuchElementException());");
        }
        private void hardcodeAllNotSubtypeOf(StringBuilder stringBuilder)
        {
            foreach (var exception in errorList)
            {
                if (!exception.Equals("Ok"))
                {
                    foreach (var exception2 in errorList)
                    {
                        if (!exception2.Equals("Ok") && !exception.Equals(exception2))
                        {
                            stringBuilder.AppendLine("axiom !$Subtype(T$" + exception + "(), T$" + exception2 + "());");
                        }
                    }
                }
            }
            //relaciones entre todas las excepciones.
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.DivideByZeroException(), T$System.NullReferenceException());");
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.NullReferenceException(), T$System.DivideByZeroException());");

            //stringBuilder.AppendLine("axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.NullReferenceException());");
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.NullReferenceException(), T$System.IndexOutOfRangeException());");

            //stringBuilder.AppendLine("axiom !$Subtype(T$System.OverflowException(), T$System.NullReferenceException());");
            //stringBuilder.AppendLine("axiom !$Subtype(T$System.NullReferenceException(), T$System.OverflowException());");
        }
        private void hardcodeExceptionEqualsToNullToQueries()
        {
            string lineToAdd = "$Exception := null;";
            var input = File.ReadAllText(this.full_path_to_boogie_file);

                string query_pattern = @"(^implementation(.*)STATE(.*)\n\{\r\n(\s\svar(.*)\n)*)";//"(implementation(.*)STATE(.)*\n{\n(.*var.*;\n)*)";
                string replacement = "$1\n"+lineToAdd+"\n";
                Regex rgx = new Regex(query_pattern,RegexOptions.Multiline);
                string result = rgx.Replace(input, replacement);
                File.WriteAllText(this.full_path_to_boogie_file, result);
        }
    }
}
