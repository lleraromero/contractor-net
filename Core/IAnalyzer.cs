using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Contractor.Core
{
	interface IAnalyzer
	{
		TimeSpan TotalAnalysisDuration { get; }
		int ExecutionsCount { get; }
		int TotalGeneratedQueriesCount { get; }
		int UnprovenQueriesCount { get; }

		ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions);
		TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets);
	}
}
