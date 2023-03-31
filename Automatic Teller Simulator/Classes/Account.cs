using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Automatic_Teller_Simulator.Classes
{
    internal class Account
    {
        protected string pinNumber { get; set; }
        protected string accountNumber { get; set; }
        protected Single accountBalance { get; set; }
        protected float maximumWithDrawal { get; set; }
        protected float maximumTransferAmount { get; set; }


        public virtual bool withdraw(float amount)
        {
            if ((amount % 10) != 0)
            {
                MessageBox.Show(
                    "The ATM accepts only transactions for which the amount " +
                    "entered is a multiple of $10");
                return false;
            }
            if (amount > accountBalance)
            {
                MessageBox.Show(
                    "Your balance is less than the transaction amount.");
                return false;
            }

            accountBalance -= amount;
            return true;
        }
        public bool deposit(float amount){
            accountBalance += amount;
            return true;
        }
        public bool transferOut(float amount)
        {
            //Check allowed max amount
            if (amount > 10000)
            {
                MessageBox.Show(
                    "Amount greater than the allowed 10,000.");
                return false;
            }
            if (amount > accountBalance)
            {
                MessageBox.Show(
                    "Your balance is less than the transaction amount");
                return false;
            }

            accountBalance -= amount;
            return true;
        }
        public bool transferIn(float amount){
            accountBalance += amount;
            return true;
        }
        public string GetPin() { return pinNumber; }
        public string GetAccountNumber() { return accountNumber; }
        public float GetBalance() { return accountBalance; }
    }
}
