using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contractor.Core
{
	// Dictionary<actionId, EpaTransitions>
	class Epa : Dictionary<int, EpaTransitions>
	{
		public Dictionary<uint, EpaState> States { get; private set; }
		public TypeAnalysisResult AnalysisResult { get; private set; }
		public bool GenerationCompleted { get; set; }
		public bool Instrumented { get; set; }

		public Epa()
		{
			this.States = new Dictionary<uint, EpaState>();
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

	class EpaState
	{
		public uint Id { get; private set; }
		public string Name { get; private set; }
		public List<int> EnabledActions { get; private set; }
		public List<int> DisabledActions { get; private set; }

		public EpaState(uint id, string name)
		{
			this.Id = id;
			this.Name = name;
			this.EnabledActions = new List<int>();
			this.DisabledActions = new List<int>();
		}
	}
}
