using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;

namespace Analysis.Cci
{
    public abstract class CciActionQueryGenerator
    {
        protected IContractAwareHost host;
        public IExpression exitCode_eq_expected;
        private string expectedExitCode;
        protected List<string> listOfExceptions;
        protected CciActionQueryGenerator(IContractAwareHost host, List<string> listOfExceptions)
        {
            this.host = host;
            this.listOfExceptions = listOfExceptions;
        }

        public abstract ActionQuery CreateQuery(State state, Action action, Action actionUnderTest, string expectedExitCode);
        protected abstract string CreateQueryName(State state, Action action, Action actionUnderTest);
        protected abstract IMethodContract CreateQueryContract(State state, Action actionUnderTest);

        protected CciAction GenerateQuery(State state, Action action, Action target, string expectedExitCode = null)
        {
            Contract.Requires(state != null && action != null && target != null);

            this.expectedExitCode = expectedExitCode;
            var queryName = CreateQueryName(state, action, target);
            var queryMethod = CreateQueryMethod(state, queryName, action, target);
            var queryContract = CreateQueryContract(state, target);

            return new CciAction(queryMethod, queryContract);
        }

        protected MethodDefinition CreateQueryMethod(State state, string name, Action action, Action target)
        {
            var parameters = new HashSet<IParameterDefinition> (action.Method.Parameters);
            
            return CreateMethod(name, action, parameters);
        }

        protected HashSet<IParameterDefinition> GetStateParameters(State state)
        {
            // Get all the parameters that the query might need
            var parameters = new HashSet<IParameterDefinition>();
            foreach (var a in state.EnabledActions)
            {
                parameters.UnionWith(a.Method.Parameters);
            }
            foreach (var a in state.DisabledActions)
            {
                parameters.UnionWith(a.Method.Parameters);
            }
            return parameters;
        }

        protected MethodDefinition CreateMethod(string name, Action action, HashSet<IParameterDefinition> parameters)
        {
            var method = new MethodDefinition
            {
                CallingConvention = CallingConvention.HasThis,
                InternFactory = host.InternFactory,
                IsStatic = false,
                Name = host.NameTable.GetNameFor(name),
                Type = action.Method.Type,
                Visibility = TypeMemberVisibility.Public,
                Parameters = parameters.ToList()
            };

            var block = InlineMethodBody(action);

            method.Body = new SourceMethodBody(host)
            {
                MethodDefinition = method,
                Block = block,
                LocalsAreZeroed = true
            };
            return method;
        }

        protected BlockStatement InlineMethodBody(Action action)
        {
            var block = new BlockStatement();

            var mc = action.Contract;

            /*if (mc != null && mc.Preconditions.Any())
            {
                var asserts = from pre in mc.Preconditions
                    select new AssumeStatement
                    {
                        Condition = pre.Condition,
                        OriginalSource = pre.OriginalSource,
                        Description = new CompileTimeConstant { Value = "Inlined method precondition", Type = host.PlatformType.SystemString }
                    };

                block.Statements.AddRange(asserts);
            }*/

            IBlockStatement actionBodyBlock = null;
            if (action.Method.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
            {
                var actionBody = (Microsoft.Cci.ILToCodeModel.SourceMethodBody) action.Method.Body;
                actionBodyBlock = actionBody.Block;
            }
            else if (action.Method.Body is SourceMethodBody)
            {
                var actionBody = (SourceMethodBody) action.Method.Body;
                actionBodyBlock = actionBody.Block;
            }
            Contract.Assert(actionBodyBlock != null);

            if (expectedExitCode != null){
                //EPA-O
                var unit = this.host.LoadedUnits.First();
                var assembly = unit as Microsoft.Cci.IAssembly;
                var coreAssembly = this.host.FindAssembly(unit.CoreAssemblySymbolicIdentity);
                /*
                var x = assembly.GetAllTypes();
                var y = coreAssembly.GetAllTypes();
                x = x.Union(y);

                var excType = x.Single(t => t.Name.Value == "Exception");

                var tryBlock = new BlockStatement();
                
                //Por tratarse de un constructor skipeamos
                //el primer statement porque es base..ctor();
                var skipCount = action.Method.IsConstructor ? 1 : 0;
                tryBlock.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));
                */
                var try_catch_gen = new CciTryCatchGenerator(listOfExceptions,false);

                var localDefExitCode = try_catch_gen.CreateLocalInt(action, coreAssembly, 0);

                var tryStmt = try_catch_gen.GenerateTryStatement(action, actionBodyBlock, host, localDefExitCode);

                /*
                var catchClauses = new List<ICatchClause>();
                //------------------------------------------------------------------------------
                //reemplazar esto por un metodo que genere las catch
                //extraer el metodo de CCI query generator
                var catchExc= new CatchClause()
                {
                    ExceptionType = excType, // this.host.NameTable.GetNameFor("Exception"),
                    Body = new BlockStatement()
                };
                catchClauses.Add(catchExc);
                //------------------------------------------------------------------------------
                var tryStmt = new TryCatchFinallyStatement
                {
                    TryBody = tryBlock,
                    CatchClauses = catchClauses
                };
                */
                block.Statements.Add(tryStmt);
            }else{
                //EPAs
                var skipCount = action.Method.IsConstructor ? 1 : 0;
                block.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));
            }

            if (mc != null && mc.Postconditions.Any())
            {
                var assumes = from post in mc.Postconditions
                    select new AssumeStatement
                    {
                        Condition = post.Condition,
                        OriginalSource = post.OriginalSource,
                        Description = new CompileTimeConstant { Value = "Inlined method postcondition", Type = host.PlatformType.SystemString }
                    };
                //Ponemos los assume antes del return
                var assume = assumes.ElementAt(0);
                this.exitCode_eq_expected = assume.Condition;
                List<AssumeStatement> finalAssumes = new List<AssumeStatement>(assumes);
                finalAssumes.RemoveAt(0);

                if (block.Statements.Count > 0 && block.Statements.Last() is IReturnStatement)
                {
                    block.Statements.InsertRange(block.Statements.Count - 1, finalAssumes);
                }
                else
                {
                    block.Statements.AddRange(finalAssumes);
                }
            }

            return block;
        }
    }
}