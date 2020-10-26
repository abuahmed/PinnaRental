using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PinnaRent.Core.Models;
using PinnaRent.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class RentalPaymentRemarkEntry : Window
    {
        public RentalPaymentRemarkEntry()
        {
            RemarkEntryViewModel.Errors = 0;
            InitializeComponent();
            
        }
        public RentalPaymentRemarkEntry(RentalPaymentRemarkDTO room)
        {
            RemarkEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RentalPaymentRemarkDTO>(room);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) RemarkEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) RemarkEntryViewModel.Errors -= 1;
        }

        private void WdwItemDetail_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ItemDetail_OnClosing(object sender, CancelEventArgs e)
        {
            RemarkEntryViewModel.CleanUp();
        }

    }
}
