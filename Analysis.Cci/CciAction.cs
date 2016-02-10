using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Action = Contractor.Core.Model.Action;

namespace Analysis.Cci
{
    public class CciAction : Action
    {
        protected IMethodDefinition method;
        protected IMethodContract contract;
        public override string Name
        {
            get { return method.GetUniqueName(); }
        }

        public override IMethodDefinition Method
        {
            get { return method; }
        }

        public override IMethodContract Contract
        {
            get { return contract; }
        }

        public CciAction(IMethodDefinition method, IMethodContract contract)
        {
            System.Diagnostics.Contracts.Contract.Requires(method != null);

            this.method = method;
            this.contract = contract;
        }

        #region IEquatable
        public override bool Equals(Action other)
        {
            return base.Equals((object)other) && Equals((CciAction)other);
        }

        public bool Equals(CciAction other)
        {
            return method.Equals(other.method);
        }

        public override int GetHashCode()
        {
            return method.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            var signature = MemberHelper.GetMethodSignature(method, NameFormattingOptions.Signature | NameFormattingOptions.OmitContainingNamespace | NameFormattingOptions.OmitContainingType);
            if (method.IsConstructor)
            {
                return signature.Replace(".ctor", TypeHelper.GetTypeName(method.ContainingTypeDefinition, NameFormattingOptions.OmitContainingNamespace | NameFormattingOptions.TypeParameters));
            }

            return signature;
        }
    }
}
