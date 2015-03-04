using System.Diagnostics.Contracts;

namespace Examples
{
    public class DoorPost
    {
        public bool emergency;
        public bool closed;
        public bool moving;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(!this.emergency || !this.closed);
        }

        public DoorPost()
        {
            Contract.Ensures(closed && !moving && !emergency);

            closed = true;
            moving = false;
            emergency = false;
        }

        public void Open()
        {
            Contract.Requires(closed && !moving);
            Contract.Ensures(!closed);

            closed = false;
        }

        public void Close()
        {
            Contract.Requires(!closed && !emergency);
            Contract.Ensures(closed);

            closed = true;
        }

        public void Start()
        {
            Contract.Requires(!moving);
            Contract.Ensures(moving && (emergency || closed));

            moving = true;
            if (!emergency) closed = true;
        }

        public void Stop()
        {
            Contract.Requires(moving);
            Contract.Ensures(!moving);

            moving = false;
        }

        public void Alarm()
        {
            Contract.Requires(!emergency);
            Contract.Ensures(emergency && !closed);

            emergency = true;
            closed = false;
        }

        public void Safe()
        {
            Contract.Requires(emergency);
            Contract.Ensures(!emergency);

            emergency = false;
        }
    }
}