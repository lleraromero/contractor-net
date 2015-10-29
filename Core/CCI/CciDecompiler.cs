using Microsoft.Cci.MutableContracts;
using System.Diagnostics.Contracts;
using System.IO;

namespace Contractor.Core
{
    public class CciDecompiler
    {
        protected CodeContractAwareHostEnvironment host;

        public CciDecompiler()
        {
            this.host = new CodeContractAwareHostEnvironment();
        }

        public AssemblyXXX Decompile(string inputFileName, string contractsFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(inputFileName) && File.Exists(inputFileName));

            return new CciAssembly(inputFileName, contractsFileName, this.host);
        }
    }
}
