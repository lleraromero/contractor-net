using System.IO;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public interface ISerializer
    {
        void Serialize(Stream stream, Epa epa);
    }
}