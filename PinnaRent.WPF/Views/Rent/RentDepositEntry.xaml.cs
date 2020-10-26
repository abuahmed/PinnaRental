using System;
using System.ComponentModel;
using System.Windows.Media;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for RentDepositEntry.xaml
    /// </summary>
    public partial class RentDepositEntry : Window
    {
        public RentDepositEntry()
        {
            RentDepositEntryViewModel.Errors = 0;
            InitializeComponent();
        }

        public RentDepositEntry(RentalContratDTO contractDTO)
        {
            RentDepositEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RentalContratDTO>(contractDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) RentDepositEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) RentDepositEntryViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPaymentPeriod.Focus();
        }

        private void RentDepositEntry_OnClosing(object sender, CancelEventArgs e)
        {
            //AddressViewModel.CleanUp();
        }
    }
}
