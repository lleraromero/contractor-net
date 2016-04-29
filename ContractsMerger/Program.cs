using System;
using Analysis.Cci;

namespace ContractsMerger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var assemblyFileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll";
            var assemblyWithContractsFileName = @"C:\Program Files (x86)\Microsoft\Contracts\Contracts\.NETFramework\v4.0\System.Contracts.dll";
            var outputFileName = @"C:\Users\Eddy\Documents\Tesis\temp\System.dll";

            var persister = new CciAssemblyPersister();

            Console.WriteLine("Loading input assembly...");
            Console.WriteLine("Loading contract reference assembly...");
            var assembly = persister.Load(assemblyFileName, assemblyWithContractsFileName);

            Console.WriteLine("Injecting contracts into output assembly...");
            Console.WriteLine("Saving output assembly...");
            persister.Save(assembly, outputFileName);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}