using System.Diagnostics.Contracts;

namespace Examples
{
    class DoorPost
    {
        public bool emergency;
        public bool closed;
        public bool moving;

        [ContractInvariantMethod]
        private void Invariant()
        {
            //Contract.Invariant(emergency ? !closed : true);
            Contract.Invariant(!emergency || !closed);
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
            Contract.Requires(!emergency && !emergency);
            Contract.Ensures(emergency && emergency && !closed);

            emergency = true;
            closed = false;
        }

        public void Safe()
        {
            Contract.Requires(emergency);
            Contract.Ensures(!emergency);

            emergency = false;
        }

        #region tests
        //s0: {stop, safe}
        //s1: {start, safe}
        private void s0_stop_s1()
        {
            Contract.Requires(moving); //stop
            Contract.Requires(emergency); //safe
            Contract.Requires(emergency); //not alarm
            Contract.Requires(!(!closed && !emergency)); //not close closed || emergency
            Contract.Requires(!closed || moving); //not open
            Contract.Requires(moving); //not start


            Contract.Ensures(!!moving); //start
            Contract.Ensures(!emergency); //safe

            Contract.Ensures(!!moving); //not stop
            Contract.Ensures((closed && !moving)); //not open
            Contract.Ensures(!emergency); //not alarm
            Contract.Ensures((!closed && !emergency)); //not close

            Stop();
        }
        #endregion
    }
}