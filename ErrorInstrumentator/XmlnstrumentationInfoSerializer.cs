using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ErrorInstrumentator
{
    public class XmlInstrumentationInfoSerializer
    {

        public static void Serialize(Stream stream, Dictionary<string, List<string>> dictionary)
        {
            var settings = new XmlWriterSettings
            {
                CloseOutput = false,
                Indent = true
            };

            using (var writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("methods");
                foreach (string method in dictionary.Keys)
                {
                    writer.WriteStartElement("method");
                    writer.WriteAttributeString("name", method);

                    List<string> list;
                    if (dictionary.TryGetValue(method, out list))
                    {
                        writer.WriteAttributeString("isPure", list.ElementAt(1));
                        writer.WriteAttributeString("return-type", list.ElementAt(0));
                        writer.WriteStartElement("exceptions");
                        for (int i = 2; i < list.Count; i++)
                        {
                            writer.WriteString(list.ElementAt(i) + " ");

                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

        }

        //public static Dictionary<string, List<string>> Deserialize(Stream stream)
        //{
        //    var result = new Dictionary<string, List<string>>();
        //    using (var reader = new XmlTextReader(stream))
        //    {
        //        reader.Read(); // Document
        //        reader.Read();
        //        reader.Read(); // Methods
        //        while (!(reader.Name == "methods" && reader.NodeType == XmlNodeType.EndElement))
        //        {
        //            while (!(reader.Name == "method" && reader.NodeType == XmlNodeType.Element))
        //            {
        //                reader.Read();
        //            }
        //            var methodName = reader.GetAttribute("name");
        //            var isPure = reader.GetAttribute("isPure");
        //            var returnType = reader.GetAttribute("return-type");
        //            while (reader.Name != "exceptions")
        //            {
        //                reader.Read();
        //            }
        //            string exceptions = "";
        //            while (!(reader.Name == "exceptions" && reader.NodeType == XmlNodeType.EndElement))
        //            {
        //                exceptions += reader.Value;
        //                reader.Read();
        //            }
        //            List<string> exceptionList = new List<string>(exceptions.Split(' '));
        //            exceptionList.RemoveAll(x => x.Equals(""));
        //            exceptionList.Insert(0, returnType);
        //            exceptionList.Insert(1, isPure);
        //            result.Add(methodName, exceptionList);
        //        }
        //    }
        //    return result;
        //}

        public static Dictionary<string, ErrorInstrumentator.MethodInfo> Deserialize(Stream stream)
        {
            var result = new Dictionary<string,ErrorInstrumentator.MethodInfo>();
            using (var reader = new XmlTextReader(stream))
            {
                reader.Read(); // Document
                reader.Read();
                reader.Read(); // Methods
                while (!(reader.Name == "methods" && reader.NodeType == XmlNodeType.EndElement))
                {
                    while (!(reader.Name == "method" && reader.NodeType == XmlNodeType.Element))
                    {
                        if (reader.NodeType == XmlNodeType.None)
                            return result;
                        reader.Read();
                    }
                    var methodName = reader.GetAttribute("name");
                    var isPure = reader.GetAttribute("isPure");
                    var isProperty = reader.GetAttribute("isProperty");
                    var returnType = reader.GetAttribute("return-type");
                    while (reader.Name != "exceptions")
                    {
                        reader.Read();
                    }
                    string exceptions = "";
                    while (!(reader.Name == "exceptions" && reader.NodeType == XmlNodeType.EndElement))
                    {
                        exceptions += reader.Value;
                        reader.Read();
                    }
                    List<string> exceptionList = new List<string>(exceptions.Split(' '));
                    exceptionList.RemoveAll(x => x.Equals(""));

                    var methodInfo = new ErrorInstrumentator.MethodInfo(methodName, returnType, isPure, exceptionList,isProperty);

                    result.Add(methodName, methodInfo);
                }
            }
            return result;
        }

    }
}
