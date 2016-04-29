using System.IO;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;

namespace Analyzer.Corral
{
    public class CorralAnalyzerFactory : IAnalyzerFactory
    {
        protected string defaultArgs;
        protected CciQueryGenerator queryGenerator;
        protected CciAssembly inputAssembly;
        protected string inputFileName;
        protected ITypeDefinition typeToAnalyze;
        protected CancellationToken token;
        protected DirectoryInfo workingDir;

        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        public CorralAnalyzerFactory(string defaultArgs, DirectoryInfo workingDir, CciQueryGenerator queryGenerator, CciAssembly inputAssembly,
            string inputFileName, ITypeDefinition typeToAnalyze, CancellationToken token)
        {
            this.defaultArgs = defaultArgs;
            this.workingDir = workingDir;
            this.queryGenerator = queryGenerator;
            this.inputAssembly = inputAssembly;
            this.inputFileName = inputFileName;
            this.typeToAnalyze = typeToAnalyze;
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
            return new CorralAnalyzer(defaultArgs, workingDir, queryGenerator, inputAssembly, inputFileName,
                typeToAnalyze, token);
        }
    }
}