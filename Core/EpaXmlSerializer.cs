using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Contractor.Core
{
    public class EpaXmlSerializer
    {
        public EpaXmlSerializer()
        {
        }

        public void Serialize(Stream stream, Epa epa)
        {
            Contract.Requires(stream != null && stream.CanWrite);
            Contract.Requires(epa != null && !string.IsNullOrEmpty(epa.Type));

            using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                writer.WriteStartElement("abstraction");
                writer.WriteAttributeString("initial_state", epa.Initial.Id.ToString());
                writer.WriteAttributeString("input_format", "code-with-pre");
                writer.WriteAttributeString("name", epa.Type);

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
            Contract.Requires(epa != null && !string.IsNullOrEmpty(epa.Type));
            Contract.Requires(writer != null);

            foreach (var s in epa.States)
            {
                writer.WriteStartElement("state");
                writer.WriteAttributeString("name", s.Id.ToString());

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
            Contract.Requires(epa != null && !string.IsNullOrEmpty(epa.Type));
            Contract.Requires(writer != null);
            Contract.Requires(s != null);

            SortedDictionary<uint, List<Transition>> transitions = new SortedDictionary<uint, List<Transition>>();
            foreach (var t in epa[s])
            {
                if (!transitions.ContainsKey(t.TargetState.Id))
                {
                    transitions.Add(t.TargetState.Id, new List<Transition>());
                }
                transitions[t.TargetState.Id].Add(t);
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

        public Epa Deserialize(Stream stream, string inputAssemblyPath)
        {
            Contract.Requires(stream != null && stream.CanRead);
            Contract.Requires(!string.IsNullOrEmpty(inputAssemblyPath));
            Contract.Ensures(Contract.Result<Epa>() != null);

            EpaBuilder epaBuilder;
            using (var reader = new XmlTextReader(stream))
            {
                reader.Read(); // Document
                reader.Read();
                reader.Read(); // Epa

                epaBuilder = new EpaBuilder(reader.GetAttribute("name"));
                uint initialState = uint.Parse(reader.GetAttribute("initial_state").Replace("Sinit", "0"));

                var resolvedType = FindType(inputAssemblyPath, epaBuilder.Type);
                Contract.Assert(resolvedType != null);
                DeserializeActions(reader);
                DeserializeStates(reader, epaBuilder, resolvedType, initialState);
            }

            return epaBuilder.Build();
        }

        private void DeserializeActions(XmlTextReader reader)
        {
            while (reader.Name != "state")
            {
                reader.Read();
            }
        }

        private NamespaceTypeDefinition FindType(string inputAssemblyPath, string type)
        {
            Contract.Requires(!string.IsNullOrEmpty(inputAssemblyPath) && !string.IsNullOrEmpty(type));

            var host = new CodeContractAwareHostEnvironment();

            // Load module
            var module = host.LoadUnitFrom(inputAssemblyPath) as IModule;
            if (module == null || module is Dummy || module == Dummy.Assembly)
                throw new Exception(string.Concat(inputAssemblyPath, " is not a PE file containing a CLR module or assembly."));

            // Load PDB
            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
                using (var pdbStream = File.OpenRead(pdbFile))
                    pdbReader = new PdbReader(pdbStream, host);

            // Decompile module
            var decompiledModule = Decompiler.GetCodeModelFromMetadataModel(host, module, pdbReader);

            // Find the type that I am trying to analyse
            var resolvedType = decompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>().First(t => TypeHelper.GetTypeName(t) == type);
            Contract.Assert(resolvedType != null);

            if (pdbReader != null)
            {
                pdbReader.Dispose();
            }

            return resolvedType;
        }

        private void DeserializeStates(XmlTextReader reader, EpaBuilder epaBuilder, NamespaceTypeDefinition type, uint initialState)
        {
            Contract.Requires(reader != null && epaBuilder != null && type != null);

            Dictionary<uint, List<Transition>> epaStructure = new Dictionary<uint, List<Transition>>();


            State s = null;
            for (bool read = true; read; reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "state":
                                if (s != null)
                                {
                                    epaBuilder.Add(s);
                                }
                                s = new State();
                                s.Id = uint.Parse(reader.GetAttribute("name").Replace("Sinit", "0").Replace("S", ""));

                                epaStructure[s.Id] = new List<Transition>();
                                break;
                            case "enabled_label":
                                var method = type.Methods.First(m => m.GetDisplayName() == reader.GetAttribute("name"));
                                s.EnabledActions.Add(new CciAction(method));
                                break;
                            case "transition":
                                method = type.Methods.First(m => m.GetDisplayName() == reader.GetAttribute("label"));
                                Contract.Assert(s != null);
                                var sourceState = s;
                                var targetState = new State() { Id = uint.Parse(reader.GetAttribute("destination").Replace("S", "")) };
                                var isUnproven = bool.Parse(reader.GetAttribute("uncertain"));
                                var t = new Transition(new CciAction(method), sourceState, targetState, isUnproven);
                                epaStructure[s.Id].Add(t);
                                break;
                            default:
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "abstraction")
                        {
                            if (s != null)
                            {
                                epaBuilder.Add(s);
                            }
                            read = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            epaBuilder.Initial = epaBuilder.States.First(state => state.Id == initialState);

            foreach (var kvp in epaStructure)
            {
                foreach (var t in kvp.Value)
                {
                    var targetState = epaBuilder.States.First(state => state.Id == t.TargetState.Id) as State;
                    var newTrans = new Transition(t.Action, t.SourceState, targetState, t.IsUnproven);
                    epaBuilder.Add(newTrans);
                }
            }
        }
    }
}
