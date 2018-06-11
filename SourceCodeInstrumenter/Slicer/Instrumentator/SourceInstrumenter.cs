using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using System;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using System.Xml;
using System.IO;

namespace DC.Slicer
{
    public class SourceInstrumenter
    {
        private Dictionary<int, string> fileIdAssoc = new Dictionary<int, string>();
        private Dictionary<int, string> lastInstrumentedFileIdAssoc;
        private Dictionary<string, int> pathToIdAssoc = new Dictionary<string, int>();
        private int fileId = 0;
        public string pathToOutputFiles;

        public SlicerConfig Config { get; private set; }

        public SourceInstrumenter(SlicerConfig conf)
        {
            Config = conf;
        }
        /*
         * Input: Original Compilation.
         * Output: Instrumented Compilation.
         * Idea: I call rewriter with the tree for that instrument. Replacement compilation with the new instrumented modified tree. Write in C: \ temp \ Instrumentado.cs the instrumented code
         */
        public CSharpCompilation Instrument(CSharpCompilation compilation)
        {
            lastInstrumentedFileIdAssoc = new Dictionary<int, string>();
            var assemblyReferences = new List<MetadataReference>();
            //assemblyReferences.Add(MetadataReference.CreateFromFile(typeof(TraceSender).Assembly.Location));
            assemblyReferences.Add(MetadataReference.CreateFromFile(typeof(ProtoBuf.ProtoWriter).Assembly.Location));
            var modifiedCompilation = compilation.AddReferences(assemblyReferences);

            foreach (var tree in compilation.SyntaxTrees)
            { 
                if (IgnoreSourceFile(tree.FilePath)) continue;          //Evita instrumentar de mas
                AssocIdToFile(tree.FilePath);
            }

            foreach (var tree in compilation.SyntaxTrees)
            {
                if (IgnoreSourceFile(tree.FilePath)) continue;          //Evita instrumentar de mas
                SemanticModel model = compilation.GetSemanticModel(tree);
                int idBelongingToThisPath = pathToIdAssoc[tree.FilePath];

                var classToRewrite = Config.FileToRewrite.Split('.').ElementAt(0);
                var rewriter = new InstrumenterRewriter(idBelongingToThisPath, model, pathToIdAssoc, classToRewrite);
                //var list = rewriter.Visit(tree.GetRoot());
                CSharpSyntaxNode newRoot = (CSharpSyntaxNode)rewriter.Visit(tree.GetRoot());
                //CSharpSyntaxNode newRoot = (CSharpSyntaxNode)list.First();

                //System.IO.File.WriteAllText(rewriter.pathToOutputFiles + "methodsINFO.xml", rewriter.methodExceptions.ToList().ToString());
                using (var stream = File.Create(pathToOutputFiles + "\\methodExceptions.xml"))
                {
                    writeToXML(stream,rewriter.methodExceptions);
                }
                //using (var stream2 = File.OpenRead(rewriter.pathToOutputFiles + "methodExceptions.xml"))
                //{
                //    var r = Deserialize(stream2);
                //}
                var instrumentedTree = (CSharpSyntaxTree)newRoot.SyntaxTree;
                System.IO.File.WriteAllText(@"C:\temp\Instrumentado.cs", instrumentedTree.ToString());
           
                instrumentedTree = (CSharpSyntaxTree)instrumentedTree.WithFilePath(tree.FilePath);
                modifiedCompilation = modifiedCompilation.ReplaceSyntaxTree(tree, instrumentedTree);
            }
            return modifiedCompilation;
        }

        private void writeToXML(Stream stream, Dictionary<string, List<string>> dictionary)
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
                    if (dictionary.TryGetValue(method,out list))
                    {
                        writer.WriteAttributeString("isPure", list.ElementAt(1));
                        writer.WriteAttributeString("isProperty", list.ElementAt(2));
                        writer.WriteAttributeString("return-type", list.ElementAt(0));
                        writer.WriteStartElement("exceptions");
                        writer.WriteString(" ");

                        for (int i = 3; i < list.Count;i++ )
                        {
                            //writer.WriteAttributeString("", list.ElementAt(i));
                            writer.WriteString(list.ElementAt(i)+" ");
                            
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            
        }

        private Dictionary<string, List<string>> Deserialize(Stream stream)
        {
            var result = new Dictionary<string, List<string>>();
            using (var reader = new XmlTextReader(stream))
            {
                reader.Read(); // Document
                reader.Read();
                reader.Read(); // Methods
                while (!(reader.Name == "methods" && reader.NodeType==XmlNodeType.EndElement))
                {
                    while (!(reader.Name == "method" && reader.NodeType == XmlNodeType.Element))
                    {
                        reader.Read();
                    }
                    var methodName = reader.GetAttribute("name");
                    var isPure = reader.GetAttribute("isPure");
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
                    exceptionList.RemoveAll(x=>x.Equals(""));
                    exceptionList.Insert(0, returnType);
                    exceptionList.Insert(1, isPure);
                    result.Add(methodName, exceptionList);
                }
             }
            return result;
        }

        /*
         *  Input: FilePath
         *  Output: false
         *  Idea: If exits AssemblyInfo.cs or AssemblyAtributes.cs files return true.
         */
        private bool IgnoreSourceFile(string filePath)
        {
            if (filePath.Contains("AssemblyInfo.cs")) return true;
            if (filePath.Contains("AssemblyAttributes.cs")) return true;
            if (!filePath.Contains(Config.FileToRewrite)) return true;
            return false;
        }

        /*
         *  Input: FilePath
         *  Output: false
         *  Idea: Set dicctionaries with actual file id.
         */
        private void AssocIdToFile(string filePath)
        {
            var id = ++fileId;
            fileIdAssoc.Add(id, filePath);
            lastInstrumentedFileIdAssoc.Add(id, filePath);
            pathToIdAssoc.Add(filePath, id);
        }

        /*
         *  Input: 
         *  Output: fileIdAssoc
         *  Idea: return fileIdAssoc
         */
        public Dictionary<int, string> FilesIds()
        {
            return fileIdAssoc;
        }

        /*
         *  Input: 
         *  Output: lastInstrumentedFileIdAssoc
         *  Idea: returna lastInstrumentedFileIdAssoc
         */
        public Dictionary<int, string> LastInstrumentedFileIds()
        {
            return lastInstrumentedFileIdAssoc;
        }
    }
}