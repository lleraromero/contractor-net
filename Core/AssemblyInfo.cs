using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using System.IO;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableContracts;
using Microsoft.Cci.Contracts;

namespace Contractor.Utils
{
	public class AssemblyInfo : IDisposable
	{
		public string FileName { get; private set; }
		public bool IsLoaded { get; private set; }
		public IMetadataHost Host { get; private set; }
		public IModule Module { get; private set; }
		public Module DecompiledModule { get; private set; }
		public PdbReader PdbReader { get; private set; }

		public AssemblyInfo(IMetadataHost host)
		{
			this.Host = host;
		}

		public void Load(string fileName)
		{
			this.Module = this.Host.LoadUnitFrom(fileName) as IModule;

			if (this.Module == null || this.Module == Dummy.Module || this.Module == Dummy.Assembly)
				throw new Exception("The input is not a valid CLR module or assembly.");

			var pdbFileName = Path.ChangeExtension(fileName, "pdb");

			if (File.Exists(pdbFileName))
			{
				using (var pdbStream = File.OpenRead(pdbFileName))
					this.PdbReader = new PdbReader(pdbStream, this.Host);
			}

			this.FileName = fileName;
			this.IsLoaded = true;
		}

		public void Decompile()
		{
			this.DecompiledModule = Decompiler.GetCodeModelFromMetadataModel(this.Host, this.Module, this.PdbReader);
		}

		public ContractProvider ExtractContracts()
		{
			var contractAwareHost = this.Host as IContractAwareHost;
			var contractProvider = ContractHelper.ExtractContracts(contractAwareHost, this.DecompiledModule, this.PdbReader, this.PdbReader);
			return contractProvider;
		}

		public void InjectContracts(ContractProvider contractProvider)
		{
			ContractHelper.InjectContractCalls(this.Host, this.DecompiledModule, contractProvider, this.PdbReader);
		}

		public void Save(string fileName)
		{
			var pdbName = Path.ChangeExtension(fileName, "pdb");

			using (var peStream = File.Create(fileName))
			{
				if (this.PdbReader == null)
				{
					PeWriter.WritePeToStream(this.DecompiledModule, this.Host, peStream);
				}
				else
				{
					using (var pdbWriter = new PdbWriter(pdbName, this.PdbReader))
						PeWriter.WritePeToStream(this.DecompiledModule, this.Host, peStream, this.PdbReader, this.PdbReader, pdbWriter);
				}
			}
		}

		public void Unload()
		{
			if (!this.IsLoaded) return;

			if (this.PdbReader != null)
			{
				this.PdbReader.Dispose();
				this.PdbReader = null;
			}

			this.Module = null;
			this.DecompiledModule = null;
			this.FileName = null;
			this.IsLoaded = false;
		}

		public void Dispose()
		{
			this.Unload();
		}
	}
}
