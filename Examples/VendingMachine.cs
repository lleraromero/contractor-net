using System.Diagnostics.Contracts;

namespace Examples
{
    public class VendingMachine
    {
        public bool selling;
        public int change;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(change > 0 ? selling : true);
        }

        public VendingMachine()
        {
            Contract.Ensures(!selling && change == 0);

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
            Contract.Ensures(selling);
            Contract.Ensures(!(money > 0) || change == 5);

            selling = true;
            if (money > 0) change = 5;
        }

        public void ReturnChange()
        {
            Contract.Requires(selling);
            Contract.Requires(change > 0);
            Contract.Ensures(change == 0);

            change = 0;
        }

        public void ReturnItem()
        {
            Contract.Requires(change == 0);
            Contract.Requires(selling);
            Contract.Ensures(!selling);

            selling = false;
        }
    }
}