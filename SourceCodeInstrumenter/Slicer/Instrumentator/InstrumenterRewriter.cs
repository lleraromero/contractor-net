using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Semantics;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC.Slicer
{
    public class InstrumenterRewriter : CSharpSyntaxRewriter
    {
        SemanticModel Model { get; set; }
        private Dictionary<string, int> pathToIdDict;
        public int sourceFileId { get; set; }
        public ISet<string> alreadyVisitedClasses;
        //public string pathToOutputFiles = "C:\\Users\\Administrador\\Desktop\\";
        public string pathToOutputFiles = "C:\\temp\\compilation\\obj\\Debug\\Decl\\";
        public Dictionary<string, List<string>> methodExceptions = new Dictionary<string, List<string>>();

        /* Usados para instrumentar funciones con parametros ref */
        private ISet<string> methodRefParams = new HashSet<string>();
        private ISet<Tuple<string, TypeSyntax>> methodOutParams = new HashSet<Tuple<string,TypeSyntax>>();
        private int tmpVariableNumber = 0;
        private string classToRewrite;

        public InstrumenterRewriter(int fileId, SemanticModel model, Dictionary<string, int> pathToIdDict,string classToRewrite)
        {
            this.classToRewrite = classToRewrite;
            Model = model;
            sourceFileId = fileId;
            this.pathToIdDict = pathToIdDict;
            alreadyVisitedClasses = new HashSet<string>();
        }
        /*
        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            AccessorDeclarationSyntax newNode = (AccessorDeclarationSyntax)base.VisitAccessorDeclaration(node);
            BlockSyntax block = newNode.Body;
            if (block == null) return newNode;

            var stmtList = new SyntaxList<StatementSyntax>();
            stmtList = stmtList.Add(TraceSyntaxGenerator.EnterMethodStatement(node, sourceFileId));
            stmtList = stmtList.AddRange(block.Statements);
            stmtList = stmtList.Add(TraceSyntaxGenerator.ExitMethodStatement(node, sourceFileId));

            return node.WithBody(block.WithStatements(stmtList));
        }
        */
        /*
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax originalClassNode)
        {
            ClassDeclarationSyntax modifiedClassNode = (ClassDeclarationSyntax)base.VisitClassDeclaration(originalClassNode);
            INamedTypeSymbol classSymbol = Model.GetDeclaredSymbol(originalClassNode);
            // Fields
            /* Si un field se inicializa fuera del constructor, se deja solo la declaracion y luego
             * se inicializa en el constructor. */ /*
            SeparatedSyntaxList<StatementSyntax> instanceFieldsSyntaxlist = new SeparatedSyntaxList<StatementSyntax>();
            SeparatedSyntaxList<StatementSyntax> staticFieldsSyntaxlist = new SeparatedSyntaxList<StatementSyntax>();
            IEnumerable<ISymbol> fieldMembers = classSymbol.GetMembers().Where(x => x is IFieldSymbol);
            foreach (var member in fieldMembers)
            {
                IFieldSymbol field = (IFieldSymbol)member;
                var syntaxRef = field.DeclaringSyntaxReferences.FirstOrDefault();
                if (syntaxRef == null) continue;

                VariableDeclaratorSyntax originalField = (VariableDeclaratorSyntax)syntaxRef.SyntaxTree.GetRoot().FindNode(syntaxRef.Span);
                if (originalField.Initializer == null && (!field.IsStatic || !Utils.IsNullableOrReference(field))) continue;

                /* Puede suceder que exista una clase partial, que tiene dos constructores, por lo que
                 * si existe un field inicializado fuera de los constructores hay que iniciarlo en
                 * ambos, por eso es necesario utilizar el modelo semantico para acceder al pedazo de
                 * sintaxis original, que puede no estar en el archivo que estamos instrumentando. 
                 * No salteamos las inicializaciones de fields nulas porque esta puede ser utilizada en 
                 * el programa sin haber sido inicializada previamente. */ /*
                int tracedFileId = pathToIdDict[syntaxRef.SyntaxTree.FilePath];

                var traceInstr = TraceSyntaxGenerator.SimpleStatement(originalField, tracedFileId);
                if (field.IsStatic)
                {
                    staticFieldsSyntaxlist = staticFieldsSyntaxlist.Add(traceInstr);
                    if (!field.IsConst)
                    {
                        /* Las constantes pueden ser números, valores booleanos, cadenas o una referencia nula.
                         * Por lo tanto, para const solo dejamos la traza y no agregamos la inicializacion, ya
                         * que es stmt simple, que no ejecuta codigo no esperado. */ /*
                        if (originalField.Initializer != null){
                            var temp = sourceFileId;
                            sourceFileId = tracedFileId;
                            SyntaxNode newSN = Visit(originalField.Initializer.Value);
                            sourceFileId = temp;
                            staticFieldsSyntaxlist = staticFieldsSyntaxlist.Add(SyntaxFactory.ParseStatement(originalField.Identifier.ToString() + " = " + newSN.ToString() + ";"));
                        }
                        else
                            staticFieldsSyntaxlist = staticFieldsSyntaxlist.Add(SyntaxFactory.ParseStatement(originalField.Identifier.ToString() + " = null;"));
                    }
                }
                else
                {
                    instanceFieldsSyntaxlist = instanceFieldsSyntaxlist.Add(traceInstr);
                    var initializer = originalField.Initializer.Value as InitializerExpressionSyntax;
                    var modifiedOriginalInitializer = (EqualsValueClauseSyntax)Visit(originalField.Initializer);
                    string newFieldDeclaration;
                    if (initializer != null && initializer.IsKind(SyntaxKind.ArrayInitializerExpression))
                    {
                        var temp = sourceFileId;
                        sourceFileId = tracedFileId;
                        SyntaxNode sn = SyntaxFactory.ParseExpression("new " + ((VariableDeclarationSyntax)originalField.Parent).Type.ToString() + originalField.Initializer.Value.ToString());
                        SyntaxNode newSN = Visit(sn);
                        sourceFileId = temp;
                        newFieldDeclaration = originalField.Identifier.ToString() + " = " + newSN.ToString() + ";";
                    }
                    else
                    {
                        newFieldDeclaration = originalField.Identifier.ToString() + " = " + modifiedOriginalInitializer.Value.ToString() + ";";
                    }
                    instanceFieldsSyntaxlist = instanceFieldsSyntaxlist.Add(SyntaxFactory.ParseStatement(newFieldDeclaration));
                }
            }

            var methods = classSymbol.GetMembers().Where(x => x is IMethodSymbol).Select(x => (IMethodSymbol)x);
            bool staticConstructorMissing = HasStaticConstructor(methods);
            bool instanceConstructorMissing = HasInstanceConstructor(methods);

            // Modificacion de constructores
            /* Si existen, se les agrega la inicializacion de fields */ /*
            foreach (var member in modifiedClassNode.Members)
            {
                if (member is ConstructorDeclarationSyntax)
                {
                    var modifiedConstructor = (ConstructorDeclarationSyntax)member;
                    SyntaxList<StatementSyntax> stmtList = new SyntaxList<StatementSyntax>();
                    BlockSyntax constructorBlock = modifiedConstructor.Body;

                    if (modifiedConstructor.Modifiers.Select(x => x.ValueText).Contains(SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText))
                    {
                        staticConstructorMissing = false;
                        stmtList = stmtList.Add(constructorBlock.Statements[0]);
                        stmtList = stmtList.AddRange(staticFieldsSyntaxlist);
                        for (int i = 1; i < constructorBlock.Statements.Count; ++i)
                        {
                            stmtList = stmtList.Add(constructorBlock.Statements[i]);
                        }
                    }
                    else
                    {
                        instanceConstructorMissing = false;
                        stmtList = stmtList.Add(constructorBlock.Statements[0]);
                        stmtList = stmtList.AddRange(instanceFieldsSyntaxlist);
                        for (int i = 1; i < constructorBlock.Statements.Count; ++i)
                        {
                            stmtList = stmtList.Add(constructorBlock.Statements[i]);
                        }
                    }

                    constructorBlock = constructorBlock.WithStatements(stmtList);
                    modifiedClassNode = modifiedClassNode.ReplaceNode(modifiedConstructor, modifiedConstructor.WithBody(constructorBlock));
                }
            }

            // Agregado de constructores
            /* Solo se agregan constructores si es la primera clase partial (es decir, solo
             * agregamos constructor una vez en todas las partial de una clase) y si no existen
             * previamente */ /*
            if (InsideFirstOfPartials(originalClassNode, classSymbol))
            {
                /* No hay que agregar constructor de instancia si la clase es estatica */ /*
                if (instanceConstructorMissing && !classSymbol.IsStatic)
                {
                    SyntaxList<StatementSyntax> stmtList = new SyntaxList<StatementSyntax>();
                    ConstructorDeclarationSyntax newConstructor = SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier(originalClassNode.Identifier.ToString())).WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(), SyntaxKind.PublicKeyword, SyntaxFactory.TriviaList(
                                    SyntaxFactory.Space))));
                    stmtList = stmtList.Add(TraceSyntaxGenerator.EnterConstructorStatement(originalClassNode, sourceFileId));
                    stmtList = stmtList.AddRange(instanceFieldsSyntaxlist);
                    stmtList = stmtList.Add(TraceSyntaxGenerator.ExitConstructorStatement(originalClassNode, sourceFileId));
                    newConstructor = newConstructor.WithBody(SyntaxFactory.Block(stmtList)).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
                    modifiedClassNode = modifiedClassNode.WithMembers(modifiedClassNode.Members.Add(newConstructor));
                }
                if (staticConstructorMissing)
                {
                    SyntaxList<StatementSyntax> stmtList = new SyntaxList<StatementSyntax>();
                    ConstructorDeclarationSyntax newConstructor = SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier(originalClassNode.Identifier.ToString())).WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(), SyntaxKind.StaticKeyword, SyntaxFactory.TriviaList(
                                    SyntaxFactory.Space))));
                    stmtList = stmtList.Add(TraceSyntaxGenerator.EnterStaticConstructorStatement(originalClassNode, sourceFileId));
                    stmtList = stmtList.AddRange(staticFieldsSyntaxlist);
                    stmtList = stmtList.Add(TraceSyntaxGenerator.ExitStaticConstructorStatement(originalClassNode, sourceFileId));
                    newConstructor = newConstructor.WithBody(SyntaxFactory.Block(stmtList)).WithTrailingTrivia(SyntaxFactory.Whitespace("\n"));
                    modifiedClassNode = modifiedClassNode.WithMembers(modifiedClassNode.Members.Add(newConstructor));
                }

                if (!classSymbol.IsStatic)
                {
                    var boolType = SyntaxFactory.ParseTypeName("int").WithTrailingTrivia(SyntaxFactory.Space);
                    var equals = SyntaxFactory.EqualsValueClause(TraceSyntaxGenerator.BeforeConstructorExpression(originalClassNode, sourceFileId));
                    var declarator = SyntaxFactory.VariableDeclarator("___ignore_me___").WithInitializer(equals);
                    var list = new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(declarator);
                    var declaration = SyntaxFactory.VariableDeclaration(boolType).WithVariables(list);
                    var ignore_me_field = SyntaxFactory.FieldDeclaration(declaration);

                    modifiedClassNode = modifiedClassNode.WithMembers(modifiedClassNode.Members.Add(ignore_me_field));
                }
            }
            alreadyVisitedClasses.Add(classSymbol.ContainingNamespace + "." + classSymbol.Name);

            return modifiedClassNode;
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            ExtractRefParameters(node);

            var modifiedConstructor = (ConstructorDeclarationSyntax)base.VisitConstructorDeclaration(node);
            SyntaxList<StatementSyntax> stmtList = new SyntaxList<StatementSyntax>();
            if (modifiedConstructor.Modifiers.Select(x => x.ValueText).Contains(SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText))
            {
                stmtList = stmtList.Add(TraceSyntaxGenerator.EnterStaticConstructorStatement(node, sourceFileId));
                stmtList = stmtList.AddRange(modifiedConstructor.Body.Statements);
                stmtList = stmtList.Add(TraceSyntaxGenerator.ExitStaticConstructorStatement(node, sourceFileId));
            }
            else
            {
                stmtList = stmtList.Add(TraceSyntaxGenerator.EnterConstructorStatement(node, sourceFileId));
                stmtList = stmtList.AddRange(modifiedConstructor.Body.Statements);
                stmtList = stmtList.Add(TraceSyntaxGenerator.ExitConstructorStatement(node, sourceFileId));
            }
            BlockSyntax constructorBody = modifiedConstructor.Body;
            constructorBody = constructorBody.WithStatements(stmtList);

            CleanRefAndOutParameters();

            return modifiedConstructor.WithBody(constructorBody);
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (Utils.ShortcircuitsBinaries.Contains(node.Kind()))
            {
                var leftExpression = base.Visit(node.Left);
                var rightExpression = base.Visit(node.Right);
                return SyntaxFactory.BinaryExpression(node.Kind(),
                    (ExpressionSyntax)leftExpression,
                    TraceSyntaxGenerator.EnterExpression(rightExpression, Model, node, sourceFileId));
            }
            
            return base.VisitBinaryExpression(node);
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var modifiedDeclarator = (VariableDeclaratorSyntax)base.VisitVariableDeclarator(node);
            var fieldNode = (FieldDeclarationSyntax)node.Ancestors().SingleOrDefault(x => x is FieldDeclarationSyntax);
            if (fieldNode != null)
            {
                if (!fieldNode.Modifiers.Select(x => x.ValueText).Contains(SyntaxFactory.Token(SyntaxKind.ConstKeyword).ValueText))
                {
                    return modifiedDeclarator.WithInitializer(null);
                }
            }
            return modifiedDeclarator;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            ExtractRefParameters(node);

            MethodDeclarationSyntax newNode = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);
            BlockSyntax block = newNode.Body;
            if (block == null) return newNode;

            var stmtList = new SyntaxList<StatementSyntax>();
            if (node.Modifiers.Select(x => x.ValueText).Contains(SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText))
            {
                stmtList = stmtList.Add(TraceSyntaxGenerator.EnterStaticMethodStatement(node, sourceFileId));
                stmtList = stmtList.AddRange(block.Statements);
                stmtList = stmtList.Add(TraceSyntaxGenerator.ExitStaticMethodStatement(node, sourceFileId));
            }
            else
            {
                stmtList = stmtList.Add(TraceSyntaxGenerator.EnterMethodStatement(node, sourceFileId));
                stmtList = stmtList.AddRange(block.Statements);
                stmtList = stmtList.Add(TraceSyntaxGenerator.ExitMethodStatement(node, sourceFileId));
            }

            var modified = newNode.WithBody(block.WithStatements(stmtList));

            CleanRefAndOutParameters();

            return modified;
        }

        public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            var basenode = base.VisitOperatorDeclaration(node);
            var newNode = (OperatorDeclarationSyntax)base.VisitOperatorDeclaration(node);
            BlockSyntax block = newNode.Body;
            if (block == null) return newNode;

            var stmtList = new SyntaxList<StatementSyntax>();
            stmtList = stmtList.Add(TraceSyntaxGenerator.EnterStaticMethodStatement(node, sourceFileId));
            stmtList = stmtList.AddRange(block.Statements);
            stmtList = stmtList.Add(TraceSyntaxGenerator.ExitStaticMethodStatement(node, sourceFileId));
            var modified = node.WithBody(block.WithStatements(stmtList));
            return modified;
        }

        public override SyntaxNode VisitSwitchSection(SwitchSectionSyntax node)
        {
            SwitchSectionSyntax newSection = (SwitchSectionSyntax)base.VisitSwitchSection(node);

            var list = new SyntaxList<StatementSyntax>();
            SyntaxList<StatementSyntax> originalStmts = node.Statements;
            SyntaxList<StatementSyntax> alreadyInstrumentedStmts = newSection.Statements;

            var positionStatement = 0;
            foreach (var stmtOriginal in originalStmts)
            {
                var stmtAlreadyInstrumentedInternally = alreadyInstrumentedStmts[positionStatement++];

                if (!(stmtOriginal is BlockSyntax) && stmtAlreadyInstrumentedInternally is BlockSyntax)
                {
                    /* Si el stmt es un bloque, el visit retorna un bloque, si es otro stmt
                     * retorna tambien un bloque (por el agregado del Trace). */ /*
                    var block = (BlockSyntax)stmtAlreadyInstrumentedInternally;
                    list = list.AddRange(block.Statements); 
                }
                else
                {
                    list = list.Add(stmtAlreadyInstrumentedInternally);
                }
            }

            return newSection.WithStatements(list);
        }
        */
        public override SyntaxNode VisitBlock(BlockSyntax block)
        {
            SyntaxList<StatementSyntax> instrumentedStmts = new SyntaxList<StatementSyntax>();
            SyntaxList<StatementSyntax> originalStmts = block.Statements;
            
            foreach (var statementOriginal in originalStmts)
            {
                var statementInstrumented = (StatementSyntax)Visit(statementOriginal);
                
                //CHECK IF CONTAINS EXPRESSIONS WITH POSSIBLE RUNTIME ERRORS
                //**************************************************************************************************************************************************
                var arrayElemAcces= statementOriginal.DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.ElementAccessExpression));
                foreach (SyntaxNode stmt in arrayElemAcces)
                {
                    var arrayName = (stmt.DescendantNodes().Where(x => x.IsKind(SyntaxKind.IdentifierName))).ToList().ElementAt(0);
                    var argument = stmt.DescendantNodes().Where(x => x.IsKind(SyntaxKind.Argument));
                    SyntaxNode argumentExpr = null;
                    bool preInc = false;
                    bool pre = false;
                    foreach (SyntaxNode argumentNode in argument.ToList().ElementAt(0).DescendantNodes())
                    {
                        if (argumentNode.IsKind(SyntaxKind.PostIncrementExpression) || argumentNode.IsKind(SyntaxKind.PostDecrementExpression))
                        {
                            continue;
                        }
                        else if (argumentNode.IsKind(SyntaxKind.PreIncrementExpression) || argumentNode.IsKind(SyntaxKind.PreDecrementExpression))
                        {
                            pre = true;
                            if(argumentNode.IsKind(SyntaxKind.PreIncrementExpression))
                                preInc = true;
                            continue;
                        }
                        else
                        {
                            argumentExpr = argumentNode;
                            break;
                        }
                    }
                    
                    //check if argument<0 || argument>= arrayName.Length

                    SyntaxNode throwStmt = SyntaxFactory.ParseStatement("throw new IndexOutOfRangeException();");
                    var method = getMethodDeclaration(block);
                    throwStmt = rewriteThrow(method, throwStmt);


                    var checkStmt = SyntaxFactory.ParseStatement("if((" + argumentExpr.ToString() + ") < 0 || (" + argumentExpr.ToString() + ") >= " +
                        arrayName.ToString() + ".Length)" +  System.Environment.NewLine + throwStmt.ToString() + System.Environment.NewLine);
                    
                    if (pre && preInc)
                    {
                        checkStmt = SyntaxFactory.ParseStatement("if(" + argumentExpr.ToString() + "+1 < 0 || " + argumentExpr.ToString() + "+1 >= " +
                        arrayName.ToString() + ".Length){" +System.Environment.NewLine 
                        + throwStmt.ToString() + System.Environment.NewLine+"}");
                    }else if(pre){
                        checkStmt = SyntaxFactory.ParseStatement("if(" + argumentExpr.ToString() + "-1 < 0 || " + argumentExpr.ToString() + "-1 >= " +
                        arrayName.ToString() + ".Length){" + System.Environment.NewLine
                        + throwStmt.ToString() + System.Environment.NewLine + "}");
                    }

                    if (!statementOriginal.IsKind(SyntaxKind.ForStatement))
                    {

                        //var method = getMethodDeclaration(block);
                        var methodDecl = method as MethodDeclarationSyntax;
                        if(methodDecl==null){
                            var methodConst = method as ConstructorDeclarationSyntax;
                            addToDictionary(methodConst.Identifier.ToString(), "IndexOutOfRangeException");
                        }else{
                            addToDictionary(methodDecl.Identifier.ToString(), "IndexOutOfRangeException");
                        }
                        

                        instrumentedStmts = instrumentedStmts.Add(checkStmt);
                    }
                }

                //**************************************************************************************************************************************************
                var arrayCreation = statementOriginal.DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.ArrayCreationExpression));
                foreach (SyntaxNode stmt in arrayCreation)
                {
                    var argument = stmt.DescendantNodes().Where(x => x.IsKind(SyntaxKind.ArrayRankSpecifier)).ToList().ElementAt(0);
                    SyntaxNode argumentExpr = null;
                    bool preInc = false;
                    foreach (SyntaxNode argumentNode in argument.DescendantNodes())
                    {
                        if (argumentNode.IsKind(SyntaxKind.PostIncrementExpression) || argumentNode.IsKind(SyntaxKind.PostDecrementExpression))
                        {
                            continue;
                        }
                        else if (argumentNode.IsKind(SyntaxKind.PreIncrementExpression) || argumentNode.IsKind(SyntaxKind.PreDecrementExpression))
                        {
                            preInc = true;
                            continue;
                        }
                        else
                        {
                            argumentExpr = argumentNode;
                            break;
                        }
                    }

                    SyntaxNode throwStmt = SyntaxFactory.ParseStatement("throw new OverflowException();");
                    var method = getMethodDeclaration(block);
                    throwStmt = rewriteThrow(method, throwStmt);

                    var checkStmt = SyntaxFactory.ParseStatement("if(" + argumentExpr.ToString() + " < 0 )" + System.Environment.NewLine +
                         throwStmt.ToString() + System.Environment.NewLine);
                    if (preInc)
                    {
                        checkStmt = SyntaxFactory.ParseStatement("if(" + argumentExpr.ToString() + "-1 < 0 )" + System.Environment.NewLine +
                         throwStmt.ToString() + System.Environment.NewLine);
                    }

                    if (!statementOriginal.IsKind(SyntaxKind.ForStatement))
                    {
                        var methodDecl = method as MethodDeclarationSyntax;
                        if(methodDecl==null){
                            var methodConst = method as ConstructorDeclarationSyntax;
                            addToDictionary(methodConst.Identifier.ToString(), "OverflowException");
                        }else{
                            addToDictionary(methodDecl.Identifier.ToString(), "OverflowException");
                        }
                        

                        instrumentedStmts = instrumentedStmts.Add(checkStmt);
                    }
                }

                //**************************************************************************************************************************************************
                var division = statementOriginal.DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.DivideExpression));
                bool foundSlash = false;
                if (division.Count() != 0)
                {
                    var divisionNodes = division.ToList().ElementAt(0).DescendantNodesAndTokens();
                    foreach (SyntaxNodeOrToken stmt in divisionNodes)
                    {
                        if (!foundSlash && !stmt.IsKind(SyntaxKind.SlashToken))
                        {
                            continue;
                        }
                        else if (stmt.IsKind(SyntaxKind.SlashToken))
                        {
                            foundSlash = true;
                            continue;
                        }

                        SyntaxNode throwStmt = SyntaxFactory.ParseStatement("throw new DivideByZeroException();");
                        var method = getMethodDeclaration(block);
                        throwStmt = rewriteThrow(method, throwStmt);

                        var checkStmt = SyntaxFactory.ParseStatement("if(" + stmt.ToString() + " == 0 )" + System.Environment.NewLine 
                            + throwStmt.ToString() + System.Environment.NewLine);
                        if (!statementOriginal.IsKind(SyntaxKind.ForStatement))
                        {
                            //var method = getMethodDeclaration(block);
                            var methodDecl = method as MethodDeclarationSyntax;
                            if(methodDecl==null){
                                var methodConst = method as ConstructorDeclarationSyntax;
                                addToDictionary(methodConst.Identifier.ToString(), "DivideByZeroException");
                            }else{
                                addToDictionary(methodDecl.Identifier.ToString(), "DivideByZeroException");
                            }
                            

                            instrumentedStmts = instrumentedStmts.Add(checkStmt);
                            break;
                        }
                    }
                }
                //**************************************************************************************************************************************************
                var assignDivision = statementOriginal.DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.DivideAssignmentExpression));
                bool foundSlashEquals = false;
                if (assignDivision.Count() != 0)
                {
                    var divisionNodes = assignDivision.ToList().ElementAt(0).DescendantNodesAndTokens();
                    foreach (SyntaxNodeOrToken stmt in divisionNodes)
                    {
                        if (!foundSlashEquals && !stmt.IsKind(SyntaxKind.SlashEqualsToken))
                        {
                            continue;
                        }
                        else if (stmt.IsKind(SyntaxKind.SlashEqualsToken))
                        {
                            foundSlashEquals = true;
                            continue;
                        }

                        SyntaxNode throwStmt = SyntaxFactory.ParseStatement("throw new DivideByZeroException();");
                        var method = getMethodDeclaration(block);
                        throwStmt = rewriteThrow(method, throwStmt);

                        var checkStmt = SyntaxFactory.ParseStatement("if(" + stmt.ToString() + " == 0 )" + System.Environment.NewLine +
                             throwStmt.ToString() + System.Environment.NewLine);
                        if (!statementOriginal.IsKind(SyntaxKind.ForStatement))
                        {
                            //var method = getMethodDeclaration(block);
                            var methodDecl = method as MethodDeclarationSyntax;
                            if(methodDecl==null){
                                var methodConst = method as ConstructorDeclarationSyntax;
                                addToDictionary(methodConst.Identifier.ToString(), "DivideByZeroException");
                            }else{
                                addToDictionary(methodDecl.Identifier.ToString(), "DivideByZeroException");
                            }
                            instrumentedStmts = instrumentedStmts.Add(checkStmt);
                            break;
                        }
                    }
                }
                //**************************************************************************************************************************************************
                var modulo = statementOriginal.DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.ModuloExpression));
                bool foundPercent = false;
                if (modulo.Count() != 0)
                {
                    var divisionNodes = modulo.ToList().ElementAt(0).DescendantNodesAndTokens();
                    foreach (SyntaxNodeOrToken stmt in divisionNodes)
                    {
                        if (!foundPercent && !stmt.IsKind(SyntaxKind.PercentToken))
                        {
                            continue;
                        }
                        else if (stmt.IsKind(SyntaxKind.PercentToken))
                        {
                            foundPercent = true;
                            continue;
                        }

                        SyntaxNode throwStmt = SyntaxFactory.ParseStatement("throw new DivideByZeroException();");
                        var method = getMethodDeclaration(block);
                        throwStmt = rewriteThrow(method, throwStmt);

                        var checkStmt = SyntaxFactory.ParseStatement("if(" + stmt.ToString() + " == 0 )" + System.Environment.NewLine +
                             throwStmt.ToString()  + System.Environment.NewLine);
                        if (!statementOriginal.IsKind(SyntaxKind.ForStatement))
                        {
                            //var method = getMethodDeclaration(block);
                            var methodDecl = method as MethodDeclarationSyntax;
                            if(methodDecl==null){
                                var methodConst = method as ConstructorDeclarationSyntax;
                                addToDictionary(methodConst.Identifier.ToString(), "DivideByZeroException");
                            }else{
                                addToDictionary(methodDecl.Identifier.ToString(), "DivideByZeroException");
                            }
                            checkStmt=checkStmt.WithLeadingTrivia(statementOriginal.GetLeadingTrivia());
                            instrumentedStmts = instrumentedStmts.Add(checkStmt);
                            break;
                        }
                    }
                }
                //**************************************************************************************************************************************************
                var assignModulo = statementOriginal.DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.ModuloAssignmentExpression));
                bool foundPercentEquals = false;
                if (assignModulo.Count() != 0)
                {
                    var divisionNodes = assignModulo.ToList().ElementAt(0).DescendantNodesAndTokens();
                    foreach (SyntaxNodeOrToken stmt in divisionNodes)
                    {
                        if (!foundPercentEquals && !stmt.IsKind(SyntaxKind.PercentEqualsToken))
                        {
                            continue;
                        }
                        else if (stmt.IsKind(SyntaxKind.PercentEqualsToken))
                        {
                            foundPercentEquals = true;
                            continue;
                        }

                        SyntaxNode throwStmt = SyntaxFactory.ParseStatement("throw new DivideByZeroException();");
                        var method = getMethodDeclaration(block);
                        throwStmt = rewriteThrow(method, throwStmt);

                        var checkStmt = SyntaxFactory.ParseStatement("if(" + stmt.ToString() + " == 0 )" + System.Environment.NewLine + throwStmt.ToString()  
                            + System.Environment.NewLine);
                        if (!statementOriginal.IsKind(SyntaxKind.ForStatement))
                        {
                            //var method = getMethodDeclaration(block);
                            var methodDecl = method as MethodDeclarationSyntax;
                            if(methodDecl==null){
                                var methodConst = method as ConstructorDeclarationSyntax;
                                addToDictionary(methodConst.Identifier.ToString(), "DivideByZeroException");
                            }else{
                                addToDictionary(methodDecl.Identifier.ToString(), "DivideByZeroException");
                            }

                            instrumentedStmts = instrumentedStmts.Add(checkStmt);
                            break;
                        }
                    }
                }
                //**************************************************************************************************************************************************
                var memberAccess = statementOriginal.DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.SimpleMemberAccessExpression));

                if (memberAccess.Count() != 0)
                {
                    var memberAccessNodes = memberAccess.ToList().ElementAt(0).DescendantNodesAndTokens();
                    var ma =memberAccess.ToList().ElementAt(0) as MemberAccessExpressionSyntax;
                    var subject = ma.Expression;
                    var subjectSymbol = Model.GetSymbolInfo(subject).Symbol;
                    var isStatic = false;
                    if (subjectSymbol!=null)
                        isStatic = subjectSymbol.ContainingSymbol.IsStatic;
                    //var isStaticB =  ((IInvocationExpression)Model.GetOperation(memberAccess.First().Parent)).TargetMethod.IsStatic;
                    
                    foreach (SyntaxNodeOrToken stmt in memberAccessNodes)
                    {
                       
                        if (isStatic)
                        {
                            break;
                        }

                        SyntaxNode throwStmt = SyntaxFactory.ParseStatement("throw new NullReferenceException();");
                        var method = getMethodDeclaration(block);
                        throwStmt = rewriteThrow(method, throwStmt);

                        var checkStmt = SyntaxFactory.ParseStatement(SyntaxFactory.ElasticEndOfLine("if(" + stmt.ToString() + " == null )") +
                            System.Environment.NewLine + throwStmt.ToString() + System.Environment.NewLine);
                        

                        if (!statementOriginal.IsKind(SyntaxKind.ForStatement))
                        {

                            //var method = getMethodDeclaration(block);
                            
                            var methodDecl = method as MethodDeclarationSyntax;
                            if(methodDecl==null){
                                var methodConst = method as ConstructorDeclarationSyntax;
                                //addToDictionary(methodConst.Identifier.ToString(), "NullReferenceException");
                            }else{
                                //addToDictionary(methodDecl.Identifier.ToString(), "NullReferenceException");
                            }

                            checkStmt = checkStmt.WithLeadingTrivia(statementOriginal.GetLeadingTrivia());
                          
                            //checkStmt.DescendantTrivia(statementOriginal.GetLeadingTrivia());
                            //checkStmt = checkStmt.WithTrailingTrivia(statementOriginal.GetTrailingTrivia());
                            instrumentedStmts = instrumentedStmts.Add(checkStmt);
                            break;
                        }
                    }
                }


                //**************************************************************************************************************************************************
                //if (!(statementInstrumented is BlockSyntax))
                //    throw new Exception("EL resultado del visit de un stmt dentro de un bloque debe ser un bloque!");
                
                //var newBlock = statementInstrumented as BlockSyntax;
                //foreach (var stmt in newBlock.Statements)
                //{
                    //IList<StatementSyntax> beforeAssignments = new List<StatementSyntax>();
                    //IList<StatementSyntax> afterAssignments = new List<StatementSyntax>();

                    //var modifiedStmt = RemoveRefAndOutArguments(stmt, beforeAssignments, afterAssignments);

                    //instrumentedStmts = instrumentedStmts.AddRange(beforeAssignments);
                    //instrumentedStmts = instrumentedStmts.Add(stmt);
                    //instrumentedStmts = instrumentedStmts.AddRange(afterAssignments);
                //}
                instrumentedStmts = instrumentedStmts.Add(statementInstrumented);
            }

            block = block.WithOpenBraceToken(base.VisitToken(block.OpenBraceToken));
            block = block.WithCloseBraceToken(base.VisitToken(block.CloseBraceToken));

            return block.WithStatements(instrumentedStmts);
        }

        private SyntaxNode rewriteThrow(SyntaxNode method, SyntaxNode throwStmt)
        {
            return throwStmt;
            var methodDecl = method as MethodDeclarationSyntax;
            var descendant = throwStmt.DescendantNodesAndTokens();
            BlockSyntax newBlock = SyntaxFactory.Block();

            bool foundNew = false;
            foreach (SyntaxNodeOrToken stmt in descendant)
            {
                if (!foundNew && !stmt.IsKind(SyntaxKind.NewKeyword))
                {
                    continue;
                }
                else if (stmt.IsKind(SyntaxKind.NewKeyword))
                {
                    foundNew = true;
                    continue;
                }
                
                newBlock = newBlock.AddStatements(SyntaxFactory.ParseStatement("exitCode=" + '"' + stmt.ToString() + '"' + ";" + System.Environment.NewLine));
                //newBlock = newBlock.AddStatements(SyntaxFactory.ParseStatement("Contract.Assume(exitCode==" + '"' + stmt.ToString() + '"' + ");" + System.Environment.NewLine));
                if (methodDecl == null || methodDecl.ReturnType.ToString().Equals("void"))
                {
                    
                    //It is the Constructor method
                    newBlock = newBlock.AddStatements(SyntaxFactory.ParseStatement("return;" + System.Environment.NewLine));
                    /*if (methodDecl == null)
                    {
                        var m = (method as ConstructorDeclarationSyntax);
                        addToDictionary(m.Identifier.ToString(), stmt.ToString());
                    }
                    else
                    {
                        addToDictionary(methodDecl.Identifier.ToString(), stmt.ToString());
                    }*/
                    
                }else{
                    newBlock = newBlock.AddStatements(SyntaxFactory.ParseStatement("return default(" + methodDecl.ReturnType + ");" + System.Environment.NewLine));
                    //addToDictionary(methodDecl.Identifier.ToString(), stmt.ToString());
                }
                
                break;
            }

            //newBlock = newBlock.AddStatements(instrumentedThrowStmt);
            
            return newBlock;
            //throw new NotImplementedException();
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax originalClassNode)
        {
            //if (!originalClassNode.Identifier.ToString().Equals(classToRewrite))
            //   return originalClassNode;  

            ClassDeclarationSyntax modifiedClassNode = (ClassDeclarationSyntax)base.VisitClassDeclaration(originalClassNode);
            //CreateFieldDeclaration
            var field = SyntaxFactory.ParseSyntaxTree("private string exitCode, expectedExitCode;" + System.Environment.NewLine);
            var firstNode =field.GetRoot().DescendantNodes().ToList().ElementAt(0);
            FieldDeclarationSyntax fieldDeclaration = firstNode as FieldDeclarationSyntax;
            //var fieldDeclaration =SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration();
            //var fieldDeclaration = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration();
            //modifiedClassNode = modifiedClassNode.WithMembers(modifiedClassNode.Members.Insert(0,fieldDeclaration));
            //var fieldDeclaration = RoslynHelpers.CreateFieldDeclaration(RoslynHelpers.GetParameterType(parameter), fieldName);
            //foreach (SyntaxNode node in modifiedClassNode.DescendantNodes())
            //{

            //}
            ////"private string exitCode, expectedExitCode;"
            //modifiedClassNode.WithAttributeLists();
            return modifiedClassNode;
        }

        private void addToDictionary(string method, string exception)
        {
            List<string> list;
            if (methodExceptions.TryGetValue(method,out list))
            {
                list.Add(exception);
            }
            else
            {
                list = new List<string> ();
                list.Add(exception);

                methodExceptions.Add(method, list);
            }
        }

        private SyntaxNode getMethodDeclaration(BlockSyntax block)
        {
            if (block == null)
                return null;

            var parent = block.Parent;
            SyntaxNode method = null;

            bool flag = true;
            while (flag)
            {
                try
                {
                    method = (MethodDeclarationSyntax) parent;
                    flag = false;
                }
                catch (Exception e)
                {
                    parent = parent.Parent;
                }
            }

            if (method == null)
            {
                method = getConstructorDeclaration(block);
            }
            return method;
        }

        private ConstructorDeclarationSyntax getConstructorDeclaration(BlockSyntax block)
        {

            var parent = block.Parent;
            ConstructorDeclarationSyntax method = null;

            bool flag = true;
            while (flag)
            {
                try
                {
                    method = (ConstructorDeclarationSyntax)parent;
                    flag = false;
                }
                catch (Exception e)
                {
                    parent = parent.Parent;
                }
            }
            return method;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            string type, name;
            bool isPure;

            name = node.Identifier.ToString();
            type = node.ReturnType.ToString();
            isPure = node.AttributeLists.Where(x => x.ToString().Contains("Pure")).Count()!=0;
            //bool isProperty

            addToDictionary(name, type);
            addToDictionary(name, isPure+"");
            addToDictionary(name, false + "");

            MethodDeclarationSyntax newNode = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);
            BlockSyntax block = newNode.Body;
            if (block == null) return newNode;

            var stmtList = new SyntaxList<StatementSyntax>();

            
            bool contractStmtsFinished = false;
            foreach (StatementSyntax blockNode in block.Statements)
            {
                if (!contractStmtsFinished && blockNode.DescendantNodes().Where(x => x.ToString().Contains("Contract.Require") || x.ToString().Contains("Contract.Ensures") || x.ToString().Contains("Contract.Invariant")).Count() == 0)
                {
                    contractStmtsFinished = true;                    

                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("if(expectedExitCode==null){throw new Exception();};" + System.Environment.NewLine));

                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("expectedExitCode = " + '"' + "Ok" + '"' + ";" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("int expectedExitCode = 0;" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("Contract.Assume(expectedExitCode==" + '"' + "Ok" + '"' + ");" + System.Environment.NewLine));

                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("exitCode=" + '"' + "Ok" + '"' + ";" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("int exitCode = 0;" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("Contract.Assume(exitCode==" + '"' + "Ok" + '"' + ");" + System.Environment.NewLine));

                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("Contract.Assert(exitCode==expectedExitCode);" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("Contract.Ensures(exitCode.Equals(expectedExitCode));" + System.Environment.NewLine));

                    stmtList = stmtList.Add(blockNode);
                }else{
                    stmtList = stmtList.Add(blockNode);
                }
            }

            var modified = newNode.WithBody(block.WithStatements(stmtList));
            
            return modified;
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            string name = node.Identifier.ToString();
            string type = node.Identifier.ToString();
            string isPure = "False";

            addToDictionary(name, type);
            addToDictionary(name, isPure + "");
            addToDictionary(name, false + "");

            ConstructorDeclarationSyntax newNode = (ConstructorDeclarationSyntax)base.VisitConstructorDeclaration(node);
            BlockSyntax block = newNode.Body;
            if (block == null) return newNode;

            var stmtList = new SyntaxList<StatementSyntax>();


            bool contractStmtsFinished = false;
            foreach (StatementSyntax blockNode in block.Statements)
            {
                if (!contractStmtsFinished && blockNode.DescendantNodes().Where(x => x.ToString().Contains("Contract.Require") || x.ToString().Contains("Contract.Ensures") || x.ToString().Contains("Contract.Invariant")).Count() == 0)
                {
                    contractStmtsFinished = true;

                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("if(expectedExitCode==null){throw new Exception();};" + System.Environment.NewLine));

                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("int expectedExitCode = 0;" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("expectedExitCode = " + '"' + "Ok" + '"' + ";" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("Contract.Assume(expectedExitCode==" + '"' + "Ok" + '"' + ");" + System.Environment.NewLine));

                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("int exitCode = 0;" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("exitCode=" + '"' + "Ok" + '"' + ";" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("Contract.Assume(exitCode==" + '"' + "Ok" + '"' + ");" + System.Environment.NewLine));

                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("Contract.Assert(exitCode==expectedExitCode);" + System.Environment.NewLine));
                    //stmtList = stmtList.Add(SyntaxFactory.ParseStatement("Contract.Ensures(exitCode.Equals(expectedExitCode));" + System.Environment.NewLine));
                    
                    stmtList = stmtList.Add(blockNode);
                }
                else
                {
                    stmtList = stmtList.Add(blockNode);
                }
            }

            var modified = newNode.WithBody(block.WithStatements(stmtList));

            return modified;
        }

        public override SyntaxNode VisitThrowStatement(ThrowStatementSyntax node)
        {
            return node;
            var descendant = node.DescendantNodesAndTokens();
            BlockSyntax newBlock = SyntaxFactory.Block();

            bool foundNew = false;
            foreach (SyntaxNodeOrToken stmt in descendant)
            {
                if (!foundNew && !stmt.IsKind(SyntaxKind.NewKeyword))
                {
                    continue;
                }
                else if (stmt.IsKind(SyntaxKind.NewKeyword))
                {
                    foundNew = true;
                    continue;
                }
                var method = getMethodDeclaration(node.Parent as BlockSyntax);
                var methodDecl = method as MethodDeclarationSyntax;
                newBlock = newBlock.AddStatements(SyntaxFactory.ParseStatement("exitCode=" + '"' + stmt.ToString() + '"' + ";" + System.Environment.NewLine));
                //newBlock = newBlock.AddStatements(SyntaxFactory.ParseStatement("Contract.Assume(exitCode==" + '"' + stmt.ToString() + '"' + ");" + System.Environment.NewLine));
                if (methodDecl == null)
                {
                    var m = (method as ConstructorDeclarationSyntax);
                    addToDictionary(m.Identifier.ToString(), stmt.ToString());
                }
                else
                {
                    addToDictionary(methodDecl.Identifier.ToString(), stmt.ToString());
                }
                if (!methodDecl.ReturnType.ToString().Equals("void"))
                {
                    newBlock = newBlock.AddStatements(SyntaxFactory.ParseStatement("return default(" + methodDecl.ReturnType + ");" + System.Environment.NewLine));
                }
                else
                {
                    newBlock = newBlock.AddStatements(SyntaxFactory.ParseStatement("return;" + System.Environment.NewLine));
                }
                break;
            }

            //newBlock = newBlock.AddStatements(instrumentedThrowStmt);
            return newBlock;
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            string type, name;
            bool isPure;

            name = node.Identifier.ToString();
            type = node.Type.ToString();
            //type = node.ReturnType.ToString();
            isPure = node.AttributeLists.Where(x => x.ToString().Contains("Pure")).Count() != 0;


            addToDictionary(name, type);
            addToDictionary(name, isPure + "");
            addToDictionary(name, true + "");

            PropertyDeclarationSyntax newNode = (PropertyDeclarationSyntax) base.VisitPropertyDeclaration(node);
            return newNode;
        }

        /*
        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            IfStatementSyntax newNode = (IfStatementSyntax)base.VisitIfStatement(node);
            IfStatementSyntax stmtIfOriginal = (IfStatementSyntax)node;
            SyntaxNode modifiedCondition = Visit(stmtIfOriginal.Condition);
            IfStatementSyntax stmtIfInstrumented = newNode;

            var instrumentedScope = InstrumentScope((BlockSyntax)stmtIfInstrumented.Statement, stmtIfOriginal);
            stmtIfInstrumented = stmtIfInstrumented.WithStatement(instrumentedScope);
            ExpressionSyntax trace = TraceSyntaxGenerator.ConditionExpression(stmtIfOriginal, modifiedCondition, sourceFileId);
            stmtIfInstrumented = stmtIfInstrumented.WithCondition(trace);

            if (stmtIfOriginal.Else != null)
            {
                stmtIfInstrumented =
                    stmtIfInstrumented.WithElse(
                        stmtIfInstrumented.Else.WithStatement(
                            InstrumentScope((BlockSyntax)stmtIfInstrumented.Else.Statement, stmtIfOriginal)));
            }
            var block = SyntaxFactory.Block();
            block = block.AddStatements(stmtIfInstrumented);
            return block;
        }

        public override SyntaxNode VisitDoStatement(DoStatementSyntax node)
        {
            DoStatementSyntax newNode = (DoStatementSyntax)base.VisitDoStatement(node);
            DoStatementSyntax stmtDoOriginal = (DoStatementSyntax)node;
            DoStatementSyntax stmtDoInstrumented = (DoStatementSyntax)newNode;

            stmtDoInstrumented =
                stmtDoInstrumented.WithStatement(
                    InstrumentScope((BlockSyntax)stmtDoInstrumented.Statement, stmtDoOriginal)
                    );
            ExpressionSyntax trace = TraceSyntaxGenerator.ConditionExpression(stmtDoOriginal, stmtDoOriginal.Condition, sourceFileId);
            stmtDoInstrumented = stmtDoInstrumented.WithCondition(trace);

            return stmtDoInstrumented;
        }

        public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            WhileStatementSyntax newNode = (WhileStatementSyntax)base.VisitWhileStatement(node);
            WhileStatementSyntax stmtWhileOriginal = (WhileStatementSyntax)node;
            WhileStatementSyntax stmtWhileInstrumented = (WhileStatementSyntax)newNode;

            stmtWhileInstrumented =
                stmtWhileInstrumented.WithStatement(
                    InstrumentScope((BlockSyntax)stmtWhileInstrumented.Statement, stmtWhileOriginal));
            var stmtWhileInstrumentedCondition = Visit(stmtWhileOriginal.Condition);
            ExpressionSyntax trace = TraceSyntaxGenerator.ConditionExpression(stmtWhileOriginal, stmtWhileInstrumentedCondition, sourceFileId);
            stmtWhileInstrumented = stmtWhileInstrumented.WithCondition(trace);
            
            var block = SyntaxFactory.Block();
            block = block.AddStatements(stmtWhileInstrumented);
            return block;
        }

        public override SyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            ConditionalExpressionSyntax newCondition = (ConditionalExpressionSyntax)base.VisitConditionalExpression(node);
            var functionConditionalOperator = TraceSyntaxGenerator.ConditionalOperatorExpression(newCondition, node, Model, sourceFileId);
            return functionConditionalOperator;
        }

        public override SyntaxNode VisitForStatement(ForStatementSyntax node)
        {
            ForStatementSyntax stmtForOriginal = (ForStatementSyntax)node;
            ForStatementSyntax stmtForInstrumented = (ForStatementSyntax)base.VisitForStatement(node);

            var emptyList = new SeparatedSyntaxList<ExpressionSyntax>();
            var stmtForInstrumentedWithIncrementors = stmtForInstrumented.WithIncrementors(emptyList);

            BlockSyntax newBlock = SyntaxFactory.Block(null);

            var newList = new SeparatedSyntaxList<ExpressionSyntax>();
            var i = 0;
            foreach (var expr in stmtForOriginal.Incrementors)
            {
                var tracing = TraceSyntaxGenerator.SimpleExpression(expr, sourceFileId);
                newList = newList.Add(tracing);
                newList = newList.Add(stmtForInstrumented.Incrementors[i]);
                i++;
            }

            foreach (var expr in newList)
                newBlock = newBlock.AddStatements(SyntaxFactory.ExpressionStatement(expr));

            stmtForInstrumented = stmtForInstrumentedWithIncrementors.WithStatement(((BlockSyntax)stmtForInstrumented.Statement).AddStatements(newBlock));
            stmtForInstrumented =
                stmtForInstrumented.WithStatement(
                    InstrumentScope((BlockSyntax)stmtForInstrumented.Statement, stmtForOriginal)
                    );
            ExpressionSyntax trace = TraceSyntaxGenerator.ConditionExpression(stmtForOriginal, stmtForInstrumented.Condition, sourceFileId);
            stmtForInstrumented = stmtForInstrumented.WithCondition(trace);


            newList = new SeparatedSyntaxList<ExpressionSyntax>();
            i = 0;
            foreach (var expr in stmtForOriginal.Initializers)
            {
                var tracing = TraceSyntaxGenerator.SimpleExpression(expr, sourceFileId);
                newList = newList.Add(tracing);
                newList = newList.Add(stmtForInstrumented.Initializers[i]);
                i++;
            }

            stmtForInstrumented = stmtForInstrumented.WithInitializers(newList);

            BlockSyntax returnBlock = SyntaxFactory.Block();
            returnBlock = returnBlock.AddStatements(stmtForInstrumented);

            return returnBlock;
        }

        public override SyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
        {
            ForEachStatementSyntax newNode = (ForEachStatementSyntax)base.VisitForEachStatement(node);
            ForEachStatementSyntax stmtForEachOriginal = (ForEachStatementSyntax)node;
            ForEachStatementSyntax stmtForEachInstrumented = (ForEachStatementSyntax)newNode;

            stmtForEachInstrumented =
                stmtForEachInstrumented.WithStatement(
                    InstrumentScope((BlockSyntax)stmtForEachInstrumented.Statement, stmtForEachOriginal)
                );

            BlockSyntax newBlock = SyntaxFactory.Block();

            var stmt = TraceSyntaxGenerator.SimpleStatement(stmtForEachOriginal, sourceFileId);
            newBlock = newBlock.AddStatements(stmt);

            newBlock = newBlock.AddStatements(stmtForEachInstrumented);
            return newBlock;
        }

        public override SyntaxNode VisitSwitchStatement(SwitchStatementSyntax node)
        {
            SwitchStatementSyntax newNode = (SwitchStatementSyntax)base.VisitSwitchStatement(node);
            SwitchStatementSyntax stmtSwitchOriginal = (SwitchStatementSyntax)node;
            SwitchStatementSyntax stmtSwitchInstrumented = (SwitchStatementSyntax)newNode;

            SyntaxList<SwitchSectionSyntax> sections = new SyntaxList<SwitchSectionSyntax>();
            foreach (var section in stmtSwitchInstrumented.Sections)
            {
                SyntaxList<StatementSyntax> stmtList = new SyntaxList<StatementSyntax>();

                stmtList = stmtList.Add(TraceSyntaxGenerator.EnterConditionStatement(stmtSwitchOriginal, sourceFileId));
                stmtList = stmtList.AddRange(section.Statements);
                stmtList = stmtList.Add(TraceSyntaxGenerator.ExitConditionStatement(stmtSwitchOriginal, sourceFileId));

                SwitchSectionSyntax newSection = section.WithStatements(stmtList);

                sections = sections.Add(newSection);
            }

            stmtSwitchInstrumented = stmtSwitchInstrumented.WithSections(sections);

            BlockSyntax newBlock = SyntaxFactory.Block();

            var stmt = TraceSyntaxGenerator.SimpleStatement(stmtSwitchOriginal, sourceFileId);
            newBlock = newBlock.AddStatements(stmt);
            newBlock = newBlock.AddStatements(stmtSwitchInstrumented);
            return newBlock;
        }

        public override SyntaxNode VisitLockStatement(LockStatementSyntax node)
        {
            LockStatementSyntax newNode = (LockStatementSyntax)base.VisitLockStatement(node);
            LockStatementSyntax lockStmt = (LockStatementSyntax)node;
            BlockSyntax newBlock = SyntaxFactory.Block();
            newBlock = newBlock.AddStatements(TraceSyntaxGenerator.SimpleStatement(lockStmt.Expression, sourceFileId));
            newBlock = newBlock.AddStatements(newNode);
            return newBlock;
        }

        public override SyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
        {
            return InstrumentAndConvertToBlock(node, node);
        }

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            var modifiedExpression = (ExpressionStatementSyntax)base.VisitExpressionStatement(node);
            return InstrumentAndConvertToBlock(node, modifiedExpression);
        }

        public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
        {
            var modifiedReturn = (ReturnStatementSyntax)base.VisitReturnStatement(node);
            return InstrumentAndConvertToBlock(node, modifiedReturn);
        }

        public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            return node;
        }
        
        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var modifiedInvocation = (InvocationExpressionSyntax)base.VisitInvocationExpression(node);
            return TraceSyntaxGenerator.InvocationWrapperExpression(modifiedInvocation, node, sourceFileId);
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var modifiedObjCreation = (ObjectCreationExpressionSyntax)base.VisitObjectCreationExpression(node);
            return TraceSyntaxGenerator.InvocationWrapperExpression(modifiedObjCreation, node, sourceFileId);
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            var modifiedDeclaration = (LocalDeclarationStatementSyntax)base.VisitLocalDeclarationStatement(node);
            IList<VariableDeclaratorSyntax> modifiedVariables = new List<VariableDeclaratorSyntax>();
            foreach (var variable in modifiedDeclaration.Declaration.Variables)
            {
                if (variable.Initializer == null)
                {
                    var init = SyntaxFactory.EqualsValueClause(SyntaxFactory.DefaultExpression(node.Declaration.Type));
                    var modifiedVar = variable.WithInitializer(init);
                    modifiedVariables.Add(modifiedVar);
                }
                else
                {
                    modifiedVariables.Add(variable);
                }
            }
            var modifiedDecl = modifiedDeclaration.Declaration.WithVariables(SyntaxFactory.SeparatedList(modifiedVariables));
            modifiedDeclaration = modifiedDeclaration.WithDeclaration(modifiedDecl);
            return InstrumentAndConvertToBlock(node, modifiedDeclaration);
        }

        public override SyntaxNode VisitBreakStatement(BreakStatementSyntax node)
        {
            BlockSyntax newBlock = SyntaxFactory.Block();
            var stmt = TraceSyntaxGenerator.BreakStatement(node, sourceFileId);
            newBlock = newBlock.AddStatements(stmt);
            newBlock = newBlock.AddStatements(node);
            return newBlock;
        }

        public override SyntaxNode VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            return base.VisitCheckedExpression(node);
        }

        public override SyntaxNode VisitContinueStatement(ContinueStatementSyntax node)
        {
            return InstrumentAndConvertToBlock(node, node);
        }

        public override SyntaxNode VisitFixedStatement(FixedStatementSyntax node)
        {
            return base.VisitFixedStatement(node);
        }

        public override SyntaxNode VisitYieldStatement(YieldStatementSyntax node)
        {
            return base.VisitYieldStatement(node);
        }

        public override SyntaxNode VisitUsingStatement(UsingStatementSyntax node)
        {
            /* Si el contenido del Using es una expresion node.Expression tiene el nodo
             * y Declarion es null. En caso contrario, exactamente lo opuesto */ /*
            var expression = (ExpressionSyntax)base.Visit(node.Expression);
            var declaration = (VariableDeclarationSyntax)base.Visit(node.Declaration); 
            var bodyBlock = (BlockSyntax)base.Visit(node.Statement);

            bodyBlock = ((BlockSyntax)bodyBlock).AddStatements(new StatementSyntax[] {  
                    TraceSyntaxGenerator.ExitUsingStatement(node, sourceFileId)
                });

            var newUsing = SyntaxFactory.UsingStatement(declaration, expression, bodyBlock);

            var block = SyntaxFactory.Block(new StatementSyntax[] {  
                    TraceSyntaxGenerator.SimpleStatement(node, sourceFileId),
                    newUsing });
            return block;
        }

        public override SyntaxNode VisitTryStatement(TryStatementSyntax node)
        {
            var instrumentedTryStmt = (TryStatementSyntax)base.VisitTryStatement(node);
            BlockSyntax newBlock = SyntaxFactory.Block();
            newBlock = newBlock.AddStatements(instrumentedTryStmt);
            return newBlock;
        }

        public override SyntaxNode VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            return base.VisitUnsafeStatement(node);
        }

        public override SyntaxNode VisitGlobalStatement(GlobalStatementSyntax node)
        {
            return base.VisitGlobalStatement(node);
        }

        public override SyntaxNode VisitGotoStatement(GotoStatementSyntax node)
        {
            var instrumentedGotoStmt = (GotoStatementSyntax)base.VisitGotoStatement(node);
            BlockSyntax newBlock = SyntaxFactory.Block();
            newBlock = newBlock.AddStatements(instrumentedGotoStmt);
            return newBlock;
        }

        public override SyntaxNode VisitThrowStatement(ThrowStatementSyntax node)
        {
            var instrumentedThrowStmt = (ThrowStatementSyntax)base.VisitThrowStatement(node);
            BlockSyntax newBlock = SyntaxFactory.Block();
            newBlock = newBlock.AddStatements(instrumentedThrowStmt);
            return newBlock;
        }

        public override SyntaxNode VisitLabeledStatement(LabeledStatementSyntax node)
        {
            var instrumentedLabelStmt = (BlockSyntax)((LabeledStatementSyntax)base.VisitLabeledStatement(node)).Statement;
            var modifiedLabel = node.WithStatement(instrumentedLabelStmt.Statements[0]);

            BlockSyntax newBlock = SyntaxFactory.Block();
            newBlock = newBlock.AddStatements(modifiedLabel, instrumentedLabelStmt.Statements[1]);
            return newBlock;
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            try
            {
                var visitedExpression = base.Visit(node.Expression);
                var modifiedMemberAccess = SyntaxFactory.MemberAccessExpression(
                    node.Kind(), (ExpressionSyntax)visitedExpression, node.OperatorToken, node.Name);

                try
                {
                    // El 1ero es para saber si es SET
                    // El 2do es por si es una invocación
                    // El 3ero es por si es un tipo, no se puede devolver
                    if ((node.Parent is AssignmentExpressionSyntax && ((AssignmentExpressionSyntax)node.Parent).Left == node) ||
                        (node.Parent is InvocationExpressionSyntax) ||
                        (node.Parent is PostfixUnaryExpressionSyntax) ||
                        (node.Parent is AttributeArgumentSyntax) ||
                        (node.Parent is CaseSwitchLabelSyntax) ||
                        (node.Parent is ArgumentSyntax && ((ArgumentSyntax)node.Parent).RefOrOutKeyword.Value != null) ||
                        (node.Parent is MemberAccessExpressionSyntax // Structs
                            && (Model.GetTypeInfo(((MemberAccessExpressionSyntax)node.Parent).Expression).Type).TypeKind == TypeKind.Struct) ||
                        (Model.GetTypeInfo(node).Type != null && Model.GetTypeInfo(node).Type.TypeKind == TypeKind.Enum
                            && (Model.GetSymbolInfo(node.Expression).Symbol.Kind == SymbolKind.NamedType)) ||
                        (node.Expression.Kind() == SyntaxKind.AliasQualifiedName) ||
                        (Model.GetSymbolInfo(node).Symbol != null &&
                                (Model.GetSymbolInfo(node).Symbol.Kind == SymbolKind.Namespace ||
                                Model.GetSymbolInfo(node).Symbol.Kind == SymbolKind.Method ||
                                Model.GetSymbolInfo(node).Symbol.Kind == SymbolKind.NamedType)))
                    {
                        //var texto = node.GetText().ToString();
                        return modifiedMemberAccess;
                    }

                    //var texto = node.GetText().ToString();
                    return TraceSyntaxGenerator.MemberAccessWrapperExpression(modifiedMemberAccess, Model, node, sourceFileId);
                }
                catch (Exception ex)
                {
                    return modifiedMemberAccess;
                }
            }
            catch (Exception ex2)
            {
                throw;
            }
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            try
            {
                var modifiedIdentifierName = base.VisitIdentifierName(node);
                try
                {
                    if (!(node.Parent is AssignmentExpressionSyntax && ((AssignmentExpressionSyntax)node.Parent).Left == node) &&
                        !(node.Parent is PostfixUnaryExpressionSyntax) &&
                        !(node.Parent is AttributeArgumentSyntax) &&
                        !(node.Parent is CaseSwitchLabelSyntax) &&
                        !(node.Parent is ArgumentSyntax && ((ArgumentSyntax)node.Parent).RefOrOutKeyword.Value != null) && // REF OUT
                        // No hay enum, no podría pasar que se encapsule algo por error en este caso
                        !(node.Parent is MemberAccessExpressionSyntax // Structs
                            && (Model.GetTypeInfo(((MemberAccessExpressionSyntax)node.Parent).Expression).Type).TypeKind == TypeKind.Struct) &&
                        (Model.GetSymbolInfo(node).Symbol != null &&
                            (Model.GetSymbolInfo(node).Symbol.Kind == SymbolKind.Field ||
                            (Model.GetSymbolInfo(node).Symbol.Kind == SymbolKind.Property &&
                            !(node.Parent.Parent is AnonymousObjectMemberDeclaratorSyntax && // AnonymousTypes
                                node.Parent is NameEqualsSyntax && ((NameEqualsSyntax)node.Parent).Name == node)))))
                    {
                        //var texto = node.GetText().ToString();
                        return TraceSyntaxGenerator.MemberAccessWrapperExpression(modifiedIdentifierName, Model, node, sourceFileId);
                    }

                    //var texto = node.GetText().ToString();
                    return modifiedIdentifierName;
                }
                catch (Exception ex)
                {
                    return modifiedIdentifierName;
                }
            }
            catch (Exception ex2)
            {
                throw;
            }
        }

        public override SyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            // Reescribimos el i++ como: () => i++ para luego de eso enviar un get.
            // TODO: ES UNA IDEA, HAY QUE REVISARLA, primero saltaría el GET y luego si hay otro callback sería por el SET
            // Otra sería la reescritura i = i + 1 pero también hay que revisar

            var rec = base.VisitPostfixUnaryExpression(node);
            if (Model.GetSymbolInfo(node.Operand).Symbol != null && 
                            (Model.GetSymbolInfo(node.Operand).Symbol.Kind == SymbolKind.Property ||
                            Model.GetSymbolInfo(node.Operand).Symbol.Kind == SymbolKind.Field))
                return TraceSyntaxGenerator.MemberAccessWrapperExpression(rec, Model, node.Operand, sourceFileId);

            return rec;
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.Kind() == SyntaxKind.RegionDirectiveTrivia || trivia.Kind() == SyntaxKind.EndRegionDirectiveTrivia)
            {
                return new SyntaxTrivia();
            }
            return base.VisitTrivia(trivia);
        }

        private BlockSyntax InstrumentAndConvertToBlock(StatementSyntax originalNode, StatementSyntax modifiedNode)
        {
            BlockSyntax newBlock = SyntaxFactory.Block();
            var traceInstr = TraceSyntaxGenerator.SimpleStatement(originalNode, sourceFileId);
            newBlock = newBlock.AddStatements(traceInstr);
            newBlock = newBlock.AddStatements(modifiedNode);
            return newBlock;
        }

        private BlockSyntax InstrumentScope(BlockSyntax blockInstrumented, StatementSyntax controllingExpressionOriginal)
        {
            SyntaxList<StatementSyntax> scopeIncludedInternalStatements = new SyntaxList<StatementSyntax>();
            int ctrlExpSpanStart = controllingExpressionOriginal.Span.Start;
            int ctrlExpSpanEnd = controllingExpressionOriginal.Span.End;

            scopeIncludedInternalStatements = scopeIncludedInternalStatements.Add(TraceSyntaxGenerator.EnterConditionStatement(controllingExpressionOriginal, sourceFileId));
            scopeIncludedInternalStatements = scopeIncludedInternalStatements.AddRange(blockInstrumented.Statements);
            scopeIncludedInternalStatements = scopeIncludedInternalStatements.Add(TraceSyntaxGenerator.ExitConditionStatement(controllingExpressionOriginal, sourceFileId));

            return SyntaxFactory.Block(scopeIncludedInternalStatements);
        }

        private bool InsideFirstOfPartials(ClassDeclarationSyntax originalClassNode, INamedTypeSymbol classSymbol)
        {
            /* Este metodo retorna true si, en el caso de que la clase sea partial, estemos
             * trabajando sobre la primera en orden de aparicion en "Locations" del classSymbol. */ /*
            bool insideFirstOfPartials = true;
            if (alreadyVisitedClasses.Contains(classSymbol.ContainingNamespace + "." + classSymbol.Name)) insideFirstOfPartials = false;
            if (classSymbol.Locations.Count() > 1)
            {
                List<string> paths = classSymbol.Locations
                    .Select(x => x.SourceTree)
                    .OrderBy(s => s.FilePath)
                    .Select(p => p.FilePath).ToList();
                if (paths[0] != originalClassNode.SyntaxTree.FilePath) insideFirstOfPartials = false;
            }
            return insideFirstOfPartials;
        }

        private static bool HasInstanceConstructor(IEnumerable<IMethodSymbol> methods)
        {
            bool instanceConstructorMissing = true;
            foreach (var method in methods)
            {
                if (method.MethodKind == MethodKind.Constructor)
                {
                    if (method.Name.Contains(".ctor"))
                    {
                        if (method.DeclaringSyntaxReferences.Count() != 0)
                        {
                            instanceConstructorMissing = false;
                            break;
                        }
                    }
                    else
                    {
                        instanceConstructorMissing = false;
                        break;
                    }
                }
            }
            return instanceConstructorMissing;
        }

        private static bool HasStaticConstructor(IEnumerable<IMethodSymbol> methods)
        {
            bool staticConstructorMissing = true;
            foreach (var method in methods)
            {
                if (method.MethodKind == MethodKind.StaticConstructor)
                {
                    if (method.Name.Contains(".cctor"))
                    {
                        if (method.DeclaringSyntaxReferences.Count() != 0)
                        {
                            staticConstructorMissing = false;
                            break;
                        }
                    }
                    else
                    {
                        staticConstructorMissing = false;
                        break;
                    }
                }
            }
            return staticConstructorMissing;
        }

        private void ExtractRefParameters(MethodDeclarationSyntax node)
        {
            /* Extrae parametros "ref". Mirar RemoveRefArguments. */ /*
            foreach (var param in node.ParameterList.Parameters)
            {
                if (param.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
                {
                    methodRefParams.Add(param.Identifier.ToString());
                }
                if (param.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)))
                {
                    methodOutParams.Add(Tuple.Create(param.Identifier.ToString(), param.Type));
                }
            }
        }

        private void ExtractRefParameters(ConstructorDeclarationSyntax node)
        {
            /* Extrae parametros "ref". Mirar RemoveRefArguments. */ /*
            foreach (var param in node.ParameterList.Parameters)
            {
                if (param.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
                {
                    methodRefParams.Add(param.Identifier.ToString());
                }
                if (param.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)))
                {
                    methodOutParams.Add(Tuple.Create(param.Identifier.ToString(), param.Type));
                }
            }
        }

        private StatementSyntax RemoveRefAndOutArguments(StatementSyntax stmt, IList<StatementSyntax> beforeAssignments, IList<StatementSyntax> afterAssignments)
        {
            /* La idea es, en VisitMethod se ejecuta ExtractRefParemeters para saber cuales
             * parametros hay que reemplazar cuando sean pasados a otras funciones. Luego
             * se hace el Visit del bloque del metodo. En VisitBlock se llama a esta funcion que 
             * asigna previamente el parametro ref a una temporal, se lo pasa a la funcion y lo
             * vuelve a asignar luego de la ejecucion del lambda. Por ultimo, de nuevo en el 
             * VisitMethod ejecutamos CleanRefParameters para poder trabajar con otro metodo. */
            /* TODO: Funciona pero tiene copy/paste y hay que mejorarla. Ademas si la funcion tira
             * una excepcion la segunda asignacion (la que restaura el valor) no se va a hacer.
             * Habria que agregarle un try y finally con la asignacion "after". */
        /*

var modifiedStmt = stmt;
var invocations = stmt.DescendantNodes().Where(x => x is InvocationExpressionSyntax || x is ObjectCreationExpressionSyntax || x is MemberAccessExpressionSyntax).ToList();
foreach (var invocation in invocations)
{
var needToReplaceOut = ((CSharpSyntaxNode)invocation).DescendantNodes().Any(x => methodOutParams.Any(y => y.Item1.Equals(x.ToString())));
var needToReplacedRef = ((CSharpSyntaxNode)invocation).DescendantNodes().Any(x => methodRefParams.Contains(x.ToString()));
if (needToReplacedRef || needToReplaceOut)
{
if (invocation is InvocationExpressionSyntax)
{
var invocationSyntax = (InvocationExpressionSyntax)invocation;
SeparatedSyntaxList<ArgumentSyntax> modifiedArgs = new SeparatedSyntaxList<ArgumentSyntax>();
foreach (var arg in invocationSyntax.ArgumentList.Arguments)
{
var modifiedArg = arg;
if (arg.Expression is IdentifierNameSyntax)
{
var outParam = methodOutParams.SingleOrDefault(x => x.Item1.Equals(arg.Expression.ToString()));
var isOutParam = !(outParam == null);
var isRefParam = methodRefParams.Contains(arg.Expression.ToString());
if (isRefParam || isOutParam)
{
if (isOutParam)
{
beforeAssignments.Add(SyntaxFactory.ParseStatement(arg.Expression.ToString() + " = default(" + outParam.Item2.ToString() + ");"));
}
string tmpVar = "_tmp_" + ++tmpVariableNumber;
string beforeAssign = "var " + tmpVar + " = " + arg.Expression.ToString() + ";";
var beforeAssignmentStmt = SyntaxFactory.ParseStatement(beforeAssign);
beforeAssignments.Add(beforeAssignmentStmt);

var tmpVarExpr = SyntaxFactory.IdentifierName(tmpVar);
modifiedArg = arg.WithExpression(tmpVarExpr);

string afterAssign = arg.Expression.ToString() + " = " + tmpVar + ";";
var afterAssignmentStmt = SyntaxFactory.ParseStatement(afterAssign);
afterAssignments.Add(afterAssignmentStmt);
}
}
modifiedArgs = modifiedArgs.Add(modifiedArg);
}
var modifiedArgumentList = invocationSyntax.ArgumentList.WithArguments(modifiedArgs);
var modifiedInvocationSyntax = invocationSyntax.WithArgumentList(modifiedArgumentList);

modifiedStmt = stmt.ReplaceNode(invocationSyntax, modifiedInvocationSyntax);
}

// BRUTO COPY/PASTE: Esto es porque ObjectCreationExpression e InvocationExpression no heredan de algo a los
// que se le pueda pedir argumentos. TODO: Mejorar lo mas que se puede.
if (invocation is ObjectCreationExpressionSyntax)
{
var objCreationSyntax = (ObjectCreationExpressionSyntax)invocation;
SeparatedSyntaxList<ArgumentSyntax> modifiedArgs = new SeparatedSyntaxList<ArgumentSyntax>();
foreach (var arg in objCreationSyntax.ArgumentList.Arguments)
{
var modifiedArg = arg;
if (arg.Expression is IdentifierNameSyntax)
{
var outParam = methodOutParams.SingleOrDefault(x => x.Item1.Equals(arg.Expression.ToString()));
var isOutParam = !(outParam == null);
var isRefParam = methodRefParams.Contains(arg.Expression.ToString());
if (isRefParam || isOutParam)
{
if (isOutParam)
{
beforeAssignments.Add(SyntaxFactory.ParseStatement(arg.Expression.ToString() + " = default(" + outParam.Item2.ToString() + ");"));
}
string tmpVar = "_tmp_" + ++tmpVariableNumber;
string beforeAssign = "var " + tmpVar + " = " + arg.Expression.ToString() + ";";
var beforeAssignmentStmt = SyntaxFactory.ParseStatement(beforeAssign);
beforeAssignments.Add(beforeAssignmentStmt);

var tmpVarExpr = SyntaxFactory.IdentifierName(tmpVar);
modifiedArg = arg.WithExpression(tmpVarExpr);

string afterAssign = arg.Expression.ToString() + " = " + tmpVar + ";";
var afterAssignmentStmt = SyntaxFactory.ParseStatement(afterAssign);
afterAssignments.Add(afterAssignmentStmt);
}
}
modifiedArgs = modifiedArgs.Add(modifiedArg);
}
var modifiedArgumentList = objCreationSyntax.ArgumentList.WithArguments(modifiedArgs);
var modifiedInvocationSyntax = objCreationSyntax.WithArgumentList(modifiedArgumentList);

modifiedStmt = stmt.ReplaceNode(objCreationSyntax, modifiedInvocationSyntax);
}

if (invocation is MemberAccessExpressionSyntax)
{
var maSyntax = (MemberAccessExpressionSyntax)invocation;
if (maSyntax.Expression is IdentifierNameSyntax && methodRefParams.Contains(maSyntax.Expression.ToString()))
{
string tmpVar = "_tmp_" + ++tmpVariableNumber;
string beforeAssign = "var " + tmpVar + " = " + maSyntax.Expression.ToString() + ";";
var beforeAssignmentStmt = SyntaxFactory.ParseStatement(beforeAssign);
beforeAssignments.Add(beforeAssignmentStmt);

modifiedStmt = stmt.ReplaceNode(maSyntax, maSyntax.WithExpression(SyntaxFactory.IdentifierName(tmpVar)));
                            
string afterAssign = maSyntax.Expression.ToString() + " = " + tmpVar + ";";
var afterAssignmentStmt = SyntaxFactory.ParseStatement(afterAssign);
afterAssignments.Add(afterAssignmentStmt);
}
}
}
}
return modifiedStmt;
}

private void CleanRefAndOutParameters()
{
methodRefParams = new HashSet<string>();
methodOutParams = new HashSet<Tuple<string,TypeSyntax>>();
tmpVariableNumber = 0;
} */
    } 
}