using System.Diagnostics.Contracts;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Analyzer.Corral
{
    internal class ReturnRewriter : CodeRewriter
    {
        protected readonly ILabeledStatement target;
        protected readonly ILocalDeclarationStatement local;

        public ReturnRewriter(IMetadataHost host, ILabeledStatement target, ILocalDeclarationStatement local)
            : base(host)
        {
            Contract.Requires(host != null && target != null);
            this.target = target;
            this.local = local;
        }

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
                        Target = new TargetExpression { Definition = local.LocalVariable, Instance = null, Type = local.LocalVariable.Type },
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