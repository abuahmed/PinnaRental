using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.Service.Interfaces;
using PinnaRent.WPF.Views;

namespace PinnaRent.WPF.ViewModel
{
    public class RemarkEntryViewModel : ViewModelBase
    {
        #region Fields
        private static IRentalPaymentRemarkService _rentalPaymentRemarkService;
        private RentalPaymentRemarkDTO _selectedRentalPaymentRemark, _selectedRentalPaymentRemarkParam;
        private ICommand _addNewRentalPaymentRemarkViewCommand,
                        _saveRentalPaymentRemarkViewCommand,
                        _closeRentalPaymentRemarkViewCommand,
                        _paymentDateViewCommand;
        private RentalContratDTO _selectedRentalContrat;
        private DateTime _selectedRemarkTime;
      
        #endregion

        #region Constructor
        public RemarkEntryViewModel()
        {
            CleanUp();
            _rentalPaymentRemarkService = new RentalPaymentRemarkService();

            CheckRoles();
            
            AddNewRentalPaymentRemark();

            Messenger.Default.Register<RentalPaymentRemarkDTO>(this, message => { SelectedRentalPaymentRemarkParam = message; });
        }

        public static void CleanUp()
        {
            if (_rentalPaymentRemarkService != null)
                _rentalPaymentRemarkService.Dispose();
        }
        #endregion

        #region Properties

        public DateTime SelectedRemarkTime
        {
            get { return _selectedRemarkTime; }
            set
            {
                _selectedRemarkTime = value;
                RaisePropertyChanged<DateTime>(() => SelectedRemarkTime);
            }
        }

        private int _rentalPaymentId;
        public RentalPaymentRemarkDTO SelectedRentalPaymentRemarkParam
        {
            get { return _selectedRentalPaymentRemarkParam; }
            set
            {
                _selectedRentalPaymentRemarkParam = value;
                RaisePropertyChanged<RentalPaymentRemarkDTO>(() => SelectedRentalPaymentRemarkParam);
                if (SelectedRentalPaymentRemarkParam != null)
                {
                    var rentalPay =
                        new RentalPaymentService(true).Find(SelectedRentalPaymentRemarkParam.RentalPaymentId.ToString());
                    _rentalPaymentId = rentalPay.Id;
                    SelectedRentalContrat =
                        new RentalContratService(true).GetAll().FirstOrDefault(c => c.Id == rentalPay.ContratId);

                    SelectedRentalPaymentRemark = _rentalPaymentRemarkService.Find(SelectedRentalPaymentRemarkParam.Id.ToString());
                    if(SelectedRentalPaymentRemark==null)
                        AddNewRentalPaymentRemark();
                }
            }
        }
        public RentalContratDTO SelectedRentalContrat
        {
            get { return _selectedRentalContrat; }
            set
            {
                _selectedRentalContrat = value;
                RaisePropertyChanged<RentalContratDTO>(() => SelectedRentalContrat);
                
            }
        }

        public RentalPaymentRemarkDTO SelectedRentalPaymentRemark
        {
            get { return _selectedRentalPaymentRemark; }
            set
            {
                _selectedRentalPaymentRemark = value;
                RaisePropertyChanged<RentalPaymentRemarkDTO>(() => SelectedRentalPaymentRemark);
                if (SelectedRentalPaymentRemark != null)
                {
                    SelectedRemarkTime = SelectedRentalPaymentRemark.RemarkDate;
                }
            }
        }

        
        #endregion

        #region Commands
        public ICommand AddNewRentalPaymentRemarkViewCommand
        {
            get
            {
                return _addNewRentalPaymentRemarkViewCommand ?? (_addNewRentalPaymentRemarkViewCommand = new RelayCommand(AddNewRentalPaymentRemark));
            }
        }
        public void AddNewRentalPaymentRemark()
        {
            SelectedRentalPaymentRemark = new RentalPaymentRemarkDTO
            {
                Type = RemarkTypes.WIllGiveQuickResponse,
                RemarkDate = DateTime.Now,
                RentalPaymentId = _rentalPaymentId
            };
        }

        public ICommand SaveCloseRentalPaymentRemarkViewCommand
        {
            get { return _saveRentalPaymentRemarkViewCommand ?? (_saveRentalPaymentRemarkViewCommand = new RelayCommand<object>(ExecuteSaveRentalPaymentRemarkViewCommand, CanSave)); }
        }
        private void ExecuteSaveRentalPaymentRemarkViewCommand(object obj)
        {
            try
            {
                var subcDate = SelectedRentalPaymentRemark.RemarkDate;
                SelectedRentalPaymentRemark.RemarkDate = new DateTime(subcDate.Year, subcDate.Month, subcDate.Day,
                   SelectedRemarkTime.Hour, SelectedRemarkTime.Minute, SelectedRemarkTime.Second);

                var stat = _rentalPaymentRemarkService.InsertOrUpdate(SelectedRentalPaymentRemark);
                if (stat != string.Empty)
                    MessageBox.Show("Got Problem while saving item, try again..." + Environment.NewLine + stat, "save error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem saving RentalPaymentRemark..." +
                                Environment.NewLine + exception.Message +
                                Environment.NewLine + exception.InnerException);
            }
        }

        public ICommand CloseRentalPaymentRemarkViewCommand
        {
            get
            {
                return _closeRentalPaymentRemarkViewCommand ?? (_closeRentalPaymentRemarkViewCommand = new RelayCommand<Object>(CloseWindow));
            }
        }
        private void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }

        public ICommand PaymentDateViewCommand
        {
            get
            {
                return _paymentDateViewCommand ??
                       (_paymentDateViewCommand = new RelayCommand(ExcutePaymentDate));
            }
        }

        public void ExcutePaymentDate()
        {
            var calConv = new CalendarConvertor(SelectedRentalPaymentRemark.RemarkDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedRentalPaymentRemark.RemarkDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }
        #endregion
        
        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }
        #endregion

        #region Previlege Visibility
        private UserRolesModel _userRoles;

        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;
        }

        #endregion
    }
}