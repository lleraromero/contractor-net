using System;
namespace Tests.Cases
{
    class AliasingGlobal
    {
        static int GlobalVariable;
        public static void Main(string[] args)
        {
            int x = 3;
            AliasingGlobal.GlobalVariable = x;
            int y = GlobalVariable;
        }
    }
}
