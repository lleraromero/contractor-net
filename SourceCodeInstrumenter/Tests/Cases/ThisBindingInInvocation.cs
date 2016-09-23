using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class ThisBindingInInvocation
    {
        public static void Main(string[] args)
        {
            string parametro = "valor de parametro";
            Binded binded = new Binded(parametro);
            string copia = binded.campo;
        }

        public class Binded
        {
            public string campo;

            public Binded(string c)
            {
                this.campo = c;
                this.Initialize();
            }

            private void Initialize()
            {
                campo = "valor cambiado";
            }
        }
    }
}
