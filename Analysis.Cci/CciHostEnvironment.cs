using Microsoft.Cci.MutableContracts;

namespace Analysis.Cci
{
    public class CciHostEnvironment
    {
        protected static CodeContractAwareHostEnvironment host;

        public static CodeContractAwareHostEnvironment GetInstance()
        {
            if (host == null)
            {
                host = new CodeContractAwareHostEnvironment();
            }
            return host;
        }
    }
}