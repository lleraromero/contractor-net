using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Contractor.Core.Model;
using Microsoft.Cci;

namespace Analysis.Cci
{
    internal class CciTypeDefinition : TypeDefinition
    {
        protected ITypeDefinition typeDefinition;

        public CciTypeDefinition(ITypeDefinition typeDefinition)
        {
            Contract.Requires(typeDefinition != null);
            this.typeDefinition = typeDefinition;
        }

        public override string Name
        {
            get { return TypeHelper.GetTypeName(typeDefinition, NameFormattingOptions.None); }
        }

        public override IReadOnlyCollection<TypeDefinition> SubTypes
        {
            get
            {
                var subTypes = new List<TypeDefinition>();
                foreach (var nestedTypeDefinition in typeDefinition.NestedTypes)
                {
                    subTypes.Add(new CciTypeDefinition(nestedTypeDefinition));
                }
                return new ReadOnlyCollection<TypeDefinition>(subTypes);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}