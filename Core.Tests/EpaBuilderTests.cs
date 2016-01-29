using System.Collections.Generic;
using System.Collections.Immutable;
using Contractor.Core;
using Contractor.Core.Model;
using FakeItEasy;
using Xunit;
using Action = Contractor.Core.Model.Action;

namespace Core.Tests
{
    public class EpaBuilderTests
    {
        [Fact]
        public void Empty_Builder_Should_Not_Have_Any_States_Nor_Transitions()
        {
            var dummyTypeDefinition = A.Dummy<ITypeDefinition>();
            var epaBuilder = new EpaBuilder(dummyTypeDefinition);

            Assert.Equal(dummyTypeDefinition, epaBuilder.Type);
            Assert.Equal(new List<State>(), epaBuilder.States);
            Assert.Equal(new List<Transition>(), epaBuilder.Transitions);
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
                A.Fake<Transition>(x => x.WithArgumentsForConstructor(() => new Transition(dummyAction, dummyState, dummyState, false)));

            epaBuilder.Add(fakeTransition);

            Assert.Equal(1, epaBuilder.Transitions.Count);
            Assert.Contains(fakeTransition, epaBuilder.Transitions);

            Assert.Equal(1, epaBuilder.States.Count);
            Assert.Contains(dummyState, epaBuilder.States);

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
                A.Fake<Transition>(x => x.WithArgumentsForConstructor(() => new Transition(action1, dummyStateSrc, dummyStateDest, false)));

            epaBuilder.Add(transition);

            Assert.Equal(1, epaBuilder.Transitions.Count);
            Assert.Contains(transition, epaBuilder.Transitions);

            Assert.Equal(2, epaBuilder.States.Count);
            Assert.Contains(dummyStateSrc, epaBuilder.States);
            Assert.Contains(dummyStateDest, epaBuilder.States);

            Assert.Equal(dummyTypeDefinition, epaBuilder.Type);
        }

        [Fact]
        public void Empty_Builder_Builds_Empty_Epa()
        {
            var dummyTypeDefinition = A.Dummy<ITypeDefinition>();
            var epaBuilder = new EpaBuilder(dummyTypeDefinition);

            var epa = epaBuilder.Build();
            Assert.Equal(dummyTypeDefinition, epa.Type);

            Assert.Equal(ImmutableHashSet<Transition>.Empty, epa.Transitions);
            Assert.Equal(1, epa.States.Count);
        }
    }
}