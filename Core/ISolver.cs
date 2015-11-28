using System.IO;

namespace Contractor.Core
{
    public interface ISolver
    {
        QueryResult Execute(FileInfo queryAssembly, Query query);
    }
}