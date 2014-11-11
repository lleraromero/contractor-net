using Microsoft.Cci;
using System;
using System.Collections.Generic;

namespace Contractor.Core
{
    class ActionAnalysisResults
    {
        public List<IMethodDefinition> EnabledActions { get; private set; }
        public List<IMethodDefinition> DisabledActions { get; private set; }

        public ActionAnalysisResults()
        {
            this.EnabledActions = new List<IMethodDefinition>();
            this.DisabledActions = new List<IMethodDefinition>();
        }
    }

    class TransitionAnalysisResult
    {
        public List<Transition> Transitions { get; private set; }

        public TransitionAnalysisResult()
        {
            this.Transitions = new List<Transition>();
        }
    }

    public class TypeAnalysisResult
    {
        public List<IState> States { get; private set; }
        public List<ITransition> Transitions { get; private set; }

        public TimeSpan TotalAnalyzerDuration { get; internal set; }
        public TimeSpan TotalDuration { get; internal set; }
        public int ExecutionsCount { get; internal set; }
        public int TotalGeneratedQueriesCount { get; internal set; }
        public int UnprovenQueriesCount { get; internal set; }

        public TypeAnalysisResult()
        {
            this.States = new List<IState>();
            this.Transitions = new List<ITransition>();
        }
    }
}
