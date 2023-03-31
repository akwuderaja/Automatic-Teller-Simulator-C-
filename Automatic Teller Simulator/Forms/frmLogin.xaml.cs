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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class frmLogin : Window
    {
        internal ATMManager atmManager { get; set; }
        internal string pin { get; set; }
        internal float amount { get; set; }
        internal bool accountsRead { get; set; }
        private List<failedAttempt> failedAttempts { get; set; }
        frmMain _frmMain;
        frmSupervisor _frmSupervisor;
        MainWindow mainWindow;
        internal bool isClosed;

        class failedAttempt
        {
            public int counter { get; set; }
            public DateTime lastFailTime{ get; set; }
            public string user{ get; set; }

            public failedAttempt(string user, DateTime lastFailTime, int counter)
            {
                this.user = user;
                this.lastFailTime = lastFailTime;
                this.counter = counter;
            }
        }

        public frmLogin(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            atmManager = new ATMManager();
            atmManager.ReadDailyBalances();
            ReadCustomers();
            _frmMain = new frmMain(this);
            _frmSupervisor = new frmSupervisor(this);
            Closing += FrmLogin_Closing;
            failedAttempts = new List<failedAttempt>();
        }

        private void FrmLogin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isClosed)
            {
                this.Visibility = Visibility.Hidden;
                mainWindow.Visibility = Visibility.Visible;
                mainWindow.Activate();
                e.Cancel = true;
            }
            else
            {
                _frmMain.isClosed = true;
                _frmSupervisor.isClosed = true;
                _frmMain.Close();
                _frmSupervisor.Close();
            }
        }
        void ReadCustomers()
        {
            if (!atmManager.ReadCustomers())
            {
                MessageBox.Show("Customer Database was not found!");
                accountsRead = false;
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtPin.Focus(); MessageBox.Show("Username is required!"); return;
            }
            if (string.IsNullOrWhiteSpace(txtPin.Password))
            {
                txtPin.Focus(); MessageBox.Show("Pin is required!"); return;
            }
            //Check for 15mins lockout after 3 trials.
            var f = failedAttempts.FirstOrDefault(x => x.user == txtUsername.Text);
            if (f != null) { 
                f.counter += 1;
                if (f.counter > 3)
                {
                    if((DateTime.Now-f.lastFailTime).Minutes < 15)
                    {
                        MessageBox.Show(
                            $"You are locked out for the next {(f.lastFailTime - DateTime.Now.AddMinutes(-15)).Minutes} mins");
                        return;
                    }
                }
            }
            else
            {
                //Save new Attempts
                f = new failedAttempt(txtUsername.Text, DateTime.Now, 1);
                failedAttempts.Add(f);
            }
            //Validate User
            if (atmManager.ValidateUser(txtUsername.Text, txtPin.Password) ||
                "Supervisor" == txtUsername.Text)
            {
                if (txtUsername.Text != "Supervisor")
                {
                    pin = txtPin.Password;
                    //Open Main form 
                    _frmMain.pin = pin;
                    _frmMain.Visibility = Visibility.Visible;
                    _frmMain.Activate();
                    this.Visibility = Visibility.Collapsed;
                    txtPin.Clear();
                    txtUsername.Clear();
                    return;
                }else if(txtPin.Password == "1234") //Login for Supervisor
                {
                    _frmSupervisor.Visibility = Visibility.Visible;
                    _frmSupervisor.Activate();
                    this.Visibility = Visibility.Collapsed;
                    return;
                }
            }

            MessageBox.Show("User not found!");
        }
        
    }
}
