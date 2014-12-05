using System;
using System.Diagnostics.Contracts;

namespace Examples
{
    class Test
    {
        public bool Pepe;

        public Test()
        {
            this.Pepe = false;
        }

        public void acot()
        {
            Contract.Requires(!this.Pepe);
            Contract.Ensures(this.Pepe);

            for (int i = 0; i < 20; i++)
            {
                if (i == 3)
                {
                    this.Pepe = true;
                }
            }
        }

        public void sum()
        {
            Contract.Requires(!this.Pepe);

            int k = 0;
            for (int i = 0; i < 10; i++)
            {
                if (i == 9)
                {
                    k = 10;
                }
            }
            if (k == 0)
            {
                this.Pepe = true;
            }
            else
            {
                this.Pepe = false;
            }
        }

        public void param(int k)
        {
            Contract.Requires(!this.Pepe);
            Contract.Ensures(!(k < 20) || this.Pepe);
            //Contract.Ensures(!this.Pepe);

            for (int i = 0; i < 20; i++)
            {
                if (i == k)
                {
                    this.Pepe = true;
                }
            }
        }

        public void rand()
        {
            Contract.Requires(!this.Pepe);
            //Contract.Ensures(this.Pepe);

            for (int i = 0; i < 20; i++)
            {
                if (i == new Random().Next())
                {
                    this.Pepe = true;
                }
            }
        }

        public void no()
        {
            Contract.Requires(!this.Pepe);
        }

        public void si()
        {
            Contract.Requires(this.Pepe);
        }
    }
}
