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
    /// Interaction logic for RentServicePaymentUC.xaml
    /// </summary>
    public partial class RentServicePaymentUC : UserControl
    {
        public RentServicePaymentUC()
        {
            ServicePaymentEntryViewModel.Errors = 0;
            //RentalPaymentEntryViewModel.Errors = 0;
            ServicePaymentUCViewModel.Errors = 0;
            InitializeComponent();
        }
        
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                ServicePaymentEntryViewModel.Errors += 1;
                //RentalPaymentEntryViewModel.Errors += 1;
                ServicePaymentUCViewModel.Errors += 1;
            }
            if (e.Action == ValidationErrorEventAction.Removed)
            {
                ServicePaymentEntryViewModel.Errors -= 1;
                //RentalPaymentEntryViewModel.Errors -= 1;
                ServicePaymentUCViewModel.Errors -= 1;
            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPaymentPeriod.Focus();
        }

        private void TxtPaymentPeriod_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
