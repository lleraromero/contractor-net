using System.IO;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using System.Collections.Generic;

namespace Analyzer.CodeContracts
{
    public class CodeContractsWithConditionsAnalyzerFactory : IAnalyzerFactory
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
        protected int dependencyQueriesCount;

        protected List<string> errorListForQueryGenerator;
        public CodeContractsWithConditionsAnalyzerFactory(DirectoryInfo workingDir, string ccCheckDefaultArgs, string libPaths, List<string> errorListForQueryGenerator,
            CciAssembly inputAssembly, string inputFileName, ITypeDefinition typeToAnalyze, CancellationToken token)
        {
            this.workingDir = workingDir;
            this.ccCheckDefaultArgs = ccCheckDefaultArgs;
            this.libPaths = libPaths;
            this.inputAssembly = inputAssembly;
            this.typeToAnalyze = typeToAnalyze;
            this.inputFileName = inputFileName;
            this.token = token;
            this.errorListForQueryGenerator = errorListForQueryGenerator;

            generatedQueriesCount = 0;
            unprovenQueriesCount = 0;
            dependencyQueriesCount = 0;
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

        public int DependencyQueriesCount
        {
            get { return dependencyQueriesCount; }
            set
            {
                lock (this)
                {
                    dependencyQueriesCount = value;
                }
            }
        }

        public IAnalyzer CreateAnalyzer()
        {
            var queryGenerator = new CciQueryGenerator(this.errorListForQueryGenerator);
            return new AnalyzerWithCondition(workingDir, ccCheckDefaultArgs, libPaths, queryGenerator, inputAssembly, inputFileName, typeToAnalyze,
                token);
        }
    }
}