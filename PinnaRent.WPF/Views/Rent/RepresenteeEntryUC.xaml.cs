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
    /// Interaction logic for RepresenteeEntryUC.xaml
    /// </summary>
    public partial class RepresenteeEntryUC : UserControl
    {
        public RepresenteeEntryUC()
        {
            RenteeEntryViewModel.Errors = 0;
            CompanyViewModel.Errors = 0;
            InitializeComponent();
        }

        public RepresenteeEntryUC(RepresenteeDTO addressDTO)
        {
            RenteeEntryViewModel.Errors = 0;
            CompanyViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<RepresenteeDTO>(addressDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                RenteeEntryViewModel.Errors += 1;
                CompanyViewModel.Errors += 1;
            }
            if (e.Action == ValidationErrorEventAction.Removed)
            {
                RenteeEntryViewModel.Errors -= 1;
                CompanyViewModel.Errors -= 1;
            }
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtMobile.Focus();
        }

        private void RepresenteeEntryUC_OnClosing(object sender, CancelEventArgs e)
        {
            //RepresenteeViewModel.CleanUp();
        }
    }
}
