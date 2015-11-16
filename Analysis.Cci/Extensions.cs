using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.Cci;

namespace Contractor.Utils
{
    public static class Extensions
    {
        [Pure]
        public static string GetUniqueName(this INamedTypeDefinition type)
        {
            var name = new StringBuilder();

            if (type is INamespaceTypeDefinition)
            {
                var rootType = type as INamespaceTypeDefinition;
                name.Append(rootType.ContainingNamespace);
            }
            else if (type is INestedTypeDefinition)
            {
                var nestedType = type as INestedTypeDefinition;
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
            var name = new StringBuilder(method.Name.Value.Replace(".", ""));

            if (method.IsGeneric)
            {
                name.Append(method.GenericParameterCount);
            }

            foreach (var parameter in method.Parameters)
            {
                var parameterTypeName = parameter.Type.ToString().Replace(".", string.Empty);

                if (parameter.IsOut)
                    name.Append("Out");
                else if (parameter.IsByReference)
                    name.Append("Ref");

                name.Append(parameterTypeName);
            }

            var returnTypeName = method.Type.ToString().Replace(".", string.Empty);
            name.Append(returnTypeName);

            return name.ToString();
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