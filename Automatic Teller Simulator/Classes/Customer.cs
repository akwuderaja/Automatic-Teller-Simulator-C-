using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic_Teller_Simulator.Classes
{
    internal class Customer
    {
        private string name { get; set; }
        private string PINNumber { get; set; }

        public Customer(string name, string PINNumber)
        {
            this.name = name;
            this.PINNumber = PINNumber;
        }

        public string GetName()
        {
            return name;
        }
        public string GetPinNumber()
        {
            return PINNumber;
        }
    }
}
