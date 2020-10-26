using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PinnaRent.WPF.ViewModel;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for ChartofAccounts.xaml
    /// </summary>
    public partial class ChartofAccounts : Window
    {
        public ChartofAccounts()
        {
            ChartofAccountViewModel.Errors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ChartofAccountViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ChartofAccountViewModel.Errors -= 1;
        }

        private void ChartofAccounts_OnClosing(object sender, CancelEventArgs e)
        {
            ChartofAccountViewModel.CleanUp();
        }

        private void BtnAddNewBa_OnClick(object sender, RoutedEventArgs e)
        {
            TxtBankBranch.Focus();
        }
    }
}
