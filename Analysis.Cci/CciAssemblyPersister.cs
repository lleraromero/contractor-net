using System.Diagnostics.Contracts;
using System.IO;
using Contractor.Core.Model;
using Microsoft.Cci.MutableContracts;

namespace Analysis.Cci
{
    public class CciAssemblyPersister
    {
        protected CodeContractAwareHostEnvironment host;

        public CciAssemblyPersister()
        {
            host = new CodeContractAwareHostEnvironment();
        }

        public IAssembly Decompile(string inputFileName, string contractsFileName)
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