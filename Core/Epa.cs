using System.Collections.Generic;

namespace Contractor.Core
{
    // Dictionary<actionUniqueName, EpaTransitions>
    class Epa : Dictionary<string, EpaTransitions>
    {
        public Dictionary<uint, IState> States { get; private set; }
        public TypeAnalysisResult AnalysisResult { get; private set; }
        public bool GenerationCompleted { get; set; }
        public bool Instrumented { get; set; }

        public Epa()
        {
            this.States = new Dictionary<uint, IState>();
            this.AnalysisResult = new TypeAnalysisResult();
            this.GenerationCompleted = false;
            this.Instrumented = false;
        }

        public new void Clear()
        {
            base.Clear();
            this.States.Clear();
            this.AnalysisResult = new TypeAnalysisResult();
            this.GenerationCompleted = false;
            this.Instrumented = false;
        }
    }

    // Dictionary<fromId, List<toId>>
    class EpaTransitions : Dictionary<uint, List<uint>>
    {
    }
}
