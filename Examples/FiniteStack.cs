using System.Diagnostics.Contracts;

namespace Examples
{
    public class FiniteStack
    {
        public int Max { get; set; }
        public int Next { get; set; }

        public FiniteStack()
        {
            Contract.Ensures(Max == 5 && Next == -1);

            Max = 5;
            Next = -1;
        }

        public FiniteStack(int size)
        {
            Contract.Requires(size > 2);
            Contract.Ensures(Max == size && Next == -1);

            Max = size;
            Next = -1;
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Max > 2 && Next >= -1 && Max > Next);
        }

        public void Pop()
        {
            Contract.Requires(Next > -1);
            Contract.Ensures(Next == Contract.OldValue<int>(Next) - 1);

            Next = Next - 1;
        }

        public void Push(int elem)
        {
            Contract.Requires(Next < Max);
            Contract.Ensures(Next == Contract.OldValue<int>(Next) + 1);

            Next = Next + 1;
        }


        //#region tests
        //// s0: {ctor}
        //// s1: {push}
        //// s2: {push, pop}
        //// s3: {pop}

        //public void run_reachability_tests()
        //{
        //    //s1_push_s1();
        //    //s1_push_s3();
        //    //s1_push_s2();
        //}

        //private void s1_push_s1()
        //{
        //    Contract.Assume(Next < Max); // push pre 
        //    Contract.Assume(!(Next > -1)); // !pop pre
        //    Contract.Assume(Max > 2 && Next >= -1 && Max > Next);
        //    Push(3);
        //    Contract.Assert(!(!(Next > -1) && Next < Max));
        //}

        //private void s1_push_s2()
        //{
        //    Contract.Assume(Next < Max); // push pre 
        //    Contract.Assume(!(Next > -1)); // !pop pre
        //    Contract.Assume(Max > 2 && Next >= -1 && Max > Next);
        //    Push(3);
        //    Contract.Assert(!(Next > -1 && Next < Max));
        //}

        //private void s1_push_s3()
        //{
        //    Contract.Assume(Next < Max); // push pre
        //    Contract.Assume(!(Next > -1)); // !pop pre
        //    Contract.Assume(Max > 2 && Next >= -1 && Max > Next);
        //    Push(3);
        //    Contract.Assert(!(Next > -1 && !(Next < Max)));
        //}

        //#endregion
    }
}