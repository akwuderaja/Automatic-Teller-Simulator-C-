using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Automatic_Teller_Simulator.Classes
{
    internal class Chequing : Account
    {
        private int maximumBillAmount { get; set; }
        private const float billFee = 1005f;
        
        internal Chequing(string pin, string accountNumber, float balance)
        {
            base.pinNumber = pin;
            base.accountNumber = accountNumber;
            base.accountBalance = balance;
        }

        public bool PayBill(float amount){
            return transferOut(amount);
        }

        override
        public bool withdraw(float amount)
        {
            if (amount > 1000)
            {
                MessageBox.Show(
                    "$1000 is the maximum per withdrawal.");
                return false;
            }
            return base.withdraw(amount);
        }
    }
}
