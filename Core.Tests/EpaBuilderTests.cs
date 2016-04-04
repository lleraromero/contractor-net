using System.Collections.Generic;
using Contractor.Core;
using Contractor.Core.Model;
using FakeItEasy;
using Xunit;

namespace Core.Tests
{
    //  TODO: arreglar nombres / resolver como crear los estados para los tests porque esta por default el estado de los constructores typedefinition
    public class EpaBuilderTests
    {
        [Fact]
        public void Empty_Builder_Should_Have_An_Empty_State_And_No_Transitions()
        {
            var dummyTypeDefinition = A.Dummy<ITypeDefinition>();
            var epaBuilder = new EpaBuilder(dummyTypeDefinition);

            Assert.Equal(dummyTypeDefinition, epaBuilder.Type);

            var emptyState = CreateEmptyState();
            Assert.Contains(emptyState, epaBuilder.States);
            Assert.Equal(1, epaBuilder.States.Count);

            Assert.Empty(epaBuilder.Transitions);
        }

        [Fact]
        public void Add_Transition_With_One_State()
        {
            var dummyTypeDefinition = A.Dummy<ITypeDefinition>();
            var epaBuilder = new EpaBuilder(dummyTypeDefinition);

            var dummyState = A.Dummy<State>();
            var dummyAction = A.Dummy<Action>();
            A.CallTo(() => dummyAction.Equals(dummyAction)).Returns(true);

            var fakeTransition =
                A.Fake<Transition>(x => x.WithArgumentsForConstructor(() => new Transition(dummyAction, dummyState, dummyState, false, "True")));

            epaBuilder.Add(fakeTransition);

            Assert.Contains(fakeTransition, epaBuilder.Transitions);
            Assert.Equal(1, epaBuilder.Transitions.Count);

            Assert.Contains(dummyState, epaBuilder.States);
            Assert.Contains(CreateEmptyState(), epaBuilder.States);
            Assert.Equal(2, epaBuilder.States.Count);

            Assert.Equal(dummyTypeDefinition, epaBuilder.Type);
        }

        [Fact]
        public void Add_Transition_With_Two_States()
        {
            var dummyTypeDefinition = A.Dummy<ITypeDefinition>();
            var epaBuilder = new EpaBuilder(dummyTypeDefinition);

            var action1 = A.Dummy<Action>();
            A.CallTo(() => action1.Equals(action1)).Returns(true);
            var action2 = A.Dummy<Action>();
            A.CallTo(() => action2.Equals(action2)).Returns(true);

            var dummyStateSrc =
                A.Fake<State>(x => x.WithArgumentsForConstructor(() => new State(new HashSet<Action> { action1 }, A.Dummy<ISet<Action>>())));
            var dummyStateDest =
                A.Fake<State>(x => x.WithArgumentsForConstructor(() => new State(new HashSet<Action> { action2 }, A.Dummy<ISet<Action>>())));

            var transition =
                A.Fake<Transition>(x => x.WithArgumentsForConstructor(() => new Transition(action1, dummyStateSrc, dummyStateDest, false, "True")));

            epaBuilder.Add(transition);

            Assert.Equal(1, epaBuilder.Transitions.Count);
            Assert.Contains(transition, epaBuilder.Transitions);

            Assert.Contains(dummyStateSrc, epaBuilder.States);
            Assert.Contains(dummyStateDest, epaBuilder.States);
            Assert.Contains(CreateEmptyState(), epaBuilder.States);
            Assert.Equal(3, epaBuilder.States.Count);

            Assert.Equal(dummyTypeDefinition, epaBuilder.Type);
        }

        [Fact]
        public void Empty_Builder_Builds_Empty_Epa()
        {
            var dummyTypeDefinition = A.Dummy<ITypeDefinition>();
            var epaBuilder = new EpaBuilder(dummyTypeDefinition);

            var epa = epaBuilder.Build();
            Assert.Equal(dummyTypeDefinition, epa.Type);

            Assert.Empty(epa.Transitions);

            Assert.Contains(CreateEmptyState(), epa.States);
            Assert.Equal(1, epa.States.Count);
        }

        protected State CreateEmptyState()
        {
            return new State(A.Dummy<ISet<Action>>(), A.Dummy<ISet<Action>>());
        }
    }
}