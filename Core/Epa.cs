using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
                if (base.Values.Count == 0)
                {
                    return new HashSet<ITransition>();
                }
                else
                {
                    return new HashSet<ITransition>(((IEnumerable<IEnumerable<ITransition>>)base.Values).Aggregate((acum, l) => acum.Union(l)));
                }
            }
        }

        public IState Initial
        {
            get
            {
                Contract.Requires(Contract.Exists<IState>(States, s => s.IsInitial));

                return States.First<IState>(s => s.IsInitial);
            }
        }

        internal bool GenerationCompleted { get; set; }
        internal bool Instrumented { get; set; }

        public Epa()
        {
            Contract.Ensures(!this.GenerationCompleted && !this.Instrumented);

            this.GenerationCompleted = false;
            this.Instrumented = false;
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Transitions.All(t => States.Contains(t.TargetState)));
        }

        public new void Clear()
        {
            Contract.Ensures(!GenerationCompleted && !Instrumented);

            base.Clear();
            this.GenerationCompleted = false;
            this.Instrumented = false;
        }

        public IState GetSourceState(ITransition t)
        {
            Contract.Requires(Transitions.Contains(t));

            foreach (var key in base.Keys)
            {
                if (base[key].Contains(t))
                {
                    return key;
                }
            }
            return null;
        }
    }
}
