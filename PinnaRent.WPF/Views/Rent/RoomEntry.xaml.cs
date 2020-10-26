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
    public partial class RoomEntry : Window
    {
        public RoomEntry()
        {
            RoomEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        public RoomEntry(RoomDTO room)
        {
            RoomEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RoomDTO>(room);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) RoomEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) RoomEntryViewModel.Errors -= 1;
        }

        private void WdwItemDetail_Loaded(object sender, RoutedEventArgs e)
        {
            TxtItemName.Focus();
        }

        private void ItemDetail_OnClosing(object sender, CancelEventArgs e)
        {
            RoomEntryViewModel.CleanUp();
        }

    }
}
