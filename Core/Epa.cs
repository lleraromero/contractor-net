using System.Collections.Generic;
using System.Linq;

namespace Contractor.Core
{
    // Dictionary<Source, TransitionsFromSource>
    public class Epa : Dictionary<IState, HashSet<ITransition>>
    {
        public HashSet<IState> States
        {
            get
            {
                return new HashSet<IState>(base.Keys);
            }
        }

        public HashSet<ITransition> Transitions
        {
            get
            {
                return new HashSet<ITransition>(((IEnumerable<IEnumerable<ITransition>>)base.Values).Aggregate((acum, l) => acum.Union(l)));
            }
        }

        internal bool GenerationCompleted { get; set; }
        internal bool Instrumented { get; set; }

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

        public IState GetSourceState(ITransition t)
        {
            foreach (var key in base.Keys)
            {
                if (base[key].Contains(t))
                    return key;
            }
            return null;
        }
    }
}
