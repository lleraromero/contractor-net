using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public class ConditionalRewriter:CodeRewriter
    {
        private static int countVar = 0;
        private Microsoft.Cci.IUnit unit;
        private Microsoft.Cci.IAssembly coreAssembly;
        private MethodDefinition method;
        public IBlockStatement actionBodyBlock;
        public ConditionalRewriter(IContractAwareHost host, MethodDefinition method, IBlockStatement actionBodyBlock)
            : base(host)
        {
            this.unit = this.host.LoadedUnits.First();
            this.coreAssembly = this.host.FindAssembly(unit.CoreAssemblySymbolicIdentity);
            this.method = method;
            this.actionBodyBlock = actionBodyBlock;
        }

        public override IExpression Rewrite(IExpression expr)
        {
            if (expr is Conditional)
            {
                if ((expr as Conditional).Condition is Conditional)
                {
                    //As Condition we have another Conditional
                    var locDef = Rewrite((expr as Conditional).Condition);
                    (expr as Conditional).Condition = locDef;

                    var varExpr = new BoundExpression()
                    {
                        Type = coreAssembly.PlatformType.SystemBoolean,
                        Definition = AddLocalVariableDefForBooleanExpression( actionBodyBlock, expr)
                    };
                    return varExpr;
                }
                else if ((expr as Conditional).Condition is LogicalNot)
                {
                    var locDef = Rewrite( ((expr as Conditional).Condition as LogicalNot).Operand);
                    ((expr as Conditional).Condition as LogicalNot).Operand = locDef;
                    var varExpr = new BoundExpression()
                    {
                        Type = coreAssembly.PlatformType.SystemBoolean,
                        Definition = AddLocalVariableDefForBooleanExpression( actionBodyBlock,  expr)
                    };
                    return varExpr;
                }
                else
                {
                    //the expr.Condition is not a Conditional
                    //so, we create a local that will replace the conditional
                    var varExpr = new BoundExpression()
                    {
                        Type = coreAssembly.PlatformType.SystemBoolean,
                        Definition = AddLocalVariableDefForBooleanExpression( actionBodyBlock, expr)
                    };
                    return varExpr;
                }
            }
            else if (expr is LogicalNot && (expr as LogicalNot).Operand is Conditional)
            {
                (expr as LogicalNot).Operand = Rewrite( (expr as LogicalNot).Operand);
                return expr;
            }
            else
            {
                //just rewrite Conditionals
                return expr;
            }
        }

        
        //Adds to method body a localDeclaration with the given expression and returns the localDefinition to use it.
        public LocalDefinition AddLocalVariableDefForBooleanExpression( IBlockStatement actionBodyBlock, IExpression expr)
        {
            countVar++;
            var varName = host.NameTable.GetNameFor("local" + countVar.ToString());
            var localVar = new LocalDefinition()
            {
                Name = varName, //Dummy.Name,
                Type = coreAssembly.PlatformType.SystemBoolean,
                MethodDefinition = method
            };
            var st = new LocalDeclarationStatement()
            {
                InitialValue = expr,
                LocalVariable = localVar
            };
            var bst = (actionBodyBlock as BlockStatement);
            if (bst.Statements.Last() is ReturnStatement)
            {
                var pos = bst.Statements.Count - 1;
                //if (pos < 0)
                //{
                //    pos = 0;
                //}
                bst.Statements.Insert(pos, st);
            }
            else
                bst.Statements.Add(st);
            return localVar;
        }
    }
}
