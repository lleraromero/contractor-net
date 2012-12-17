using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace API_Examples
{
    public class EnrichedStackSimple
    {
        public uint state = 0;
		public const int capacity = 5;
		public int count;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(count >= 0);
            Contract.Invariant(count <= capacity);

            //Contract.Invariant(this.state == 1 ? (this.count < capacity) && !(this.count > 0) : true);
            //Contract.Invariant(this.state == 2 ? (this.count < capacity) && (this.count > 0) : true);
            //Contract.Invariant(this.state == 3 ? !(this.count < capacity) && (this.count > 0) : true);
        }

        public EnrichedStackSimple()
        {
            //Contract.Ensures(this.state == 1);

            Contract.Ensures(this.count == 0);

            try
            {
                this.count = 0;
            }
            finally
            {
                switch (this.state)
                {
                    case 0:
                        this.state = 1;
                        break;
                }
            }
        }

        public void Push()
        {
            //Contract.Requires(this.state == 1 || this.state == 2);
            //Contract.Ensures(Contract.OldValue<uint>(this.state) == 1 ? this.state == 2 : true);
            //Contract.Ensures(Contract.OldValue<uint>(this.state) == 2 ? (this.count < capacity ? this.state == 2 : this.state == 3) : true);

            Contract.Requires(this.count < capacity);
            Contract.Ensures(this.count == Contract.OldValue<int>(this.count) + 1);

            try
            {
                this.count++;
            }
            finally
            {
                switch (this.state)
                {
                    case 1:
                        this.state = 2;
                        break;

                    case 2:
                        //if ((this.count < capacity) && (this.count > 0))
                        if (this.count < capacity)
                        {
                            this.state = 2;
                        }
                        //else if (this.count == capacity)
                        else
                        {
                            this.state = 3;
                        }
                        break;
                }
            }
        }

        public void Pop()
        {
            //Contract.Requires(this.state == 2 || this.state == 3);
            //Contract.Ensures(Contract.OldValue<uint>(this.state) == 2 ? (this.count > 0 ? this.state == 2 : this.state == 1) : true);
            //Contract.Ensures(Contract.OldValue<uint>(this.state) == 3 ? this.state == 2 : true);

            Contract.Requires(this.count > 0);
            Contract.Ensures(this.count == Contract.OldValue<int>(this.count) - 1);

            try
            {
                this.count--;
            }
            finally
            {
                switch (this.state)
                {
                    case 2:
                        //if ((this.count > 0) && (this.count < capacity))
                        if (this.count > 0)
                        {
                            this.state = 2;
                        }
                        //else if (this.count == 0)
                        else
                        {
                            this.state = 1;
                        }
                        break;

                    case 3:
                        this.state = 2;
                        break;
                }
            }
        }
    }
}
