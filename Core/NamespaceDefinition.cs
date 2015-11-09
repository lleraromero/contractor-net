using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public class NamespaceDefinition
    {
        private readonly IReadOnlyCollection<TypeDefinition> types;

        public NamespaceDefinition(IEnumerable<TypeDefinition> types)
        {
            Contract.Requires(types != null && types.Any());

            this.types = new ReadOnlyCollection<TypeDefinition>(types.ToList());
        }

        private IReadOnlyCollection<TypeDefinition> Types()
        {
            return types;
        }
    }
}