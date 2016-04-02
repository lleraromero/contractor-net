using System.Diagnostics.Contracts;

namespace Examples.Zoppi
{
    public class Microwave
    {
        public bool cooking;
        public bool doorOpen;
        public bool pause;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(!pause || doorOpen);
            Contract.Invariant(!pause || cooking);

            // Este no se si hace falta, pensarlo mejor.
            // Creo que reemplaza a los otros 2
            Contract.Invariant(!cooking || !doorOpen || pause);
        }

        public Microwave()
        {
			//Contract.Ensures(!cooking && !doorOpen && !pause);

            cooking = false;
            doorOpen = false;
            pause = false;
        }

        public void Start(int seconds)
        {
            Contract.Requires(!cooking && !doorOpen);
            //Contract.Ensures(cooking);

            cooking = true;
        }

        public void Stop()
        {
            Contract.Requires(cooking);
            //Contract.Ensures(!cooking && !pause);

            cooking = false;
            pause = false;
        }

        public void Finish()
        {
            Contract.Requires(cooking && !pause);
            //Contract.Ensures(!cooking);

            cooking = false;
        }

        public void DoorOpen()
        {
            Contract.Requires(!doorOpen);
            //Contract.Ensures(doorOpen);
            //Contract.Ensures(!cooking || pause);

            doorOpen = true;
            if (cooking) pause = true;
        }

        public void DoorClosed()
        {
            Contract.Requires(doorOpen);
            //Contract.Ensures(!doorOpen);
            //Contract.Ensures(!pause);

            doorOpen = false;
            pause = false;
        }
    }
}
