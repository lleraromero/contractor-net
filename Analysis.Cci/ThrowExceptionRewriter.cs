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
    public class ThrowExceptionRewriter : CodeVisitor
    {
        private Microsoft.Cci.IUnit unit;
        private Microsoft.Cci.IAssembly coreAssembly;
        private LocalDeclarationStatement localDefExitCode;
        protected ExceptionEncoder exceptionEncoder;
        private IExpression exitCode_eq_expected;
        private IContractAwareHost host;
        private IStatement lastThrowStatement;

        public IStatement LastThrowStatement { get { return lastThrowStatement; } set { lastThrowStatement = value; } }

        public ThrowExceptionRewriter(IContractAwareHost host, List<string> listOfExceptions, LocalDeclarationStatement localDefExitCode, IExpression exitCode_eq_expected)
            //: base()
        {
            this.host = host;
            this.unit = this.host.LoadedUnits.First();
            this.coreAssembly = this.host.FindAssembly(unit.CoreAssemblySymbolicIdentity);
            this.localDefExitCode = localDefExitCode;
            this.exceptionEncoder = new ExceptionEncoder(listOfExceptions);
            this.exitCode_eq_expected = exitCode_eq_expected;
            
        }

        public override void Visit(IThrowStatement throwStatement)
        {
            if (exitCode_eq_expected == null)
            {
                base.Visit(throwStatement);
                lastThrowStatement = throwStatement;
                return;
            }
            var exc = throwStatement.Exception;
            //***********************
            var newBlock = new BlockStatement();
            var assignExcNumber = new ExpressionStatement()
            {
                Expression = new Assignment()
                {
                    Source = new CompileTimeConstant
                    {
                        Type = coreAssembly.PlatformType.SystemInt32,
                        Value = exceptionEncoder.ExceptionToInt(exc.Type.ResolvedType.ToString().Split('.').Last())
                    },
                    Target = new TargetExpression()
                    {
                        Definition = ((ILocalDeclarationStatement)localDefExitCode).LocalVariable,
                        Type = coreAssembly.PlatformType.SystemInt32
                    },
                    Type = coreAssembly.PlatformType.SystemInt32
                }
            };
            //***********************
            //var assertCall;
            var methodCall = new MethodCall
            {
                Arguments =
                        new List<IExpression>
                        {
                            this.exitCode_eq_expected,
                            new CompileTimeConstant { Type = host.PlatformType.SystemString, Value = "Helping CCCheck when exception thrown." }
                        },
                IsStaticCall = true,
                MethodToCall = new Microsoft.Cci.MethodReference(host, host.PlatformType.SystemDiagnosticsContractsContract, CallingConvention.Default,
                    host.PlatformType.SystemVoid, host.NameTable.GetNameFor("Assert"), 0, host.PlatformType.SystemBoolean,
                    host.PlatformType.SystemString),
                Type = host.PlatformType.SystemVoid,
                Locations = new List<ILocation>()
            };
            var assertCall = new ExpressionStatement
            {
                Expression = methodCall
            };
            //***********************
            newBlock.Statements.Add(assignExcNumber);
            newBlock.Statements.Add(assertCall);
            base.Visit(throwStatement);
            newBlock.Statements.Add(throwStatement);
            lastThrowStatement = newBlock;
            return;
        }


    }
}