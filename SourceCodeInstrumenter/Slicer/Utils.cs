using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC.Slicer
{
    public class Utils
    {
        public static SyntaxKind[] ShortcircuitsBinaries = new SyntaxKind[] { 
            SyntaxKind.CoalesceExpression,
            SyntaxKind.LogicalOrExpression,
            SyntaxKind.LogicalAndExpression
        };

        public static string CleanupDotNodeLabel(string s)
        {
            return s.Replace("\"", "'");
        }
        
        public static bool IsNullable(ITypeSymbol typeSymbol)
        {
            return typeSymbol.Name.Equals("Nullable");
        }

        public static bool IsNullableOrReference(ITypeSymbol typeSymbol)
        {
            return typeSymbol.Name.Equals("Nullable") || typeSymbol.CustomIsReferenceType();
        }

        public static bool IsNullableOrReference(IFieldSymbol fieldSymbol)
        {
            return IsNullableOrReference(fieldSymbol.Type);
        }
        /*
        public static bool IsStaticTrace(TraceType traceType)
        {
            return traceType == TraceType.EnterStaticConstructor || traceType == TraceType.EnterStaticMethod;
        }

        public static bool IsMethod(TraceType traceType)
        {
            return traceType == TraceType.EnterMethod || traceType == TraceType.EnterStaticMethod;
        }

        public static bool IsEnterMethodOrConstructor(TraceType traceType)
        {
            return traceType == TraceType.EnterMethod || traceType == TraceType.EnterStaticMethod ||
                traceType == TraceType.EnterConstructor || traceType == TraceType.EnterStaticConstructor ||
                traceType == TraceType.BeforeConstructor;
        }*/
    }

    public static class IOperationExtensions
    {
        public static bool IsInSource(this IOperation operation)
        {
            if (operation.Kind == OperationKind.ObjectCreationExpression)
                return (((IObjectCreationExpression)operation).Constructor.Locations.All(x => x.IsInSource));
            if (operation.Kind == OperationKind.InvocationExpression)
                if (((IInvocationExpression)operation).TargetMethod.MethodKind == MethodKind.DelegateInvoke)
                    return false;
                else
                    return (((IInvocationExpression)operation).TargetMethod.Locations.All(x => x.IsInSource));
            if (operation.Kind == OperationKind.PropertyReferenceExpression)
                return (((IPropertyReferenceExpression)operation).Property.Locations.All(x => x.IsInSource));

            return true;
        }

        public static bool CustomIsReferenceType(this ITypeSymbol typeSymbol)
        {
            return (typeSymbol.IsReferenceType || typeSymbol.Kind == SymbolKind.TypeParameter) && !typeSymbol.Name.ToLower().Equals("string");
        }

        public static int GetParametersCount(this CSharpSyntaxNode node)
        {
            var parametersCount = 0;
            if (node is ConstructorDeclarationSyntax)
            {
                var constructor = node as ConstructorDeclarationSyntax;
                parametersCount = constructor.ParameterList.Parameters.Count;
            }
            if (node is MethodDeclarationSyntax)
            {
                MethodDeclarationSyntax method = node as MethodDeclarationSyntax;
                parametersCount = method.ParameterList.Parameters.Count;
            }
            return parametersCount;
        }
    }
}
