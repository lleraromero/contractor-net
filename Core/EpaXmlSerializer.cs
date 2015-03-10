using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
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

                writer.WriteStartElement("Epa");
                writer.WriteAttributeString("type", epa.Type);

                SerializeStates(epa, writer);
                SerializeTransitions(epa, writer);

                writer.WriteEndDocument();
            }
        }

        private void SerializeStates(Epa epa, XmlTextWriter writer)
        {
            Contract.Requires(epa != null && !string.IsNullOrEmpty(epa.Type));
            Contract.Requires(writer != null);

            writer.WriteStartElement("States");
            foreach (var s in epa.States)
            {
                writer.WriteStartElement("State");

                writer.WriteAttributeString("Id", s.Id.ToString());
                writer.WriteAttributeString("IsInitial", s.IsInitial.ToString());
                writer.WriteAttributeString("Name", s.Name);

                writer.WriteStartElement("EnabledActions");
                foreach (var a in s.EnabledActions)
                {
                    writer.WriteStartElement("Action");
                    writer.WriteAttributeString("Name", a);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndElement(); // State
            }
            writer.WriteEndElement(); //States
        }

        private void SerializeTransitions(Epa epa, XmlTextWriter writer)
        {
            Contract.Requires(epa != null && !string.IsNullOrEmpty(epa.Type));
            Contract.Requires(writer != null);

            writer.WriteStartElement("Transitions");
            foreach (var t in epa.Transitions)
            {
                writer.WriteStartElement("Transition");

                writer.WriteAttributeString("Name", t.Action);
                writer.WriteAttributeString("SourceState", t.SourceState.Id.ToString());
                writer.WriteAttributeString("TargetState", t.TargetState.Id.ToString());
                writer.WriteAttributeString("IsUnproven", t.IsUnproven.ToString());

                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // Transitions
        }

        public Epa Deserialize(Stream stream, string inputAssemblyPath)
        {
            Contract.Requires(stream != null && stream.CanRead);
            Contract.Requires(!string.IsNullOrEmpty(inputAssemblyPath));
            Contract.Ensures(Contract.Result<Epa>() != null);

            var epa = new Epa();
            using (var reader = new XmlTextReader(stream))
            {
                reader.Read(); // Document
                reader.Read();
                reader.Read(); // Epa
                epa.Type = reader.GetAttribute("type");

                var resolvedType = FindType(inputAssemblyPath, epa.Type);
                Contract.Assert(resolvedType != null);

                DeserializeStates(reader, epa, resolvedType);
                DeserializeTransitions(reader, epa, resolvedType);
            }

            return epa;
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

        private void DeserializeStates(XmlTextReader reader, Epa epa, NamespaceTypeDefinition type)
        {
            Contract.Requires(reader != null && epa != null && type != null);

            State s = null;
            bool enabledAction = true;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "State":
                                s = new State();
                                s.Id = uint.Parse(reader.GetAttribute("Id"));
                                s.IsInitial = bool.Parse(reader.GetAttribute("IsInitial"));
                                break;
                            case "EnabledActions":
                                enabledAction = true;
                                break;
                            case "DisabledActions":
                                enabledAction = false;
                                break;
                            case "Action":
                                var method = type.Methods.First(m => m.GetDisplayName() == reader.GetAttribute("Name"));
                                if (enabledAction)
                                {
                                    s.EnabledActions.Add(method);
                                }
                                else
                                {
                                    s.DisabledActions.Add(method);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "States") return;
                        if (reader.Name == "State")
                        {
                            Contract.Assert(s != null);
                            epa.AddState(s);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void DeserializeTransitions(XmlTextReader reader, Epa epa, NamespaceTypeDefinition type)
        {
            Contract.Requires(reader != null && epa != null && type != null);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "Transition":
                                var method = type.Methods.First(m => m.GetDisplayName() == reader.GetAttribute("Name"));
                                var sourceState = epa.States.First(s => s.Id == uint.Parse(reader.GetAttribute("SourceState"))) as State;
                                var targetState = epa.States.First(s => s.Id == uint.Parse(reader.GetAttribute("TargetState"))) as State;
                                var isUnproven = bool.Parse(reader.GetAttribute("IsUnproven"));
                                var t = new Transition(method, sourceState, targetState, isUnproven);
                                epa.AddTransition(t);
                                break;
                            default:
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "Transitions") return;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
