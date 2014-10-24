using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace Contractor.Utils
{
    public class AssemblyInfo
    {
        public string FileName { get; private set; }
        public IMetadataHost Host { get; private set; }
        public IModule Module { get; private set; }
        public Module DecompiledModule { get; private set; }
        public PdbReader PdbReader { get; private set; }

        public AssemblyInfo(IMetadataHost host)
        {
            Contract.Requires(host != null);

            this.Host = host;
        }

        public AssemblyInfo(IMetadataHost host, IModule module)
        {
            Contract.Requires(host != null && module != null);

            this.Host = host;
            this.Module = module;
            this.DecompiledModule = module as Module;
            if (this.DecompiledModule == null)
                Decompile();
        }

        ~AssemblyInfo()
        {
            if (PdbReader != null)
                this.PdbReader.Dispose();
        }

        public void Load(string fileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName) && File.Exists(fileName));

            this.FileName = fileName;
            this.Module = LoadModule(fileName, this.Host);
            this.PdbReader = GetPDBReader(this.Module, this.Host);
            Decompile();
        }

        private void Decompile()
        {
            Contract.Requires(Module != null);

            this.DecompiledModule = Decompiler.GetCodeModelFromMetadataModel(this.Host, this.Module, this.PdbReader);
        }

        public ContractProvider ExtractContracts()
        {
            var contractAwareHost = this.Host as IContractAwareHost;
            ContractProvider contractProvider = null;

            // Extracting contracts from this assembly and the contract reference assembly previously loaded with this host
            var contractExtractor = contractAwareHost.GetContractExtractor(this.Module.UnitIdentity);
            contractProvider = new AggregatingContractProvider(contractExtractor);
            
            // Extracting contracts from this assembly
            //contractProvider = ContractHelper.ExtractContracts(contractAwareHost, this.DecompiledModule, this.PdbReader, this.PdbReader);

            return contractProvider;
        }

        public void InjectContracts(ContractProvider contractProvider)
        {
            ContractHelper.InjectContractCalls(this.Host, this.DecompiledModule, contractProvider, this.PdbReader);
        }

        public void Save(string fileName)
        {
            Contract.Requires(DecompiledModule != null && Host != null && !string.IsNullOrEmpty(fileName));

            using (var peStream = File.OpenWrite(fileName))
            {
                if (this.PdbReader == null)
                {
                    PeWriter.WritePeToStream(this.DecompiledModule, this.Host, peStream);
                }
                else
                {
                    var pdbName = Path.ChangeExtension(fileName, "pdb");
                    using (var pdbWriter = new PdbWriter(pdbName, this.PdbReader))
                        PeWriter.WritePeToStream(this.DecompiledModule, this.Host, peStream, this.PdbReader, this.PdbReader, pdbWriter);
                }
            }
        }

        private IModule LoadModule(string filename, IMetadataHost host)
        {
            Contract.Requires(host != null && !string.IsNullOrEmpty(filename));

            var module = host.LoadUnitFrom(filename) as IModule;
            if (module == null || module is Dummy || this.Module == Dummy.Assembly)
                throw new Exception(string.Concat(filename, " is not a PE file containing a CLR module or assembly."));
            return module;
        }

        private PdbReader GetPDBReader(IModule module, IMetadataHost host)
        {
            Contract.Requires(host != null && module != null);

            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
                using (var pdbStream = File.OpenRead(pdbFile))
                    pdbReader = new PdbReader(pdbStream, host);
            return pdbReader;
        }
    }
}