using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorInstrumentator
{
    public class ExpectedExitCodeRewriter
    {
        public string expectedExitCode;

        public ExpectedExitCodeRewriter(string expectedExitCode)
            
        {
            this.expectedExitCode = expectedExitCode;
        }

        public Microsoft.Cci.IMethodDefinition Rewrite(Microsoft.Cci.IMethodDefinition method)
        {
            var sourceMethodBody = method.Body as SourceMethodBody;
            if (sourceMethodBody == null) return method;

            var newStatements = new List<IStatement>();
            var block = sourceMethodBody.Block as BlockStatement;
            //var existingStatements = new List<IStatement>(((BlockStatement)sourceMethodBody.Block.Statements.ElementAt(1)).Statements);
            var existingStatements = new List<IStatement>(sourceMethodBody.Block.Statements);
            bool foundAssignment= false;
            //bool foundAssume = false;

            CompileTimeConstant stringConstant=null;
            
            //var conditionalStatements = existingStatements.Where(s => s is ConditionalStatement);
            //var blockContainigThrow = conditionalStatements.Select(s=> ((BlockStatement)(s as ConditionalStatement).TrueBranch));
            //var throwStmt = (blockContainigThrow.ElementAt(0).Statements.ElementAt(0) as ThrowStatement);

            //var existingStatementsCopy = new List<IStatement>();
            //existingStatementsCopy.AddRange(existingStatements);
            //existingStatementsCopy.Remove(conditionalStatements.ElementAt(0));

            foreach (Statement statement in existingStatements)
            {
                //var statement = Rewrite(statement);
                if (!foundAssignment)
                {
                    //if (!foundAssignment)
                    //{
                        if(statement is LocalDeclarationStatement){
                            newStatements.Add(statement);
                            continue;
                        }
                        var expr = ((ExpressionStatement)statement).Expression;
                        //var methodCall = expr as MethodCall;
                        //var methodToCall =methodCall.MethodToCall;
                        var assignment = expr as IAssignment;
                        if (assignment == null)
                        {
                            // no es una asignacion asi que la dejo como esta
                            newStatements.Add(statement);
                        }
                        else if (assignment.Target.Definition.ToString().Contains("expectedExitCode"))
                        { // es la asignacion que quiero cambiar el valor
                            Assignment ass = new Assignment(assignment);
                            //CompileTimeConstant con = new CompileTimeConstant((ICompileTimeConstant)ass.Source);
                            //con.Value = this.expectedExitCode;
                            //ass.Source = con;
                            //assignment = ass;
                            
                            foundAssignment = true;

                            stringConstant = new CompileTimeConstant
                                    {
                                        Type = ass.Source.Type,
                                        Value = this.expectedExitCode
                                    };

                            var assignStmt = new ExpressionStatement
                            {
                                Expression = new Assignment
                                {
                                    Type = ass.Type,
                                    Target = ass.Target,
                                    Source = stringConstant
                                }
                            };
                            newStatements.Add(assignStmt);
                        }
                        else
                        { // es una asignacion pero no la que queria reescribir
                            //************************************
                            //fmartinelli: Si es una asignacion reescribir, esto es un parche para solucionar el bug de no soportar multiples salidas.
                            //var assign = Rewrite(assignment,throwStmt);
                            //**************************************
                            newStatements.Add(statement);
                        }
                    }
                    else
                    { // si ya encontre la asignacion dejo el resto como esta 
                        newStatements.Add(statement);
                    }                    
                //}
                //else
                //{ // ya encontre la asignacion y el assume asi que el resto va como estaba
                    
                    //***********************
                    //fmartinelli: Si hay una asignacion hay que reescribirla, esto es un parche para solucionar el bug de no soportar multiples salidas.
                    /*
                    if (statement is ExpressionStatement && ((ExpressionStatement)statement).Expression is IAssignment)
                    {
                        var assignment = ((ExpressionStatement)statement).Expression as IAssignment;
                        var assign = Rewrite(assignment, throwStmt);
                        newStatements.Add(assign);
                    }
                    else
                    */
                    //***********************
                    //newStatements.Add(Rewrite(statement, throwStmt));
                    
                //}
                
            }

                block.Statements = newStatements;
            return method;

        }
        /*
        public override Microsoft.Cci.IExpression Rewrite(Microsoft.Cci.IAssignment assignment)
        {
            var newAssignment = (Microsoft.Cci.IAssignment)base.Rewrite(assignment);
            if (newAssignment.Target.Definition.ToString().Contains("expectedExitCode"))
            {
                Assignment ass = new Assignment(newAssignment);
                CompileTimeConstant con = new CompileTimeConstant((ICompileTimeConstant)ass.Source);
                con.Value = this.expectedExitCode;
                ass.Source = con;
                newAssignment = ass;
            }
            return newAssignment;
        }
        */

        public Microsoft.Cci.IStatement Rewrite(Microsoft.Cci.IAssignment assignment, ThrowStatement throwStmt)
        {
            //var newAssignment = (Microsoft.Cci.IAssignment)base.Rewrite(assignment);
            if (assignment.Target.Definition.ToString().Contains("exitCode") && !assignment.Source.ToString().Equals("Ok") && this.expectedExitCode != null && !assignment.Source.ToString().Equals(this.expectedExitCode))
            {
                Assignment ass = new Assignment(assignment);
                CompileTimeConstant con = new CompileTimeConstant((ICompileTimeConstant)ass.Source);
                con.Value = this.expectedExitCode;
                ass.Source = con;
                //newAssignment = ass;

                //CSharpSourceEmitter
                /*
                TypeReference type = new NamespaceTypeReference(){
                    //Name = SystemException
                };

                MethodReference method = new MethodReference()

                CreateObjectInstance createObj = new CreateObjectInstance()
                {
                    MethodToCall = "System.Exception..ctor", 
                    Type = 
                };
                
                //return a throw new Exception();
                var throwStmt = new ThrowStatement()
                {
                    
                    //Exception= createObj
                };
                //var x=Activator.CreateInstance(System.Exception);
                */
                return throwStmt as Statement;
            }
            var res = new ExpressionStatement()
            {
                Expression = assignment
            };
            //var res = assignment as ExpressionStatement;
            return res;

        }


        public Microsoft.Cci.IStatement Rewrite(Microsoft.Cci.IStatement statement, ThrowStatement throwStmt)
        {
            if (statement is ExpressionStatement && ((ExpressionStatement)statement).Expression is IAssignment)
            {
                return Rewrite(((ExpressionStatement)statement).Expression as Assignment, throwStmt);
            }
            if (statement is ConditionalStatement)
            {
                var conditionalStatement = statement as ConditionalStatement;
                
                var newConditionalStatement = new ConditionalStatement(conditionalStatement);
                if (conditionalStatement.TrueBranch is BlockStatement){
                    var block = ((BlockStatement)conditionalStatement.TrueBranch);
                    var newStatements = new List<IStatement> ();
                    foreach (IStatement substatement in block.Statements)
                    {
                        newStatements.Add(Rewrite(substatement,throwStmt));
                    }
                    BlockStatement st = new BlockStatement() { 
                        Locations= block.Locations,
                        UseCheckedArithmetic = block.UseCheckedArithmetic,
                        Statements = newStatements
                    };
                    //block.Statements=newStatements;
                    newConditionalStatement.TrueBranch = st;
                }
                if (conditionalStatement.FalseBranch is BlockStatement)
                {
                    var block = ((BlockStatement)conditionalStatement.FalseBranch);
                    var newStatements = new List<IStatement>();
                    foreach (IStatement substatement in block.Statements)
                    {
                        newStatements.Add(Rewrite(substatement, throwStmt));
                    }
                    BlockStatement st = new BlockStatement()
                    {
                        Locations = block.Locations,
                        UseCheckedArithmetic = block.UseCheckedArithmetic,
                        Statements = newStatements
                    };
                    //block.Statements = newStatements;
                    newConditionalStatement.FalseBranch = st;
                }
                return newConditionalStatement;     
            }
            return statement;
        }
    }
}
