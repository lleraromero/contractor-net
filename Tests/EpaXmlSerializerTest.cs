using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contractor.Core;
using System.IO;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Tests
{
    [TestClass]
    public class EpaXmlSerializerTest
    {
        private static EpaGenerator epaGenerator;
        private const string InputFilePath = @"..\..\..\Examples\obj\Debug\Decl\Examples.dll";

        [ClassInitialize()]
        public static void GenerateEPAs(TestContext tc)
        {
            Configuration.Initialize();
            Configuration.TempPath = Directory.GetParent(tc.TestDir).ToString();
            Configuration.InlineMethodsBody = true;

            epaGenerator = new EpaGenerator(EpaGenerator.Backend.Corral);
            var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, InputFilePath));
            epaGenerator.LoadAssembly(ExamplesPath);
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            epaGenerator.Dispose();
        }

        [TestMethod]
        public void SerializeAndDeserialize01()
        {
            var cancellationSource = new CancellationTokenSource();
            var epa = epaGenerator.GenerateEpa("Examples.Linear", cancellationSource.Token).EPA;

            SerializeAndDeserialize(epa);
        }
        public void SerializeAndDeserialize(Epa epa)
        {
            var filePath = Path.Combine(Configuration.TempPath, "epa.xml");
            // Export the EPA as an XML
            var serializer = new EpaXmlSerializer();
            using (Stream oStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                serializer.Serialize(oStream, epa);
            }
            using (Stream oStream = new FileStream(filePath, FileMode.Open))
            {
                var deserializedEPA = serializer.Deserialize(oStream, InputFilePath);
                Contract.Assert(epa.Equals(deserializedEPA));
            }
        }
    }
}
