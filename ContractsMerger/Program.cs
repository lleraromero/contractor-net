using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;

namespace ContractsMerger
{
	class Program
	{
		private struct AssemblyInfo
		{
			public IContractAwareHost Host;
			public Module Module;
			public PdbReader PdbReader;

			public AssemblyInfo(IContractAwareHost host, Module module, PdbReader pdbReader)
				: this()
			{
				this.Host = host;
				this.Module = module;
				this.PdbReader = pdbReader;
			}
		}

		private string _AssemblyFileName;
		private string _AssemblyWithContractsFileName;

		static void Main(string[] args)
		{
			//var assemblyFileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll";
			//var assemblyWithContractsFileName = @"C:\Program Files (x86)\Microsoft\Contracts\Contracts\.NETFramework\v4.0\System.Contracts.dll";
			//var outputFileName = @"C:\Users\Eddy\Documents\Tesis\temp\System.dll";

			var assemblyFileName = @"C:\Users\Eddy\Documents\Tesis\Contractor\Test\API_Examples\bin\Debug\API_Examples.dll";
			var assemblyWithContractsFileName = @"C:\Users\Eddy\Documents\Tesis\Contractor\Test\API_Examples\bin\Debug\CodeContracts\API_Examples.Contracts.dll";
			var outputFileName = @"C:\Users\Eddy\Documents\Tesis\temp\API_Examples_merged.dll";

			var program = new Program(assemblyFileName, assemblyWithContractsFileName);
			program.InjectContracts(outputFileName);

			Console.WriteLine("Done!");
			Console.ReadKey();
		}

		public Program(string assemblyFileName, string assemblyWithContractsFileName)
		{
			_AssemblyFileName = assemblyFileName;
			_AssemblyWithContractsFileName = assemblyWithContractsFileName;
		}

		private void InjectContracts(string outputFileName)
		{
			var host = new CodeContractAwareHostEnvironment(true);
			var assembly = loadAssembly(host, _AssemblyFileName);
			var assemblyWithContracts = loadAssembly(host, _AssemblyWithContractsFileName);

			var contractsProviderForAssemblyWithContracts = ContractHelper.ExtractContracts(host, assemblyWithContracts.Module, assemblyWithContracts.PdbReader, assemblyWithContracts.PdbReader);
			var contractsProviderForAssembly = new ContractProvider(contractsProviderForAssemblyWithContracts.ContractMethods, assembly.Module);

			var typesWithContracts = assemblyWithContracts.Module.AllTypes.ToDictionary(x => x.ToString());
			var types = assembly.Module.AllTypes;

			foreach (var type in types)
			{
				var typeName = type.ToString();

				if (!typesWithContracts.ContainsKey(typeName))
					continue;

				var typeWithContracts = typesWithContracts[typeName];
				var typeContract = contractsProviderForAssemblyWithContracts.GetTypeContractFor(typeWithContracts);

				if (typeContract != null)
					contractsProviderForAssembly.AssociateTypeWithContract(type, typeContract);

				var methodsWithContracts = typeWithContracts.Methods.ToDictionary(x => x.ToString());
				var methods = type.Methods;

				foreach (var method in methods)
				{
					var methodName = method.ToString();

					if (!methodsWithContracts.ContainsKey(methodName))
						continue;

					var methodWithContracts = methodsWithContracts[methodName];
					var methodContract = contractsProviderForAssemblyWithContracts.GetMethodContractFor(methodWithContracts);

					if (methodContract != null)
						contractsProviderForAssembly.AssociateMethodWithContract(method, methodContract);
				}
			}

			ContractHelper.InjectContractCalls(host, assembly.Module, contractsProviderForAssembly, assembly.PdbReader);
			saveAssembly(assembly, outputFileName);

			if (assembly.PdbReader != null)
				assembly.PdbReader.Dispose();

			if (assemblyWithContracts.PdbReader != null)
				assemblyWithContracts.PdbReader.Dispose();

			host.Dispose();
		}

		private static AssemblyInfo loadAssembly(IContractAwareHost host, string assemblyFileName)
		{
			var staticModule = host.LoadUnitFrom(assemblyFileName) as IModule;

			if (staticModule == null || staticModule == Dummy.Module || staticModule == Dummy.Assembly)
				throw new Exception("The input is not a valid CLR module or assembly.");

			string pdbFileName = Path.ChangeExtension(assemblyFileName, "pdb");
			PdbReader pdbReader = null;

			if (File.Exists(pdbFileName))
			{
				using (var pdbStream = File.OpenRead(pdbFileName))
					pdbReader = new PdbReader(pdbStream, host);
			}

			var module = Decompiler.GetCodeModelFromMetadataModel(host, staticModule, pdbReader);
			return new AssemblyInfo(host, module, pdbReader);
		}

		private static void saveAssembly(AssemblyInfo assembly, string assemblyFileName)
		{
			var pdbName = Path.ChangeExtension(assemblyFileName, "pdb");

			using (var peStream = File.Create(assemblyFileName))
			{
				if (assembly.PdbReader == null)
				{
					PeWriter.WritePeToStream(assembly.Module, assembly.Host, peStream);
				}
				else
				{
					using (var pdbWriter = new PdbWriter(pdbName, assembly.PdbReader))
						PeWriter.WritePeToStream(assembly.Module, assembly.Host, peStream, assembly.PdbReader, assembly.PdbReader, pdbWriter);
				}
			}
		}
	}
}
