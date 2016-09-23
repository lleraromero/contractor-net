using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;  //POR AHORA POR EL MAIN QUE HAY QUE SACAR
using Microsoft.CodeAnalysis.MSBuild;
using System.Threading;  //POR AHORA POR EL MAIN QUE HAY QUE SACAR

// CON ESTO PUEDO AGREGAR UN USING.... ME LO GUARDO POR LAS DUDAS
//private SyntaxTree AddUsingTracer(SyntaxTree tree)
//{
//    var compUnit = (CompilationUnitSyntax)tree.GetRoot();
//    NameSyntax name = SyntaxFactory.IdentifierName(" TracerStub");
//    UsingDirectiveSyntax usingTracer = SyntaxFactory.UsingDirective(name);
//    var compUnitModified = compUnit.WithUsings(compUnit.Usings.Add(usingTracer));
//    return compUnitModified.SyntaxTree;
//}

namespace Ppl.Pragma.Slicing
{
    class SourceIntrumenter
    {
        public CSharpCompilation Instrument(CSharpCompilation compilation)
        {
            prepareFile(); //BORRAR!! SOLO PARA PROBAR!

            CancellationToken cancellationToken;
            var entryPoint = compilation.GetEntryPoint(cancellationToken);

            Console.WriteLine(entryPoint.ContainingType.ToString());

            var entryPointSyntaxReference = entryPoint.DeclaringSyntaxReferences.First();
            var entryPointSyntaxTree = entryPointSyntaxReference.SyntaxTree;
            var entryPointMethod = (MethodDeclarationSyntax)entryPointSyntaxReference.GetSyntax();

            var stmtList = new SyntaxList<StatementSyntax>();
            var globalIdStmt = SyntaxFactory.ParseStatement("public static int globalStatementId = 0;");

            //I wish to use this but i cant, because this method inserts at the end of the BlockSyntax
            //var newMethod = node.AddBodyStatements(addedStmt);
            stmtList = stmtList.Add(globalIdStmt);
            foreach (var stmt in entryPointMethod.Body.Statements)
            {
                stmtList = stmtList.Add(stmt);
            }

            var modifiedBody = entryPointMethod.Body.WithStatements(stmtList);

            var modifiedEntryPointSyntaxTree = entryPointSyntaxTree.GetRoot().ReplaceNode(entryPointMethod.Body, modifiedBody);

            var modifiedCompilation = compilation.ReplaceSyntaxTree(entryPointSyntaxTree, modifiedEntryPointSyntaxTree.SyntaxTree);

            writeFile(modifiedCompilation); //BORRAR!! SOLO PARA PROBAR!

            return modifiedCompilation;

            //foreach (var tree in modifiedComp.SyntaxTrees)
            //{
            //    Console.WriteLine(tree.ToString());
            //}
            

            //foreach (var tree in compilation.SyntaxTrees)
            //{
            //    var newTree = Instrument((CSharpSyntaxTree)tree);
            //}
        }

        private void prepareFile()
        {
            File.Delete(@"C:\Users\julian\Desktop\Pragma_Dynamic_Output-Based_Slicing\trunk\Ejemplos\Sumatoria\Sumatoria\Program.cs");
            File.Copy(@"C:\Users\julian\Desktop\Pragma_Dynamic_Output-Based_Slicing\trunk\Ejemplos\Sumatoria\Sumatoria\Program.ori.cs", @"C:\Users\julian\Desktop\Pragma_Dynamic_Output-Based_Slicing\trunk\Ejemplos\Sumatoria\Sumatoria\Program.cs");
        }

        private void writeFile(CSharpCompilation comp)
        {
            File.WriteAllText(@"C:\Users\julian\Desktop\Pragma_Dynamic_Output-Based_Slicing\trunk\Ejemplos\Sumatoria\Sumatoria\Program.cs", comp.SyntaxTrees.First().GetRoot().SyntaxTree.ToString());
        }

        public CSharpSyntaxTree Instrument(CSharpSyntaxTree tree)
        {
            var rewriter = new InstrumenterRewriter();
            CSharpSyntaxNode newRoot = (CSharpSyntaxNode)rewriter.Visit(tree.GetRoot());
            var newTree = (CSharpSyntaxTree)newRoot.SyntaxTree;
            return newTree;
        }

        public class InstrumenterRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitBlock(BlockSyntax block)
            {
                BlockSyntax instrumentedBlock;
                instrumentedBlock = (BlockSyntax)base.VisitBlock(block);
                var stmtList = new SyntaxList<StatementSyntax>();
                
                int positionStmt = 0;
                foreach (var originalStmt in block.Statements)
                {
                    var stmtAlreadyInstrumentedInternally = instrumentedBlock.Statements[positionStmt++];

                    StatementSyntax addedStmt;

                    int id = loadStmtInfo(originalStmt);

                    if (originalStmt is ForStatementSyntax ||
                        originalStmt is WhileStatementSyntax ||
                        originalStmt is IfStatementSyntax)
                    {
                        addedStmt = SyntaxFactory.ParseStatement(
                                        string.Format(
                                            @"System.Console.WriteLine(@""Constorl {0}"");",
                                            id));
                    }
                    else
                    {
                        addedStmt = SyntaxFactory.ParseStatement(
                                        string.Format(
                                            @"System.Console.WriteLine(@""Datos {0}"");",
                                            id));
                    }

                    // New block must go before the original statement.
                    stmtList = stmtList.Add(addedStmt);
                    stmtList = stmtList.Add(stmtAlreadyInstrumentedInternally);
                }

                return instrumentedBlock.WithStatements(stmtList);
            }

            private int loadStmtInfo(StatementSyntax originalStmt)
            {
                return 1;
            }
        }
    }
}
