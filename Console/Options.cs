using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;

namespace Contractor.Console
{
	class Options : OptionParsing
	{
		[OptionDescription("Name of the input assembly to analyze ", ShortForm = "i")]
		public string input = null;

		[OptionDescription("Full name of the type to analyze ", ShortForm = "t")]
		public string type = null;

		[OptionDescription("Generate the strengthened output assembly ", ShortForm = "ga")]
		public bool generateAssembly = false;

		[OptionDescription("Name of the strengthened output assembly ", ShortForm = "o")]
		public string output = null;

		[OptionDescription("Directory used to store the output graphs ", ShortForm = "g")]
		public string graph = null;

		[OptionDescription("Directory used to store temporary files ", ShortForm = "tmp")]
		public string temp = null;

		[OptionDescription("Full path and file name were find cccheck.exe ", ShortForm = "c")]
		public string cccheck = null;

		[OptionDescription("Command line arguments passed to cccheck.exe ", ShortForm = "ca")]
		public string cccheckArgs = null;

		[OptionDescription("Collapse transitions between states ", ShortForm = "ct")]
		public bool collapseTransitions = true;

		[OptionDescription("Distinguish unproven transitions with '?' ", ShortForm = "ut")]
		public bool unprovenTransitions = true;

		[OptionDescription("Inline methods body instead of method calls ", ShortForm = "il")]
		public bool inline = true;

		[OptionDescription("Show states descriptions ", ShortForm = "sd")]
		public bool stateDescription = true;

        [OptionDescription("Backend used to analyze the assembly (CodeContracts / Corral) ", ShortForm = "b")]
        public string backend = "CodeContracts";
	}
}
