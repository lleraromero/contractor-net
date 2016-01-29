using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core.Model
{
    public class NamespaceDefinition
    {
        protected readonly string name;
        protected readonly IReadOnlyCollection<TypeDefinition> types;

        public NamespaceDefinition(string name, IEnumerable<TypeDefinition> types)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(types.Any());

            this.name = name;
            this.types = new ReadOnlyCollection<TypeDefinition>(types.ToList());
        }

        public string Name()
        {
            return name;
        }

        public IReadOnlyCollection<TypeDefinition> Types()
        {
            return types;
        }
    }
}