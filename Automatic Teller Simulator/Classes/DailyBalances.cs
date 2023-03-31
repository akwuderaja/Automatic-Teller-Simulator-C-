using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Automatic_Teller_Simulator.Classes
{
    static internal class DailyBalances
    {

        static public bool Add(ATMManager aTM, float amount) //to update atmBalance when supervisor fills up ATM
        {
            if(aTM.currentAccountBalance + amount >= 20000)
            {
                MessageBox.Show("You can't refill more than $20000.");
                return false;
            }
            aTM.AddDailyBalance(amount);
            return true;
        }
    }
}
