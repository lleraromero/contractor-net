using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contractor.Core
{
    public class ImplementedExceptions
    {
        private static ImplementedExceptions singleton;
        private List<string> allExceptions;
        private ImplementedExceptions(List<string> allExceptions)
        {
            this.allExceptions = allExceptions;
        }

        public static void AddAllExceptionsTo(List<string> errorList)
        {
            if(singleton==null)
                throw new InvalidOperationException("You need to create an instance before using the singleton");

            errorList.AddRange(singleton.allExceptions);
        }

        public static void CreateInstance(List<string> allExceptions)
        {
            singleton = new ImplementedExceptions(allExceptions);
        }
    }
}
