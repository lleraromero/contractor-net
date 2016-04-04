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
                writer.WriteAttributeString("name", a.Name);
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
                    writer.WriteAttributeString("name", a.Name);
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
                writer.WriteAttributeString("label", t.Action.Name);
                writer.WriteAttributeString("uncertain", t.IsUnproven.ToString().ToLower());
                writer.WriteAttributeString("violates_invariant", "false"); //Contractor.NET does not support this attribute
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
                reader.Read(); // Epa

                var typename = reader.GetAttribute("name");

                var actions = DeserializeActions(reader);

                var constructorName = typename.Substring(typename.LastIndexOf('.')+1);

                var constructors= GetConstructors(actions.Item2);
                var typeDefinition = new StringTypeDefinition(typename,constructors,convert(actions.Item2));

                var states = DeserializeStates(reader);

                var translator = new Dictionary<string, State>();

                epaBuilder = new EpaBuilder(typeDefinition);

                foreach (var state in states)
                {
                    var enabledActions = new HashSet<Action>();
                    foreach (var t in state.Value)
                    {
                        enabledActions.Add(t.Item2);
                    }
                    var disabledActions = new HashSet<Action>(actions.Item1);
                    disabledActions.ExceptWith(enabledActions);

                    //IF IT'S A CONSTRUCTOR METHOD SET DISABLEDACTIONS TO A EMPTY SET. 
                    //BECAUSE CONSTRUCTOR IS NOT A NORMAL METHOD.
                    if (state.Value[0].Item2.ToString().Contains("_ctor"))
                    {
                        disabledActions = new HashSet<Action>();
                    }

                    var s = new State(enabledActions, disabledActions);

                    //s.Name = state.Key;

                    var stateName = state.Key;
                    translator[stateName] = s;
                }

                foreach (var state in states)
                {
                    foreach (var transition in state.Value)
                    {
                        epaBuilder.Add(new Transition(transition.Item2, translator[transition.Item1], translator[transition.Item3], transition.Item4,"True"));
                    }
                }
            }

            return epaBuilder.Build();
        }

        protected ISet<Action> convert(IReadOnlyCollection<Action> actions)
        {
            var result = new HashSet<Action>();
            foreach (var action in actions)
            {
                result.Add(action);
            }
            return result;
        }

        protected ISet<Action> GetConstructors(String typename, IReadOnlyCollection<Action> actions)
        {
            var constructors = new HashSet<Action>();
            foreach (var action in actions)
            {
                if (action.Name.Equals(typename) || action.Name.Contains(typename + "("))
                {
                    constructors.Add(action);
                }
            }
            return constructors;
        }

        protected ISet<Action> GetConstructors(IReadOnlyCollection<Action> actions)
        {
            var constructors = new HashSet<Action>();
            foreach (var action in actions)
            {
                if (action.Name.Contains("_ctor"))
                {
                    constructors.Add(action);
                }
            }
            return constructors;
        }

        protected Tuple<IReadOnlyCollection<Action>,IReadOnlyCollection<Action>> DeserializeActions(XmlTextReader reader)
        {
            var actions = new List<Action>();
            var actionsNames = new List<Action>();

            while (reader.Name != "state")
            {
                if(reader.Name == "")
                    reader.Read();
                if (reader.Name != "state")
                {
                    if (reader.Name == "label"){
                        actions.Add(new StringAction(reader.Name));
                        actionsNames.Add(new StringAction(reader.GetAttribute(0)));
                    }
                    reader.Read();
                }
            }
            return new Tuple<IReadOnlyCollection<Action>, IReadOnlyCollection<Action>>(actions, actionsNames);
        }

        protected Dictionary<string, List<Tuple<string, Action, string, bool>>> DeserializeStates(XmlTextReader reader)
        {   
            var states = new Dictionary<string, List<Tuple<string, Action, string, bool>>>();
            string name = null;
            do
            {
                var nodeType = reader.NodeType;
                if (nodeType.Equals(XmlNodeType.Element))
                {
                    switch (reader.Name)
                    {
                        case "state":
                            name = reader.GetAttribute("name");
                            states[name] = new List<Tuple<string, Action, string, bool>>();
                            break;
                        case "enabled_label":
                            break;
                        case "transition":
                            var sourceState = name;
                            var targetState = reader.GetAttribute("destination");
                            var isUnproven = bool.Parse(reader.GetAttribute("uncertain"));
                            var action = new StringAction(reader.GetAttribute("label"));
                            states[name].Add(new Tuple<string, Action, string, bool>(sourceState, action, targetState, isUnproven));
                            break;
                    }
                }
            } while (reader.Read());

            return states;
        }
    }
}