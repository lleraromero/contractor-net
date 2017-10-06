using System.Collections.Generic;
using Contractor.Core;
using Contractor.Core.Model;
using FakeItEasy;
using Xunit;

namespace Core.Tests
{
    public class EpaGeneratorTest
    {
        [Fact]
        public void GeneratesEmptyEpa()
        {
            var emptyTypeDefinition = A.Fake<ITypeDefinition>();
            A.CallTo(() => emptyTypeDefinition.Constructors())
                .Returns(A.Dummy<ISet<Action>>());
            A.CallTo(() => emptyTypeDefinition.Actions())
                .Returns(A.Dummy<ISet<Action>>());

            var dummyAnalyzer = A.Dummy<IAnalyzerFactory>();

            var epaGenerator = new EpaGenerator(dummyAnalyzer,-1,true,4);
            var epaGenerationTask = epaGenerator.GenerateEpa(emptyTypeDefinition, A.Dummy<IEpaBuilder>());

            epaGenerationTask.Wait();
            Assert.False(epaGenerationTask.IsFaulted);
            Assert.False(epaGenerationTask.IsCanceled);

            var epa = epaGenerationTask.Result.Epa;
            //Assert.AreEqual(emptyTypeDefinition, epa.Type);
            //TODO: reemplazar por objetos que sean iguales al resultado. No romper el encapsulamiento usando .Count
            Assert.Equal(1, epa.States.Count);
            Assert.Equal(0, epa.Initial.EnabledActions.Count);
            Assert.Equal(0, epa.Initial.DisabledActions.Count);
            Assert.Equal(0, epa.Transitions.Count);
        }
    }
}