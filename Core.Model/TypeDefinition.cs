using System.Collections.Generic;

namespace Contractor.Core.Model
{
    public abstract class TypeDefinition
    {
        public abstract string Name { get; }
        public abstract IReadOnlyCollection<TypeDefinition> SubTypes { get; }
    }
}
