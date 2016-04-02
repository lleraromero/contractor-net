using System.Diagnostics.Contracts;

namespace Examples.Zoppi
{
    public class VendingMachine
    {
        public bool selling;
        public uint change;

        [ContractInvariantMethod]
        private void Invariant()
        {
            //Contract.Invariant(change == 0 || selling);
            Contract.Invariant(change > 0 ? selling : true);
        }

        public VendingMachine()
        {
            selling = false;
            change = 0;
        }

        public void DisplayPrice(uint item)
        {
            Contract.Requires(!selling);
        }

        public void Buy(uint item, uint money)
        {
            Contract.Requires(!selling);

            selling = true;
            if (money > 0) change = 5;
        }

        public void ReturnChange()
        {
            Contract.Requires(selling);
            Contract.Requires(change > 0);

            change = 0;
        }

        public void ReturnItem()
        {
            Contract.Requires(change == 0);
            Contract.Requires(selling);

            selling = false;
        }
    }
}
