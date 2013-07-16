using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System.IO;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableContracts;
using Microsoft.Cci.Contracts;

namespace Contractor.Utils
{
	public class AssemblyInfo : IDisposable
	{
		private bool _ContractsInjected;

		public string FileName { get; private set; }
		public bool IsLoaded { get; private set; }
		public IMetadataHost Host { get; private set; }
		public IModule Module { get; private set; }
		public Module DecompiledModule { get; private set; }
		public PdbReader PdbReader { get; private set; }

		public AssemblyInfo(IMetadataHost host)
		{
			this.Host = host;
		}

		public void Load(string fileName)
		{
			this.Module = this.Host.LoadUnitFrom(fileName) as IModule;

			if (this.Module == null || this.Module == Dummy.Module || this.Module == Dummy.Assembly)
				throw new Exception("The input is not a valid CLR module or assembly.");

			var pdbFileName = Path.ChangeExtension(fileName, "pdb");

			if (File.Exists(pdbFileName))
			{
				using (var pdbStream = File.OpenRead(pdbFileName))
					this.PdbReader = new PdbReader(pdbStream, this.Host);
			}

			this.FileName = fileName;
			this.IsLoaded = true;
		}

		public void Decompile()
		{
			this.DecompiledModule = Decompiler.GetCodeModelFromMetadataModel(this.Host, this.Module, this.PdbReader);
		}

		public ContractProvider ExtractContractsFromContractReferenceAssembly(IContractAwareHost host)
		{
			var contractAwareHost = this.Host as IContractAwareHost;
			var contractExtractor = contractAwareHost.GetContractExtractor(this.Module.UnitIdentity);
			var contractProvider = new AggregatingContractProvider(contractExtractor);

			return contractProvider;
		}

		public ContractProvider ExtractContracts()
		{
			var contractAwareHost = this.Host as IContractAwareHost;
			ContractProvider contractProvider;

			if (_ContractsInjected)
			{
				// Extracting contracts from this assembly
				contractProvider = ContractHelper.ExtractContracts(contractAwareHost, this.DecompiledModule, this.PdbReader, this.PdbReader);
			}
			else
			{
				// Extracting contracts from this assembly and the contract reference assembly previously loaded with this host
				var contractExtractor = contractAwareHost.GetContractExtractor(this.Module.UnitIdentity);
				contractProvider = new AggregatingContractProvider(contractExtractor);
			}

			return contractProvider;
		}

		public void InjectContracts(ContractProvider contractProvider)
		{
			ContractHelper.InjectContractCalls(this.Host, this.DecompiledModule, contractProvider, this.PdbReader);
			_ContractsInjected = true;
		}

		public void Save(string fileName)
		{
			var pdbName = Path.ChangeExtension(fileName, "pdb");

			using (var peStream = File.Create(fileName))
			{
				if (this.PdbReader == null)
				{
					PeWriter.WritePeToStream(this.DecompiledModule, this.Host, peStream);
				}
				else
				{
					using (var pdbWriter = new PdbWriter(pdbName, this.PdbReader))
						PeWriter.WritePeToStream(this.DecompiledModule, this.Host, peStream, this.PdbReader, this.PdbReader, pdbWriter);
				}
			}
		}

		public void Unload()
		{
			if (!this.IsLoaded) return;

			if (this.PdbReader != null)
			{
				this.PdbReader.Dispose();
				this.PdbReader = null;
			}

			this.Module = null;
			this.DecompiledModule = null;
			this.FileName = null;
			this.IsLoaded = false;
		}

		public void Dispose()
		{
			this.Unload();
		}

		public static void MergeContractsWithCode(AssemblyInfo assembly, AssemblyInfo assemblyWithContracts)
		{
			var contractProviderForAssemblyWithContracts = assemblyWithContracts.ExtractContracts();
			var contractProviderForAssembly = new ContractProvider(contractProviderForAssemblyWithContracts.ContractMethods, assembly.Module);

			var typesWithContracts = assemblyWithContracts.DecompiledModule.AllTypes;
			//var types = assembly.DecompiledModule.AllTypes.ToDictionary(Extensions.GetUniqueName);

			var types = new Dictionary<string, INamedTypeDefinition>();

			foreach (var t in assembly.DecompiledModule.AllTypes)
			{
				var name = t.GetUniqueName();

				if (!types.ContainsKey(name))
				{
					types.Add(name, t);
				}
				else
				{

				}
			}

			foreach (var typeWithContracts in typesWithContracts)
			{
				var typeName = typeWithContracts.GetUniqueName();
				if (!types.ContainsKey(typeName)) continue;

				var type = types[typeName];
				AssemblyInfo.MergeContractsForType(contractProviderForAssemblyWithContracts, contractProviderForAssembly, typeWithContracts, type);
			}

			assembly.InjectContracts(contractProviderForAssembly);
		}

		private static void MergeContractsForType(ContractProvider contractsProviderForAssemblyWithContracts, ContractProvider contractsProviderForAssembly, INamedTypeDefinition typeWithContracts, INamedTypeDefinition type)
		{
			var typeContract = contractsProviderForAssemblyWithContracts.GetTypeContractFor(typeWithContracts);

			if (typeContract != null)
				contractsProviderForAssembly.AssociateTypeWithContract(type, typeContract);

			var methodsWithContracts = typeWithContracts.Methods;
			//var methods = type.Methods.ToDictionary(Extensions.GetUniqueName);

			var methods = new Dictionary<string, IMethodDefinition>();

			foreach (var m in type.Methods)
			{
				var name = m.GetUniqueName();

				if (!methods.ContainsKey(name))
				{
					methods.Add(name, m);
				}
				else
				{

				}
			}

			foreach (var methodWithContracts in methodsWithContracts)
			{
				var methodName = methodWithContracts.GetUniqueName();
				if (!methods.ContainsKey(methodName)) continue;

				var method = methods[methodName];
				var methodContract = contractsProviderForAssemblyWithContracts.GetMethodContractFor(methodWithContracts);

				if (methodContract != null)
					contractsProviderForAssembly.AssociateMethodWithContract(method, methodContract);
			}
		}
	}
}
