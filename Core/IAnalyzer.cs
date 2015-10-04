using Microsoft.Cci;
using System;
using System.Collections.Generic;

namespace Contractor.Core
{
    interface IAnalyzer
    {
        TimeSpan TotalAnalysisDuration { get; }
        int ExecutionsCount { get; }
        int TotalGeneratedQueriesCount { get; }
        int UnprovenQueriesCount { get; }

        ActionAnalysisResults AnalyzeActions(CciState source, IMethodDefinition action, List<IMethodDefinition> actions);

        TransitionAnalysisResult AnalyzeTransitions(CciState source, IMethodDefinition action, List<CciState> targets);
    }
}