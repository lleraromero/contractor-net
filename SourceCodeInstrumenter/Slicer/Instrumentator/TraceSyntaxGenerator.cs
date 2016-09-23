using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC.Slicer
{
    static class TraceSyntaxGenerator
    {
        public static StatementSyntax SimpleStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceSimpleStatement({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static ExpressionSyntax SimpleExpression(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceSimpleStatement({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseExpression(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static ExpressionSyntax ConditionExpression(SyntaxNode originalNode, SyntaxNode condition, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceSimpleStatement({0},{1},{2}) && ({3}) ;", sourceFileId, originalNode.Span.Start, originalNode.Span.End, condition.ToString());
            return SyntaxFactory.ParseExpression(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax EnterConditionStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceEnterCondition({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax ExitConditionStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceExitCondition({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax BreakStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceBreak({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax ExitUsingStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceExitUsing({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax EnterMethodStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceEnterMethod({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax ExitMethodStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceExitMethod({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static ExpressionSyntax BeforeConstructorExpression(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceBeforeConstructor({0},{1},{2})", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseExpression(traceStr);
        }

        public static StatementSyntax EnterConstructorStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceEnterConstructor({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax ExitConstructorStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceExitConstructor({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax EnterStaticMethodStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceEnterStaticMethod({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax ExitStaticMethodStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceExitStaticMethod({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax EnterStaticConstructorStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceEnterStaticConstructor({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static StatementSyntax ExitStaticConstructorStatement(SyntaxNode originalNode, int sourceFileId)
        {
            var traceStr = string.Format("Tracer.TraceSender.TraceExitStaticConstructor({0},{1},{2});", sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseStatement(traceStr).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
        }

        public static ExpressionSyntax ConditionalOperatorExpression(SyntaxNode rewritedNode, SyntaxNode originalNode, SemanticModel model, int sourceFileId)
        {
            var originalCondition = ((ConditionalExpressionSyntax)originalNode).Condition;
            var originalWhenTrue = ((ConditionalExpressionSyntax)originalNode).WhenTrue;
            var originalWhenFalse = ((ConditionalExpressionSyntax)originalNode).WhenFalse;

            var conditional = (ConditionalExpressionSyntax)rewritedNode;
            var whenTrueExp = (ExpressionSyntax)conditional.WhenTrue;
            var whenFalseExp = (ExpressionSyntax)conditional.WhenFalse;
            var type = model.GetTypeInfo(originalWhenTrue).Type;
            if (model.GetTypeInfo(originalWhenTrue).Type == null)
                type = model.GetTypeInfo(originalWhenFalse).Type;            
            else
                type = model.GetTypeInfo(originalWhenTrue).Type;
            var traceStr = string.Format("Tracer.TraceSender.TraceConditionalOperator<{0}>({1},() => {2},{3},{4}, () => {5},{6},{7},() => {8},{9},{10})", type.ToString(), sourceFileId, conditional.Condition, originalCondition.Span.Start, originalCondition.Span.End, conditional.WhenTrue, originalWhenTrue.Span.Start, originalWhenTrue.Span.End, conditional.WhenFalse, originalWhenFalse.Span.Start, originalWhenFalse.Span.End);
            return SyntaxFactory.ParseExpression(traceStr);
        }

        public static ExpressionSyntax InvocationWrapperExpression(SyntaxNode modifiedNode, SyntaxNode originalNode, int sourceFileId)
        {
            var lambda = "() => " + modifiedNode.ToString();
            var traceStr = string.Format("Tracer.TraceSender.TraceInvocationWrapper({0},{1},{2},{3})", lambda, sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseExpression(traceStr);
        }

        public static ExpressionSyntax MemberAccessWrapperExpression(SyntaxNode modifiedNode, SemanticModel model, SyntaxNode originalNode, int sourceFileId)
        {
            var lambda = "() => " + modifiedNode.ToString();
            //var type = model.GetTypeInfo(originalNode).Type;
            //var traceStr = string.Format("Tracer.TraceSender.TraceMemberAccessWrapper<{4}>({0},{1},{2},{3})", lambda, sourceFileId, originalNode.Span.Start, originalNode.Span.End, type.ToString());
            var traceStr = string.Format("Tracer.TraceSender.TraceMemberAccessWrapper({0},{1},{2},{3})", lambda, sourceFileId, originalNode.Span.Start, originalNode.Span.End);
            return SyntaxFactory.ParseExpression(traceStr);
        }

        public static ExpressionSyntax EnterExpression(SyntaxNode modifiedNode, SemanticModel model, SyntaxNode originalNode, int sourceFileId)
        {
            var lambda = "() => " + modifiedNode.ToString();
            var type = model.GetTypeInfo(originalNode).Type;
            var traceStr = string.Format("Tracer.TraceSender.TraceEnterExpression<{4}>({0},{1},{2},{3})", 
                lambda, sourceFileId, originalNode.Span.Start, originalNode.Span.End, type.ToString());
            return SyntaxFactory.ParseExpression(traceStr);
        }
    }
}