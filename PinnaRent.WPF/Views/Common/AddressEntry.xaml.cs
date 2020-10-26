﻿using System;
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
    /// Interaction logic for AddressEntry.xaml
    /// </summary>
    public partial class AddressEntry : Window
    {
        public AddressEntry()
        {
            AddressViewModel.Errors = 0;
            InitializeComponent();
        }

        public AddressEntry(AddressDTO addressDTO)
        {
            AddressViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<AddressDTO>(addressDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) AddressViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) AddressViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtMobile.Focus();
        }

        private void AddressEntry_OnClosing(object sender, CancelEventArgs e)
        {
            //AddressViewModel.CleanUp();
        }
    }
}
