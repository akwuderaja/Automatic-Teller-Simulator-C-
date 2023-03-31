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
    /// Interaction logic for frmSupervisor.xaml
    /// </summary>
    public partial class frmSupervisor : Window
    {
        private frmLogin loginWindow;
        internal bool isClosed;

        public frmSupervisor(frmLogin loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
            Loaded += FrmMain_Loaded;
            Closing += FrmMain_Closing; ;
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            loginWindow.Visibility = Visibility.Visible;
            loginWindow.Activate();
        }

        void ListAccounts()
        {
            string fileLines = "";
            foreach (var item in loginWindow.atmManager.chequingAccounts)
                fileLines +=($"C,{item.GetPin()},{item.GetAccountNumber()},{item.GetBalance()}\n");

            foreach (var item in loginWindow.atmManager.savingsAccounts)
                fileLines += ($"S,{item.GetPin()},{item.GetAccountNumber()},{item.GetBalance()}\n");

            txtReport.Text = fileLines;
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument flowDocument = new FlowDocument();
                flowDocument.PagePadding = new Thickness(50);
                flowDocument.Blocks.Add(new Paragraph(new Run(txtReport.Text)));

                printDialog.PrintDocument((((
                    IDocumentPaginatorSource)flowDocument).DocumentPaginator), 
                    "Using Paginator");
            }
        }

        private void BtnRefill_Click(object sender, RoutedEventArgs e)
        {
            if (DailyBalances.Add(loginWindow.atmManager,
                loginWindow.atmManager.bank.refillATM()))
                MessageBox.Show(
                    $"Atm refilled with {loginWindow.atmManager.bank.refillATM()}");
        }

        private void BtnAtmState_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.atmManager.isInService =
                !loginWindow.atmManager.isInService;

            btnAtmState.Content = loginWindow.atmManager.isInService ?
                "Take Offline" : "Bring Online";
        }

        private void BtnPayInterest_Click(object sender, RoutedEventArgs e)
        {
            float amount = 0;
            if (!float.TryParse(txtInterest.Text, out amount))
            {
                MessageBox.Show("Invalid amount"); return;
            }

            foreach (var acct in loginWindow.atmManager.savingsAccounts)
            {
                acct.PayInterest(amount);
            }
            MessageBox.Show("Operation Completed!");
        }
    }
}
