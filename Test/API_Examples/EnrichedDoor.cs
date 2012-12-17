using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace API_Examples
{
	public class EnrichedDoor
	{
		public uint state;

		public bool emergency;
		public bool closed;
		public bool moving;

		public EnrichedDoor()
		{
			Contract.Ensures(state == 1);
			state = 1;

			closed = true;
			moving = false;
			emergency = false;
		}

		public void Open()
		{
			Contract.Requires(state == 1);

			Contract.Ensures(Contract.OldValue<uint>(state) == 1 ? state == 2 : true);
			switch (state)
			{
				case 1: state = 2; break;
			}

            closed = false;
		}

		public void Close()
		{
			Contract.Requires(state == 2 || state == 6);

			Contract.Ensures(Contract.OldValue<uint>(state) == 2 ? state == 1 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 6 ? state == 3 : true);
			switch (state)
			{
				case 2: state = 1; break;
				case 6: state = 3; break;
			}

			closed = true;
		}

		public void Start()
		{
			Contract.Requires(state == 1 || state == 2 || state == 4);

			Contract.Ensures(Contract.OldValue<uint>(state) == 1 ? state == 3 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 2 ? state == 3 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 4 ? state == 5 : true);
			switch (state)
			{
				case 1: state = 3; break;
				case 2: state = 3; break;
				case 4: state = 5; break;
			}

			moving = true;
            if (!emergency) closed = true;
		}

		public void Stop()
		{
			Contract.Requires(state == 3 || state == 6 || state == 5);

			Contract.Ensures(Contract.OldValue<uint>(state) == 3 ? state == 1 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 6 ? state == 2 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 5 ? state == 4 : true);
			switch (state)
			{
				case 3: state = 1; break;
				case 6: state = 2; break;
				case 5: state = 4; break;
			}

			moving = false;
		}

		public void Alarm()
		{
			Contract.Requires(state == 3 || state == 6 || state == 2 || state == 1);

			Contract.Ensures(Contract.OldValue<uint>(state) == 3 ? state == 5 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 6 ? state == 5 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 2 ? state == 4 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 1 ? state == 4 : true);
			switch (state)
			{
				case 3: state = 5; break;
				case 6: state = 5; break;
				case 2: state = 4; break;
				case 1: state = 4; break;
			}

			emergency = true;
			closed = false;
		}

		public void Safe()
		{
			Contract.Requires(state == 4 || state == 5);

			Contract.Ensures(Contract.OldValue<uint>(state) == 4 ? state == 2 : true);
			Contract.Ensures(Contract.OldValue<uint>(state) == 5 ? state == 6 : true);
			switch (state)
			{
				case 4: state = 2; break;
				case 5: state = 6; break;
			}

			emergency = false;
            //if (moving) closed = true;
		}
	}
}
