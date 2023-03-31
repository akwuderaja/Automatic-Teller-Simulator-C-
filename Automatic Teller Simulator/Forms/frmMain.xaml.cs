using Automatic_Teller_Simulator.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Automatic_Teller_Simulator.Forms
{
    /// <summary>
    /// Interaction logic for frmMain.xaml
    /// </summary>
    public partial class frmMain : Window
    {
        private frmLogin loginWindow;
        private string user;
        internal string pin;
        private int counter;
        internal bool isClosed;

        public frmMain(frmLogin loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
            Loaded += FrmMain_Loaded;
            Closing += FrmMain_Closing;
        }

        private void FrmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isClosed)
            {
                this.Visibility = Visibility.Hidden;
                loginWindow.Visibility = Visibility.Visible;
                loginWindow.Activate();

                e.Cancel = true;
            }
        }

        private void FrmMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loginWindow.accountsRead)
            {
                loginWindow.accountsRead = loginWindow.atmManager.ReadAccounts();
            }
        }

        void InputNumber(string number) //validates the amount entered by the user
        {
            int r = 0;
            if (!int.TryParse(number, out r) && number != "-")
            {
                MessageBox.Show("Invalid number");
            }
            else
                txtAmount.Text += r.ToString();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            loginWindow.Visibility = Visibility.Visible;
            loginWindow.Activate();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!loginWindow.atmManager.isInService)
            {
                MessageBox.Show("Atm is out of service!"); return;
            }


            float amount = 0;
            if (!float.TryParse(txtAmount.Text, out amount))
            {
                MessageBox.Show("Invalid amount"); return;
            }

            bool result = false;
            //Check the selected Account and perform operation
            if (rTDeposit.IsChecked.Value)
            {
                if (rAChecking.IsChecked.Value)
                    result = loginWindow.atmManager.DepositChequing(pin, amount);
                else
                    result = loginWindow.atmManager.DepositSavings(pin, amount);
            }else if (rTWithdrawal.IsChecked.Value)
            {
                //CHeck machine funds
                if (amount > loginWindow.atmManager.currentAccountBalance)
                {
                    //Get balance and advice.
                    var balance = ((rAChecking.IsChecked.Value) ?
                        loginWindow.atmManager.chequingAccounts.FirstOrDefault(
                            x => x.GetPin() == pin).GetBalance() :
                        loginWindow.atmManager.savingsAccounts.FirstOrDefault(
                            x => x.GetPin() == pin).GetBalance());
                    string advice =
                        balance >= loginWindow.atmManager.currentAccountBalance ?
                        "Try a smaller amount" : "";
                    MessageBox.Show(
                        $"Machine has insufficient funds\n{advice}");
                    return;
                }
                //Withdraw
                if (rAChecking.IsChecked.Value)
                    result = loginWindow.atmManager.WithdrawChequing(pin, amount);
                else
                    result = loginWindow.atmManager.WithdrawSavings(pin, amount);

                //Deduct from machine on successfull withdrawal
                if(result == true)
                    loginWindow.atmManager.currentAccountBalance -= amount;

            }else if (rTPayBill.IsChecked.Value)
            {
                if (rAChecking.IsChecked.Value)
                    result = loginWindow.atmManager.PayBillPayment(pin, amount);

            }else if (rTTransferFunds.IsChecked.Value)
            {
                if (rAChecking.IsChecked.Value)
                    result = loginWindow.atmManager.TransferFunds(
                        pin, amount, AccountType.Chequing
                    );
                else
                    result = loginWindow.atmManager.TransferFunds(
                        pin, amount, AccountType.Savings
                    );
            }


            if (!result)
            {
                //MessageBox.Show("Opreation Aborted");
            }else
                MessageBox.Show(
                    "Opreation was successfull\nNew balance is: " +
                    ((rAChecking.IsChecked.Value) ? 
                    loginWindow.atmManager.chequingAccounts.FirstOrDefault(
                        x => x.GetPin() == pin).GetBalance() :
                    loginWindow.atmManager.savingsAccounts.FirstOrDefault(
                        x => x.GetPin() == pin).GetBalance()));

            loginWindow.atmManager.WriteAccounts();
        }

        private void btnKeyPadsClicked(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            InputNumber(btn.Content.ToString());
        }
    }
}
