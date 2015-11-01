using Contractor.Core;
using Microsoft.Cci.MutableContracts;
using System.Diagnostics.Contracts;
using System.IO;

namespace Analysis.Cci
{
    public class CciDecompiler
    {
        protected CodeContractAwareHostEnvironment host;

        public CciDecompiler()
        {
            this.host = new CodeContractAwareHostEnvironment();
        }

        public IAssemblyXXX Decompile(string inputFileName, string contractsFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(inputFileName) && File.Exists(inputFileName));

            return new CciAssembly(inputFileName, contractsFileName, this.host);
        }

        public CciQueryGenerator CreateQueryGenerator() 
        {
            return new CciQueryGenerator(this.host);
        }
    }
}
