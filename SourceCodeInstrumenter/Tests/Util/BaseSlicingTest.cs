using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tracer.Poco;
using DC.Slicer;

namespace Tests.Util
{
    public abstract class BaseSlicingTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        protected void TestSimpleSlice(Type type, TestResult expectedResult)
        {
            var outputDir = Path.GetTempPath() + "slicer" + new Random().Next(100);
            try
            {
                string baseCaseDir = Path.GetFullPath(Directory.GetCurrentDirectory() + @"\..\..\Cases\");
                string classFile = System.IO.Directory.GetFiles(baseCaseDir, type.Name + ".cs", SearchOption.AllDirectories).First();
                //string classFile = baseCaseDir + "\\" + type.Name + ".cs";

                Directory.CreateDirectory(outputDir);
                var workspace = new AdhocWorkspace();
                var msCorLibReference = MetadataReference.CreateFromFile(typeof (object).Assembly.Location);
                var linqReference = MetadataReference.CreateFromFile(typeof (IQueryable).Assembly.Location);
                var binarySampleReference = MetadataReference.CreateFromFile(typeof (BinarySample.Binary).Assembly.Location);
                var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "TestProject", "TestProject",
                    LanguageNames.CSharp,
                    metadataReferences : new List<MetadataReference> { msCorLibReference, linqReference, binarySampleReference });
                var newProject = workspace.AddProject(projectInfo);
                workspace.AddDocument(newProject.Id, type.Name + ".cs", SourceText.From(File.ReadAllText(classFile)));

                Orchestrator orchestrator = new Orchestrator(
                    new SlicerConfig
                    {
                        CriteriaFile = expectedResult.Criteria.FileName,
                        CriteriaLineOfCode = expectedResult.Criteria.Line,
                        OutputDir = outputDir,
                        Project = "TestProject",
                        Solution = workspace.CurrentSolution,
                        TraceInput = null,
                        //TraceInput = @"C:\temp\trace.txt",
                        FileTraceInput = true
                    }
                ) { UserInteraction = false };
                orchestrator.Orchestrate(typeof(BinarySample.Binary));

                Assert.IsNotNull(orchestrator.SlicedStmts);
                Assert.IsNotNull(expectedResult.Sliced);
                Assert.AreEqual(expectedResult.Sliced.Count, orchestrator.SlicedStmts.Count);
                Assert.IsTrue(AreStmtSetsEquivalent(orchestrator.SlicedStmts, expectedResult.Sliced));
                //var expectedResultLines = new HashSet<int>(expectedResult.Sliced.Select(x => x.Line));
                //var slicedLines = new HashSet<int>(orchestrator.SlicedStmts.Select(x => x.Line));
                //Assert.AreEqual(expectedResultLines.Count, slicedLines.Count);
                //Assert.IsTrue(expectedResultLines.SetEquals(slicedLines));
            }
            finally
            {
                // HACK -- Cleanup. May throw errors so will need to retry several times
                bool stillExists;
                do
                {
                    stillExists = Directory.Exists(outputDir);
                    try
                    {
                        Directory.Delete(outputDir, true);
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
                while (stillExists);
            }
        }

        private static bool AreStmtSetsEquivalent(IEnumerable<Stmt> a, IEnumerable<Stmt> b)
        {
            var hash = new HashSet<Stmt>(a, new StmtFileAndLineEqualityComparer());
            hash.SymmetricExceptWith(b);

            return hash.Count == 0;
        }

        protected SameFileStmtBuilder SameFileStmtBuilder(string filename)
        {
            return new SameFileStmtBuilder(filename);
        }
        protected SameFileStmtBuilder SameFileStmtBuilder(Type type)
        {
            return new SameFileStmtBuilder(type.Name + ".cs");
        }
    }

    public class SameFileStmtBuilder
    {
        private readonly string _filename;

        public SameFileStmtBuilder(string filename)
        {
            _filename = filename;
        }

        public Stmt WithLine(int line)
        {
            return new Stmt {FileName = _filename, Line = line};
        }
    }
}
