using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic_Teller_Simulator.Classes
{
    internal class Bank : Account
    {
        internal const float maximumTopUp = 100000000;
        private const float refillAmount = 5000;

        public float refillATM(){
            return refillAmount;
        }
    }
}
