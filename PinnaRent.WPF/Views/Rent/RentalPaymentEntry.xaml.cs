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
    /// Interaction logic for RentalPaymentEntry.xaml
    /// </summary>
    public partial class RentalPaymentEntry : Window
    {
        public RentalPaymentEntry()
        {
            RentalPaymentEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        public RentalPaymentEntry(RentalContratDTO rentalContrat)
        {
            RentalPaymentEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RentalContratDTO>(rentalContrat);
            Messenger.Reset();
        }
        public RentalPaymentEntry(RoomDTO room)
        {
            ServicePaymentUCViewModel.Errors = 0;
            RentalPaymentEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RoomDTO>(room);
            Messenger.Reset();
        }
        public RentalPaymentEntry(RoomDTO room, bool isRenewal)
        {
            //ServicePaymentUCViewModel.Errors = 0;
            RentalPaymentEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<bool>(isRenewal);
            Messenger.Default.Send<RoomDTO>(room);
            Messenger.Reset();
        }
        public RentalPaymentEntry(RentalPaymentDTO rentalPayment)
        {
            RentalPaymentEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RentalPaymentDTO>(rentalPayment);
            Messenger.Reset();
        }
        public RentalPaymentEntry(RentalContratDTO rentalContrat,bool isRenewal)
        {
            RentalPaymentEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<bool>(isRenewal);
            Messenger.Default.Send<RentalContratDTO>(rentalContrat);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) RentalPaymentEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) RentalPaymentEntryViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtPaymentPeriod.Focus();
        }

        private void RentalPaymentEntry_OnClosing(object sender, CancelEventArgs e)
        {
            RentalPaymentEntryViewModel.CleanUp();

        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            //TxtPaymentPeriod.Focus();
        }
    }
}
