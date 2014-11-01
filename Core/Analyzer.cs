using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Contractor.Core
{
    abstract class Analyzer : IAnalyzer
    {
        #region IAnalyzer interface

        public TimeSpan TotalAnalysisDuration { get; protected set; }
        public int ExecutionsCount { get; protected set; }
        public int TotalGeneratedQueriesCount { get; protected set; }
        public int UnprovenQueriesCount { get; protected set; }

        public abstract ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions);

        public abstract TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets);

        #endregion IAnalyzer interface

        protected readonly IContractAwareHost host;
        protected readonly AssemblyInfo inputAssembly;
        protected readonly ContractProvider inputContractProvider;
        protected AssemblyInfo queryAssembly;
        protected ContractProvider queryContractProvider;
        protected readonly NamespaceTypeDefinition typeToAnalyze;

        protected Analyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type)
        {
            Contract.Requires(host != null && module != null && type != null);

            this.host = host;
            this.inputAssembly = new AssemblyInfo(host);
            inputAssembly.Load(module.Location);

            this.typeToAnalyze = type;
            this.inputContractProvider = inputAssembly.ExtractContracts();

            // Create a clone of the module as a working copy.
            CreateQueryAssembly(type);
            this.queryContractProvider = new ContractProvider(new ContractMethods(this.host), this.host.FindUnit(this.queryAssembly.Module.UnitIdentity));
        }

        ~Analyzer()
        {
            // Delete the working copy of the module.
            try
            {
                File.Delete(GetQueryAssemblyPath());
            }
            catch
            { }
        }

        protected void CreateQueryAssembly(NamespaceTypeDefinition type)
        {
            // Load original module
            IModule module = this.host.LoadUnitFrom(inputAssembly.Module.Location) as IModule;
            // Make a editable copy
            module = new MetadataDeepCopier(this.host).Copy(module);
            this.queryAssembly = new AssemblyInfo(this.host, module);

            // Remove types that we don't need to analyse in the query assembly.
            var types = this.queryAssembly.DecompiledModule.GetAnalyzableTypes().ToList();
            foreach (var t in types)
            {
                var tMutable = t as NamespaceTypeDefinition;
                if (tMutable != null && tMutable.ContainingUnitNamespace.Name == type.ContainingUnitNamespace.Name && tMutable.Name != type.Name)
                {
                    this.queryAssembly.DecompiledModule.AllTypes.Remove(t);
                }
                // TODO: removed types are still present as RootNamespace members, remove them.
            }
        }

        protected string GetQueryAssemblyPath()
        {
            Contract.Requires(this.inputAssembly != null);

            return Path.Combine(Configuration.TempPath, this.inputAssembly.Module.ModuleName.Value + ".tmp");
        }

        protected PdbReader GetPDBReader(IModule module, IContractAwareHost host)
        {
            Contract.Requires(module != null && host != null);

            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
                using (var pdbStream = File.OpenRead(pdbFile))
                    pdbReader = new PdbReader(pdbStream, host);
            return pdbReader;
        }
    }
}