#region
using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaRent.Core.Models;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class ServicePaymentEntryViewModel : ViewModelBase
    {
        #region Fields
        readonly static ServicePaymentUCViewModel ServicePaymentUC = new ViewModelLocator().ServicePaymentUC;
        private ViewModelBase _currentViewModel;
        private PaymentUCTemp _selectedPaymentUCTemp;
        private ICommand _saveRentalPaymentViewCommand;
        #endregion

        #region Constructor

        public ServicePaymentEntryViewModel()
        {
            CurrentViewModel = ServicePaymentUC;
            
            Messenger.Default.Register<PaymentUCTemp>(this, message => { SelectedPaymentUCTemp = message; });
        }

        #endregion

        #region Public Properties

        public PaymentUCTemp SelectedPaymentUCTemp
        {
            get { return _selectedPaymentUCTemp; }
            set
            {
                _selectedPaymentUCTemp = value;
                RaisePropertyChanged<PaymentUCTemp>(() => SelectedPaymentUCTemp);

                if (SelectedPaymentUCTemp != null)
                {
                    ServicePaymentUC.SelectedRoom = SelectedPaymentUCTemp.SelectedRoom;
                    ServicePaymentUC.SelectedRentalContrat = SelectedPaymentUCTemp.SelectedRentalContrat;
                    ServicePaymentUC.LastRentalPayment = SelectedPaymentUCTemp.LastRentalPayment;
                    ServicePaymentUC.SelectedRentalPayment = SelectedPaymentUCTemp.SelectedRentalPayment;
                }
            }
        }

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                RaisePropertyChanged("RenteeAddressViewModel");
            }
        }
        
        #endregion

        #region Commands

        public ICommand SaveRentalPaymentViewCommand
        {
            get
            {
                return _saveRentalPaymentViewCommand ??
                       (_saveRentalPaymentViewCommand = new RelayCommand<Object>(SaveRentalPayment, CanSave));
            }
        }

        private void SaveRentalPayment(object obj)
        {
            try
            {
                SelectedPaymentUCTemp.SelectedRentalPayment = ServicePaymentUC.SelectedRentalPayment;
                SelectedPaymentUCTemp.SelectedRentalPayment.StartDate = ServicePaymentUC.StartDate;
                SelectedPaymentUCTemp.SelectedRentalPayment.PaymentPeriod = ServicePaymentUC.PaymentPeriod;
                SelectedPaymentUCTemp.SelectedRentalPayment.AdditionalDays = ServicePaymentUC.AdditionalDays;
                SelectedPaymentUCTemp.SelectedRentalPayment.ExtraPaymentAmount = ServicePaymentUC.ExtraPenality;
                
                CloseWindow(obj);

            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }

        #endregion
        
        #endregion
    }
}