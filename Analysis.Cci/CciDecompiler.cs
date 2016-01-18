using System.Diagnostics.Contracts;
using System.IO;
using Contractor.Core.Model;
using Microsoft.Cci.MutableContracts;

namespace Analysis.Cci
{
    public class CciDecompiler
    {
        protected CodeContractAwareHostEnvironment host;

        public CciDecompiler()
        {
            host = new CodeContractAwareHostEnvironment();
        }

        public IAssemblyXXX Decompile(string inputFileName, string contractsFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(inputFileName) && File.Exists(inputFileName));

            return new CciAssembly(inputFileName, contractsFileName, host);
        }

        public CciQueryGenerator CreateQueryGenerator()
        {
            return new CciQueryGenerator(host);
        }
    }
}