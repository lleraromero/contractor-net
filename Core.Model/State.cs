using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core.Model
{
    public class State : IEquatable<State>
    {
        protected static int id_counter=1;
        protected static Dictionary<string, int> states = new Dictionary<string, int>();
        protected HashSet<Action> enabledActions;
        protected HashSet<Action> disabledActions;
        protected int id;

        public IImmutableSet<Action> EnabledActions { get { return enabledActions.ToImmutableHashSet<Action>(); } }
        public IImmutableSet<Action> DisabledActions { get { return disabledActions.ToImmutableHashSet<Action>(); } }

        public State(ISet<Action> enabledActions, ISet<Action> disabledActions)
        {
            Contract.Requires(enabledActions != null && !enabledActions.Contains(null));
            Contract.Requires(disabledActions != null && !disabledActions.Contains(null));
            Contract.Requires(!enabledActions.Intersect(disabledActions).Any(), "Cannot have an action in both sets");

            this.enabledActions =  new HashSet<Action>(enabledActions);
            this.disabledActions = new HashSet<Action>(disabledActions);
            var name= this.Name;
            if(states.TryGetValue(Name, out this.id)){
                
            }
            else
            {
                this.id = id_counter++;
                states.Add(name, this.id);
            }
            
        }

        public int Id
        {
            get
            {
                return this.id;
            }
        }
        public string Name
        {
            get
            {
                const string methodNameDelimiter = "$";
                return "STATE$" + ((this.EnabledActions.Count > 0) ? string.Join(methodNameDelimiter, from a in this.enabledActions orderby a.Name select a.Name)
                                                     : "deadlock");
            }
        }

        #region IEquatable
        public override bool Equals(object obj)
        {
            // Again just optimization
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            // Actually check the type, should not throw exception from Equals override
            if (obj.GetType() != this.GetType()) return false;

            return Equals((State)obj);
        }
        public bool Equals(State other)
        {
            var setComparer = HashSet<Action>.CreateSetComparer();
            return setComparer.Equals(enabledActions, other.enabledActions) &&
                   setComparer.Equals(disabledActions, other.disabledActions);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return (this.enabledActions.Count > 0) ? string.Join(Environment.NewLine, from a in this.enabledActions orderby a.ToString() select a.ToString())
                                                       : "{no methods}";
        }
    }
}