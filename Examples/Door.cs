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

    public class DoorWithPost
    {
        public bool emergency;
        public bool closed;
        public bool moving;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(!this.emergency || !this.closed);
        }

        public DoorWithPost()
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
            // TODO: Investigar por que duplicar las condiciones hace fallar el algoritmo 
            //Contract.Requires(!emergency && !emergency);
            //Contract.Ensures(emergency && emergency && !closed);

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