using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;

namespace Analysis.Cci
{
    internal class CciTypeDefinition : TypeDefinition
    {
        protected IContractProvider contractProvider;
        protected ITypeDefinition typeDefinition;

        public CciTypeDefinition(ITypeDefinition typeDefinition, IContractProvider contractProvider)
        {
            Contract.Requires(typeDefinition != null);
            Contract.Requires(contractProvider != null);

            this.typeDefinition = typeDefinition;
            this.contractProvider = contractProvider;
        }

        public override string Name
        {
            get { return TypeHelper.GetTypeName(typeDefinition, NameFormattingOptions.None); }
        }

        public override ISet<Action> Constructors()
        {
            return new HashSet<Action>(from m in typeDefinition.Methods
                where m.IsConstructor
                select new CciAction(m, contractProvider.GetMethodContractFor(m)));
        }

        public override ISet<Action> Actions()
        {
            return new HashSet<Action>(from m in typeDefinition.Methods
                where !m.IsConstructor && m.Visibility == TypeMemberVisibility.Public &&
                      !m.IsStatic && !m.IsStaticConstructor
                select new CciAction(m, contractProvider.GetMethodContractFor(m)));
        }

        public override IMethodContract GetContractFor(IMethodDefinition method)
        {
            return contractProvider.GetMethodContractFor(method);
        }

        public override string ToString()
        {
            var name =
                new StringBuilder(TypeHelper.GetTypeName(typeDefinition, NameFormattingOptions.OmitContainingNamespace | NameFormattingOptions.None));

            if (typeDefinition.IsGeneric)
            {
                var genericParameters = string.Join(",", typeDefinition.GenericParameters);
                name.Append('<');
                name.Append(genericParameters);
                name.Append('>');
            }

            return name.ToString();
        }
    }
}