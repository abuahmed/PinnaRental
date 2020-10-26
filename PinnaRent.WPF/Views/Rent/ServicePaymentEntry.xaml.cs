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
    /// Interaction logic for ServicePaymentEntry.xaml
    /// </summary>
    public partial class ServicePaymentEntry : Window
    {
        public ServicePaymentEntry()
        {
            ServicePaymentEntryViewModel.Errors = 0;
            InitializeComponent();
        }

        public ServicePaymentEntry(RentalPaymentDTO rentalPaymentDTO)
        {
            ServicePaymentEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RentalPaymentDTO>(rentalPaymentDTO);
            Messenger.Reset();
        }
        public ServicePaymentEntry(PaymentUCTemp rentalPaymentDTO)
        {
            ServicePaymentEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<PaymentUCTemp>(rentalPaymentDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ServicePaymentEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ServicePaymentEntryViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtPaymentPeriod.Focus();
        }

        private void ServicePaymentEntry_OnClosing(object sender, CancelEventArgs e)
        {
            //ServicePaymentEntryViewModel.CleanUp();
        }
    }
}
