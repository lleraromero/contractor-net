using System;
using System.Diagnostics.Contracts;
using System.IO;
using Contractor.Core.Model;

namespace Contractor.Core
{
    [ContractClass(typeof (ISerializerContracts))]
    public interface ISerializer
    {
        void Serialize(Stream stream, Epa epa);
    }

    [ContractClassFor(typeof (ISerializer))]
    internal abstract class ISerializerContracts : ISerializer
    {
        public void Serialize(Stream stream, Epa epa)
        {
            Contract.Requires(stream.CanWrite);
            Contract.Requires(epa != null);

            throw new NotImplementedException();
        }
    }
}