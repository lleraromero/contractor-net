using System.Diagnostics.Contracts;

namespace Examples.Zoppi
{
    public class Elevator
    {
        public bool overweight;
        public bool moving;
        public bool requested;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!overweight || !moving);
            Contract.Invariant(!overweight || !requested);
            Contract.Invariant(!moving || !requested);
        }

        public Elevator()
        {
            overweight = false;
            moving = false;
            requested = false;
        }

        public void Request(int floor)
        {
            Contract.Requires(!requested);
            Contract.Requires(!moving);
            Contract.Requires(!overweight);

            requested = true;
        }

        public void Arrive()
        {
            Contract.Requires(moving || requested);

            moving = false;
            requested = false;
        }

        public void GoTo(int floor)
        {
            Contract.Requires(!moving);
            Contract.Requires(!requested);
            Contract.Requires(!overweight);

            moving = true;
        }

        public void Alarm()
        {
            Contract.Requires(!overweight);
            Contract.Requires(!requested);
            Contract.Requires(!moving);

            overweight = true;
        }

        public void Safe()
        {
            Contract.Requires(overweight);

            overweight = false;
        }
    }
}
