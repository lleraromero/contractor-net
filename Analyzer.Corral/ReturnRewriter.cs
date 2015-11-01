using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System.Diagnostics.Contracts;

namespace Analyzer.Corral
{
    class ReturnRewriter : CodeRewriter
    {
        private ILabeledStatement target;
        private ILocalDeclarationStatement local;

        public ReturnRewriter(IMetadataHost host, ILabeledStatement target, ILocalDeclarationStatement local)
            : base(host)
        {
            Contract.Requires(host != null && target != null);
            this.target = target;
            this.local = local;
        }

        /// <summary>
        /// Rewrites the return statement.
        /// </summary>
        /// <param name="returnStatement"></param>
        public override IStatement Rewrite(IReturnStatement returnStatement)
        {
            Contract.Ensures(Contract.Result<IStatement>() != null);

            var blockStmt = new BlockStatement();

            blockStmt.Statements.Add(new ExpressionStatement()
            {
                Expression = new Assignment()
                {
                    Target = new TargetExpression() { Definition = local.LocalVariable, Instance = null, Type = local.LocalVariable.Type },
                    Source = this.Rewrite(returnStatement.Expression),
                    Type = returnStatement.Expression.Type
                }
            });

            blockStmt.Statements.Add(new GotoStatement() { TargetStatement = target });

            return blockStmt;
        }
    }
}