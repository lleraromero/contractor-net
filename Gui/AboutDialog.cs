using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Contractor.Gui
{
	partial class AboutDialog : Form
	{
		public AboutDialog()
		{
			InitializeComponent();

			labelVersion.Text = this.AssemblyVersion;
			labelProductName.Text = this.AssemblyProduct;
			labelCopyright.Text = this.AssemblyCopyright;
			labelDescription.Text = this.AssemblyDescription;

			this.LoadLicense();
		}

		public string AssemblyVersion
		{
			get
			{
				var assembly = Assembly.GetExecutingAssembly();
				var value = string.Format("Version {0}", assembly.GetName().Version);
				return value;
			}
		}

		public string AssemblyDescription
		{
			get
			{
				var value = this.GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description);
				return value;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				var value = this.GetAttributeValue<AssemblyProductAttribute>(a => a.Product);
				return value;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				var value = this.GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright);
				return value;
			}
		}

		private string GetAttributeValue<T>(Func<T, string> getValue) where T : class
		{
			var assembly = Assembly.GetExecutingAssembly();
			var attributes = assembly.GetCustomAttributes(typeof(T), false);
			var firstAttribute = attributes.FirstOrDefault() as T;

			if (firstAttribute == null) return string.Empty;
			else return getValue(firstAttribute);
		}

		private void LoadLicense()
		{
			var licenseFileName = Path.Combine(Application.StartupPath, @"Resources\License.rtf");

			if (File.Exists(licenseFileName))
				richtextboxLicense.LoadFile(licenseFileName, RichTextBoxStreamType.RichText);
		}

		private void OnUrlLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(linklabelUrl.Text);
		}
	}
}
