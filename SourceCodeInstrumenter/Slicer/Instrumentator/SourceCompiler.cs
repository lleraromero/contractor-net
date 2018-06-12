using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Xml;
using Microsoft.Build.BuildEngine;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis.MSBuild;
using System.Threading;
using Microsoft.CodeAnalysis.Emit;
using System.Resources;
using System.Collections;
using System.Diagnostics;
using Microsoft.Build.Utilities;

namespace DC.Slicer
{
    class SourceCompiler
    {
        public SlicerConfig Config { get; private set; }

        public SourceCompiler(SlicerConfig config)
        {
            Config = config;
        }

        public async Task<InstrumentationResult> InstrumentAndCompile(Type[] additionalTypes, bool emit)
        {
            string originalRootPath = null;
            string newSolFile = null;
            bool copyFiles = false;
            string outputFilePath = null;
            if (Config.SlnFile != null)
            {
                var fileInfo = new FileInfo(Config.SlnFile);
                originalRootPath = fileInfo.DirectoryName;
                DirectoryCopy(originalRootPath, Config.OutputDir, true);
                newSolFile = Config.OutputDir + "\\" + fileInfo.Name;
                copyFiles = true;
            }
            else
            {
                if (!emit) throw new Exception("Si compilamos con MSBuild, el SLN debe existir (al menos por ahora)");
            }

            Dictionary<string, SemanticModel> pathToSemanticModelDict = new Dictionary<string, SemanticModel>();
            Dictionary<int, CSharpSyntaxTree> idsToSyntaxTreesDict = new Dictionary<int, CSharpSyntaxTree>();

            string compiledAssemblyName = null;
            CSharpSyntaxNode entryPoint = null;
            var instrumenter = new SourceInstrumenter(Config);
            
            instrumenter.pathToOutputFiles = Config.OutputDir;

            List<Type> typesList = new List<Type>();
            //typesList.Add(typeof(TraceType));
            typesList.Add(typeof(ProtoBuf.Serializer));
            foreach (Type additional in additionalTypes) typesList.Add(additional);
            foreach (var type in typesList)
            {
                string dllOrigin = type.Assembly.Location;
                string dllDestination = Config.OutputDir + Path.DirectorySeparatorChar + Path.GetFileName(dllOrigin);
                if (File.Exists(dllDestination)) File.Delete(dllDestination);
                File.Copy(dllOrigin, dllDestination);
            }

            foreach (var project in Config.Solution.Projects)
            {
                CSharpCompilation originalCompilation = (CSharpCompilation)await project.GetCompilationAsync();
                CSharpCompilation modifiedCompilation = instrumenter.Instrument(originalCompilation);

                var pathToIdDict = instrumenter.FilesIds().ToDictionary(x => x.Value, x => x.Key);
                foreach (var st in originalCompilation.SyntaxTrees)
                {
                    if (pathToIdDict.ContainsKey(st.FilePath))
                    {
                        var annotation = new SyntaxAnnotation("FileId", pathToIdDict[st.FilePath].ToString());
                        originalCompilation = originalCompilation.ReplaceSyntaxTree(st, st.WithRootAndOptions(st.GetCompilationUnitRoot().WithAdditionalAnnotations(annotation), st.Options));
                    }
                }

                foreach (var entry in instrumenter.LastInstrumentedFileIds())
                {
                    string path = entry.Value;
                    var ast = originalCompilation.SyntaxTrees.Single(x => x.FilePath == path);
                    idsToSyntaxTreesDict.Add(entry.Key, (CSharpSyntaxTree)ast);
                    var curSemanticModel = originalCompilation.GetSemanticModel(ast);
                    pathToSemanticModelDict.Add(path, curSemanticModel);
                }

                if (copyFiles)
                {
                    foreach (var st in modifiedCompilation.SyntaxTrees)
                    {
                        if (st.FilePath.StartsWith(originalRootPath))
                        {
                            var pathFromRoot = st.FilePath.Remove(0, originalRootPath.Length);
                            var str = Config.OutputDir + pathFromRoot;
                            string sourceText = st.GetText().ToString();
                            File.WriteAllText(str, sourceText);
                        }
                    }
                    var projPath = project.FilePath.Remove(0, originalRootPath.Length);
                    addDependenciesToCsproj(Config.OutputDir + projPath, Config.OutputDir);
                }

                if (project.Name == Config.Project)
                {

                    //entryPoint = (CSharpSyntaxNode)(originalCompilation.GetEntryPoint(new CancellationToken()).DeclaringSyntaxReferences.Single().GetSyntax());
                }
            }

                // Compilacion con MSBuild (Se compila la solucion entera).
                /* Compila un sln. XXX: Hay dos cosas a tener en cuenta:
                 *  - Copiar los dll a Config.OutputDir para este caso no sirve. Por suerte al modificar
                 *    los *.proj con las nuevas dependencia el msbuild los copia al output que tiene el proj.
                 *  - Aca el compiledAssemblyName es el outputFilePath del project que es donde los tira el
                 *    msbuild.
                 */
                string msbuildExe = ToolLocationHelper.GetPathToBuildToolsFile("msbuild.exe", "14.0", DotNetFrameworkArchitecture.Bitness64);
                ProcessStartInfo startInfo = new ProcessStartInfo(msbuildExe);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "/v:m /m " + newSolFile;
                Process.Start(startInfo);
                var workspace = MSBuildWorkspace.Create();
                var newSol = await workspace.OpenSolutionAsync(newSolFile);
                compiledAssemblyName = newSol.Projects.Single(x => x.Name == Config.Project).OutputFilePath;
                // Fin compilacion MSBuild.

            return new InstrumentationResult
            {
                Executable = compiledAssemblyName,
                IdToFileDictionary = instrumenter.FilesIds(),
                FileToIdDictionary = instrumenter.FilesIds().ToDictionary(x => x.Value, x => x.Key),
                fileIdToSyntaxTree = idsToSyntaxTreesDict,
                filePathToSemanticModel = pathToSemanticModelDict,
                EntryPoint = entryPoint
            };
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        void addDependenciesToCsproj(string csproj, string rootPath)
        {
            //Fill a list with the lines from the txt file.
            List<string> lines = new List<string>();
            bool alreadyAdded = false;
            foreach (var line in File.ReadAllLines(csproj).ToList())
            {
                if (line.Contains("<Reference Include=") && line.Contains("/>") && !alreadyAdded)
                {
                    lines.Add("<Reference Include=\"Tracer\"><HintPath>" + rootPath + "\\Tracer.dll</HintPath></Reference>");
                    lines.Add(line);
                    alreadyAdded = true;
                }
                else
                {
                    lines.Add(line);
                }
            }

            //Add the lines including the new one.
            File.WriteAllLines(csproj, lines);
        }
    }
}
