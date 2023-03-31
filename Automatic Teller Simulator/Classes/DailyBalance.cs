using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic_Teller_Simulator.Classes
{
    internal class DailyBalance
    {
        public DateTime atmDate;
        public float atmBalance { get; set; }
        private const float minimumAmount = 1000;

        public DailyBalance(string atmDate, float bal)
        {
            DateTime.TryParse(atmDate, out this.atmDate);
            this.atmBalance = bal;
        }

        public bool UpdateDailyBalance(float amount) //to update atmBalance when supervisor fills up ATM
        {
            atmBalance += amount;
            return true;
        }
    }
}
