using System;
using System.Windows;
using PinnaRent.Core.Enumerations;
using Telerik.Windows;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            new ChangePassword().ShowDialog();
        }

        private void UsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Users().Show();
        }

        private void BackupRestoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new BackupRestore(this).ShowDialog();
        }
        
        private void CompanyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Company().Show();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Settings(this).Show();
        }
        
        private void Rentees_Click(object sender, RoutedEventArgs e)
        {
            new RenteeEntry().ShowDialog();    
        }
        
        private void CalendarConvertor_Click(object sender, RoutedEventArgs e)
        {
            new CalendarConvertor(DateTime.Now).Show();
        }

        private void PaymentList_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.PaymentList).ShowDialog();
        }

        private void NotPaidPaymentList_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.NotPaid).ShowDialog();
        }

        private void RentDeposits_Click(object sender, RoutedEventArgs e)
        {
            new RentDepositEntry().Show();
        }

        private void DiscontinuedContratsList_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.Discontinued).ShowDialog();
        }

        private void RentOnlyList_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.RentOnly).ShowDialog();
        }

        private void ServiceOnlyList_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.ServiceOnly).ShowDialog();
        }

        private void PenalityOnlyList_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.PenalityOnly).ShowDialog();
        }

        private void StoresMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Warehouses().Show();
        }
        
        private void IohMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new OnHandInventories().Show();
        }

        private void StockReceiveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ReceiveStock(TransactionTypes.RecieveStock).Show();
        }

        private void TransferStockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ReceiveStock(TransactionTypes.TransferStock).Show();
        }

        private void SuppliersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new BusinessPartnerEntry().Show();
        }

        private void ExpensesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Expenses().Show();
        }

        private void ItemsMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            new Items().Show();
        }

        private void BankAccountsMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            new BankAccounts().Show();
        }

        private void ChartOfAccountsMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            new ChartofAccounts().Show();
        }

        private void ExpenseEntryMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            new ExpenseEntry(PaymentTypes.CashOut).Show();
        }
    }
}
