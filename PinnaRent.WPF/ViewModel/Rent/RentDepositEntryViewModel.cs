#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PinnaRent.WPF.Reports;
using PinnaRent.WPF.Views;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class RentDepositEntryViewModel : ViewModelBase
    {
        #region Fields

        private static IRentDepositService _rentDepositService;
        private static IRoomService _roomService;
        private RentDepositDTO _selectedRentDeposit;

        private ICommand _addNewRentDepositViewCommand,_refreshViewCommand,_printViewCommand,
            _saveRentDepositViewCommand,
            _deleteRentDepositViewCommand,
            _memberSubscribedDateViewCommand;

        private string _subscriptionText, _totalAmountDeposied, _detailVisibility, _windowWidth, _windowHeight;
        private int _contratPeriod, _windowWidth2;
        private decimal _amountToPay;
        private RentalContratDTO _selectedRentalContrat, _selectedRentalContratTemp;
        private bool _subscriptionDateEnability, _depositReturnChecked, _depositUsedChecked;
        private ObservableCollection<RentDepositDTO> _rentDeposits;

        #endregion

        #region Constructor

        public RentDepositEntryViewModel()
        {
            //WindowWidth2 = 400;
            CleanUp();
            _rentDepositService = new RentDepositService();
            _roomService = new RoomService();
            FilterStartDate = DateTime.Now.AddYears(-5);
            FilterEndDate = DateTime.Now;
            
            FillCpoTypesCombo();
            CheckRoles();

            ListVisibility = "Visible";

            Messenger.Default.Register<RentalContratDTO>(this, (message) => { SelectedRentalContratTemp = message; });
        }

        public static void CleanUp()
        {
            if (_rentDepositService != null)
                _rentDepositService.Dispose();
            if (_roomService != null)
                _roomService.Dispose();
        }

        #endregion

        #region Public Properties

        public RentalContratDTO SelectedRentalContratTemp
        {
            get { return _selectedRentalContratTemp; }
            set
            {
                _selectedRentalContratTemp = value;
                RaisePropertyChanged<RentalContratDTO>(() => SelectedRentalContratTemp);

                if (SelectedRentalContratTemp != null && SelectedRentalContratTemp.Id != 0)
                {
                    SelectedRentalContrat = SelectedRentalContratTemp;
                    LoadSubscription();
                    ListVisibility = "Collapsed";
                    DetailVisibility = "Visible";
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

        public void LoadSubscription()
        {
            var criteria = new SearchCriteria<RentDepositDTO>();
            criteria.FiList.Add(m => m.ContratId == SelectedRentalContratTemp.Id);

            var rentDeposit = _rentDepositService.GetAll(criteria).FirstOrDefault();

            SelectedRentDeposit = rentDeposit ?? new RentDepositDTO
            {
                ContratId = SelectedRentalContrat.Id,
                DepositedDate = DateTime.Now
            };

            PaymentPeriod = rentDeposit != null ? rentDeposit.DepositMonths : 1;
        }

        public int PaymentPeriod
        {
            get { return _contratPeriod; }
            set
            {
                _contratPeriod = value;
                RaisePropertyChanged<int>(() => PaymentPeriod);

                CalculateAmount();
            }
        }

        public string WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                _windowWidth = value;
                RaisePropertyChanged<string>(() => WindowWidth);
            }
        }
        public int WindowWidth2
        {
            get { return _windowWidth2; }
            set
            {
                _windowWidth2 = value;
                RaisePropertyChanged<int>(() => WindowWidth2);
            }
        }

        public string WindowHeight
        {
            get { return _windowHeight; }
            set
            {
                _windowHeight = value;
                RaisePropertyChanged<string>(() => WindowHeight);
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

        public void CalculateAmount()
        {
            if (PaymentPeriod != 0)
            {
                AmountToPay = (decimal) (PaymentPeriod*SelectedRentalContrat.Room.RentalFee);
                if (SelectedRentDeposit != null)
                {
                    if (SelectedRentDeposit.Id == 0)
                        SelectedRentDeposit.TotalDepositAmount = AmountToPay;
                }
            }
        }

        public string ListVisibility
        {
            get { return _subscriptionText; }
            set
            {
                _subscriptionText = value;
                RaisePropertyChanged<string>(() => ListVisibility);
                if (ListVisibility == "Visible")
                {
                    SelectedCpoType = CpoTypes.FirstOrDefault(l => l.Value == 1);
                    if (DetailVisibility == "Visible")
                    {
                        WindowWidth = "Auto";
                        WindowHeight = "*";
                    }

                    else
                    {
                        WindowWidth = "Auto";
                        WindowHeight = "*";
                    }

                }
                else
                {
                    WindowWidth = "*";
                    WindowHeight = "Auto";
                }

            }
        }

        public string DetailVisibility
        {
            get { return _detailVisibility; }
            set
            {
                _detailVisibility = value;
                RaisePropertyChanged<string>(() => DetailVisibility);
                //if (DetailVisibility == "Visible")
                //{
                //    WindowWidth = "Auto";
                //    WindowHeight = "Auto";
                //}

                //else
                //{
                //    WindowWidth = "*";
                //    WindowHeight = "Auto";
                //}
            }
        }

        public string TotalAmountDeposited
        {
            get { return _totalAmountDeposied; }
            set
            {
                _totalAmountDeposied = value;
                RaisePropertyChanged<string>(() => TotalAmountDeposited);
            }
        }


        public bool SubscriptionDateEnability
        {
            get { return _subscriptionDateEnability; }
            set
            {
                _subscriptionDateEnability = value;
                RaisePropertyChanged<bool>(() => SubscriptionDateEnability);
            }
        }

        public bool DepositReturnChecked
        {
            get { return _depositReturnChecked; }
            set
            {
                _depositReturnChecked = value;
                RaisePropertyChanged<bool>(() => DepositReturnChecked);
                if (DepositReturnChecked)
                {
                    DepositUsedChecked = false;
                    SelectedRentDeposit.ReturnedDate = DateTime.Now;
                }
                else SelectedRentDeposit.ReturnedDate = null;
            }
        }

        public bool DepositUsedChecked
        {
            get { return _depositUsedChecked; }
            set
            {
                _depositUsedChecked = value;
                RaisePropertyChanged<bool>(() => DepositUsedChecked);
                if (DepositUsedChecked)
                {
                    DepositReturnChecked = false;
                    SelectedRentDeposit.UsedDate = DateTime.Now;
                }
                else SelectedRentDeposit.UsedDate = null;
            }
        }

        public RentDepositDTO SelectedRentDeposit
        {
            get { return _selectedRentDeposit; }
            set
            {
                _selectedRentDeposit = value;
                RaisePropertyChanged<RentDepositDTO>(() => SelectedRentDeposit);
                if (SelectedRentDeposit != null)
                {
                    if (SelectedRentDeposit.Contrat != null)
                    {
                        SelectedRentalContrat = SelectedRentDeposit.Contrat;
                    }
                    DepositReturnChecked = SelectedRentDeposit.ReturnedDate != null;
                    DepositUsedChecked = SelectedRentDeposit.UsedDate != null;
                    PaymentPeriod = SelectedRentDeposit != null ? SelectedRentDeposit.DepositMonths : 1;
                    DetailVisibility = "Visible";
                }
            }
        }

        public ObservableCollection<RentDepositDTO> RentDeposits
        {
            get { return _rentDeposits; }
            set
            {
                _rentDeposits = value;
                RaisePropertyChanged<ObservableCollection<RentDepositDTO>>(() => RentDeposits);
                if (RentDeposits != null)
                    TotalAmountDeposited = RentDeposits.Sum(d => d.TotalDepositAmount).ToString("N2");
            }
        }

        public void LoadDeposits()
        {
            var cri = new SearchCriteria<RentDepositDTO>
            {
                CurrentUserId = Singleton.User.UserId,
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate
            };


            if (SelectedCpoType != null)
                switch (SelectedCpoType.Value)
                {
                    case 1:
                        cri.FiList.Add(d => d.ReturnedDate == null && d.UsedDate == null);
                        cri.IncludePhoto = false;
                        break;
                    case 2:
                        cri.FiList.Add(d => d.ReturnedDate != null || d.UsedDate != null);
                        cri.IncludePhoto = true;
                        break;
                }

            var deposits = _rentDepositService.GetAll(cri).ToList();
            var sNo = 1;
            foreach (var depositDTO in deposits)
            {
                depositDTO.SerialNumber = sNo;
                sNo++;
            }
            RentDeposits = new ObservableCollection<RentDepositDTO>(deposits);
            DetailVisibility = "Collapsed";
        }

        #endregion

        #region Commands
        public ICommand RefreshWindowCommand
        {
            get
            {
                return _refreshViewCommand ?? (_refreshViewCommand = new RelayCommand(Refresh));
            }
        }

        private void Refresh()
        {
           LoadDeposits(); 
        }

        public ICommand PrintWindowCommand
        {
            get
            {
                return _printViewCommand ?? (_printViewCommand = new RelayCommand(Print));
            }
        }

        private void Print()
        {
           LoadDeposits();

           var myDataSet = new ReportDataSet();
           var serNo = 1;
           var selectedCompany = new CompanyService(true).GetCompany();

           string datecaption = ReportUtility.GetEthCalendar(FilterStartDate, true) + "(" +
                                FilterStartDate.ToShortDateString() + ")";

           if (FilterStartDate.Day != FilterEndDate.Day || FilterStartDate.Month != FilterEndDate.Month || FilterStartDate.Year != FilterEndDate.Year)
           {
               datecaption = "ከ " + datecaption + " እስከ " + ReportUtility.GetEthCalendar(FilterEndDate, true) + "(" +
                                FilterEndDate.ToShortDateString() + ")";
           }
            var returned = "";
            if (SelectedCpoType.Value != 1)
                returned = "የተመለሰበት ቀን";

           foreach (var rentalPaymentDTO in RentDeposits)
           {
               var rentee = rentalPaymentDTO.Contrat.Rentee.DisplayName;
               if (rentee.Length > 20)
                   rentee = rentee.Substring(0, 18) + "...";
               myDataSet.RentalPayment.Rows.Add(
                   serNo, 
                   SelectedCpoType.Display,
                   datecaption,
                   rentalPaymentDTO.DepositedDateStringAmharicFormatted,
                   rentalPaymentDTO.Contrat.Room.Number,
                   rentee,
                   "",
                   0.0, 0.0, 0.0,
                   0.0, 0.0,
                   rentalPaymentDTO.TotalDepositAmount, 
                   rentalPaymentDTO.ReturnedDateStringAmharicFormatted,
                   0, 0.0, returned, "", "", "", selectedCompany.Header, null, "");
               serNo++;
           }

           var myReport4 = new DepositList();
           myReport4.SetDataSource(myDataSet);

           var report = new ReportViewerCommon(myReport4);
           report.Show(); 
        }

        public ICommand AddNewRentDepositViewCommand
        {
            get
            {
                return _addNewRentDepositViewCommand ??
                       (_addNewRentDepositViewCommand = new RelayCommand(AddNewRentDeposit));
            }
        }

        private void AddNewRentDeposit()
        {
            SelectedRentDeposit = new RentDepositDTO()
            {
                ContratId = SelectedRentalContrat.Id,
                DepositedDate = DateTime.Now
            };
            PaymentPeriod = 1;
        }

        private void AddPayments()
        {
            var rentDepositPayment = new PaymentDTO
            {
                Type = PaymentTypes.Deposit,
                RentDeposit = SelectedRentDeposit,
                PaymentDate = SelectedRentDeposit.DepositedDate,
                ReceiptDate = DateTime.Now,
                Amount = SelectedRentDeposit.TotalDepositAmount,
                Reason = "Deposit",
                PaymentMethod = PaymentMethods.Cash
            };
            SelectedRentDeposit.Payments.Add(rentDepositPayment);
        }

        public ICommand SaveRentDepositViewCommand
        {
            get
            {
                return _saveRentDepositViewCommand ??
                       (_saveRentDepositViewCommand = new RelayCommand<object>(SaveRentDeposit, CanSave));
            }
        }

        private void SaveRentDeposit(object obj)
        {
            try
            {
                
                SelectedRentDeposit.DepositMonths = PaymentPeriod;
                AddPayments();
                var stat = _rentDepositService.InsertOrUpdate(SelectedRentDeposit);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (ListVisibility=="Collapsed")
                {
                    //MessageBox.Show("Deposit Successfully Added");
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

        public ICommand DeleteRentDepositViewCommand
        {
            get
            {
                return _deleteRentDepositViewCommand ??
                       (_deleteRentDepositViewCommand =
                           new RelayCommand<Object>(DeleteRentDeposit, CanSave));
            }
        }

        private void DeleteRentDeposit(object obj)
        {
            if (
                MessageBox.Show("Are you Sure You want to Delete this RentDeposit?", "Delete RentDeposit",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedRentDeposit.Enabled = false;
                    var stat = _rentDepositService.Disable(SelectedRentDeposit);
                    if (stat == string.Empty)
                    {
                        //RentDeposits.Remove(SelectedRentDeposit);
                    }
                    else
                    {
                        MessageBox.Show("Can't Delete, may be the data is already in use..."
                                        + Environment.NewLine + stat, "Can't Delete",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't Delete, may be the data is already in use..."
                                    + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException,
                        "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ICommand MemberSubscribedDateViewCommand
        {
            get
            {
                return _memberSubscribedDateViewCommand ??
                       (_memberSubscribedDateViewCommand = new RelayCommand(RentDeposit));
            }
        }

        public void RentDeposit()
        {
            var calConv = new CalendarConvertor(SelectedRentDeposit.DepositedDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedRentDeposit.DepositedDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        #endregion

        #region Filter List

        private DateTime _filterStartDate, _filterEndDate;
        private List<ListDataItem> _cpoTypes;
        private ListDataItem _selectedCpoType;
        private string _startDateText, _endDateText;
        private ICommand _memberStartDateViewCommand, _memberEndDateViewCommand;

        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                _filterStartDate = value;
                RaisePropertyChanged<DateTime>(() => FilterStartDate);

                if (FilterStartDate.Year > 2000)
                    StartDateText = ReportUtility.GetEthCalendar(FilterStartDate, true);
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
                    EndDateText = ReportUtility.GetEthCalendar(FilterEndDate, true);
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

        public ICommand MemberStartDateViewCommand
        {
            get
            {
                return _memberStartDateViewCommand ??
                       (_memberStartDateViewCommand = new RelayCommand(MemberStartDate));
            }
        }

        public void MemberStartDate()
        {
            var calConv = new CalendarConvertor(DateTime.Now);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterStartDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand MemberEndDateViewCommand
        {
            get
            {
                return _memberEndDateViewCommand ??
                       (_memberEndDateViewCommand = new RelayCommand(MemberEndDate));
            }
        }

        public void MemberEndDate()
        {
            var calConv = new CalendarConvertor(DateTime.Now);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterEndDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        public List<ListDataItem> CpoTypes
        {
            get { return _cpoTypes; }
            set
            {
                _cpoTypes = value;
                RaisePropertyChanged<List<ListDataItem>>(() => CpoTypes);
            }
        }

        public ListDataItem SelectedCpoType
        {
            get { return _selectedCpoType; }
            set
            {
                _selectedCpoType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedCpoType);
                LoadDeposits();
            }
        }

        private void FillCpoTypesCombo()
        {
            CpoTypes = new List<ListDataItem>
            {
                new ListDataItem {Display = "ተቀማጭ/ተመላሽ የተደረጉ", Value = 0},
                new ListDataItem {Display = "ተቀማጭ የተደረጉ", Value = 1},
                new ListDataItem {Display = "ተመላሽ የተደረጉ", Value = 2}
            };
        }

        #endregion

        #region StartDate

        private DateTime _startDate;

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged<DateTime>(() => StartDate);
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
            SubscriptionDateEnability = UserRoles.ContratEdit == "Visible";
        }

        #endregion
    }
}