using System.Diagnostics.Contracts;

namespace Examples
{
    public class Door
    {
        public bool emergency;
        public bool closed;
        public bool moving;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(emergency ? !closed : true);
        }

        public Door()
        {
            closed = true;
            moving = false;
            emergency = false;
        }

        public void Open()
        {
            Contract.Requires(closed && !moving);

            closed = false;
        }

        public void Close()
        {
            Contract.Requires(!closed && !emergency);

            closed = true;
        }

        public void Start()
        {
            Contract.Requires(!moving);

            moving = true;
            if (!emergency) closed = true;
        }

        public void Stop()
        {
            Contract.Requires(moving);

            moving = false;
        }

        public void Alarm()
        {
            Contract.Requires(!emergency);

            emergency = true;
            closed = false;
        }

        public void Safe()
        {
            Contract.Requires(emergency);

            emergency = false;
        }
    }
}