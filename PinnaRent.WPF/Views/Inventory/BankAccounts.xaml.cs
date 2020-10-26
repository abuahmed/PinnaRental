using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PinnaRent.WPF.ViewModel;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for BankAccounts.xaml
    /// </summary>
    public partial class BankAccounts : Window
    {
        public BankAccounts()
        {
            BankAccountViewModel.Errors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) BankAccountViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) BankAccountViewModel.Errors -= 1;
        }

        private void BankAccounts_OnClosing(object sender, CancelEventArgs e)
        {
            BankAccountViewModel.CleanUp();
        }

        private void BtnAddNewBa_OnClick(object sender, RoutedEventArgs e)
        {
            TxtBankBranch.Focus();
        }
    }
}
