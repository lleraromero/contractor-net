using System;

namespace Tests.Cases
{
    class NonInstrumentedStatic
    {
        public static void Main(string[] args)
        {
            int x = Int32.Parse("33");
            int y = x * 25;
            Console.WriteLine(y);
        }
    }
}
