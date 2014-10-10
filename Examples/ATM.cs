using System.Diagnostics.Contracts;

namespace Examples
{
    public class ATM
    {
        public bool theCardIn;
        public bool carHalfway;
        public bool passwordGiven;
        public int card;
        public int passwd;

        public ATM()
        {
            Contract.Ensures(!theCardIn && !carHalfway && !passwordGiven);
            Contract.Ensures(card == 0 && passwd == 0);

            theCardIn = false;
            carHalfway = false;
            passwordGiven = false;
            card = 0;
            passwd = 0;
        }

        public void InsertCard(int c)
        {
            Contract.Requires(!theCardIn && c > 0);
            Contract.Ensures(theCardIn && card == c);

            theCardIn = true;
            card = c;
        }

        public void EnterPassword(int q)
        {
            Contract.Requires(!passwordGiven && q > 0);
            Contract.Ensures(passwordGiven && passwd == q);

            passwordGiven = true;
            passwd = q;
        }

        public void TakeCard()
        {
            Contract.Requires(carHalfway);
            Contract.Ensures(!carHalfway && !theCardIn);

            carHalfway = false;
            theCardIn = false;
        }

        public void DisplayMainScreen()
        {
            Contract.Requires(!theCardIn && !carHalfway);
        }

        public void RequestPassword()
        {
            Contract.Requires(!passwordGiven);
        }

        public void EjectCard()
        {
            Contract.Requires(theCardIn);
            Contract.Ensures(!theCardIn && carHalfway && card == 0 && passwd == 0 && !passwordGiven);

            theCardIn = false;
            carHalfway = true;
            card = 0;
            passwd = 0;
            passwordGiven = false;
        }

        public void RequestTakeCard()
        {
            Contract.Requires(carHalfway);
        }

        public void CanceledMessage()
        {
            Contract.Requires(theCardIn);
        }

        //#region tests
        //private void s0_ctor_dismainscreen()
        //{
        //    Init();

        //    Contract.Assert(!(!theCardIn && !carHalfway)); // display main screen
        //    Contract.Assert(!(!passwordGiven)); //
        //    //Contract.Assert(!(!theCardIn && c > 0));
        //}
        //#endregion
    }
}