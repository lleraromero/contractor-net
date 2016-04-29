using System.Diagnostics.Contracts;

namespace Examples.Zoppi
{
    public class EclatStack
    {
        private int[] elems;
        private int numberOfElements;
        private int max;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(0 <= this.numberOfElements);
            Contract.Invariant(this.numberOfElements <= this.max);
            Contract.Invariant(this.elems.Length == this.max);
            Contract.Invariant(this.max > 0);
            Contract.Invariant(Contract.ForAll(0, this.numberOfElements,
                i => Contract.ForAll(0, this.numberOfElements, j =>
                    i != j || this.elems[i] == this.elems[j])));
        }

        public EclatStack()
        {
            numberOfElements = 0;
            max = 2;
            elems = new int[max];
        }

        public void Push(int k)
        {
            Contract.Requires(this.IsMember(k) || !this.IsFull());

            int index;
            bool alreadyMember;
            alreadyMember = false;

            for (index = 0; index < numberOfElements; index++)
            {
                if (k == elems[index])
                {
                    alreadyMember = true;
                    break;
                }
            }

            if (alreadyMember)
            {
                for (int j = index; j < numberOfElements - 1; j++)
                {
                    elems[j] = elems[j + 1];
                }
                elems[numberOfElements - 1] = k;
            }
            else
            {
                if (numberOfElements < max)
                {
                    elems[numberOfElements] = k;
                    numberOfElements++;
                    return;
                }
                else
                {
                    //System.out.println("Stack full, cannot push");
                    return;
                }
            }
        }

        public void Pop()
        {
            Contract.Requires(!this.IsEmpty());

            numberOfElements--;
        }

        [Pure]
        public int Top()
        {
            Contract.Requires(!this.IsEmpty());

            if (numberOfElements < 1)
            {
                //System.out.println("Empty Stack");
                return -1;
            }
            else
                return elems[numberOfElements - 1];
        }

        [Pure]
        public bool IsEmpty()
        {
            Contract.Ensures(Contract.Result<bool>() == (this.numberOfElements == 0));

            if (numberOfElements == 0)
                return true;
            else
                return false;
        }

        [Pure]
        public int MaxSize()
        {
            Contract.Ensures(Contract.Result<int>() == this.max);

            return max;
        }

        [Pure]
        public bool IsMember(int k)
        {
            Contract.Ensures(Contract.Result<bool>() == Contract.Exists(0, this.numberOfElements, i =>
                k == this.elems[i]));

            for (int index = 0; index < numberOfElements; index++)
                if (k == elems[index])
                    return true;
            return false;
        }

        [Pure]
        public bool Equals(EclatStack s)
        {
            Contract.Requires(s != null);

            if (s.MaxSize() != max)
                return false;
            if (s.GetNumberOfElements() != numberOfElements)
                return false;
            int[] sElems = s.GetArray();
            for (int j = 0; j < numberOfElements; j++)
            {
                if (elems[j] != sElems[j])
                    return false;
            }
            return true;
        }

        [Pure]
        public int[] GetArray()
        {
            int[] a;
            a = new int[max];
            for (int j = 0; j < numberOfElements; j++)
                a[j] = elems[j];
            return a;
        }

        [Pure]
        public int GetNumberOfElements()
        {
            return numberOfElements;
        }

        [Pure]
        public bool IsFull()
        {
            return numberOfElements == max;
        }
    }
}
