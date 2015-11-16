using System.Linq;
using System.Text;
using Contractor.Utils;
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
            return GetDisplayName(method);
        }

        protected string GetDisplayName(IMethodDefinition methodToBeDisplayed)
        {
            var name = new StringBuilder();

            if (methodToBeDisplayed.IsConstructor)
            {
                name.Append(TypeHelper.GetTypeName(methodToBeDisplayed.ContainingTypeDefinition, NameFormattingOptions.OmitContainingNamespace));
            }
            else
            {
                name.Append(methodToBeDisplayed.Name.Value);
            }

            if (methodToBeDisplayed.IsGeneric)
            {
                var genericParameters = string.Join(",", methodToBeDisplayed.GenericParameters);
                name.Append('<');
                name.Append(genericParameters);
                name.Append('>');
            }

            var hasOverloads = methodToBeDisplayed.ContainingTypeDefinition.Methods.Count(m => m.Name.Value == methodToBeDisplayed.Name.Value) > 1;
            if (methodToBeDisplayed.ParameterCount > 0 && hasOverloads)
            {
                var parametersTypes = methodToBeDisplayed.Parameters.Select(p => p.Type);
                var parameters = string.Join(",", parametersTypes);

                name.Append('(');
                name.Append(parameters);
                name.Append(')');
            }
            return name.ToString();
        }
    }
}
