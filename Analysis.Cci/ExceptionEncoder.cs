using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public class ExceptionEncoder
    {
        private List<string> listOfExceptions;

        public ExceptionEncoder(List<string> listOfExceptions)
        {
            this.listOfExceptions = listOfExceptions;
        }
        public int ExceptionToInt(string exitCode){
            Contract.Requires(exitCode != null);
            return listOfExceptions.IndexOf(exitCode);
        }
    }
}
