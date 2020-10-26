using System.ComponentModel;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for RentalContratEntry.xaml
    /// </summary>
    public partial class RentalContratEntry : Window
    {
        public RentalContratEntry()
        {
            RentalContratEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        public RentalContratEntry(RentalContratDTO contrat)
        {
            RentalContratEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RentalContratDTO>(contrat);
            Messenger.Reset();
        }
        
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) RentalContratEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) RentalContratEntryViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }

        private void RentalContratEntry_OnClosing(object sender, CancelEventArgs e)
        {
            RentalContratEntryViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }

        private void TxtAmountPaid_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
