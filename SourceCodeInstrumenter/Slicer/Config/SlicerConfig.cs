using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace DC.Slicer
{
    public class SlicerConfig
    {
        public Solution Solution { get; set; }
        public string SlnFile { get; set; }

        public string FileToRewrite { get; set; }
        public string Project { get; set; }
        public string CriteriaFile { get; set; }
        public int CriteriaLineOfCode { get; set; }
        public string OutputDir { get; set; }
        public bool MSBuildCompilation { get; set; }

        public static async Task<SlicerConfig> FromXml(string xmlFile)
        {
            SlicerConfig conf = new SlicerConfig();

            var xml = File.ReadAllText(xmlFile);

            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                var doc = new XmlDocument();
                doc.Load(reader);
                var solutionTag = doc.GetElementsByTagName("Solution")[0];
                var criteriaTag = doc.GetElementsByTagName("Criteria")[0];
                var compilationOutputTag = doc.GetElementsByTagName("CompilationOutput")[0];
                var msBuildCompilation = doc.GetElementsByTagName("MSBuildCompilation")[0];
                var traceInput = doc.GetElementsByTagName("TraceInput")[0];
                var fileTraceInput = doc.GetElementsByTagName("FileTraceInput")[0];

                if (solutionTag == null || solutionTag.Attributes == null
                    || criteriaTag == null || criteriaTag.Attributes == null
                    || compilationOutputTag == null || compilationOutputTag.Attributes == null)
                {
                    throw new ArgumentException("SlicerConf XML is not well formed");
                }

                string solFile = solutionTag.Attributes["File"].Value;
                if (!File.Exists(solFile)) throw new ArgumentException("SLN solution does not exists.");

                bool overwrite = msBuildCompilation == null ? false : true;
                string outputDir = compilationOutputTag.Attributes["Dir"].Value;

                var workspace = MSBuildWorkspace.Create();
                conf.SlnFile = solFile;
                conf.Solution = await workspace.OpenSolutionAsync(solFile);
                conf.Project = solutionTag.Attributes["ProjectRef"].Value;
                conf.CriteriaFile = criteriaTag.Attributes["File"].Value;
                conf.CriteriaLineOfCode = Int32.Parse(criteriaTag.Attributes["LineOfCode"].Value);
                conf.OutputDir = outputDir;
                conf.MSBuildCompilation = overwrite;
                conf.FileTraceInput = fileTraceInput == null ? false : true;
                if (traceInput != null)
                {
                    conf.TraceInput = traceInput.Attributes["File"].Value;
                    conf.FileTraceInput = true;
                }

                //if (!Directory.Exists(conf.OutputDir)) throw new ArgumentException("Output dir does not exists");
                if (Directory.Exists(conf.OutputDir))
                {
                    Directory.Delete(conf.OutputDir, true);
                }
                Directory.CreateDirectory(conf.OutputDir);

            }

            return conf;
        }

        public static async Task<SlicerConfig> FromXml(string xmlFile, string outputDirectory, string solutionFile, string projectRef, string fileToRewrite)
        {
            SlicerConfig conf = new SlicerConfig();

            var xml = File.ReadAllText(xmlFile);

            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                var doc = new XmlDocument();
                doc.Load(reader);
                var solutionTag = doc.GetElementsByTagName("Solution")[0];
                var criteriaTag = doc.GetElementsByTagName("Criteria")[0];
                var compilationOutputTag = doc.GetElementsByTagName("CompilationOutput")[0];
                var msBuildCompilation = doc.GetElementsByTagName("MSBuildCompilation")[0];
                var traceInput = doc.GetElementsByTagName("TraceInput")[0];
                var fileTraceInput = doc.GetElementsByTagName("FileTraceInput")[0];

                if (solutionTag == null || solutionTag.Attributes == null
                    || criteriaTag == null || criteriaTag.Attributes == null
                    || compilationOutputTag == null || compilationOutputTag.Attributes == null)
                {
                    throw new ArgumentException("SlicerConf XML is not well formed");
                }

                string solFile = solutionFile;
                if (!File.Exists(solFile)) throw new ArgumentException("SLN solution does not exists.");

                bool overwrite = msBuildCompilation == null ? false : true;
                string outputDir = outputDirectory;

                var workspace = MSBuildWorkspace.Create();
                conf.SlnFile = solFile;
                conf.Solution = await workspace.OpenSolutionAsync(solFile);
                conf.Project = projectRef;
                conf.CriteriaFile = criteriaTag.Attributes["File"].Value;
                conf.CriteriaLineOfCode = Int32.Parse(criteriaTag.Attributes["LineOfCode"].Value);
                conf.OutputDir = outputDir;
                conf.FileToRewrite = fileToRewrite;
                conf.MSBuildCompilation = overwrite;
                conf.FileTraceInput = fileTraceInput == null ? false : true;
                if (traceInput != null)
                {
                    conf.TraceInput = traceInput.Attributes["File"].Value;
                    conf.FileTraceInput = true;
                }

                //if (!Directory.Exists(conf.OutputDir)) throw new ArgumentException("Output dir does not exists");
                if (Directory.Exists(conf.OutputDir))
                {
                    Directory.Delete(conf.OutputDir, true);
                }
                Directory.CreateDirectory(conf.OutputDir);

            }

            return conf;
        }

        public string TraceInput { get; set; }

        public bool FileTraceInput { get; set; }
    }
}
