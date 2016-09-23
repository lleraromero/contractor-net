using System;
namespace Tests.Cases
{
    class NWAliasingGlobalPrefix
    {
        static AA GlobalVariable = new AA();
        public static void Main(string[] args)
        {
            int x = 3;
            NWAliasingGlobalPrefix.GlobalVariable.GV = x;
            int y = GlobalVariable.GV;
        }
        public class AA
        {
            public int GV;
        }
    }
}
