using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC.Slicer
{
    public class CodeAdapterRewriter : CSharpSyntaxRewriter
    {
        // to rewrite assert calls and Math.abs
        ///////public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node);
        //public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node);

        //public override SyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node);
        //seria assign= cond;
        public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            if (node.Right is ConditionalExpressionSyntax)
            {
                return SyntaxFactory.ParseStatement(node.ToString());
            }
            return node;
        }
        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            return node;
        }
        /*public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var result = new BlockSyntax();
            StatementSyntax[] new_statements;
            if (node.Right is ConditionalExpressionSyntax)
            {
                if (node.Left is IdentifierNameSyntax)
                {
                    new_statements = new StatementSyntax[1];
                    new_statements[0] = SyntaxFactory.ParseStatement("if(");
                }
                else
                {
                    new_statements = new StatementSyntax[2];
                    new_statements[0] = SyntaxFactory.ParseStatement("");
                    new_statements[1] = SyntaxFactory.ParseStatement("if(");
                }
                result.AddStatements(new_statements);
                return result;
            }
            new_statements = new StatementSyntax[1];
            new_statements[0] = SyntaxFactory.ParseStatement(node.ToString());
            result.AddStatements(new_statements);
            return node;
        }*/


        // to fix problem with default values 
        //public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node);
        //public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node);
        
        ////////public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node);
        //public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node);
        //public override SyntaxNode VisitConstructorInitializer(ConstructorInitializerSyntax node);

        // to fix problem with return-values queries
        ////////public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node);

    } 
}