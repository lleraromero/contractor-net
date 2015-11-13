using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Gui
{
    internal interface IMethodFilterScreen
    {
        IEnumerable<Action> SelectedMethods { get; }
        void LoadMethods(TypeDefinition typeDefinition);
    }

    public partial class MethodFilterScreen : UserControl, IMethodFilterScreen
    {
        public MethodFilterScreen()
        {
            InitializeComponent();
        }

        public void LoadMethods(TypeDefinition typeDefinition)
        {
            lsbMethods.BeginUpdate();

            lsbMethods.Items.Clear();

            foreach (var constructor in typeDefinition.Constructors())
            {
                lsbMethods.Items.Add(constructor, CheckState.Checked);
            }
            
            foreach (var action in typeDefinition.Actions())
            {
                lsbMethods.Items.Add(action, CheckState.Checked);
            }

            lsbMethods.EndUpdate();
        }

        public IEnumerable<Action> SelectedMethods
        {
            get { return lsbMethods.CheckedItems.Cast<Action>(); }
        }

        protected void btnAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < lsbMethods.Items.Count; ++i)
            {
                lsbMethods.SetItemChecked(i, true);
            }
        }

        protected void btnNone_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < lsbMethods.Items.Count; ++i)
            {
                lsbMethods.SetItemChecked(i, false);
            }
        }
    }
}