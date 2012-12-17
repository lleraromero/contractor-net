using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace API_Examples
{
    public class ATM
    {
        public bool cardInserted;
        public bool authenticated;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(!authenticated || cardInserted);
        }

        public ATM()
        {
            cardInserted = false;
            authenticated = false;
        }

        public void InsertCard(int number)
        {
            Contract.Requires(!cardInserted);

            cardInserted = true;
        }

        public void RemoveCard()
        {
            Contract.Requires(cardInserted);

            cardInserted = false;
            authenticated = false;
        }

        public void Authenticate(int pin)
        {
            Contract.Requires(cardInserted);
            Contract.Requires(!authenticated);

            authenticated = true;
        }

        public void PrintTicket()
        {
            Contract.Requires(authenticated);
        }

        public void Extract(int amount)
        {
            Contract.Requires(authenticated);
        }

        public void Deposit(int amount)
        {
            Contract.Requires(authenticated);
        }

        public void ChangePin(int pin)
        {
            Contract.Requires(authenticated);
        }
    }
}
