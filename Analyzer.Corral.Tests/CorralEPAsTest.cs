using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable InconsistentNaming

namespace Analyzer.Corral.Tests
{
    [TestClass]
    public class CorralEPAsTest
    {
        //[TestMethod]
        //public void TestVendingMachine()
        //{
        //    var typeToAnalyze = FindTypeDefinitionInAssemblyWithName("Examples.VendingMachine");
        //    var epaGenerator = new EpaGenerator(new AnalyzerMock());
        //    var result = epaGenerator.GenerateEpa(typeToAnalyze, A.Dummy<IEpaBuilder>());

        //    Assert.IsFalse(result.IsFaulted);
        //    Assert.IsFalse(result.IsCanceled);

        //    Assert.IsNotNull(result.Result.Epa);
        //}

        protected ITypeDefinition FindTypeDefinitionInAssemblyWithName(string typeName)
        {
            var examplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));
            var inputAssembly = new CciAssemblyPersister().Load(examplesPath, null);

            return inputAssembly.Types().First(t => t.Name.Equals(typeName));
        }

        //[TestMethod]
        //public void TestLinear()
        //{
        //    GenerateEpa("Examples.Linear");
        //}

        //[TestMethod]
        //public void TestDoor()
        //{
        //    GenerateEpa("Examples.Door");
        //}

        //[TestMethod]
        //public void TestFiniteStack()
        //{
        //    GenerateEpa("Examples.FiniteStack");
        //}
    }
}