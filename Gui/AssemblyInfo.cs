using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System.IO;
using Microsoft.Cci.ILToCodeModel;

namespace Contractor.Gui
{
	class AssemblyInfo
	{
		public PeReader.DefaultHost Host { get; private set; }
		public IModule Module { get; private set; }
		public PdbReader PdbReader { get; private set; }
		public bool IsLoaded { get; private set; }
		public string FileName { get; private set; }

		public void Load(string fileName)
		{
			this.Host = new PeReader.DefaultHost();
			this.Module = this.Host.LoadUnitFrom(fileName) as IModule;

			if (this.Module == null || this.Module == Dummy.Module || this.Module == Dummy.Assembly)
				throw new Exception("The input is not a valid CLR module or assembly.");

			string pdbFileName = Path.ChangeExtension(fileName, "pdb");

			if (File.Exists(pdbFileName))
			{
				using (var pdbStream = File.OpenRead(pdbFileName))
					this.PdbReader = new PdbReader(pdbStream, this.Host);
			}

			this.FileName = fileName;
			this.IsLoaded = true;
		}

		public void Dispose()
		{
			if (this.PdbReader != null)
			{
				this.PdbReader.Dispose();
				this.PdbReader = null;
			}

			if (this.PdbReader != null)
			{
				this.PdbReader.Dispose();
				this.PdbReader = null;
			}

			if (this.Host != null)
			{
				this.Host.Dispose();
				this.Host = null;
			}

			this.FileName = null;
			this.IsLoaded = false;
		}
	}
}
