using System.Collections.Generic;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace DC.Slicer
{
    public class InstrumentationResult
    {
        public Dictionary<int, CSharpSyntaxTree> fileIdToSyntaxTree { get; set; }
        public Dictionary<string, SemanticModel> filePathToSemanticModel { get; set; }
        public Dictionary<int, string> IdToFileDictionary { get; set; }
        public Dictionary<string, int> FileToIdDictionary { get; set; }
        public string Executable { get; set; }
        public CSharpSyntaxNode EntryPoint { get; set; }
    }
}
