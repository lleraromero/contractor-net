using System.Diagnostics.Contracts;

namespace Examples.Zoppi
{
    // Tambien funciona con un array generico de tipo T
	public class BoundedStack
	{
		public object[] elems;
		public int next;

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(this.elems != null);
			Contract.Invariant(this.next >= 0);
			Contract.Invariant(this.next <= this.elems.Length);
		}

		public BoundedStack(int size)
		{
			Contract.Requires(size > 0);

			this.elems = new object[size];
			this.next = 0;
		}

		public void Push(object elem)
		{
			Contract.Requires(this.next < this.elems.Length);

			this.elems[this.next] = elem;
            this.next++;
		}

		public object Pop()
		{
			Contract.Requires(this.next > 0);

            this.next--;
			return this.elems[this.next];
		}
	}
}
