using System;
using System.Diagnostics.Contracts;

namespace Examples
{
    public enum Bebida { Coca, Light, Zero, Sprite, Agua }

    public class MaquinaExpendedora
    {
        protected bool ventaEnCurso;
        protected int dineroDisponible;

        [Pure]
        public bool EstaVendiendo() { return ventaEnCurso; }
        [Pure]
        public int DineroDisponible() { return dineroDisponible; }
        [Pure]
        public int PrecioDeLaBotella() { return 15; }

        public MaquinaExpendedora()
        {
            ventaEnCurso = false;
            dineroDisponible = 0;
        }

        public void IngresarDinero(int montoReconocido)
        {
            //Contract.Requires(PREState(asda));
            //Contract.Requires(PreParam(dasdas));
            Contract.Requires(montoReconocido > 0);
            Contract.Requires(!EstaVendiendo());

            //if (montoReconocido <= 0)
            //    return;

            dineroDisponible += montoReconocido;
        }

        public void LiberarBotella(Bebida bebidaSeleccionada)
        {
            Contract.Requires(!EstaVendiendo());
            Contract.Requires(DineroDisponible() >= PrecioDeLaBotella());

            ventaEnCurso = true;
            dineroDisponible -= PrecioDeLaBotella();

            if (dineroDisponible == 0)
                ventaEnCurso = false;
        }

        public void DarVuelto()
        {
            Contract.Requires(EstaVendiendo());
            Contract.Requires(DineroDisponible() > 0);

            dineroDisponible = 0;
            ventaEnCurso = false;
        }
    }

    class TestCorral
    {
        public int Estado;

        public TestCorral()
        {
            Estado = 0;
        }

        /// <summary>
        /// Deberia llevarme solamente al TestigoEstado10, ya que siempre parte de 0 y llega a 10 con el loop.
        /// </summary>
        public void CicloMasLargoQueRecursionBound()
        {
            Contract.Requires(Estado == 0);

            for (int i = 0; i < 10; i++)
            {
                Estado++;
            }
        }

        public void CicloMasChicoQueRecursionBound()
        {
            Contract.Requires(Estado == 0);
            for (int i = 0; i < 1; i++)
            {
                Estado++;
            }
        }

        public void TestigoEstado1()
        {
            Contract.Requires(Estado == 1);
        }

        public void TestigoEstado10()
        {
            Contract.Requires(Estado == 10);
        }

        public void CicloBasadoEnParam(int cota)
        {
            Contract.Requires(Estado == 0);

            for (int i = 0; i < cota; i++)
            {
                Estado++;
            }
        }

        public void CicloDeberiaIrA10()
        {
            Contract.Requires(Estado == 0);

            for (int i = 0; i < 30; i++)
            {
                Estado++;
            }
            Estado = 10;
        }

        public void CicloNoDeberiaIrA10()
        {
            Contract.Requires(Estado == 0);

            for (int i = 0; i < 30; i++)
            {
                return;
            }
            Estado = 10;
        }

        public void CicloDeberiaIrA10ConIf()
        {
            Contract.Requires(Estado == 0);

            for (int i = 0; i < 30; i++)
            {
                Estado++;
            }
            if (Estado == 30)
                Estado = 10;
        }

        public void CicloNoDeberiaIrA10ConIf()
        {
            Contract.Requires(Estado == 0);

            for (int i = 0; i < 30; i++)
            {
                if (i == 10)
                    return;
            }
            Estado = 10;
        }
    }

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
            Contract.Ensures(!this.Pepe);

            for (int i = 0; i < 20; i++)
            {
                if (i == 21)
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
            Contract.Requires(k > 0);
            //Contract.Ensures(!(k < 20) || this.Pepe);
            //Contract.Ensures(!this.Pepe);

            for (int i = 0; i <= k; i++)
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

    
    public class Test2
    {
        public int x;
        public int y;

        public Test2()
        {
            x = 0;
            y = 0;
        }

        public void m1()
        {
            Contract.Requires(x == 0 && y == 0);
            //Contract.Ensures(x == 5);

            x = 5;
        }

        public void m2()
        {
            Contract.Requires(x > 0 /* x == 5 */ && y == 0);
            //Contract.Ensures(y == 7);
            y = 7;
        }

        public void m3()
        {
            Contract.Requires(x == 5 && y == 7);
            x = 3;
            y = 3;
        }        
    }

    class Test3
    {
        public bool B { get; private set; }
        public bool F { get; private set; }
        public int I { get; private set; }

        public Test3()
        {
            B = true;
            F = false;
            I = 0;
        }

        public void m1()
        {
            Contract.Requires(B && I == 0);

            I = 5;
        }

        public void m2()
        {
            Contract.Requires(B && I == 0);

            I = 3;
        }

        public void m3()
        {
            Contract.Requires(B && I == 5);

            B = false;
        }

        public void m4()
        {
            Contract.Requires(B && I == 3);

            B = false;
        }

        public void m5()
        {
            Contract.Requires(!B && I >= 0 && !F);

            F = true;
        }

        public void m6()
        {
            Contract.Requires(I == 0 && F);

            I = 7;
        }
    }
}