using System.Diagnostics.Contracts;

namespace Examples.Zoppi
{
	public class Door
	{
		public bool emergency;
		public bool closed;
		public bool moving;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(emergency ? closed == false : true);
        }

		public Door()
		{
			//Contract.Ensures(closed == true);
			//Contract.Ensures(moving == false);
			//Contract.Ensures(emergency == false);

			closed = true;
			moving = false;
			emergency = false;
		}

		public void Open()
		{
            Contract.Requires(closed && !moving);

			//Contract.Ensures(closed == false);
			//Contract.Ensures(moving == Contract.OldValue<bool>(moving));
			//Contract.Ensures(emergency == Contract.OldValue<bool>(emergency));

            closed = false;
		}

		public void Close()
		{
            Contract.Requires(!closed && !emergency);

			//Contract.Ensures(closed == true);
			//Contract.Ensures(moving == Contract.OldValue<bool>(moving));
			//Contract.Ensures(emergency == Contract.OldValue<bool>(emergency));

			closed = true;
		}

		public void Start()
		{
            Contract.Requires(!moving);

			//Contract.Ensures(moving == true);
			//Contract.Ensures(emergency == Contract.OldValue<bool>(emergency));
			//Contract.Ensures(closed == (!emergency ? true : Contract.OldValue<bool>(closed)));

			moving = true;
            if (!emergency) closed = true;
		}

		public void Stop()
		{
            Contract.Requires(moving);

			//Contract.Ensures(moving == false);
			//Contract.Ensures(closed == Contract.OldValue<bool>(closed));
			//Contract.Ensures(emergency == Contract.OldValue<bool>(emergency));

			moving = false;
		}

		public void Alarm()
		{
            Contract.Requires(!emergency);

			//Contract.Ensures(emergency == true);
			//Contract.Ensures(closed == false);
			//Contract.Ensures(moving == Contract.OldValue<bool>(moving));

			emergency = true;
			closed = false;
		}

		public void Safe()
		{
            Contract.Requires(emergency);

			//Contract.Ensures(emergency == false);
			//Contract.Ensures(moving == Contract.OldValue<bool>(moving));
			//Contract.Ensures(closed == Contract.OldValue<bool>(closed));

			emergency = false;
            //if (moving) closed = true;
		}
	}
}
