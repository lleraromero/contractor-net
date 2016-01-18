using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.Cci;

namespace Analysis.Cci
{
    public static class Extensions
    {
        [Pure]
        public static string GetUniqueName(this INamedTypeDefinition type)
        {
            var name = new StringBuilder();

            if (type is INamespaceTypeDefinition)
            {
                var rootType = (INamespaceTypeDefinition) type;
                name.Append(rootType.ContainingNamespace);
            }
            else if (type is INestedTypeDefinition)
            {
                var nestedType = (INestedTypeDefinition) type;
                var containingType = nestedType.ContainingTypeDefinition as INamedTypeDefinition;
                name.Append(containingType.GetUniqueName());
            }

            name.Append('.');
            name.Append(type.Name.Value);

            if (type.IsGeneric)
            {
                name.Append(type.GenericParameterCount);
            }

            return name.ToString();
        }

        public static string GetUniqueName(this IMethodDefinition method)
        {
            return MemberHelper.GetMethodSignature(method, NameFormattingOptions.DocumentationId);
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