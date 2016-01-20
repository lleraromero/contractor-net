using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contractor.Core;
using Contractor.Core.Model;
using Xunit;

namespace Core.Tests
{
    public class EpaXmlSerializerTest
    {
        protected Epa NoConstructorsNoActionsEpa()
        {
            var constructors = new HashSet<Action>();
            var actions = new HashSet<Action>();
            var typeDefinition = new StringTypeDefinition("NoConstructorsNoActiosnEpa", constructors, actions);

            return new Epa(typeDefinition, new List<Transition>());
        }

        protected Epa OneConstructorNoActionsEpa()
        {
            var constructors = new HashSet<Action>();
            var constructor = new StringAction("ctor");
            constructors.Add(constructor);
            var actions = new HashSet<Action>();
            var typeDefinition = new StringTypeDefinition("OneConstructorNoActionsEpa", constructors, actions);

            var transitions = new HashSet<Transition>();
            var s1 = new State(constructors, new HashSet<Action>());
            var s2 = new State(new HashSet<Action>(), new HashSet<Action>());

            transitions.Add(new Transition(constructor, s1, s2, false));

            return new Epa(typeDefinition, transitions.ToList());
        }

        [Fact]
        public void SerializeEpaWithNoConstructorsAndNoActions()
        {
            var epa = NoConstructorsNoActionsEpa();
            using (var stream = new MemoryStream())
            {
                new EpaXmlSerializer().Serialize(stream, epa);

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    const string expected =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
<abstraction initial_state=""STATE$deadlock"" input_format=""code-with-pre"" name=""NoConstructorsNoActiosnEpa"">
  <state name=""STATE$deadlock"" />
</abstraction>";
                    Assert.Equal(expected, reader.ReadToEnd());
                }
            }
        }

        [Fact]
        public void SerializeEpaWithOneConstructorAndNoActions()
        {
            var epa = OneConstructorNoActionsEpa();
            using (var stream = new MemoryStream())
            {
                new EpaXmlSerializer().Serialize(stream, epa);

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    const string expected =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
<abstraction initial_state=""STATE$ctor"" input_format=""code-with-pre"" name=""OneConstructorNoActionsEpa"">
  <label name=""ctor"" />
  <state name=""STATE$ctor"">
    <enabled_label name=""ctor"" />
    <transition destination=""STATE$deadlock"" label=""ctor"" uncertain=""false"" violates_invariant=""false"" />
  </state>
  <state name=""STATE$deadlock"" />
</abstraction>";
                    Assert.Equal(expected, reader.ReadToEnd());
                }
            }
        }
    }
}