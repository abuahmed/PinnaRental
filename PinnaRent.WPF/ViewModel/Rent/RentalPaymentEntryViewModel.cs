#region

using System;
using System.Collections.Generic;
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

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class RentalPaymentEntryViewModel : ViewModelBase
    {
        #region Fields

        private ServicePaymentUCViewModel _servicePaymentUcvm;

        private static IRentalPaymentService _rentalPaymentService;
        private static IRoomService _roomService;

        private RentalPaymentDTO _selectedRentalPayment,
            _selectedRentalPaymentTemp,
            _lastRentalPayment;

        private RoomDTO _selectedRoom, _selectedRoomTemp;

        private ICommand _addNewRentalPaymentViewCommand,
            _saveRentalPaymentViewCommand,
            _receiptDateViewCommand,
            _addNewRentalContractCommand,
            _editRentalContratViewCommand;

        private RentalContratDTO _selectedRentalContrat;
        private bool _isRenewal;

        #endregion

        #region Constructor

        public RentalPaymentEntryViewModel()
        {
            CleanUp();
            _rentalPaymentService = new RentalPaymentService();
            _roomService = new RoomService();

            ServicePaymentUCVM = new ViewModelLocator().ServicePaymentUC;

            CheckRoles();

            Messenger.Default.Register<bool>(this, message => { IsRenewal = message; });

            Messenger.Default.Register<RoomDTO>(this, message => { SelectedRoomTemp = message; });

            Messenger.Default.Register<RentalPaymentDTO>(this, message => { SelectedRentalPaymentTemp = message; });
        }

        public static void CleanUp()
        {
            if (_rentalPaymentService != null)
                _rentalPaymentService.Dispose();
            if (_roomService != null)
                _roomService.Dispose();
        }

        #endregion

        #region Public Properties

        public ServicePaymentUCViewModel ServicePaymentUCVM
        {
            get { return _servicePaymentUcvm; }
            set
            {
                if (_servicePaymentUcvm == value)
                    return;
                _servicePaymentUcvm = value;
                RaisePropertyChanged("ServicePaymentUCVM");
            }
        }

        public bool IsRenewal
        {
            get { return _isRenewal; }
            set
            {
                _isRenewal = value;
                RaisePropertyChanged<bool>(() => IsRenewal);
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

        public RoomDTO SelectedRoomTemp
        {
            get { return _selectedRoomTemp; }
            set
            {
                _selectedRoomTemp = value;
                RaisePropertyChanged<RoomDTO>(() => SelectedRoomTemp);

                if (SelectedRoomTemp != null)
                {
                    SelectedRoom = SelectedRoomTemp;

                    if (SelectedRoom.IsArchived)
                    {
                        SelectedRentalContrat = new RentalContratDTO();
                        if (ExcuteAddNewRentalContractCommand2())
                        {
                            AddNewRentalPayment();
                            SelectedRentalPayment.IsArchived = SelectedRoom.IsArchived;
                            return;
                        }
                    }

                    if ((SelectedRoomTemp.PaymentType == PaymentTypes.Rent && SelectedRoomTemp.LastRentalPayment != null) ||
                        (SelectedRoomTemp.PaymentType == PaymentTypes.Service &&
                         SelectedRoomTemp.LastServicePayment != null))
                    {
                        var criteria = new SearchCriteria<RentalPaymentDTO>();
                        if (SelectedRoomTemp.PaymentType == PaymentTypes.Rent)
                            criteria.FiList.Add(m => m.Id == SelectedRoomTemp.LastRentalPaymentId);
                        if (SelectedRoomTemp.PaymentType == PaymentTypes.Service)
                            criteria.FiList.Add(m => m.Id == SelectedRoomTemp.LastServicePaymentId);

                        LastRentalPayment = _rentalPaymentService.GetAll(criteria).FirstOrDefault();

                        if (LastRentalPayment != null)
                            SelectedRentalContrat = LastRentalPayment.Contrat;
                        
                        if (IsRenewal)
                            AddNewRentalPayment();
                        else
                            SelectedRentalPayment = LastRentalPayment;
                    }
                    else
                    {
                        SelectedRentalContrat = new RentalContratDTO();
                        ExcuteAddNewRentalContractCommand();
                        AddNewRentalPayment();
                    }
                }
            }
        }

        public RoomDTO SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                RaisePropertyChanged<RoomDTO>(() => SelectedRoom);
            }
        }

        public RentalPaymentDTO SelectedRentalPaymentTemp
        {
            get { return _selectedRentalPaymentTemp; }
            set
            {
                _selectedRentalPaymentTemp = value;
                RaisePropertyChanged<RentalPaymentDTO>(() => SelectedRentalPaymentTemp);

                if (SelectedRentalPaymentTemp != null)
                {
                    var criteria = new SearchCriteria<RentalPaymentDTO>();
                    criteria.FiList.Add(m => m.Id == SelectedRentalPaymentTemp.Id);

                    var rentalPayment = _rentalPaymentService.GetAll(criteria).FirstOrDefault();

                    if (rentalPayment != null)
                    {
                        rentalPayment.IsArchived = SelectedRentalPaymentTemp.IsArchived;
                        SelectedRentalContrat = rentalPayment.Contrat;
                        SelectedRoom = rentalPayment.Contrat.Room;
                        SelectedRentalPayment = rentalPayment;
                    }
                }
            }
        }

        public RentalPaymentDTO SelectedRentalPayment
        {
            get { return _selectedRentalPayment; }
            set
            {
                _selectedRentalPayment = value;
                RaisePropertyChanged<RentalPaymentDTO>(() => SelectedRentalPayment);
                if (SelectedRentalPayment != null)
                {
                    ServicePaymentUCVM.SelectedRentalPayment = SelectedRentalPayment;
                    ServicePaymentUCVM.SelectedRentalContrat = SelectedRentalContrat;
                    ServicePaymentUCVM.SelectedRoom = SelectedRoom;
                    ServicePaymentUCVM.LastRentalPayment = LastRentalPayment;
                    ServicePaymentUCVM.IsRenewal = IsRenewal;

                    if (SelectedRentalPayment.ServicePayment != null)
                    {
                        ServicePaymentUCVM.AdditionalServicePayment = SelectedRentalPayment.ServicePayment;
                        ServicePaymentUCVM.GetSummary();
                    }
                }
            }
        }

        public RentalPaymentDTO LastRentalPayment
        {
            get { return _lastRentalPayment; }
            set
            {
                _lastRentalPayment = value;
                RaisePropertyChanged<RentalPaymentDTO>(() => LastRentalPayment);
            }
        }
        
        #endregion

        #region Commands

        public ICommand AddNewRentalPaymentViewCommand
        {
            get
            {
                return _addNewRentalPaymentViewCommand ??
                       (_addNewRentalPaymentViewCommand = new RelayCommand(AddNewRentalPayment));
            }
        }

        private void AddNewRentalPayment()
        {
            if (SelectedRentalContrat == null)
                return;
            if (SelectedRentalContrat != null &&
                (SelectedRentalContrat.Room == null || SelectedRentalContrat.Rentee == null))
                return;

            SelectedRentalPayment = new RentalPaymentDTO
            {
                PaymentPeriod = 1,
                Type = SelectedRoomTemp.PaymentType,
                ContratId = SelectedRentalContrat.Id,
                EndDate = DateTime.Now.AddMonths(1),
                PaymentDate = DateTime.Now,
                ReceiptDate = DateTime.Now,
                StartDate = DateTime.Now,
                RentAmount = SelectedRentalContrat.Room.RentalFee,
                ServiceAmount = SelectedRentalContrat.Room.ServiceFee
            };

            if (LastRentalPayment != null)
            {
                SelectedRentalPayment.PreviousPaymentId = LastRentalPayment.Id;
                ServicePaymentUCVM.PaymentPeriod = CommonUtility.GetMonthsFromDays(LastRentalPayment.EndDate,
                    DateTime.Now);
            }
        }

        private void AddPayments()
        {
            if (SelectedRentalPayment.Payments.Count > 0)
            {
                SelectedRentalPayment.Payments = new List<PaymentDTO>();
            }

            var rentPayment = new PaymentDTO
            {
                Type = PaymentTypes.Rent,
                RentalPayment = SelectedRentalPayment,
                PaymentDate = SelectedRentalPayment.PaymentDate,
                ReceiptDate = SelectedRentalPayment.ReceiptDate,
                Amount = (decimal) (SelectedRentalPayment.PaymentPeriod*SelectedRentalPayment.RentAmount)/(decimal) 1.15,
                ReceiptNumber = SelectedRentalPayment.ReceiptNumber,
                ReceiptType = SelectedRentalPayment.ReceiptType,
                Reason = SelectedRentalContrat.Room.Number +
                         " Rent From " + SelectedRentalPayment.StartDateString +
                         " To " + SelectedRentalPayment.EndDateString + " (" +
                         "ከ" + SelectedRentalPayment.StartDateStringAmharic + "-" +
                         "" + SelectedRentalPayment.EndDateStringAmharic + ")",
                UnitOfMeasure = "Month",
                UnitQty = SelectedRentalPayment.PaymentPeriod,
                EachPrice = SelectedRentalPayment.RentAmount/(decimal) 1.15,
                PaymentMethod = PaymentMethods.Cash
            };
            SelectedRentalPayment.Payments.Add(rentPayment);

            if (SelectedRentalPayment.Penality != null && SelectedRentalPayment.Penality > 0)
            {
                var overdueMonths = CommonUtility.GetMonthsFromDays(LastRentalPayment.EndDate,
                    SelectedRentalPayment.PaymentDate);
                var penalityPayment = new PaymentDTO
                {
                    Type = PaymentTypes.Penality,
                    RentalPayment = SelectedRentalPayment,
                    PaymentDate = SelectedRentalPayment.PaymentDate,
                    ReceiptDate = SelectedRentalPayment.ReceiptDate,
                    Amount = (decimal) SelectedRentalPayment.Penality/(decimal) 1.15,
                    ReceiptNumber = SelectedRentalPayment.ReceiptNumber,
                    ReceiptType = SelectedRentalPayment.ReceiptType,
                    Reason = "Penality",
                    UnitOfMeasure = "",
                    UnitQty = overdueMonths,
                    EachPrice = SelectedRentalPayment.RentAmount*(decimal) 0.1/(decimal) 1.15,
                    PaymentMethod = PaymentMethods.Cash
                };

                SelectedRentalPayment.Payments.Add(penalityPayment);
            }
            if (SelectedRentalPayment.ExtraPaymentAmount != null && SelectedRentalPayment.ExtraPaymentAmount > 0)
            {
                var extraPenalityPayment = new PaymentDTO
                {
                    Type = PaymentTypes.Penality,
                    RentalPayment = SelectedRentalPayment,
                    PaymentDate = SelectedRentalPayment.PaymentDate,
                    ReceiptDate = SelectedRentalPayment.ReceiptDate,
                    Amount = (decimal) SelectedRentalPayment.ExtraPaymentAmount/(decimal) 1.15,
                    ReceiptNumber = SelectedRentalPayment.ReceiptNumber,
                    ReceiptType = SelectedRentalPayment.ReceiptType,
                    Reason = "Extra Penality",
                    UnitOfMeasure = "",
                    UnitQty = 1,
                    EachPrice = SelectedRentalPayment.ExtraPaymentAmount/(decimal) 1.15,
                    PaymentMethod = PaymentMethods.Cash
                };

                SelectedRentalPayment.Payments.Add(extraPenalityPayment);
            }
            if (SelectedRentalPayment.AdditionalDays != null && SelectedRentalPayment.AdditionalDays > 0)
            {
                var additionalDaysPenality = new PaymentDTO
                {
                    Type = PaymentTypes.Rent,
                    RentalPayment = SelectedRentalPayment,
                    PaymentDate = SelectedRentalPayment.PaymentDate,
                    ReceiptDate = SelectedRentalPayment.ReceiptDate,
                    Amount =
                        (decimal) (((decimal) SelectedRentalPayment.AdditionalDays/30)*SelectedRentalPayment.RentAmount)/
                        (decimal) 1.15,
                    ReceiptNumber = SelectedRentalPayment.ReceiptNumber,
                    ReceiptType = SelectedRentalPayment.ReceiptType,
                    Reason = "Extra Days Rent",
                    UnitOfMeasure = "Day",
                    UnitQty = SelectedRentalPayment.AdditionalDays,
                    EachPrice = (SelectedRentalPayment.RentAmount/30)/(decimal) 1.15,
                    PaymentMethod = PaymentMethods.Cash
                };

                SelectedRentalPayment.Payments.Add(additionalDaysPenality);
            }
            if (SelectedRentalPayment.ServicePayment != null)
            {
                var additionalServicePayment = new PaymentDTO
                {
                    Type = PaymentTypes.Service,
                    RentalPayment = SelectedRentalPayment,
                    PaymentDate = SelectedRentalPayment.PaymentDate,
                    ReceiptDate = SelectedRentalPayment.ReceiptDate,
                    Amount =
                        (decimal)
                            (SelectedRentalPayment.ServicePayment.PaymentPeriod*SelectedRentalPayment.ServiceAmount)/
                        (decimal) 1.15,
                    ReceiptNumber = SelectedRentalPayment.ReceiptNumber,
                    ReceiptType = SelectedRentalPayment.ReceiptType,
                    Reason = SelectedRentalContrat.Room.Number +
                             " For Service From " + SelectedRentalPayment.ServicePayment.StartDateString +
                             " To " + SelectedRentalPayment.ServicePayment.EndDateString + " (" +
                             "ከ" + SelectedRentalPayment.ServicePayment.StartDateStringAmharic + "-" +
                             "" + SelectedRentalPayment.ServicePayment.EndDateStringAmharic + ")",
                    UnitOfMeasure = "Month",
                    UnitQty = SelectedRentalPayment.ServicePayment.PaymentPeriod,
                    EachPrice = SelectedRentalPayment.ServiceAmount/(decimal) 1.15,
                    PaymentMethod = PaymentMethods.Cash
                };

                SelectedRentalPayment.Payments.Add(additionalServicePayment);
            }
        }
        
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
                var prevPayments = SelectedRentalPayment.Payments.ToList();

                SelectedRentalPayment.TotalAmountRequired = ServicePaymentUCVM.GrandTotal;

                SelectedRentalPayment.StartDate = ServicePaymentUCVM.StartDate;
                SelectedRentalPayment.PaymentPeriod = ServicePaymentUCVM.PaymentPeriod;
                SelectedRentalPayment.AdditionalDays = ServicePaymentUCVM.AdditionalDays;
                SelectedRentalPayment.ExtraPaymentAmount = ServicePaymentUCVM.ExtraPenality;

                if (ServicePaymentUCVM.AdditionalServicePayment != null &&
                    ServicePaymentUCVM.AdditionalServicePayment.TotalAmountRequired > 0)
                {
                    SelectedRentalPayment.ServicePayment = ServicePaymentUCVM.AdditionalServicePayment;
                    SelectedRentalPayment.ServicePayment.ReceiptNumber = "S"+SelectedRentalPayment.ReceiptNumber;
                }
                else SelectedRentalPayment.ServicePayment = null;

                AddPayments();

                var stat = _rentalPaymentService.InsertOrUpdateWithPayment(SelectedRentalPayment, prevPayments);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                {
                    CloseWindow(obj);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        public ICommand ReceiptDateViewCommand
        {
            get
            {
                return _receiptDateViewCommand ??
                       (_receiptDateViewCommand = new RelayCommand(ReceiptDate));
            }
        }

        private void ReceiptDate()
        {
            var calConv = new CalendarConvertor(SelectedRentalPayment.ReceiptDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedRentalPayment.ReceiptDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand EditRentalContratViewCommand
        {
            get
            {
                return _editRentalContratViewCommand ??
                       (_editRentalContratViewCommand = new RelayCommand(EditRentalContrat));
            }
        }

        private void EditRentalContrat()
        {
            ShowDiag(SelectedRentalContrat);
        }

        public ICommand AddNewRentalContractCommand
        {
            get
            {
                return _addNewRentalContractCommand ??
                       (_addNewRentalContractCommand = new RelayCommand(ExcuteAddNewRentalContractCommand));
            }
        }

        private void ExcuteAddNewRentalContractCommand()
        {
            var selectedRentalContrat2 = new RentalContratDTO
            {
                RoomId = SelectedRoom.Id
            };
            ShowDiag(selectedRentalContrat2);
        }

        private bool ExcuteAddNewRentalContractCommand2()
        {
            var selectedRentalContrat2 = new RentalContratDTO
            {
                RoomId = SelectedRoom.Id
            };
            return ShowDiag(selectedRentalContrat2);
        }

        private bool ShowDiag(RentalContratDTO selectedRentalContrat)
        {
            var contratEntry = new RentalContratEntry(selectedRentalContrat);
            contratEntry.ShowDialog();
            var dialogueResult = contratEntry.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                var conId = Convert.ToInt32(contratEntry.TxtId.Text);
                SelectedRentalContrat = new RentalContratService(true).GetAll().FirstOrDefault(c => c.Id == conId);
                return true;
            }
            return false;
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
            //ServicePaymentUCVM.EndDateEnability = UserRoles.ContratEdit == "Visible";
        }

        #endregion
    }
}