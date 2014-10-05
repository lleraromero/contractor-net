using Contractor.Utils;
using Microsoft.Cci.MutableContracts;
using System;

namespace ContractsMerger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var assemblyFileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll";
            var assemblyWithContractsFileName = @"C:\Program Files (x86)\Microsoft\Contracts\Contracts\.NETFramework\v4.0\System.Contracts.dll";
            var outputFileName = @"C:\Users\Eddy\Documents\Tesis\temp\System.dll";

            //var assemblyFileName = @"C:\Users\Eddy\Documents\Tesis\Contractor\Test\API_Examples\bin\Debug\API_Examples.dll";
            //var assemblyWithContractsFileName = @"C:\Users\Eddy\Documents\Tesis\Contractor\Test\API_Examples\bin\Debug\CodeContracts\API_Examples.Contracts.dll";
            //var outputFileName = @"C:\Users\Eddy\Documents\Tesis\temp\API_Examples.dll";

            using (var host = new CodeContractAwareHostEnvironment(true))
            {
                var assembly = new AssemblyInfo(host);
                var assemblyWithContracts = new AssemblyInfo(host);

                Console.WriteLine("Loading input assembly...");
                assembly.Load(assemblyFileName);

                Console.WriteLine("Decompiling input assembly...");
                assembly.Decompile();

                Console.WriteLine("Loading contract reference assembly...");
                assemblyWithContracts.Load(assemblyWithContractsFileName);

                Console.WriteLine("Extracting contracts from contract reference assembly...");
                var contractProvider = assembly.ExtractContracts();

                Console.WriteLine("Injecting contracts into output assembly...");
                assembly.InjectContracts(contractProvider);

                Console.WriteLine("Saving output assembly...");
                assembly.Save(outputFileName);
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}