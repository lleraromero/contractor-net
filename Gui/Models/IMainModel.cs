using System;
using System.IO;
using System.Threading.Tasks;
using Contractor.Core;
using Contractor.Core.Model;

namespace Contractor.Gui.Models
{
    internal interface IMainModel
    {
        Epa GeneratedEpa { get; }
        IAssemblyXXX InputAssembly { get; }
        void Stop();
        Task<TypeAnalysisResult> Start(AnalysisEventArgs analysisEventArgs);
        Task LoadAssembly(FileInfo inputFile);
        Task LoadContracts(FileInfo contractFileInfo);

        event EventHandler<StateAddedEventArgs> StateAdded;
        event EventHandler<TransitionAddedEventArgs> TransitionAdded;
    }
}