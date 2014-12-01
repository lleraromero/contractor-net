using System;
using System.Diagnostics.Contracts;

namespace Examples
{
    class Test
    {
        public bool Pepe { get; set; }

        public Test()
        {
            Pepe = false;
        }

        public void acot()
        {
            Contract.Requires(!Pepe);
            Contract.Ensures(Pepe);

            for (int i = 0; i < 20; i++)
            {
                if (i == 5)
                {
                    Pepe = true;
                }
            }
        }

        public void param(int k)
        {
            Contract.Requires(!Pepe);
            Contract.Ensures(!(k < 20) || Pepe);

            for (int i = 0; i < 20; i++)
            {
                if (i == k)
                {
                    Pepe = true;
                }
            }
        }

        public void rand()
        {
            Contract.Requires(!Pepe);
            //Contract.Ensures(Pepe);

            for (int i = 0; i < 20; i++)
            {
                if (i == new Random().Next())
                {
                    Pepe = true;
                }
            }
        }

        public void no()
        {
            Contract.Requires(!Pepe);
        }

        public void si()
        {
            Contract.Requires(Pepe);
        }
    }
}
