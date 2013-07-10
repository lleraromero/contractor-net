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
using Contractor.Utils;

namespace ContractsMerger
{
	class Program
	{
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
			//program.LoadAndSave(assemblyFileName, outputFileName);
			//program.LoadAndSave(outputFileName, outputFileName);

			Console.WriteLine("Done!");
			Console.ReadKey();
		}

		public Program(string assemblyFileName, string assemblyWithContractsFileName)
		{
			_AssemblyFileName = assemblyFileName;
			_AssemblyWithContractsFileName = assemblyWithContractsFileName;
		}

		//public void LoadAndSave(string inputFileName, string outputFileName)
		//{
		//    var host = new CodeContractAwareHostEnvironment(true);
		//    var assembly = new AssemblyInfo(host);

		//    Console.WriteLine("Loading assembly...");
		//    assembly.Load(inputFileName);
		//    assembly.Decompile();

		//    Console.WriteLine("Saving output assembly...");
		//    assembly.Save(outputFileName);

		//    assembly.Dispose();
		//    host.Dispose();
		//}

		private void InjectContracts(string typeFullName, string outputFileName)
		{
			var host = new CodeContractAwareHostEnvironment(true);
			var assembly = new AssemblyInfo(host);
			var assemblyWithContracts = new AssemblyInfo(host);

			Console.WriteLine("Loading assembly...");
			assembly.Load(_AssemblyFileName);
			assembly.Decompile();

			Console.WriteLine("Loading contract reference assembly...");
			assemblyWithContracts.Load(_AssemblyWithContractsFileName);
			assemblyWithContracts.Decompile();

			Console.WriteLine("Extracting contracts from contract reference assembly...");
			var contractsProviderForAssemblyWithContracts = assemblyWithContracts.ExtractContracts();
			var contractsProviderForAssembly = new ContractProvider(contractsProviderForAssemblyWithContracts.ContractMethods, assembly.Module);

			var typeWithContracts = assemblyWithContracts.DecompiledModule.AllTypes.Find(x => typeFullName == this.GetTypeFullName(x));
			var type = assembly.DecompiledModule.AllTypes.Find(x => typeFullName == this.GetTypeFullName(x));

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
				InjectContractsForType(contractsProviderForAssemblyWithContracts, contractsProviderForAssembly, typeWithContracts, type);

				Console.WriteLine("Injecting contracts into output assembly...");
				assembly.InjectContracts(contractsProviderForAssembly);

				Console.WriteLine("Saving output assembly...");
				assembly.Save(outputFileName);
			}

			assemblyWithContracts.Dispose();
			assembly.Dispose();
			host.Dispose();
		}

		private void InjectContracts(string outputFileName)
		{
			var host = new CodeContractAwareHostEnvironment(true);
			var assembly = new AssemblyInfo(host);
			var assemblyWithContracts = new AssemblyInfo(host);

			Console.WriteLine("Loading assembly...");
			assembly.Load(_AssemblyFileName);
			assembly.Decompile();

			Console.WriteLine("Loading contract reference assembly...");
			assemblyWithContracts.Load(_AssemblyWithContractsFileName);
			assemblyWithContracts.Decompile();

			Console.WriteLine("Extracting contracts from contract reference assembly...");
			var contractsProviderForAssemblyWithContracts = assemblyWithContracts.ExtractContracts();
			var contractsProviderForAssembly = new ContractProvider(contractsProviderForAssemblyWithContracts.ContractMethods, assembly.Module);

			var typesWithContracts = assemblyWithContracts.DecompiledModule.AllTypes;
			var types = assembly.DecompiledModule.AllTypes.ToDictionary(this.GetTypeFullName);

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
				InjectContractsForType(contractsProviderForAssemblyWithContracts, contractsProviderForAssembly, typeWithContracts, type);
			}

			Console.WriteLine("Injecting contracts into output assembly...");
			assembly.InjectContracts(contractsProviderForAssembly);

			Console.WriteLine("Saving output assembly...");
			assembly.Save(outputFileName);

			assemblyWithContracts.Dispose();
			assembly.Dispose();
			host.Dispose();
		}

		private static void InjectContractsForType(ContractProvider contractsProviderForAssemblyWithContracts, ContractProvider contractsProviderForAssembly, INamedTypeDefinition typeWithContracts, INamedTypeDefinition type)
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

		private string GetTypeFullName(INamedTypeDefinition type)
		{
			var genericParameters = type.IsGeneric ? type.GenericParameterCount.ToString() : string.Empty;
			return string.Format("{0}{1}", type, genericParameters);
		}
	}
}
