using System.Diagnostics.Contracts;
using System.IO;

namespace Contractor.Core
{
    [ContractClass(typeof(ISolverContracts))]
    public interface ISolver
    {
        QueryResult Execute(FileInfo queryAssembly, Query query);
    }

    [ContractClassFor(typeof(ISolver))]
    public abstract class ISolverContracts : ISolver
    {
        public QueryResult Execute(FileInfo queryAssembly, Query query)
        {
            Contract.Requires(queryAssembly.Exists);
            Contract.Requires(query != null);
            throw new System.NotImplementedException();
        }
    }
}