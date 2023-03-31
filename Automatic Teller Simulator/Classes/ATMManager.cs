using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Automatic_Teller_Simulator.Classes
{
    internal enum AccountType
    {
        Chequing,
        Savings
    }
    internal class ATMManager
    {
        public Bank bank { get; set; }
        public List<Customer> customers { get; set; }
        public List<Chequing> chequingAccounts { get; set; }
        public List<Savings> savingsAccounts { get; set; }
        public float currentAccountBalance = 20000;
        public List<DailyBalance> dailyBalances { get; set; }
        internal bool isInService = true;


        public ATMManager()
        {
            bank = new Bank();
            customers = new List<Customer>();
            //chequingAccounts = new List<Chequing>();
            //savingsAccounts = new List<Savings>();
            dailyBalances = new List<DailyBalance>();
        }

        public bool ValidateUser(string name, string pin)
        {
            foreach (var customer in customers)
            {
                if (customer.GetName() == name &&
                    customer.GetPinNumber() == pin)
                {
                    return true;
                }
            }
            return false;
        }
        public bool WithdrawChequing(string pin, float amount)
        {
            var account = chequingAccounts.FirstOrDefault(x => x.GetPin() == pin);
            if(account != null)
            {
                return account.withdraw(amount);
            }
            return false;
        }

        public bool WithdrawSavings(string pin, float amount)
        {
            var account = savingsAccounts.FirstOrDefault(x => x.GetPin() == pin);
            if (account != null)
            {
                if (account.withdraw(amount))
                    return true;
            }
            return false;
        }

        public bool DepositChequing(string pin, float amount)
        {
            var account = chequingAccounts.FirstOrDefault(x => x.GetPin() == pin);
            if (account != null)
            {
                if (account.deposit(amount))
                    return true;
            }
            return false;
        }

        public bool DepositSavings(string pin, float amount)
        {
            var account = savingsAccounts.FirstOrDefault(x => x.GetPin() == pin);
            if (account != null)
            {
                if (account.deposit(amount))
                    return true;
            }
            return false;
        }

        public bool PayBillPayment(string pin, float amount)
        {
            var account = chequingAccounts.FirstOrDefault(x => x.GetPin() == pin);
            if (account != null)
            {
                //Check allowed max amount
                if (amount > 10000)
                {
                    MessageBox.Show(
                        "Amount greater than the allowed 10,000.");
                    return false;
                }
                return account.PayBill(amount + 1.25f);
            }
            return false;
        }

        //The account type determines from which
        //account to transfer out, and which account to
        //transfer in
        public bool TransferFunds(string pin, float amount, AccountType accountType)
        {
            var accountSaving = savingsAccounts.FirstOrDefault(x => x.GetPin() == pin);
            var accountChequing = chequingAccounts.FirstOrDefault(x => x.GetPin() == pin);
            if (accountSaving == null)
            {
                MessageBox.Show("Savings account not found for the given user!");
                return false;
            }else if (accountChequing == null)
            {
                MessageBox.Show("Chequing account not found for the given user!");
                return false;
            }
            if (accountType == AccountType.Savings)
            {
                if (accountSaving.transferOut(amount))
                    return accountChequing.transferIn(amount);
            }else if (accountType == AccountType.Chequing)
                if (accountChequing.transferOut(amount))
                    return accountSaving.transferIn(amount);

            return false;
        }

        public bool ReadCustomers()
        {
            try
            {
                var filename = Environment.CurrentDirectory + "\\Database\\Customers.txt";
                using (var streamReader = new StreamReader(filename))
                {
                    customers = new List<Customer>();
                    while (streamReader.Peek() >= 0)
                    {
                        string line = streamReader.ReadLine();
                        string[] str = line.Split(Convert.ToChar(","));
                        if (str.Length > 0)
                        {
                            customers.Add(new Customer(str[0], str[1]));
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File Exception thrown {0} ", e.Message);
                return false;
            }
            return true;
        }
        
        public bool ReadAccounts()
        {
            //Do not read the first character(C or S) found in
            //the Accounts.txt into the object instances. Use
            //it to determine to which collection the account
            //should be added(chequingAccounts or
            //savingAccounts)

            try
            {
                var filename = Environment.CurrentDirectory + "\\Database\\Accounts.txt";
                using (var streamReader = new StreamReader(filename))
                {
                    savingsAccounts = new List<Savings>();
                    chequingAccounts = new List<Chequing>();
                    while (streamReader.Peek() >= 0)
                    {
                        string line = streamReader.ReadLine();
                        string[] str = line.Split(Convert.ToChar(","));
                        if (str.Length >= 4)
                        {
                            float bal;
                            float.TryParse(str[3], out bal);

                            if (str[0] == "C")
                                chequingAccounts.Add(new Chequing(str[1], str[2], bal));
                            else if (str[0] == "S")
                                savingsAccounts.Add(new Savings(str[1], str[2], bal));
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File Exception thrown {0} ", e.Message);
                return false;
            }
            return true;
        }
        
        public bool WriteAccounts()
        {
            //Write the bank, all chequingAccounts, and all
            //savingsAccounts to Accounts.txt.
            // Add a first character(C or S) to the beginning
            //of each account string before writing to
            //Accounts.txt
            
            try
            {
                var filename = Environment.CurrentDirectory + "\\Database\\Accounts.txt";
                using (var streamWriter = new StreamWriter(filename))
                {
                    foreach (var item in chequingAccounts)
                        streamWriter.WriteLine($"C,{item.GetPin()},{item.GetAccountNumber()},{item.GetBalance()}");
            
                    foreach (var item in savingsAccounts)
                        streamWriter.WriteLine($"S,{item.GetPin()},{item.GetAccountNumber()},{item.GetBalance()}");                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File Exception thrown {0} ", e.Message);
                return false;
            }
            return true;
        }
        
        public bool ReadDailyBalances()
        {
            try
            {
                var filename = Environment.CurrentDirectory + "\\Database\\DailyBalances.txt";
                if (!File.Exists(filename))
                {
                    AddDailyBalance(20000);
                    return false;
                }
                using (var streamReader = new StreamReader(filename))
                {
                    dailyBalances = new List<DailyBalance>();
                    while (streamReader.Peek() >= 0)
                    {
                        string line = streamReader.ReadLine();
                        string[] str = line.Split(Convert.ToChar(","));
                        if (str.Length >= 4)
                        {
                            float bal;
                            float.TryParse(str[1], out bal);
                                dailyBalances.Add(new DailyBalance(str[0], bal));
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File Exception thrown {0} ", e.Message);
                return false;
            }
            return false;
        }

        public bool CheckDailyBalances()
        {
            //Checks DailyBalances.txt for today’s date
            return dailyBalances.FirstOrDefault(
                    x => x.atmDate.Date == DateTime.Now.Date) != null;
        }

        public void AddDailyBalance(float amount)
        {
            //Adds new record for today’s date and default
            //ATM balance
            if (!CheckDailyBalances())
            {
                dailyBalances.Add(new DailyBalance(DateTime.Now.ToString(),
                    amount));
            }
            else
            {
                dailyBalances.FirstOrDefault(
                    x => x.atmDate.Date == DateTime.Now.Date
                ).atmBalance += amount;
            }
            WriteDailyBalances();
        }

        public void WriteDailyBalances()
        {
            //Writes all records to DailyBalances.txt
            try
            {
                var filename = Environment.CurrentDirectory + "\\Database\\DailyBalances.txt";
                using (var streamWriter = new StreamWriter(filename))
                {
                    foreach (var item in dailyBalances)
                        streamWriter.WriteLine(
                            $"C,{item.atmDate},{item.atmBalance}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File Exception thrown {0} ", e.Message);
            }
        }
    }
}
