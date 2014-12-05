using System;
using System.Collections.Generic;
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
            using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                var typeFullName = "Epa.AnalyzedType";

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("Epa");
                writer.WriteAttributeString("type", typeFullName);

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

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                
                writer.WriteStartElement("Transitions");
                foreach (var t in epa.Transitions)
                {
                    writer.WriteStartElement("Transition");

                    writer.WriteAttributeString("Name", t.Name);
                    // TODO: find the source state for this transition
                    //writer.WriteAttributeString("SourceState", t.SourceState.Id.ToString());
                    writer.WriteAttributeString("TargetState", t.TargetState.Id.ToString());
                    writer.WriteAttributeString("IsUnproven", t.IsUnproven.ToString());

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
