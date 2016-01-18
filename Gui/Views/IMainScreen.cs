using System;
using System.IO;
using Contractor.Core;
using Contractor.Core.Model;

namespace Contractor.Gui
{
    internal interface IMainScreen
    {
        string Engine { get; }

        void UpdateStatus(string msg);
        void UpdateOutput(string msg);
        void StateAdded(StateAddedEventArgs e);
        void TransitionAdded(TransitionAddedEventArgs e);
        void ShowTypes(IAssemblyXXX inputAssembly);
        void DisableInterfaceWhileAnalyzing();
        void EnableInterfaceAfterAnalysis();

        event EventHandler<FileInfo> LoadAssembly;
        event EventHandler<FileInfo> LoadContracts;
        event EventHandler<AnalysisEventArgs> StartAnalysis;
        event EventHandler StopAnalysis;
        event EventHandler<FileInfo> ExportGraph;
        event EventHandler<FileInfo> GenerateAssembly;
    }
}