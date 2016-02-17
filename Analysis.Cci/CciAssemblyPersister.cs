using System;
using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;

namespace Analysis.Cci
{
    public class CciAssemblyPersister
    {
        protected CodeContractAwareHostEnvironment host;
        // Object used to provide a managed mutex to avoid race-conditions in CCI static classes
        public static object turnstile = new object();

        public CciAssemblyPersister()
        {
            host = CciHostEnvironment.GetInstance();
        }

        public CciAssembly Load(string inputFileName, string contractsFileName)
        {
            Contract.Requires(File.Exists(inputFileName));

            var module = GetMutableModule(inputFileName, contractsFileName);
            module = new CodeAndContractDeepCopier(host).Copy(module);

            var contractExtractor = host.GetContractExtractor(module.UnitIdentity);
            var contractProvider = new AggregatingContractProvider(contractExtractor);

            return new CciAssembly(module, contractProvider);
        }

        public void Save(CciAssembly assembly, string path)
        {
            Contract.Requires(assembly != null);
            Contract.Requires(!File.Exists(path));

            var sourceLocationProvider = GetPdbReader(assembly.Module);
            lock (turnstile)
            {
                ContractHelper.InjectContractCalls(host, assembly.Module, assembly.ContractProvider, sourceLocationProvider);
            }
            using (var peStream = File.Create(path))
            {
                lock (turnstile)
                {
                    PeWriter.WritePeToStream(assembly.Module, host, peStream);
                }
            }
        }

        protected Module GetMutableModule(string inputFileName, string contractsFileName)
        {
            Contract.Requires(File.Exists(inputFileName));

            var module = LoadModuleInHost(new FileInfo(inputFileName));

            if (File.Exists(contractsFileName))
            {
                // Registers the contracts for the input file. It isn't neccesary to get the result 
                LoadModuleInHost(new FileInfo(contractsFileName));
            }

            using (var pdbReader = GetPdbReader(module))
            {
                return Decompiler.GetCodeModelFromMetadataModel(host, module, pdbReader);
            }
        }

        protected IModule LoadModuleInHost(FileInfo inputFile)
        {
            Contract.Requires(inputFile.Exists);

            var module = host.LoadUnitFrom(inputFile.FullName) as IModule;
            if (module == null || module is Dummy || module == Dummy.Assembly)
            {
                throw new Exception(string.Format("{0} is not a PE file containing a CLR module or assembly.", inputFile.FullName));
            }
            return module;
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