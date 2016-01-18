using Contractor.Core;
using Contractor.Core.Model;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Contractor.Core.Model.Action;
using System.Collections.Generic;

namespace Core.Tests
{
    [TestClass]
    public class EpaGeneratorTest
    {
        [TestMethod]
        public void GeneratesEmptyEpa()
        {
            var emptyTypeDefinition = A.Fake<TypeDefinition>();
            A.CallTo(() => emptyTypeDefinition.Constructors())
                       .Returns(A.Dummy<ISet<Action>>());
            A.CallTo(() => emptyTypeDefinition.Actions())
                       .Returns(A.Dummy<ISet<Action>>());

            var dummyAnalyzer = A.Dummy<IAnalyzer>();

            var epaGenerator = new EpaGenerator(dummyAnalyzer);
            var epaGenerationTask = epaGenerator.GenerateEpa(emptyTypeDefinition, A.Dummy<IEpaBuilder>());
            
            epaGenerationTask.Wait();
            Assert.IsFalse(epaGenerationTask.IsFaulted);
            Assert.IsFalse(epaGenerationTask.IsCanceled);

            var epa = epaGenerationTask.Result.Epa;
            //Assert.AreEqual(emptyTypeDefinition, epa.Type);
            //TODO: reemplazar por objetos que sean iguales al resultado. No romper el encapsulamiento usando .Count
            Assert.AreEqual(1, epa.States.Count);
            Assert.AreEqual(0, epa.Initial.EnabledActions.Count);
            Assert.AreEqual(0, epa.Initial.DisabledActions.Count);
            Assert.AreEqual(0, epa.Transitions.Count);
        }
    }
}