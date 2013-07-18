using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contractor.Core
{
	// Dictionary<actionUniqueName, EpaTransitions>
	class Epa : Dictionary<string, EpaTransitions>
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
		public string UniqueName { get; private set; }
		public List<string> EnabledActions { get; private set; }
		public List<string> DisabledActions { get; private set; }

		public EpaState(uint id, string uniqueName)
		{
			this.Id = id;
			this.UniqueName = uniqueName;
			this.EnabledActions = new List<string>();
			this.DisabledActions = new List<string>();
		}
	}
}
