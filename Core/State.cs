using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Cci;

namespace Contractor.Core
{
	class State : IState, IEquatable<State>
	{
		private class NamedEntityComparer : Comparer<INamedEntity>
		{
			public override int Compare(INamedEntity x, INamedEntity y)
			{
				return x.Name.UniqueKey.CompareTo(y.Name.UniqueKey);
			}
		}

		public uint Id;
		public string Name;
		public bool IsInitial;
		public List<IMethodDefinition> EnabledActions { get; private set; }
		public List<IMethodDefinition> DisabledActions { get; private set; }

		public State()
		{
			this.Id = 0;
			this.Name = string.Empty;
			this.IsInitial = false;
			this.EnabledActions = new List<IMethodDefinition>();
			this.DisabledActions = new List<IMethodDefinition>();
		}

		public void Sort()
		{
			if (this.EnabledActions.Count > 0)
			{
				this.EnabledActions.Sort(new NamedEntityComparer());
				this.Name = string.Join(string.Empty, from a in this.EnabledActions select a.Name.Value);
			}
			else
			{
				this.Name = "empty";
			}
		}

		public EpaState EpaState
		{
			get
			{
				var s = new EpaState(this.Id, this.Name);
				s.EnabledActions.AddRange(from a in this.EnabledActions select a.Name.UniqueKey);
				s.DisabledActions.AddRange(from a in this.DisabledActions select a.Name.UniqueKey);
				return s;
			}
		}

		#region IState

		string IState.Name
		{
			get { return this.Name; }
		}

		bool IState.IsInitial
		{
			get { return this.IsInitial; }
		}

		IEnumerable<string> IState.EnabledActions
		{
			get { return from a in this.EnabledActions select a.Name.Value; }
		}

		IEnumerable<string> IState.DisabledActions
		{
			get { return from a in this.DisabledActions select a.Name.Value; }
		}

		#endregion

		public bool Equals(State other)
		{
			return this.Name.Equals(other.Name);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}
