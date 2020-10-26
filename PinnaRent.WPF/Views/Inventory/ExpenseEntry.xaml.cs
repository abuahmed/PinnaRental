using System.Windows;
using System.Windows.Controls;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for ExpenseEntry.xaml
    /// </summary>
    public partial class ExpenseEntry : Window
    {
        public ExpenseEntry()
        {
            ExpenseEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        public ExpenseEntry(PaymentTypes paymentType)
        {
            ExpenseEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<PaymentTypes>(paymentType);
        }
        public ExpenseEntry(PaymentDTO paymentDTO)
        {
            ExpenseEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<PaymentDTO>(paymentDTO);
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ExpenseEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ExpenseEntryViewModel.Errors -= 1;
        }

        private void WdwExpenseLoanEntry_Loaded(object sender, RoutedEventArgs e)
        {
            TxtReason.Focus();
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button == null) return;
            if (TxtPaymentToFrom == null) return;

            if (button.Tag.ToString() == "Vendor")
            {
                TxtPaymentToFrom.IsEnabled = false;
                LstItemsAutoCompleteBox.IsEnabled = true;
            }
            else
            {
                TxtPaymentToFrom.IsEnabled = true;
                LstItemsAutoCompleteBox.IsEnabled = false;
            }
                
        }
    }
}
