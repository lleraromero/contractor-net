using System.Collections.Generic;
using Contractor.Core;

namespace Analyzer.Corral
{
    internal interface ICorralRunner
    {
        IEnumerable<Query> Run();
    }
}