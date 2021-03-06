﻿using System;
using System.ComponentModel;
using System.Windows.Media;
using PinnaRent.Core.Enumerations;
using PinnaRent.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaRent.WPF.Views
{
    /// <summary>
    /// Interaction logic for Trainers.xaml
    /// </summary>
    public partial class BusinessPartnerEntry : Window
    {
        public BusinessPartnerEntry()
        {
            BusinessPartnerEntryViewModel.Errors = 0;
            InitializeComponent();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) BusinessPartnerEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) BusinessPartnerEntryViewModel.Errors -= 1;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TXtCustName.Focus();
        }

        private void BusinessPartners_OnClosing(object sender, CancelEventArgs e)
        {
            BusinessPartnerEntryViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TXtCustName.Focus();
        }
        private void LstItemsAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }
    }
}
