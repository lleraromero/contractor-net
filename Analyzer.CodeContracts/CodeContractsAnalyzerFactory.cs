using System.IO;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;

namespace Analyzer.CodeContracts
{
    public class CodeContractsAnalyzerFactory : IAnalyzerFactory
    {
        protected CciQueryGenerator queryGenerator;
        protected CciAssembly inputAssembly;
        protected string inputFileName;
        protected ITypeDefinition typeToAnalyze;
        protected CancellationToken token;
        protected DirectoryInfo workingDir;
        protected string ccCheckDefaultArgs;
        protected string libPaths;

        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        public CodeContractsAnalyzerFactory(DirectoryInfo workingDir, string ccCheckDefaultArgs, string libPaths, CciQueryGenerator queryGenerator,
            CciAssembly inputAssembly, string inputFileName, ITypeDefinition typeToAnalyze, CancellationToken token)
        {
            this.workingDir = workingDir;
            this.ccCheckDefaultArgs = ccCheckDefaultArgs;
            this.libPaths = libPaths;
            this.queryGenerator = queryGenerator;
            this.inputAssembly = inputAssembly;
            this.typeToAnalyze = typeToAnalyze;
            this.inputFileName = inputFileName;
            this.token = token;

            generatedQueriesCount = 0;
            unprovenQueriesCount = 0;
        }

        public int GeneratedQueriesCount
        {
            get { return generatedQueriesCount; }
            set
            {
                lock (this)
                {
                    generatedQueriesCount = value;
                }
            }
        }

        public int UnprovenQueriesCount
        {
            get { return unprovenQueriesCount; }
            set
            {
                lock (this)
                {
                    unprovenQueriesCount = value;
                }
            }
        }

        public IAnalyzer CreateAnalyzer()
        {
            return new CodeContractsAnalyzer(workingDir, ccCheckDefaultArgs, libPaths, queryGenerator, inputAssembly, inputFileName, typeToAnalyze,
                token);
        }
    }
}