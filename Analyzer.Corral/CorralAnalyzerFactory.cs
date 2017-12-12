using System.IO;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using System.Collections.Generic;

namespace Analyzer.Corral
{
    public class CorralAnalyzerFactory : IAnalyzerFactory
    {
        protected string defaultArgs;
        protected CciAssembly inputAssembly;
        protected string inputFileName;
        protected ITypeDefinition typeToAnalyze;
        protected CancellationToken token;
        protected DirectoryInfo workingDir;

        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;
        protected int dependencyQueriesCount;

        private List<string> errorList;
        private List<string> errorListForQueryGenerator;
        private int maxDegreeOfParallelism;
        

        public CorralAnalyzerFactory(string defaultArgs, DirectoryInfo workingDir, List<string> errorListForQueryGenerator, CciAssembly inputAssembly,
            string inputFileName, ITypeDefinition typeToAnalyze, CancellationToken token, List<string> errorList, int maxDegreeOfParallelism)
        {
            this.defaultArgs = defaultArgs;
            this.workingDir = workingDir;
            this.inputAssembly = inputAssembly;
            this.inputFileName = inputFileName;
            this.typeToAnalyze = typeToAnalyze;
            this.token = token;
            this.errorListForQueryGenerator = errorListForQueryGenerator;
            this.errorList = errorList;
            generatedQueriesCount = 0;
            unprovenQueriesCount = 0;
            dependencyQueriesCount = 0;
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
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
            return new CorralAnalyzer(defaultArgs, workingDir, queryGenerator, inputAssembly, inputFileName,
                typeToAnalyze, token, errorList, maxDegreeOfParallelism);
        }
    }
}