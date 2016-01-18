using System.Configuration;

namespace Contractor.Gui
{
    internal class Options
    {
        public Options()
        {
            CollapseTransitions = true;
            UnprovenTransitions = true;
            StateDescription = true;
        }

        public bool CollapseTransitions { get; set; }
        public bool UnprovenTransitions { get; set; }
        public bool StateDescription { get; set; }

        public string CheckerArguments
        {
            get { return ConfigurationManager.AppSettings["CccheckArgs"]; }
            set { ConfigurationManager.AppSettings.Set("CccheckArgs", value); }
        }
    }
}