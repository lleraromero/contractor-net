//using System.Diagnostics.Contracts;

//namespace Examples
//{
//    class TestBCT
//    {
//        private int y = 0;

//        public void run(int x)
//        {
//            Contract.Requires(x > 0); // BCT ignores this line
//            Contract.Assume(x > 0, "Requires"); // This line is translated correctly
//            y = x;
//            Contract.Ensures(y == 0); // Ignored
//            Contract.Assert(y == 0, "Ensures"); // Assert translated correctly
//        }
//    }
//}
