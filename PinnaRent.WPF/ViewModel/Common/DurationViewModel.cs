#region

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
using PinnaRent.Core.Extensions;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.WPF.Reports;
using PinnaRent.WPF.Views;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class DurationViewModel : ViewModelBase
    {
        #region Fields
        private ReportTypes _reportType;

        private ICommand _durationStartDateViewCommand,
            _durationEndDateViewCommand,
            _printSummaryListCommandView,
            _closeItemViewCommand;

        private string _startDateText, _endDateText, _headerText;

        #endregion

        #region Constructor

        public DurationViewModel()
        {
            FilterStartDate = DateTime.Now;
            FilterEndDate = DateTime.Now;

            Messenger.Default.Register<ReportTypes>(this, (message) => { ReportType = message; });
        }

        #endregion

        #region Public Properties

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }

        public ReportTypes ReportType
        {
            get { return _reportType; }
            set
            {
                _reportType = value;
                RaisePropertyChanged<ReportTypes>(() => ReportType);
                HeaderText = EnumUtil.GetEnumDesc(ReportType) + " ሪፖርት ማውጫ";
            }
        }

        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                _filterStartDate = value;
                RaisePropertyChanged<DateTime>(() => FilterStartDate);
                if (FilterStartDate.Year > 2000)
                    StartDateText = ReportUtility.GetEthCalendar(FilterStartDate, true) +
                                    " (" + FilterStartDate.ToString("dd-MM-yyyy") + ")";
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set
            {
                _filterEndDate = value;
                RaisePropertyChanged<DateTime>(() => FilterEndDate);

                if (FilterEndDate.Year > 2000)
                    EndDateText = ReportUtility.GetEthCalendar(FilterEndDate, true) + " (" +
                                  FilterEndDate.ToString("dd-MM-yyyy") + ")";
            }
        }

        public string StartDateText
        {
            get { return _startDateText; }
            set
            {
                _startDateText = value;
                RaisePropertyChanged<string>(() => StartDateText);
            }
        }

        public string EndDateText
        {
            get { return _endDateText; }
            set
            {
                _endDateText = value;
                RaisePropertyChanged<string>(() => EndDateText);
            }
        }

        #endregion

        #region Commands

        public ICommand DurationStartDateViewCommand
        {
            get
            {
                return _durationStartDateViewCommand ??
                       (_durationStartDateViewCommand = new RelayCommand(DurationStartDate));
            }
        }

        public void DurationStartDate()
        {
            var calConv = new CalendarConvertor(DateTime.Now);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterStartDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand DurationEndDateViewCommand
        {
            get
            {
                return _durationEndDateViewCommand ??
                       (_durationEndDateViewCommand = new RelayCommand(DurationEndDate));
            }
        }

        public void DurationEndDate()
        {
            var calConv = new CalendarConvertor(DateTime.Now);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterEndDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand PrintSummaryListCommandView
        {
            get
            {
                return _printSummaryListCommandView ??
                       (_printSummaryListCommandView = new RelayCommand<Object>(PrintSummaryList));
            }
        }

        public void PrintSummaryList(object obj)
        {
            switch (ReportType)
            {
                case ReportTypes.PenalityOnly:
                case ReportTypes.ServiceOnly:
                case ReportTypes.RentOnly:
                case ReportTypes.PaymentList:
                {
                    #region Payment List

                    var selectedCompany = new CompanyService(true).GetCompany();
                    var datecaption = ReportUtility.GetDateCaption(FilterStartDate, FilterEndDate);
                    var myDataSet = new ReportDataSet();

                    #region Criteria

                    var criteria = new SearchCriteria<RentalPaymentDTO>();
                    if (FilterStartDate.Year > 2000 && FilterEndDate.Year > 2000)
                    {
                        criteria.CurrentUserId = Singleton.User.UserId;
                        criteria.BeginingDate = FilterStartDate;
                        criteria.EndingDate = FilterEndDate;
                    }

                    if (ReportType == ReportTypes.PaymentList)
                        criteria.FiList.Add(p => p.Type != PaymentTypes.ServiceWithRent);

                    if (ReportType == ReportTypes.RentOnly)
                        criteria.FiList.Add(p => p.Type == PaymentTypes.Rent);

                    if (ReportType == ReportTypes.ServiceOnly)
                        criteria.FiList.Add(
                            p => p.Type == PaymentTypes.Service || p.Type == PaymentTypes.ServiceWithRent);

                    if (ReportType == ReportTypes.PenalityOnly)
                        criteria.FiList.Add(p => p.TotalPenalityAmount > 0);

                    #endregion

                    var payments = new RentalPaymentService(true).GetAll(criteria).OrderBy(p => p.ReceiptDate).ToList();
                    var serNo = 1;

                    #region For All Payments

                    if (ReportType == ReportTypes.PaymentList)
                        foreach (var rentalPaymentDTO in payments)
                        {
                            var rentee = rentalPaymentDTO.Contrat.Rentee.DisplayName;
                            if (rentee.Length > 20)
                                rentee = rentee.Substring(0, 18) + "...";
                            myDataSet.RentalPayment.Rows.Add(
                                serNo,
                                "አጠቃላይ ከቤት ኪራይ,ከአገልግሎት,ከቅጣት የተሰበሰበ ገቢ",
                                datecaption,
                                rentalPaymentDTO.ReceiptDateStringAmharicFormatted,
                                rentalPaymentDTO.Contrat.Room.Number,
                                rentee,
                                "",
                                rentalPaymentDTO.TotalAmountRequiredBeforeVat, 0.0, 0.0,
                                rentalPaymentDTO.TotalAmountRequiredVatAmount, 0.0,
                                rentalPaymentDTO.TotalAmountRequired, rentalPaymentDTO.ReceiptNumber, 0
                                , 0.0, "", "", "", "", selectedCompany.Header, null, "");
                            serNo++;
                        }

                    #endregion

                    #region For Rent/Service Only Payments

                    if (ReportType == ReportTypes.RentOnly || ReportType == ReportTypes.ServiceOnly)
                        foreach (var rentalPaymentDTO in payments)
                        {
                            var rentee = rentalPaymentDTO.Contrat.Rentee.DisplayName;
                            if (rentee.Length > 20)
                                rentee = rentee.Substring(0, 18) + "...";
                            myDataSet.RentalPayment.Rows.Add(
                                serNo,
                                "አጠቃላይ ከ" + EnumUtil.GetEnumDesc(ReportType) + " የተሰበሰበ ገቢ",
                                datecaption,
                                rentalPaymentDTO.ReceiptDateStringAmharicFormatted,
                                rentalPaymentDTO.Contrat.Room.Number,
                                rentee,
                                "",
                                rentalPaymentDTO.AmountRequiredBeforeVat, 0.0, 0.0,
                                rentalPaymentDTO.AmountRequiredVatAmount, 0.0,
                                rentalPaymentDTO.AmountRequired, rentalPaymentDTO.ReceiptNumber, 0
                                , 0.0, "", "", "", "", selectedCompany.Header, null, "");
                            serNo++;
                        }

                    #endregion

                    #region Penality Only

                    if (ReportType == ReportTypes.PenalityOnly)
                        foreach (var rentalPaymentDTO in payments)
                        {
                            var rentee = rentalPaymentDTO.Contrat.Rentee.DisplayName;
                            if (rentee.Length > 20)
                                rentee = rentee.Substring(0, 18) + "...";
                            myDataSet.RentalPayment.Rows.Add(
                                serNo,
                                "አጠቃላይ ከ" + EnumUtil.GetEnumDesc(ReportType) + " የተሰበሰበ ገቢ",
                                datecaption,
                                rentalPaymentDTO.ReceiptDateStringAmharicFormatted,
                                rentalPaymentDTO.Contrat.Room.Number,
                                rentee,
                                "",
                                rentalPaymentDTO.TotalPenalityAmountBeforeVat, 0.0, 0.0,
                                rentalPaymentDTO.TotalPenalityAmountVatAmount, 0.0,
                                rentalPaymentDTO.TotalPenalityAmount, rentalPaymentDTO.ReceiptNumber, 0
                                , 0.0, "", "", "", "", selectedCompany.Header, null, "");
                            serNo++;
                        }

                    #endregion

                    var myReport4 = new PaymentList();
                    myReport4.SetDataSource(myDataSet);

                    var report = new ReportViewerCommon(myReport4);
                    report.Show();

                    #endregion
                }
                    break;
                case ReportTypes.Discontinued:
                {
                    #region Discontinued

                    var criteria = new SearchCriteria<RentalContratDTO>();
                    if (FilterStartDate.Year > 2000 && FilterEndDate.Year > 2000)
                    {
                        criteria.BeginingDate = FilterStartDate;
                        criteria.EndingDate = FilterEndDate;
                    }

                    var myDataSet = new ReportDataSet();
                    var serNo = 1;
                    var selectedCompany = new CompanyService(true).GetCompany();
                    var payments = new RentalContratService(true).GetAll(criteria).ToList();
                    if (payments.Count == 0)
                    {
                        MessageBox.Show("No data found on the givenn duration");
                        return;
                    }

                    string datecaption = ReportUtility.GetEthCalendar(FilterStartDate, true) + "(" +
                                         FilterStartDate.ToShortDateString() + ")";

                    if (FilterStartDate.Day != FilterEndDate.Day || FilterStartDate.Month != FilterEndDate.Month ||
                        FilterStartDate.Year != FilterEndDate.Year)
                    {
                        datecaption = "ከ " + datecaption + " እስከ " + ReportUtility.GetEthCalendar(FilterEndDate, true) +
                                      "(" +
                                      FilterEndDate.ToShortDateString() + ")";
                    }


                    foreach (var contratDTO in payments)
                    {
                        var rentee = contratDTO.Rentee.DisplayName;
                        if (rentee.Length > 20)
                            rentee = rentee.Substring(0, 18) + "...";
                        myDataSet.RentalPayment.Rows.Add(
                            serNo,
                            "የተቋረጡ ውሎች ዝርዝር",
                            datecaption,
                            contratDTO.LastContractDiscontinuedDateStringAmharic,
                            contratDTO.Room.Number,
                            rentee,
                            "",
                            0.0, 0.0, 0.0,
                            0.0, 0.0,
                            0.0, "", 0
                            , 0.0, "", "", "", "", selectedCompany.Header, null, "");
                        serNo++;
                    }

                    var myReport4 = new ContractDiscontinued();
                    myReport4.SetDataSource(myDataSet);

                    var report = new ReportViewerCommon(myReport4);
                    report.Show();

                    #endregion
                }
                    break;
                case ReportTypes.NotPaid:
                {
                    #region NotPaid

                    var criteria = new SearchCriteria<RoomDTO>();
                    criteria.FiList.Add(r => r.LastRentalPayment != null);

                    var endDate = FilterEndDate.AddDays(-1);
                    criteria.FiList.Add(p => p.LastRentalPayment.EndDate < endDate ||
                                             (p.LastServicePayment != null && p.LastServicePayment.EndDate < endDate));
                    var myDataSet = new ReportDataSet();
                    var serNo = 1;
                    var selectedCompany = new CompanyService(true).GetCompany();

                    var rooms = new RoomService(true).GetAll(criteria).ToList();

                    string datecaption = ReportUtility.GetEthCalendar(FilterStartDate, true) + "(" +
                                         FilterStartDate.ToShortDateString() + ")";

                    if (FilterStartDate.Day != FilterEndDate.Day || FilterStartDate.Month != FilterEndDate.Month ||
                        FilterStartDate.Year != FilterEndDate.Year)
                    {
                        datecaption = "ከ " + datecaption + " እስከ " + ReportUtility.GetEthCalendar(FilterEndDate, true) +
                                      "(" +
                                      FilterEndDate.ToShortDateString() + ")";
                    }


                    foreach (var roomDTO in rooms)
                    {
                        var rentee = roomDTO.LastRentee.DisplayName;
                        if (rentee.Length > 20)
                            rentee = rentee.Substring(0, 18) + "...";

                        var overdueMonths = CommonUtility.GetMonthsFromDays(roomDTO.LastRentalPayment.EndDate,
                            endDate);
                        var fee = overdueMonths*roomDTO.RentalFee;
                        if (roomDTO.LastServicePayment != null)
                        {
                            overdueMonths = CommonUtility.GetMonthsFromDays(roomDTO.LastServicePayment.EndDate,
                                endDate);
                            fee += overdueMonths*roomDTO.ServiceFee;
                        }

                        myDataSet.RentalPayment.Rows.Add(
                            serNo,
                            "መክፈል ሲገባቸው ያልከፈሉ",
                            datecaption,
                            roomDTO.LastRentalPayment.EndDateStringAmharic,
                            roomDTO.Number,
                            rentee,
                            "",
                            fee/(decimal) 1.15, 0.0, 0.0,
                            (fee/(decimal) 1.15)*(decimal) 0.15, 0.0,
                            fee,
                            roomDTO.LastRentalPayment.ReceiptNumber, 0
                            , 0.0, "", "", "", "ያበቃበት ቀን", selectedCompany.Header, null, "");
                        serNo++;
                    }

                    var myReport4 = new PaymentList2();
                    myReport4.SetDataSource(myDataSet);

                    var report = new ReportViewerCommon(myReport4);
                    report.Show();

                    #endregion
                }
                    break;
            }
        }

        public ICommand CloseItemViewCommand
        {
            get { return _closeItemViewCommand ?? (_closeItemViewCommand = new RelayCommand<Object>(CloseWindow)); }
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
        private DateTime _filterStartDate;
        private DateTime _filterEndDate;


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