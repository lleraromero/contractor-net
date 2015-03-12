using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using System.Diagnostics.Contracts;

namespace Contractor.Utils
{
	public static class Extensions
	{
		public static string GetDisplayName(this INamedTypeDefinition type)
		{
			var name = new StringBuilder(type.Name.Value);

			if (type.IsGeneric)
			{
				var genericParameters = string.Join(",", type.GenericParameters);
				name.Append('<');
				name.Append(genericParameters);
				name.Append('>');
			}

			return name.ToString();
		}

		public static string GetDisplayName(this IMethodDefinition method)
		{
			var name = new StringBuilder();

            if (method.IsConstructor)
            {
                name.Append(TypeHelper.GetTypeName(method.ContainingTypeDefinition, NameFormattingOptions.OmitContainingNamespace));
            }
            else
            {
                name.Append(method.Name.Value);
            }

			if (method.IsGeneric)
			{
				var genericParameters = string.Join(",", method.GenericParameters);
				name.Append('<');
				name.Append(genericParameters);
				name.Append('>');
			}

            bool hasOverloads = method.ContainingTypeDefinition.Methods.Where(m => m.Name.Value == method.Name.Value).Count() > 1;
            if (method.ParameterCount > 0 && hasOverloads)
            {
                var parametersTypes = method.Parameters.Select(p => p.Type);
                var parameters = string.Join(",", parametersTypes);

                name.Append('(');
                name.Append(parameters);
                name.Append(')');
            }
			return name.ToString();
		}

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

		public static IEnumerable<IMethodDefinition> GetPublicInstanceMethods(this INamedTypeDefinition type)
		{
			var methods = from m in type.Methods
						  where m.Visibility == TypeMemberVisibility.Public &&
								!m.IsStatic &&
								!m.IsStaticConstructor
						  select m;

			return methods;
		}
	}
}
