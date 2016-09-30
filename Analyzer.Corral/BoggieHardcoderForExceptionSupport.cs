using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Analyzer.Corral
{
    class BoggieHardcoderForExceptionSupport
    {
        private string full_path_to_boogie_file;
        public BoggieHardcoderForExceptionSupport() { }

        public void hardcodeExceptionsToFile(string file){
            this.full_path_to_boogie_file=file;
            hardcodeExceptionEqualsToNullToQueries();
            hardcodeSubtypeAxioms();
        }

        private void hardcodeSubtypeAxioms()
        {
            StringBuilder stringBuilder = new StringBuilder();
            hardcodeExceptionNotSubtypeOf(stringBuilder);
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
            stringBuilder.AppendLine("axiom $Subtype(T$System.DivideByZeroException(), T$System.DivideByZeroException());");
            stringBuilder.AppendLine("axiom $Subtype(T$System.Exception(), T$System.Exception());");
            stringBuilder.AppendLine("axiom $Subtype(T$System.NullReferenceException(), T$System.NullReferenceException());");
            stringBuilder.AppendLine("axiom $Subtype(T$System.IndexOutOfRangeException(), T$System.IndexOutOfRangeException());");
            stringBuilder.AppendLine("axiom $Subtype(T$System.OverflowException(), T$System.OverflowException());");

        }

        private void harcodeAllSubtypeOfException(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("axiom $Subtype(T$System.DivideByZeroException(), T$System.Exception());");
            stringBuilder.AppendLine("axiom $Subtype(T$System.NullReferenceException(), T$System.Exception());");
            stringBuilder.AppendLine("axiom $Subtype(T$System.IndexOutOfRangeException(), T$System.Exception());");
            stringBuilder.AppendLine("axiom $Subtype(T$System.OverflowException(), T$System.Exception());");
        }

        private void hardcodeExceptionNotSubtypeOf(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.DivideByZeroException());");
            stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.NullReferenceException());");
            stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.OverflowException());");
            stringBuilder.AppendLine("axiom !$Subtype(T$System.Exception(), T$System.IndexOutOfRangeException());");
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
