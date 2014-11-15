using System.Collections.Generic;
using System.Linq;

namespace Contractor.Core
{
    // Dictionary<Source, TransitionsFromSource>
    public class Epa : Dictionary<IState, List<ITransition>>
    {
        public bool GenerationCompleted { get; set; }
        public bool Instrumented { get; set; }

        public Epa()
        {
            this.GenerationCompleted = false;
            this.Instrumented = false;
        }

        public new void Clear()
        {
            base.Clear();
            this.GenerationCompleted = false;
            this.Instrumented = false;
        }

        public IEnumerable<IState> States { get { return base.Keys; } }
        public IEnumerable<ITransition> Transitions 
        {
            get
            {
                return ((IEnumerable<IEnumerable<ITransition>>)base.Values).Aggregate((acum, l) => acum.Union(l));      
            }
        }
    }
}
