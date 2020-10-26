﻿using System.ComponentModel;
using System.Windows;
using PinnaRent.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for BackupRestore.xaml
    /// </summary>
    public partial class BackupRestore : Window
    {
        public BackupRestore()
        {
            InitializeComponent();
        }

        public BackupRestore(object obj)
        {            
            InitializeComponent();
            Messenger.Default.Send<object>(obj);
            Messenger.Reset();
        }

        private void BackupRestore_OnClosing(object sender, CancelEventArgs e)
        {
            BackupRestoreViewModel.CleanUp();
        }
    }
}
