using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic_Teller_Simulator.Classes
{
    internal class Savings : Account
    {
        private const float interestRate = 2.5f;

        internal Savings(string pin, string accountNumber, float balance)
        {
            base.pinNumber =  pin;
            base.accountNumber = accountNumber;
            base.accountBalance = balance;
        }

        public bool PayInterest(float amount)
        {
            accountBalance += (amount * 0.01f);
            return true;
        }
    }
}
