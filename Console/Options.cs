using System;
using CommandLine;
using CommandLine.Text;

namespace Contractor.Console
{
    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input assembly to analyze.")]
        public string InputAssembly { get; set; }

        [Option('t', "type", Required = true, HelpText = "Full name of the type to analyze.")]
        public string TypeToAnalyze { get; set; }

        [Option("ga", HelpText = "Generate the strengthened output assembly.", DefaultValue = false)]
        public bool GenerateStrengthenedAssembly { get; set; }

        [Option('o', "output", HelpText = "Name of the strengthened output assembly.")]
        public string OutputAssembly { get; set; }

        [Option('g', "graph", HelpText = "Directory used to store the output graphs.", DefaultValue = "C:\\")]
        public string GraphDirectory { get; set; }

        [Option("tmp", HelpText = "Directory used to store temporary files.", DefaultValue = "C:\\")]
        public string TempDirectory { get; set; }

        [Option('c', "cccheck", HelpText = "Full path and file name were find cccheck.exe.")]
        public string CccheckPath { get; set; }

        [Option("ca", HelpText = "Command line arguments passed to cccheck.exe.")]
        public string CccheckArgs { get; set; }

        [Option("ct", HelpText = "Collapse transitions between states.", DefaultValue = true)]
        public bool CollapseTransitions { get; set; }

        [Option("du", HelpText = "Distinguish unproven transitions with '?'.", DefaultValue = true)]
        public bool DistinguishUnproven { get; set; }

        [Option("il", HelpText = "Inline methods body instead of method calls.", DefaultValue = true)]
        public bool InlineMethods { get; set; }

        [Option("sd", HelpText = "Show states descriptions.", DefaultValue = true)]
        public bool ShowStateDescription { get; set; }

        [Option('b', "backend", HelpText = "Backend used to analyze the assembly (CodeContracts / Corral).",
            DefaultValue = "Corral")]
        public string Backend { get; set; }

        [Option("xml", HelpText = "Generate an XML file as output.", DefaultValue = false)]
        public bool XML { get; set; }

        [Option("cutter", HelpText = "Stop EPA generation when the possible targets are greater than (Default is not cutting)", DefaultValue = -1)]
        public int Cutter { get; set; }

        [Option("wc", HelpText = "Transition with conditions.", DefaultValue = false)]
        public bool TransitionsWithConditions { get; set; }

        [Option("methods", HelpText = "List of methods to analyze. It's important to write the method with parameters types. For example: add(String),hasNext()", DefaultValue = "All")]
        public String Methods { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("Contractor.NET", "0.7.1"),
                Copyright = "Copyright (C) LaFHIS - UBA. All rights reserved.",
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine(Environment.NewLine);
            help.AddPreOptionsLine("Usage: <general-option>*");
            help.AddPreOptionsLine("where <general-option> is one of");
            help.AddOptions(this);
            return help;
        }
    }
}