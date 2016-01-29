using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;

namespace Analysis.Cci
{
    internal class CciTypeDefinition : ITypeDefinition
    {
        protected IContractProvider contractProvider;
        protected Microsoft.Cci.ITypeDefinition typeDefinition;

        public CciTypeDefinition(Microsoft.Cci.ITypeDefinition typeDefinition, IContractProvider contractProvider)
        {
            Contract.Requires(typeDefinition != null);
            Contract.Requires(contractProvider != null);

            this.typeDefinition = typeDefinition;
            this.contractProvider = contractProvider;
        }

        public string Name
        {
            get { return TypeHelper.GetTypeName(typeDefinition); }
        }

        public ISet<Action> Constructors()
        {
            return new HashSet<Action>(from m in typeDefinition.Methods
                where m.IsConstructor
                select new CciAction(m, contractProvider.GetMethodContractFor(m)));
        }

        public ISet<Action> Actions()
        {
            return new HashSet<Action>(from m in typeDefinition.Methods
                where !m.IsConstructor && m.Visibility == TypeMemberVisibility.Public &&
                      !m.IsStatic && !m.IsStaticConstructor
                select new CciAction(m, contractProvider.GetMethodContractFor(m)));
        }

        public IMethodContract GetContractFor(IMethodDefinition method)
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