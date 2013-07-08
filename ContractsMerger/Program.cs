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
			public MetadataReaderHost Host;
			public Module Module;
			public PdbReader PdbReader;

			public AssemblyInfo(MetadataReaderHost host, Module module, PdbReader pdbReader)
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
			var assemblyFileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll";
			var assemblyWithContractsFileName = @"C:\Program Files (x86)\Microsoft\Contracts\Contracts\.NETFramework\v4.0\mscorlib.Contracts.dll";
			var outputFileName = @"C:\Users\Eddy\Documents\Tesis\temp\mscorlib.dll";
			var typeFullName = @"System.Collections.ArrayList";

			//var assemblyFileName = @"C:\Users\Eddy\Documents\Tesis\Contractor\Test\API_Examples\bin\Debug\API_Examples.dll";
			//var assemblyWithContractsFileName = @"C:\Users\Eddy\Documents\Tesis\Contractor\Test\API_Examples\bin\Debug\CodeContracts\API_Examples.Contracts.dll";
			//var outputFileName = @"C:\Users\Eddy\Documents\Tesis\temp\API_Examples_merged.dll";

			var program = new Program(assemblyFileName, assemblyWithContractsFileName);
			program.InjectContracts(typeFullName, outputFileName);

			Console.WriteLine("Done!");
			Console.ReadKey();
		}

		public Program(string assemblyFileName, string assemblyWithContractsFileName)
		{
			_AssemblyFileName = assemblyFileName;
			_AssemblyWithContractsFileName = assemblyWithContractsFileName;
		}

		private void InjectContracts(string typeFullName, string outputFileName)
		{
			var host = new CodeContractAwareHostEnvironment(true);

			Console.WriteLine("Loading assembly...");
			var assembly = LoadAssembly(host, _AssemblyFileName);

			Console.WriteLine("Loading contract reference assembly...");
			var assemblyWithContracts = LoadAssembly(host, _AssemblyWithContractsFileName);

			Console.WriteLine("Extracting contracts from contract reference assembly...");
			var contractsProviderForAssemblyWithContracts = ContractHelper.ExtractContracts(host, assemblyWithContracts.Module, assemblyWithContracts.PdbReader, assemblyWithContracts.PdbReader);
			var contractsProviderForAssembly = new ContractProvider(contractsProviderForAssemblyWithContracts.ContractMethods, assembly.Module);

			var typeWithContracts = assemblyWithContracts.Module.AllTypes.Find(x => typeFullName == this.GetTypeFullName(x));
			var type = assembly.Module.AllTypes.Find(x => typeFullName == this.GetTypeFullName(x));

			if (type == null)
			{
				Console.WriteLine("Could not find type '{0}' in assembly.", type);
			}
			else if (typeWithContracts == null)
			{
				Console.WriteLine("Could not find type '{0}' in contract reference assembly.", typeWithContracts);
			}
			else
			{
				Console.WriteLine("Associating contracts with specified type...");
				InjectContractsForType(contractsProviderForAssemblyWithContracts, contractsProviderForAssembly, type, typeWithContracts);

				Console.WriteLine("Injecting contracts into output assembly...");
				ContractHelper.InjectContractCalls(host, assembly.Module, contractsProviderForAssembly, assembly.PdbReader);

				Console.WriteLine("Saving output assembly...");
				SaveAssembly(assembly, outputFileName);
			}

			if (assembly.PdbReader != null)
				assembly.PdbReader.Dispose();

			if (assemblyWithContracts.PdbReader != null)
				assemblyWithContracts.PdbReader.Dispose();

			host.Dispose();
		}

		private void InjectContracts(string outputFileName)
		{
			var host = new CodeContractAwareHostEnvironment(true);

			Console.WriteLine("Loading assembly...");
			var assembly = LoadAssembly(host, _AssemblyFileName);

			Console.WriteLine("Loading contract reference assembly...");
			var assemblyWithContracts = LoadAssembly(host, _AssemblyWithContractsFileName);

			Console.WriteLine("Extracting contracts from contract reference assembly...");
			var contractsProviderForAssemblyWithContracts = ContractHelper.ExtractContracts(host, assemblyWithContracts.Module, assemblyWithContracts.PdbReader, assemblyWithContracts.PdbReader);
			var contractsProviderForAssembly = new ContractProvider(contractsProviderForAssemblyWithContracts.ContractMethods, assembly.Module);

			var typesWithContracts = assemblyWithContracts.Module.AllTypes;
			var types = assembly.Module.AllTypes.ToDictionary(this.GetTypeFullName);

			Console.WriteLine("Associating contracts with types...");

			foreach (var typeWithContracts in typesWithContracts)
			{
				var typeName = this.GetTypeFullName(typeWithContracts);

				if (!types.ContainsKey(typeName))
				{
					Console.WriteLine("Could not find type '{0}' in the specified assembly.", typeWithContracts);
					continue;
				}

				var type = types[typeName];
				InjectContractsForType(contractsProviderForAssemblyWithContracts, contractsProviderForAssembly, type, typeWithContracts);
			}

			Console.WriteLine("Injecting contracts into output assembly...");
			ContractHelper.InjectContractCalls(host, assembly.Module, contractsProviderForAssembly, assembly.PdbReader);

			Console.WriteLine("Saving output assembly...");
			SaveAssembly(assembly, outputFileName);

			if (assembly.PdbReader != null)
				assembly.PdbReader.Dispose();

			if (assemblyWithContracts.PdbReader != null)
				assemblyWithContracts.PdbReader.Dispose();

			host.Dispose();
		}

		private static void InjectContractsForType(ContractProvider contractsProviderForAssemblyWithContracts, ContractProvider contractsProviderForAssembly, INamedTypeDefinition type, INamedTypeDefinition typeWithContracts)
		{
			var typeContract = contractsProviderForAssemblyWithContracts.GetTypeContractFor(typeWithContracts);

			if (typeContract != null)
				contractsProviderForAssembly.AssociateTypeWithContract(type, typeContract);

			var methodsWithContracts = typeWithContracts.Methods;
			var methods = type.Methods.ToDictionary(x => x.ToString());

			foreach (var methodWithContracts in methodsWithContracts)
			{
				var methodName = methodWithContracts.ToString();

				if (!methods.ContainsKey(methodName))
				{
					Console.WriteLine("Could not find method '{0}' in the specified assembly.", methodWithContracts);
					continue;
				}

				var method = methods[methodName];
				var methodContract = contractsProviderForAssemblyWithContracts.GetMethodContractFor(methodWithContracts);

				if (methodContract != null)
					contractsProviderForAssembly.AssociateMethodWithContract(method, methodContract);
			}
		}

		private static AssemblyInfo LoadAssembly(MetadataReaderHost host, string assemblyFileName)
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

		private static void SaveAssembly(AssemblyInfo assembly, string assemblyFileName)
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

		private string GetTypeFullName(INamedTypeDefinition type)
		{
			var genericParameters = type.IsGeneric ? type.GenericParameterCount.ToString() : string.Empty;
			return string.Format("{0}{1}", type, genericParameters);
		}
	}
}
