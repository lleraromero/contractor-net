namespace Tests.Cases
{
    class SlicingDispatchLookupNonInstrumented
    {
        public static void Main(string[] args)
        {
            A a = new A();
            System.Console.WriteLine(a.ToString());
        }

        class A { }
    }
}
