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
    public partial class RoomResourceEntry : Window
    {
        public RoomResourceEntry()
        {
            RoomResourceEntryViewModel.Errors = 0;
            InitializeComponent();
            TxtItemName.Focus();
        }
        public RoomResourceEntry(RoomDTO room)
        {
            RoomResourceEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RoomDTO>(room);
            Messenger.Reset();
        }
        public RoomResourceEntry(RoomDTO room,RoomResourceDTO roomResource)
        {
            RoomResourceEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RoomDTO>(room);
            Messenger.Default.Send<RoomResourceDTO>(roomResource);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) RoomResourceEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) RoomResourceEntryViewModel.Errors -= 1;
        }

        private void WdwItemDetail_Loaded(object sender, RoutedEventArgs e)
        {
            TxtItemName.Focus();
        }

        private void ItemDetail_OnClosing(object sender, CancelEventArgs e)
        {
            RoomResourceEntryViewModel.CleanUp();
        }

    }
}
