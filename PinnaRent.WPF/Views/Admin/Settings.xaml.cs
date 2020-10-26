using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaRent.WPF.ViewModel;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            SettingViewModel.Errors = 0;
            InitializeComponent();
        }
        public Settings(object obj)
        {
            SettingViewModel.Errors = 0;         
            InitializeComponent();
            Messenger.Default.Send<object>(obj);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) SettingViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) SettingViewModel.Errors -= 1;
        }

        private void Settings_OnClosing(object sender, CancelEventArgs e)
        {
            SettingViewModel.CleanUp();
        }

    }
}
