//using System;
//using System.Diagnostics;
//using System.Diagnostics.Contracts;

//namespace Examples
//{
//    class TestBoogie
//    {
//        public int y = 0;

//        public int run(int x)
//        {
//            Contract.Requires(x > 0); // BCT ignora requires / ensures
//            Contract.Assume(x > 0, "Requires");
//            //Contract.Invariant(y >= 0);
            
//            x = aux();
//            y++;
            
//            Contract.Assert(x != 0, "Ensures"); // Deberia fallar x = 0!
//            //Contract.Assert(y == 1 + Contract.OldValue<int>(y));
//            //Contract.Ensures(Contract.Result<int>() == 0); 
//            return x;
//        }

//        private int aux()
//        {
//            return 0;
//        }

//        public void pepe(int j)
//        {
//            Contract.Assume(j == 0);
//            ++j;
//            Contract.Assert(j == 0); // Deberia fallar j = 1!
//        }
//    }
//}
