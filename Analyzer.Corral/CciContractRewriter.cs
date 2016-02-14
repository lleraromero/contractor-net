using System.Diagnostics.Contracts;
using System.IO;
using Analysis.Cci;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;

namespace Analyzer.Corral
{
    /// <summary>
    ///     Replaces Pre/Post with Assume/Assert
    /// </summary>
    internal class CciContractRewriter
    {
        protected readonly CodeContractAwareHostEnvironment host;

        public CciContractRewriter()
        {
            host = CciHostEnvironment.GetInstance();
        }

        public CciAssembly Rewrite(CciAssembly assembly)
        {
            ISourceLocationProvider sourceLocationProvider = GetPdbReader(assembly.Module);
            var trans = new ContractRewriter(host, assembly.ContractProvider, sourceLocationProvider);
            var newModule = trans.Rewrite(assembly.Module) as Module;
            return new CciAssembly(newModule, assembly.ContractProvider);
        }

        protected PdbReader GetPdbReader(IModule module)
        {
            Contract.Requires(module != null);

            PdbReader pdbReader = null;
            var pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
            {
                using (var pdbStream = File.OpenRead(pdbFile))
                {
                    pdbReader = new PdbReader(pdbStream, host);
                }
            }
            return pdbReader;
        }
    }
}