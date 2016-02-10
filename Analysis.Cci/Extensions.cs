using Microsoft.Cci;

namespace Analysis.Cci
{
    public static class Extensions
    {
        public static string GetUniqueName(this IMethodDefinition method)
        {
            var uniqueName = MemberHelper.GetMethodSignature(method,
                NameFormattingOptions.DocumentationId & ~NameFormattingOptions.DocumentationIdMemberKind);
            uniqueName = uniqueName.Replace("(", "")
                .Replace(")", "")
                .Replace(".", "_")
                .Replace(",", "")
                .Replace("#", "");
            return uniqueName;
        }
    }
}