using System.Text.RegularExpressions;
using Contractor.Core;
using Microsoft.Cci;

namespace Analyzer.Corral
{
    /// <summary>
    ///     BCT-TranslationHelper.cs
    /// </summary>
    internal class BctTranslator
    {
        public static string CreateUniqueMethodName(Query query)
        {
            return CreateUniqueMethodName(query.Action.Method);
        }

        public static string CreateUniqueMethodName(IMethodReference method)
        {
            var containingTypeName = TypeHelper.GetTypeName(method.ContainingType, NameFormattingOptions.None);
            var s = MemberHelper.GetMethodSignature(method, NameFormattingOptions.DocumentationId);
            s = s.Substring(2);
            s = s.TrimEnd(')');
            s = TurnStringIntoValidIdentifier(s);
            return s;
        }

        protected static string TurnStringIntoValidIdentifier(string s)
        {
            // Do this specially just to make the resulting string a little bit more readable.
            // REVIEW: Just let the main replacement take care of it?
            s = s.Replace("[0:,0:]", "2DArray"); // TODO: Do this programmatically to handle arbitrary arity
            s = s.Replace("[0:,0:,0:]", "3DArray");
            s = s.Replace("[0:,0:,0:,0:]", "4DArray");
            s = s.Replace("[0:,0:,0:,0:,0:]", "5DArray");
            s = s.Replace("[]", "array");

            // The definition of a Boogie identifier is from BoogiePL.atg.
            // Just negate that to get which characters should be replaced with a dollar sign.

            // letter = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".
            // digit = "0123456789".
            // special = "'~#$^_.?`".
            // nondigit = letter + special.
            // ident =  [ '\\' ] nondigit {nondigit | digit}.

            s = Regex.Replace(s, "[^A-Za-z0-9'~#$^_.?`]", "$");

            s = GetRidOfSurrogateCharacters(s);
            return s;
        }

        /// <summary>
        ///     Unicode surrogates cannot be handled by Boogie.
        ///     http://msdn.microsoft.com/en-us/library/dd374069(v=VS.85).aspx
        /// </summary>
        protected static string GetRidOfSurrogateCharacters(string s)
        {
            //  TODO this is not enough! Actually Boogie cannot support UTF8
            var cs = s.ToCharArray();
            var okayChars = new char[cs.Length];
            for (int i = 0, j = 0; i < cs.Length; i++)
            {
                if (char.IsSurrogate(cs[i])) continue;
                okayChars[j++] = cs[i];
            }
            var raw = string.Concat(okayChars);
            return raw.Trim('\0');
        }
    }
}