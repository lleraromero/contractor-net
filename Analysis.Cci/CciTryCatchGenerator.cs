using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contractor.Core;
using Contractor.Core.Model;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableContracts;
using Action = Contractor.Core.Model.Action;

namespace Analysis.Cci
{
    public class CciTryCatchGenerator
    {
        private List<string> listOfExceptions;
        protected ExceptionEncoder exceptionEncoder;
        private bool forCS;
        private IExpression exitCode_eq_expected;

        public CciTryCatchGenerator(List<string> listOfExceptions, bool forCS)
        {
            this.listOfExceptions = listOfExceptions;
            this.exceptionEncoder = new ExceptionEncoder(listOfExceptions);
            this.forCS = forCS;
            //this.exitCode_eq_expected = exitCode_eq_expected;
        }
        public TryCatchFinallyStatement GenerateTryStatement(Action action, IBlockStatement actionBodyBlock, IContractAwareHost host, LocalDeclarationStatement localDefExitCode)
        {
            var unit = host.LoadedUnits.First();
            var assembly = unit as Microsoft.Cci.IAssembly;
            var coreAssembly = host.FindAssembly(unit.CoreAssemblySymbolicIdentity);

            //AGREGAR LOS STATEMENT DEL ACTION AL TRYBLOCK EN VEZ DE AL BLOCK
            var tryBlock = new BlockStatement();

            //Por tratarse de un constructor skipeamos
            //el primer statement porque es base..ctor();
            var skipCount = action.Method.IsConstructor ? 1 : 0;

            if (forCS)
            {               
                var throwRewiter = new ThrowExceptionRewriter(host, listOfExceptions, localDefExitCode, exitCode_eq_expected);
                var newSt = new List<IStatement>();
                var statements= new List<IStatement>(actionBodyBlock.Statements.Skip(skipCount));
                foreach (var st in statements)
                {
                    var copy = new CodeDeepCopier(host).Copy(st);
                    newSt.Add(throwRewiter.Rewrite(copy));
                }
                tryBlock.Statements.AddRange(newSt);
            }
            else
            {
                tryBlock.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));
            }

            var catchClauses = GenerateCatchClauses(action, assembly, coreAssembly, localDefExitCode);

            var tryStmt = new TryCatchFinallyStatement
            {
                TryBody = tryBlock,
                CatchClauses = catchClauses
            };
            return tryStmt;
        }
        private List<ICatchClause> GenerateCatchClauses(Action action, Microsoft.Cci.IAssembly assembly, Microsoft.Cci.IAssembly coreAssembly, LocalDeclarationStatement localDefExitCode)
        {
            var catchClauses = new List<ICatchClause>();

            var intType = coreAssembly.PlatformType.SystemInt32;

            var x = assembly.GetAllTypes();
            var y = coreAssembly.GetAllTypes();
            x = x.Union(y);
            foreach (var exception in listOfExceptions)
            {
                if (exception.Equals("Ok"))
                    continue;
                try
                {
                    var excType = x.Single(t => t.Name.Value == exception.Split('.').Last());
                    var variable = new LocalDefinition()
                    {
                        Name = Dummy.Name,
                        Type = excType,
                        MethodDefinition = action.Method
                    };
                    var catchExc = GenerateCatchClauseFor(coreAssembly, variable, excType, localDefExitCode);

                    catchClauses.Add(catchExc);
                }
                catch (Exception)
                {
                    System.Console.WriteLine("exception does not exists: " + exception);
                }
            }
            return catchClauses;
        }

        private CatchClause GenerateCatchClauseFor(Microsoft.Cci.IAssembly coreAssembly, LocalDefinition variable, INamedTypeDefinition nullExcType, LocalDeclarationStatement localDefExitCode)
        {
            var nullExcBody = new BlockStatement();
            var assign2 = new ExpressionStatement()
            {
                Expression = new Assignment()
                {
                    Source = new CompileTimeConstant
                    {
                        Type = coreAssembly.PlatformType.SystemInt32,
                        Value = exceptionEncoder.ExceptionToInt(nullExcType.Name.Value)
                    },
                    Target = new TargetExpression()
                    {
                        Definition = ((ILocalDeclarationStatement)localDefExitCode).LocalVariable,
                        Type = coreAssembly.PlatformType.SystemInt32
                    },
                    Type = coreAssembly.PlatformType.SystemInt32
                }
            };

            nullExcBody.Statements.Add(assign2);

            var catchNullExc = new CatchClause()
            {
                ExceptionType = nullExcType,
                Body = nullExcBody,
                ExceptionContainer = variable
            };
            return catchNullExc;
        }

        public LocalDeclarationStatement CreateLocalInt(Action action, Microsoft.Cci.IAssembly coreAssembly, int defaultValue)
        {
            var local = new LocalDeclarationStatement()
            {
                InitialValue = new CompileTimeConstant
                {
                    Type = coreAssembly.PlatformType.SystemInt32,
                    Value = defaultValue
                },
                LocalVariable = new LocalDefinition()
                {
                    Name = Dummy.Name,
                    Type = coreAssembly.PlatformType.SystemInt32,
                    MethodDefinition = action.Method
                }
            };
            return local;
        }

        internal void SetEquality(IExpression exitCode_eq_expected)
        {
            this.exitCode_eq_expected = exitCode_eq_expected;
        }
    }
}
