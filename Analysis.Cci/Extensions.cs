using System.Collections.Generic;
using System.Linq;
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

        public static IEnumerable<INamespaceTypeDefinition> GetAnalyzableTypes(this IModule module)
        {
            var types = from t in module.GetAllTypes().OfType<INamespaceTypeDefinition>()
                where !(t.ContainingUnitNamespace is IRootUnitNamespace) &&
                      (t.IsClass || t.IsStruct) &&
                      !t.IsStatic &&
                      !t.IsEnum &&
                      !t.IsInterface
                select t;

            return types;
        }
    }
}