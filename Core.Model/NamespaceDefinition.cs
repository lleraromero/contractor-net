using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core.Model
{
    public class NamespaceDefinition
    {
        protected readonly string name;
        protected readonly IReadOnlyCollection<ITypeDefinition> types;

        public NamespaceDefinition(string name, IEnumerable<ITypeDefinition> types)
        {
            Contract.Requires(name != null);
            Contract.Requires(types.Any());

            this.name = name;
            this.types = new ReadOnlyCollection<ITypeDefinition>(types.ToList());
        }

        public string Name()
        {
            return name;
        }

        public IReadOnlyCollection<ITypeDefinition> Types()
        {
            return types;
        }
    }
}