using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Examples
{
    public class Linear
    {
        public int N { get; private set; }

        public Linear()
        {
            Contract.Ensures(N == 0);

            N = 0;
        }

        public void a0()
        {
            Contract.Requires(N == 0);
            Contract.Ensures(N == Contract.OldValue<int>(N) + 1);
            N++;
        }

        public void a1()
        {
            Contract.Requires(N == 1);
            Contract.Ensures(N == Contract.OldValue<int>(N) + 1);
            N++;
        }

        public void a2()
        {
            Contract.Requires(N == 2);
            Contract.Ensures(N == Contract.OldValue<int>(N) + 1);
            N++;
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(N >= 0 && N <= 3);
        }
    }
}
