using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class EpaXmlSerializer : ISerializer
    {
        public void Serialize(Stream stream, Epa epa)
        {
            var settings = new XmlWriterSettings
            {
                CloseOutput = false,
                Indent = true
            };

            using (var writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("abstraction");
                writer.WriteAttributeString("initial_state", epa.Initial.Name);
                writer.WriteAttributeString("input_format", "code-with-pre");
                writer.WriteAttributeString("name", epa.Type.Name);

                SerializeActions(epa, writer);
                SerializeStates(epa, writer);

                writer.WriteEndDocument();
            }
        }

        protected void SerializeActions(Epa epa, XmlWriter writer)
        {
            Contract.Requires(epa != null);
            Contract.Requires(writer != null);

            var actions = new HashSet<Action>(from t in epa.Transitions orderby t.Action.Name select t.Action);
            foreach (var a in actions)
            {
                writer.WriteStartElement("label");
                writer.WriteAttributeString("name", a.ToString());
                writer.WriteEndElement();
            }
        }

        protected void SerializeStates(Epa epa, XmlWriter writer)
        {
            Contract.Requires(epa != null && epa.Type != null);
            Contract.Requires(writer != null);

            var states = new List<State>(from s in epa.States orderby s.Name select s);
            foreach (var s in states)
            {
                writer.WriteStartElement("state");
                writer.WriteAttributeString("name", s.Name);

                var enabledActions = new List<Action>(from a in s.EnabledActions orderby a.Name select a);
                foreach (var a in enabledActions)
                {
                    writer.WriteStartElement("enabled_label");
                    writer.WriteAttributeString("name", a.ToString());
                    writer.WriteEndElement();
                }

                SerializeTransitions(epa, writer, s);

                writer.WriteEndElement(); // State
            }
        }

        protected void SerializeTransitions(Epa epa, XmlWriter writer, State s)
        {
            Contract.Requires(epa != null && epa.Type != null);
            Contract.Requires(writer != null);
            Contract.Requires(s != null);
            
            var transitionsFromState = epa.Transitions.Where(t => t.SourceState.Equals(s));
            var transitions = (from t in transitionsFromState select t).OrderBy(x => x.TargetState.Name).ThenByDescending(x => x.Action.Name);
            foreach (var t in transitions)
            {
                writer.WriteStartElement("transition");
                writer.WriteAttributeString("destination", t.TargetState.Name);
                writer.WriteAttributeString("label", t.Action.ToString());
                writer.WriteAttributeString("uncertain", t.IsUnproven.ToString().ToLower());
                //Contractor.NET does not support this attribute
                writer.WriteAttributeString("violates_invariant", "false");
                writer.WriteEndElement();
            }
        }

        public Epa Deserialize(Stream stream)
        {
            Contract.Requires(stream != null && stream.CanRead);
            Contract.Ensures(Contract.Result<Epa>() != null);

            EpaBuilder epaBuilder;
            using (var reader = new XmlTextReader(stream))
            {
                reader.Read(); // Document
                reader.Read();
                reader.Read(); // Abstraction

                var typename = reader.GetAttribute("name");

                var actions = DeserializeActions(reader);
                // TODO: arreglar, ctor no garantiza que sea un constructor
                var constructors = new HashSet<Action>(actions.Where(a => a.Name.Contains("MaquinaExpendedora")));
                var typeDefinition = new StringTypeDefinition(typename, constructors, new HashSet<Action>(actions.Except(constructors)));
                
                var states = DeserializeStates(reader, actions);

                var translator = new Dictionary<string, State>();

                foreach (var state in states)
                {
                    translator[state.Key] = state.Value.First().Item1;
                }
                
                epaBuilder = new EpaBuilder(typeDefinition);

                foreach (var state in states)
                {
                    foreach (var transition in state.Value)
                    {
                        epaBuilder.Add(new Transition(transition.Item2, transition.Item1, translator[transition.Item3], transition.Item4));
                    }
                }
            }

            return epaBuilder.Build();
        }

        protected IReadOnlyCollection<Action> DeserializeActions(XmlTextReader reader)
        {
            var actions = new List<Action>();

            reader.Read();
            reader.Read();
            while (reader.Name.Equals("label"))
            {
                actions.Add(new StringAction(reader.GetAttribute("name")));

                reader.Read();
                reader.Read();
            }

            return actions;
        }

        protected Dictionary<string, List<Tuple<State, Action, string, bool>>> DeserializeStates(XmlTextReader reader, IReadOnlyCollection<Action> actions)
        {   
            var states = new Dictionary<string, List<Tuple<State, Action, string, bool>>>();

            while (reader.Name.Equals("state"))
            {
                var stateName = reader.GetAttribute("name");

                var enabledActions = new HashSet<Action>();

                reader.Read();
                reader.Read();
                while (reader.Name.Equals("enabled_label"))
                {
                    var actionName = reader.GetAttribute("name");
                    enabledActions.Add(actions.First(a => a.Name.Equals(actionName)));

                    reader.Read();
                    reader.Read();
                }

                var disabledActions = enabledActions.All(a => a.Name.Contains("MaquinaExpendedora")) ? new HashSet<Action>(actions.Except(enabledActions)) : new HashSet<Action>();

                var state = new State(enabledActions, disabledActions);
                
                var transitions = new List<Tuple<State, Action, string, bool>>();
                while (reader.Name.Equals("transition"))
                {
                    var targetState = reader.GetAttribute("destination");
                    var label = reader.GetAttribute("label");
                    var action = actions.First(a => a.Name.Equals(label));
                    var uncertain = bool.Parse(reader.GetAttribute("uncertain"));

                    var transition = new Tuple<State, Action, string, bool>(state, action, targetState, uncertain);
                    transitions.Add(transition);

                    reader.Read();
                    reader.Read();
                }

                states.Add(stateName, transitions);

                reader.Read();
                reader.Read();
            }

            return states;
        }
    }
}