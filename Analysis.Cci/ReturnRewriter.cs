using System.Diagnostics.Contracts;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Analysis.Cci
{
    internal class ReturnRewriter : CodeRewriter
    {
        protected readonly ILabeledStatement target;
        protected readonly ILocalDeclarationStatement local;
        //public string expectedExitCode;
        protected TargetExpression targetExpression;

        public ReturnRewriter(IMetadataHost host, ILabeledStatement target, ILocalDeclarationStatement local)
            : base(host)
        {
            Contract.Requires(host != null && target != null);
            this.target = target;
            this.local = local;
            if(local!=null)
                this.targetExpression = new TargetExpression { Definition = local.LocalVariable, Instance = null, Type = local.LocalVariable.Type };
        }
        
        /*
         * public override Microsoft.Cci.IExpression Rewrite(Microsoft.Cci.IAssignment assignment)
        {
            var newAssignment = (Microsoft.Cci.IAssignment)base.Rewrite(assignment);
            if (newAssignment.Target.Definition.ToString().Contains("expectedExitCode") && this.expectedExitCode!=null)
            {
                Assignment ass = new Assignment(newAssignment);
                CompileTimeConstant con = new CompileTimeConstant((ICompileTimeConstant)ass.Source);
                con.Value = this.expectedExitCode;
                ass.Source = con;
                newAssignment = ass;
            }
            return newAssignment;
        }
         * */
                
        /// <summary>
        ///     Rewrites the return statement.
        /// </summary>
        /// <param name="returnStatement"></param>
        public override IStatement Rewrite(IReturnStatement returnStatement)
        {
            Contract.Ensures(Contract.Result<IStatement>() != null);

            var blockStmt = new BlockStatement();

            // If local is null it means that the type of the method that contains this return is 'void'
            if (local != null)
            {
                blockStmt.Statements.Add(new ExpressionStatement
                {
                    Expression = new Assignment
                    {
                        Target = targetExpression,
                        Source = Rewrite(returnStatement.Expression),
                        Type = returnStatement.Expression.Type
                    }
                });
            }

            blockStmt.Statements.Add(new GotoStatement { TargetStatement = target });

            return blockStmt;
        }
    }
}