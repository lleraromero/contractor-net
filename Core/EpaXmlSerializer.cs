using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class EpaXmlSerializer : ISerializer
    {
        public void Serialize(Stream stream, Epa epa)
        {
            Contract.Requires(stream != null && stream.CanWrite);
            Contract.Requires(epa != null && epa.Type != null);

            var settings = new XmlWriterSettings()
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

        private void SerializeActions(Epa epa, XmlWriter writer)
        {
            Contract.Requires(epa != null);
            Contract.Requires(writer != null);

            var actions = new SortedSet<string>(from t in epa.Transitions select t.Action.ToString());
            foreach (var a in actions)
            {
                writer.WriteStartElement("label");
                writer.WriteAttributeString("name", a);
                writer.WriteEndElement();
            }
        }

        private void SerializeStates(Epa epa, XmlWriter writer)
        {
            Contract.Requires(epa != null && epa.Type != null);
            Contract.Requires(writer != null);

            foreach (var s in epa.States)
            {
                writer.WriteStartElement("state");
                writer.WriteAttributeString("name", s.Name);

                foreach (var a in s.EnabledActions)
                {
                    writer.WriteStartElement("enabled_label");
                    writer.WriteAttributeString("name", a.ToString());
                    writer.WriteEndElement();
                }

                SerializeTransitions(epa, writer, s);

                writer.WriteEndElement(); // State
            }
        }

        private void SerializeTransitions(Epa epa, XmlWriter writer, State s)
        {
            Contract.Requires(epa != null && epa.Type != null);
            Contract.Requires(writer != null);
            Contract.Requires(s != null);

            var transitions = new SortedDictionary<string, List<Transition>>();
            foreach (var t in epa.Transitions.Where(t => t.SourceState.Equals(s)))
            {
                if (!transitions.ContainsKey(t.TargetState.Name))
                {
                    transitions.Add(t.TargetState.Name, new List<Transition>());
                }
                transitions[t.TargetState.Name].Add(t);
            }

            foreach (var kvp in transitions)
            {
                foreach (var t in kvp.Value)
                {
                    writer.WriteStartElement("transition");
                    writer.WriteAttributeString("destination", kvp.Key);
                    writer.WriteAttributeString("label", t.Action.ToString());
                    writer.WriteAttributeString("uncertain", t.IsUnproven.ToString().ToLower());
                    writer.WriteAttributeString("violates_invariant", "false"); //Contractor.NET does not support this attribute
                    writer.WriteEndElement();
                }
            }
        }

        public Epa Deserialize(Stream stream)
        {
            Contract.Requires(stream != null && stream.CanRead);
            Contract.Ensures(Contract.Result<Epa>() != null);

            //TODO: arreglar
            throw new NotImplementedException();
            //EpaBuilder epaBuilder;
            //using (var reader = new XmlTextReader(stream))
            //{
            //    reader.Read(); // Document
            //    reader.Read();
            //    reader.Read(); // Epa

            //    string type = reader.GetAttribute("name");
            //    string initialState = reader.GetAttribute("initial_state");

            //    DeserializeActions(reader);
            //    epaBuilder = DeserializeStates(reader, type, initialState);
            //}

            //return epaBuilder.Build();
        }

        private void DeserializeActions(XmlTextReader reader)
        {
            while (reader.Name != "state")
            {
                reader.Read();
            }
        }

        private EpaBuilder DeserializeStates(XmlTextReader reader, TypeDefinition type, string initialState)
        {
            Contract.Requires(reader != null);
            Contract.Requires(type != null);
            Contract.Requires(!string.IsNullOrEmpty(initialState));

            var transitions = new HashSet<Tuple<string, Action, string, bool>>();
            var epaActions = new Dictionary<string, HashSet<Action>>();


            string name = null;
            for (var read = true; read; reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "state":
                                name = reader.GetAttribute("name");
                                epaActions[name] = new HashSet<Action>();
                                break;
                            case "enabled_label":
                                epaActions[name].Add(new StringAction(reader.GetAttribute("name")));
                                break;
                            case "transition":
                                var sourceState = name;
                                var targetState = reader.GetAttribute("destination");
                                var isUnproven = bool.Parse(reader.GetAttribute("uncertain"));
                                var action = new StringAction(reader.GetAttribute("label"));
                                var t = new Tuple<string, Action, string, bool>(sourceState, action, targetState, isUnproven);
                                transitions.Add(t);
                                break;
                            default:
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "abstraction")
                        {
                            read = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            var translator = new Dictionary<string, State>();
            //TODO: arreglar las que estan deshabilitadas
            var initial = new State(epaActions.First(kvp => kvp.Key.Equals(initialState)).Value, new HashSet<Action>());

            var epaBuilder = new EpaBuilder(type);
            epaBuilder.SetStateAsInitial(initial);

            foreach (var kvp in epaActions)
            {
                //TODO: arreglar las que estan deshabilitadas
                var s = new State(kvp.Value, new HashSet<Action>());

                if (!kvp.Key.Equals(initialState))
                {
                    epaBuilder.Add(s);
                }

                translator[kvp.Key] = s;
            }

            foreach (var t in transitions)
            {
                var transition = new Transition(t.Item2, translator[t.Item1], translator[t.Item3], t.Item4);
                epaBuilder.Add(transition);
            }

            return epaBuilder;
        }
    }
}