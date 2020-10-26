#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Extensions;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.WPF.Views;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class ServicePaymentUCViewModel : ViewModelBase
    {
        #region Fields

        private RentalPaymentDTO _selectedRentalPayment, _lastRentalPayment, _additionalServicePayment;
        private RoomDTO _selectedRoom;
        private RentalContratDTO _selectedRentalContrat;

        private ICommand _servicePaymentEntryViewCommand,_servicePaymentDeleteViewCommand,
            _paymentDateViewCommand,
            _startDateViewCommand,
            _endDateViewCommand;

        private string _subscriptionText, _amountPaidVisibility, _servicePaymentVisibility, _inWords;
        private int? _contratPeriod, _additionalDays;
        private decimal? _extraPenality;
        private decimal _amountToPay, _subTotal, _vat, _grandTotal, _rentServiceFee;
        private bool _isRenewal, _startDateEnability, _endDateEnability;
        private DateTime _startDate;

        #endregion

        #region Constructor

        public ServicePaymentUCViewModel()
        {
            CheckRoles();
        }

        #endregion

        #region Public Properties

        public bool IsRenewal
        {
            get { return _isRenewal; }
            set
            {
                _isRenewal = value;
                RaisePropertyChanged<bool>(() => IsRenewal);
                if (IsRenewal)
                {
                    StartDateEnability = false;
                    EndDateEnability = false;
                }
                else
                {
                    StartDateEnability = SelectedRentalPayment.PreviousPaymentId == null;
                    EndDateEnability = false;
                }
            }
        }
        
        public int? PaymentPeriod
        {
            get { return _contratPeriod; }
            set
            {
                _contratPeriod = value;
                RaisePropertyChanged<int?>(() => PaymentPeriod);

                CalculateAmount();
            }
        }

        public int? AdditionalDays
        {
            get { return _additionalDays; }
            set
            {
                _additionalDays = value;
                RaisePropertyChanged<int?>(() => AdditionalDays);

                CalculateAmount();
            }
        }

        public decimal? ExtraPenality
        {
            get { return _extraPenality; }
            set
            {
                _extraPenality = value;
                RaisePropertyChanged<decimal?>(() => ExtraPenality);

                CalculateAmount();
            }
        }

        public decimal AmountToPay
        {
            get { return _amountToPay; }
            set
            {
                _amountToPay = value;
                RaisePropertyChanged<decimal>(() => AmountToPay);
            }
        }

        public decimal RentServiceFee
        {
            get { return _rentServiceFee; }
            set
            {
                _rentServiceFee = value;
                RaisePropertyChanged<decimal>(() => RentServiceFee);
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged<DateTime>(() => StartDate);

                SelectedRentalPayment.StartDate = StartDate;

                if (PaymentPeriod != null)
                {
                    int days = (int) PaymentPeriod*30;
                    if (AdditionalDays != null && AdditionalDays > 0)
                    {
                        days += (int) AdditionalDays;
                    }
                    SelectedRentalPayment.EndDate = StartDate.AddDays(days).AddDays(-1);
                }
            }
        }

        public string RentalPaymentText
        {
            get { return _subscriptionText; }
            set
            {
                _subscriptionText = value;
                RaisePropertyChanged<string>(() => RentalPaymentText);
            }
        }

        public string AmountPaidVisibility
        {
            get { return _amountPaidVisibility; }
            set
            {
                _amountPaidVisibility = value;
                RaisePropertyChanged<string>(() => AmountPaidVisibility);
            }
        }

        public string ServicePaymentVisibility
        {
            get { return _servicePaymentVisibility; }
            set
            {
                _servicePaymentVisibility = value;
                RaisePropertyChanged<string>(() => ServicePaymentVisibility);
            }
        }

        public bool StartDateEnability
        {
            get { return _startDateEnability; }
            set
            {
                _startDateEnability = value;
                RaisePropertyChanged<bool>(() => StartDateEnability);
            }
        }

        public bool EndDateEnability
        {
            get { return _endDateEnability; }
            set
            {
                _endDateEnability = value;
                RaisePropertyChanged<bool>(() => EndDateEnability);
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

        public RoomDTO SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                RaisePropertyChanged<RoomDTO>(() => SelectedRoom);
                if (SelectedRoom != null)
                    CheckServicePaymentVisibility();
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
                    RentalPaymentText = EnumUtil.GetEnumDesc(SelectedRentalPayment.Type);

                    if (SelectedRentalPayment.Type == PaymentTypes.Rent)
                    {
                        RentalPaymentText = "ኪራይ";
                        if (SelectedRentalPayment.RentAmount != null)
                            RentServiceFee = (decimal) SelectedRentalPayment.RentAmount;
                        else RentServiceFee = 0;
                    }
                    else if (SelectedRentalPayment.Type == PaymentTypes.Service || SelectedRentalPayment.Type == PaymentTypes.ServiceWithRent)
                    {
                        if (SelectedRentalPayment.ServiceAmount != null)
                            RentServiceFee = (decimal) SelectedRentalPayment.ServiceAmount;
                        else RentServiceFee = 0;
                        RentalPaymentText = "አገልግሎት";
                    }

                    PaymentPeriod = SelectedRentalPayment.PaymentPeriod;
                    AdditionalDays = SelectedRentalPayment.AdditionalDays;
                    ExtraPenality = SelectedRentalPayment.ExtraPaymentAmount;
                    StartDate = SelectedRentalPayment.StartDate;

                    CheckServicePaymentVisibility();
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

        public RentalPaymentDTO AdditionalServicePayment
        {
            get { return _additionalServicePayment; }
            set
            {
                _additionalServicePayment = value;
                RaisePropertyChanged<RentalPaymentDTO>(() => AdditionalServicePayment);
            }
        }

        public decimal CalculatePenality()
        {
            try
            {
                if (LastRentalPayment != null && SelectedRentalPayment != null)
                {
                    var overdueMonths = CommonUtility.GetMonthsFromDays(LastRentalPayment.EndDate,
                        SelectedRentalPayment.PaymentDate);
                    if (PaymentPeriod < overdueMonths)
                        if (PaymentPeriod != null) overdueMonths = (int) PaymentPeriod;
                    SelectedRentalPayment.OverDueDays = overdueMonths;
                    var roomAmount = RentServiceFee;
                    return (decimal) 0.1*roomAmount*overdueMonths;
                   
                }
            }
            catch
            {
            }

            return 0;
        }

        public void CalculateAmount()
        {
            try
            {
                if (PaymentPeriod != null && PaymentPeriod != 0 && SelectedRentalPayment != null)
                {
                    AmountToPay = (decimal) (PaymentPeriod*RentServiceFee);

                    if (AdditionalDays != null && AdditionalDays > 0)
                    {
                        var extraAmt = (decimal) (AdditionalDays);
                        extraAmt = extraAmt/30;
                        AmountToPay += extraAmt*RentServiceFee;
                    }

                    SelectedRentalPayment.AmountRequired = AmountToPay;

                    SelectedRentalPayment.Penality = CalculatePenality();
                    SelectedRentalPayment.TotalPenalityAmount = SelectedRentalPayment.Penality;

                    if (SelectedRentalPayment.Penality != null)
                        AmountToPay = (decimal) (AmountToPay + SelectedRentalPayment.Penality);

                    if (ExtraPenality != null)
                    {
                        if (SelectedRentalPayment.TotalPenalityAmount == null)
                            SelectedRentalPayment.TotalPenalityAmount = 0;
                        SelectedRentalPayment.ExtraPaymentAmount = ExtraPenality;
                        SelectedRentalPayment.TotalPenalityAmount += SelectedRentalPayment.ExtraPaymentAmount;
                        AmountToPay = (decimal) (AmountToPay + ExtraPenality);
                    }

                    SelectedRentalPayment.TotalAmountRequired = AmountToPay;
                    SelectedRentalPayment.TotalAmountPaid = AmountToPay;

                    CalculateDurations();
                    GetSummary();
                }
            }
            catch
            {
            }
        }

        public void CalculateDurations()
        {
            try
            {
                if (SelectedRentalContrat != null && SelectedRentalContrat.Room != null && SelectedRentalPayment != null)
                {
                    if (SelectedRentalPayment.Id == 0)
                    {
                        if (IsRenewal) //IS RENEWAL
                        {
                            var lastEndDate = LastRentalPayment.EndDate;
                            lastEndDate = new DateTime(lastEndDate.Year, lastEndDate.Month, lastEndDate.Day, 23, 59, 59);
                            StartDate = lastEndDate.AddDays(1);
                        }
                        else
                            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    }
                    else StartDate = SelectedRentalPayment.StartDate;
                }
            }
            catch
            {
            }
        }

        public void CheckServicePaymentVisibility()
        {
            if (SelectedRentalPayment != null)
            {
                if (SelectedRentalPayment.Type == PaymentTypes.Rent)
                {
                    if (SelectedRoom != null && SelectedRoom.ServiceFee > 0)
                        ServicePaymentVisibility = "Visible";
                    else ServicePaymentVisibility = "Collapsed";
                }
                else if (SelectedRentalPayment.Type == PaymentTypes.Service || SelectedRentalPayment.Type == PaymentTypes.ServiceWithRent)
                    ServicePaymentVisibility = "Collapsed";
            }
        }

        #endregion

        #region Summary

        public decimal SubTotal
        {
            get { return _subTotal; }
            set
            {
                _subTotal = value;
                RaisePropertyChanged<decimal>(() => SubTotal);
            }
        }

        public decimal Vat
        {
            get { return _vat; }
            set
            {
                _vat = value;
                RaisePropertyChanged<decimal>(() => Vat);
            }
        }

        public decimal GrandTotal
        {
            get { return _grandTotal; }
            set
            {
                _grandTotal = value;
                RaisePropertyChanged<decimal>(() => GrandTotal);
            }
        }

        public string InWords
        {
            get { return _inWords; }
            set
            {
                _inWords = value;
                RaisePropertyChanged<string>(() => InWords);
            }
        }

        public void GetSummary()
        {
            try
            {
                GrandTotal = SelectedRentalPayment.TotalAmountRequired;
                if (AdditionalServicePayment != null)
                    GrandTotal += AdditionalServicePayment.TotalAmountRequired;

                SubTotal = GrandTotal/(decimal) 1.15;
                Vat = SubTotal*(decimal) 0.15;
                InWords = CommonUtility.GetNumberInWords(GrandTotal.ToString(CultureInfo.InvariantCulture), false);
            }
            catch
            {
            }
        }

        #endregion

        #region Commands

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
            var calConv = new CalendarConvertor(SelectedRentalPayment.PaymentDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedRentalPayment.PaymentDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand StartDateViewCommand
        {
            get
            {
                return _startDateViewCommand ??
                       (_startDateViewCommand = new RelayCommand(ExcuteStartDate));
            }
        }

        public void ExcuteStartDate()
        {
            var calConv = new CalendarConvertor(SelectedRentalPayment.StartDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    StartDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand EndDateViewCommand
        {
            get
            {
                return _endDateViewCommand ??
                       (_endDateViewCommand = new RelayCommand(ExcuteEndDate));
            }
        }

        public void ExcuteEndDate()
        {
            var calConv = new CalendarConvertor(SelectedRentalPayment.EndDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedRentalPayment.EndDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand AdditionalServicePaymentEntryViewCommand
        {
            get
            {
                return _servicePaymentEntryViewCommand ??
                       (_servicePaymentEntryViewCommand = new RelayCommand<Object>(AddServicePayment, CanSave));
            }
        }

        private void AddServicePayment(object obj)
        {
            if (AdditionalServicePayment == null)
            {
                AdditionalServicePayment = new RentalPaymentDTO
                {
                    PaymentPeriod = 1,
                    Type = PaymentTypes.ServiceWithRent,
                    ContratId = SelectedRentalContrat.Id,
                    EndDate = DateTime.Now.AddMonths(1),
                    PaymentDate = DateTime.Now,
                    ReceiptDate = DateTime.Now,
                    StartDate = DateTime.Now,
                    //ReceiptNumber = new Random().Next(111111111, 999999999).ToString(),
                    ReceiptType = ReceiptTypes.FiscalPrinter,
                    RentAmount = SelectedRentalContrat.Room.RentalFee,
                    ServiceAmount = SelectedRentalContrat.Room.ServiceFee
                };
            }
            

            var paymentUCTemp = new PaymentUCTemp
            {
                SelectedRentalPayment = AdditionalServicePayment,
                LastRentalPayment = SelectedRoom != null ? SelectedRoom.LastServicePayment : null,
                SelectedRoom = SelectedRoom,
                SelectedRentalContrat = SelectedRentalContrat
            };

            var servicePaymentEntry = new ServicePaymentEntry(paymentUCTemp);
            servicePaymentEntry.ShowDialog();
            var dialogueResult = servicePaymentEntry.DialogResult;
            if (dialogueResult != null) // && (bool) dialogueResult)
            {
                AdditionalServicePayment = paymentUCTemp.SelectedRentalPayment;
                GetSummary();
            }
        }

        public ICommand AdditionalServicePaymentDeleteViewCommand
        {
            get
            {
                return _servicePaymentDeleteViewCommand ??
                       (_servicePaymentDeleteViewCommand = new RelayCommand<Object>(DeleteServicePayment, CanSave));
            }
        }

        private void DeleteServicePayment(object obj)
        {
            try
            {
                var rentPayServ = new RentalPaymentService();
                var rentalPymnt = rentPayServ.Find(SelectedRentalPayment.Id.ToString());
                if (rentalPymnt != null)
                {
                    rentalPymnt.ServicePayment = null;
                    rentPayServ.InsertOrUpdate(rentalPymnt);
                    rentPayServ.Delete(AdditionalServicePayment.Id.ToString());
                    AdditionalServicePayment = null;
                    GetSummary();
                }
                rentPayServ.Dispose();
            }
            catch
            {
                MessageBox.Show("Problem Deleting Service, Try again later!");
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
            EndDateEnability = UserRoles.ContratEdit == "Visible";
        }

        #endregion
    }

    public class PaymentUCTemp
    {
        public RentalContratDTO SelectedRentalContrat { get; set; }
        public RoomDTO SelectedRoom { get; set; }
        public RentalPaymentDTO SelectedRentalPayment { get; set; }
        public RentalPaymentDTO LastRentalPayment { get; set; }
    }
}