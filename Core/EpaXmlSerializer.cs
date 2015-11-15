using Contractor.Core.Model;
using Contractor.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class EpaXmlSerializer : ISerializer
    {
        public EpaXmlSerializer()
        {
        }

        public void Serialize(Stream stream, Epa epa)
        {
            Contract.Requires(stream != null && stream.CanWrite);
            Contract.Requires(epa != null && epa.Type != null);

            using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
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

        private void SerializeActions(Epa epa, XmlTextWriter writer)
        {
            Contract.Requires(epa != null);
            Contract.Requires(writer != null);

            SortedSet<string> actions = new SortedSet<string>(from t in epa.Transitions select t.Action.ToString());
            foreach (var a in actions)
            {
                writer.WriteStartElement("label");
                writer.WriteAttributeString("name", a);
                writer.WriteEndElement();
            }
        }

        private void SerializeStates(Epa epa, XmlTextWriter writer)
        {
            Contract.Requires(epa != null && epa.Type != null);
            Contract.Requires(writer != null);

            foreach (var s in epa.States)
            {
                writer.WriteStartElement("state");
                writer.WriteAttributeString("name", s.Name);

                foreach (var a in (s as State).EnabledActions)
                {
                    writer.WriteStartElement("enabled_label");
                    writer.WriteAttributeString("name", a.Method.GetDisplayName());
                    writer.WriteEndElement();
                }

                SerializeTransitions(epa, writer, s);

                writer.WriteEndElement(); // State
            }

        }

        private void SerializeTransitions(Epa epa, XmlTextWriter writer, State s)
        {
            Contract.Requires(epa != null && epa.Type != null);
            Contract.Requires(writer != null);
            Contract.Requires(s != null);

            SortedDictionary<string, List<Transition>> transitions = new SortedDictionary<string, List<Transition>>();
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
                    writer.WriteAttributeString("destination", kvp.Key.ToString());
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

            HashSet<Tuple<string, Action, string, bool>> transitions = new HashSet<Tuple<string, Model.Action, string, bool>>();
            Dictionary<string, HashSet<Action>> epaActions = new Dictionary<string, HashSet<Action>>();


            string name = null;
            for (bool read = true; read; reader.Read())
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

            Dictionary<string, State> translator = new Dictionary<string, State>();
            //TODO: arreglar las que estan deshabilitadas
            State initial = new State(epaActions.First(kvp => kvp.Key.Equals(initialState)).Value, new HashSet<Action>());
            
            EpaBuilder epaBuilder = new EpaBuilder(type, initial);
            
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
