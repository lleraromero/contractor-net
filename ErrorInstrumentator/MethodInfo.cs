using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorInstrumentator
{
    public class MethodInfo
    {
        private string returnType;
        private IReadOnlyList<string> exceptions;
        private string methodName;
        private string isPure;
        private string isProperty;

        public MethodInfo(string methodName, string returnType, string isPure, List<string> exceptionList, string isProperty)
        {
 
            this.methodName = methodName;
            this.returnType = returnType;
            this.isPure = isPure;
            this.exceptions = exceptionList;
            this.isProperty = isProperty;
        }

        [Pure]
        public string Name
        {
            get { return methodName; }
            set { methodName = value; }
        }

        [Pure]
        public string ReturnType
        {
            get { return returnType; } 
            set { returnType= value; }
        }

        [Pure]
        public string IsPure
        {
            get { return isPure; }
            set { isPure = value; }
        }

        [Pure]
        public string IsProperty
        {
            get { return isProperty; }
            set { isProperty = value; }
        }

        public IReadOnlyList<string> ExceptionList 
        {
            get { return exceptions; }
            set { exceptions = value; }
        }
    }
}
