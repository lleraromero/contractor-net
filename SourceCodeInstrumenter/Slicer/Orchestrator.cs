using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DC.Slicer
{
    public class Orchestrator
    {
        private readonly SlicerConfig slicerConfig;
        public InstrumentationResult InstrumentationResult { get; private set; }
        public bool ContinueReceiving { get; set; }

        //public ISet<Stmt> SlicedStmts { get; private set; }

        public bool UserInteraction { get; set; }

        public Orchestrator(SlicerConfig slicerConfig)
        {
            this.slicerConfig = slicerConfig;
        }

        public void Orchestrate(params Type[] additionalTypes)
        {
            SourceCompiler compiler = new SourceCompiler(slicerConfig);
            
            var compile = compiler.InstrumentAndCompile(additionalTypes, false);
            try
            {
                InstrumentationResult = compile.Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }
    }
}
