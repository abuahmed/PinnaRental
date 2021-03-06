﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.Service;
using PinnaRent.Service.Interfaces;
using PinnaRent.WPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.MessageBox;

namespace PinnaRent.WPF.ViewModel
{
    public class ExpensesViewModel : ViewModelBase
    {
        #region Fields
        private ICommand _refreshWindowCommand;
        private static IPaymentService _paymentService;
        #endregion

        #region Constructor
        public ExpensesViewModel()
        {
            FillPeriodCombo();
            CheckRoles();
            //SelectedPeriod = FilterPeriods.FirstOrDefault();

            var currentMonth = Convert.ToInt32(ReportUtility.GetEthCalendar(DateTime.Now, false).Substring(2, 2));
            var currentYear = Convert.ToInt32(ReportUtility.GetEthCalendar(DateTime.Now, false).Substring(4, 4));

            FilterStartDate = ReportUtility.GetGregorCalendar(currentYear, currentMonth, 1);
            FilterEndDate = ReportUtility.GetGregorCalendar(currentYear, currentMonth, 30);

            if(currentMonth==13)
                FilterEndDate = ReportUtility.GetGregorCalendar(currentYear, currentMonth, 5);

            FillPaymentTypesCombo();
            SelectedPaymentType = PaymentTypesFilter.FirstOrDefault();

            GetWarehouses();
            Load();
        }

        public void Load()
        {
            CleanUp();
            _paymentService = new PaymentService(DbContextUtil.GetDbContextInstance());

            if (Warehouses != null && Warehouses.Any())
            {
                if (SelectedWarehouse == null)
                    SelectedWarehouse =Warehouses.FirstOrDefault();
                // Warehouses.FirstOrDefault(w => w.IsDefault) ?? Warehouses.FirstOrDefault();
                else
                    SelectedWarehouse = SelectedWarehouse;
            }
        }

        public static void CleanUp()
        {
            if (_paymentService != null)
                _paymentService.Dispose();
        }
        public ICommand RefreshWindowCommand
        {
            get
            {
                return _refreshWindowCommand ?? (_refreshWindowCommand = new RelayCommand(Load));
            }
        }
        #endregion

        #region Expenses

        #region Fields
        private int _totalNumberOfPayments;
        private string _totalValueOfPayments;
        private bool _addNewPaymentCommandVisibility, _savePaymentCommandVisibility;
        private PaymentDTO _selectedPayment;
        private IEnumerable<PaymentDTO> _paymentList;
        private ObservableCollection<PaymentDTO> _payments;
        private ICommand _addNewPaymentCommand, _deletePaymentCommand;
        #endregion

        #region Public Properties
        public int TotalNumberOfPayments
        {
            get { return _totalNumberOfPayments; }
            set
            {
                _totalNumberOfPayments = value;
                RaisePropertyChanged<int>(() => TotalNumberOfPayments);
            }
        }
        public string TotalValueOfPayments
        {
            get { return _totalValueOfPayments; }
            set
            {
                _totalValueOfPayments = value;
                RaisePropertyChanged<string>(() => TotalValueOfPayments);
            }
        }
        public bool AddNewPaymentCommandVisibility
        {
            get { return _addNewPaymentCommandVisibility; }
            set
            {
                _addNewPaymentCommandVisibility = value;
                RaisePropertyChanged<bool>(() => AddNewPaymentCommandVisibility);
            }
        }
        public bool SavePaymentCommandVisibility
        {
            get { return _savePaymentCommandVisibility; }
            set
            {
                _savePaymentCommandVisibility = value;
                RaisePropertyChanged<bool>(() => SavePaymentCommandVisibility);
            }
        }
        public PaymentDTO SelectedPayment
        {
            get { return _selectedPayment; }
            set
            {
                _selectedPayment = value;
                RaisePropertyChanged<PaymentDTO>(() => SelectedPayment);
                if (SelectedPayment != null)
                {
                    SavePaymentCommandVisibility = SelectedWarehouse != null && SelectedWarehouse.Id != -1;
                }
            }
        }
        public ObservableCollection<PaymentDTO> Payments
        {
            get { return _payments; }
            set
            {
                _payments = value;
                RaisePropertyChanged<ObservableCollection<PaymentDTO>>(() => Payments);

                var cashOut = Payments.Where(p => p.Type == PaymentTypes.CashOut).Sum(p => p.Amount);
                //var cashIn = Payments.Where(p => p.Type == PaymentTypes.CashIn).Sum(p => p.Amount);

                //var diff = cashIn - cashOut;
                TotalValueOfPayments = cashOut.ToString("N");

            }
        }
        public IEnumerable<PaymentDTO> PaymentList
        {
            get { return _paymentList; }
            set
            {
                _paymentList = value;
                RaisePropertyChanged<IEnumerable<PaymentDTO>>(() => PaymentList);
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewPaymentCommand
        {
            get
            {
                return _addNewPaymentCommand ?? (_addNewPaymentCommand = new RelayCommand<Object>(ExcuteAddNewPaymentCommand));
            }
        }
        private void ExcuteAddNewPaymentCommand(object button)
        {
            try
            {
                var btn = button as System.Windows.Controls.Button;
                if (btn != null)
                {
                    string buttonTag = btn.Tag.ToString();
                    switch (buttonTag)
                    {
                        case "Expense":
                            var expenseEntryWindow = new ExpenseEntry(PaymentTypes.CashOut);
                            expenseEntryWindow.ShowDialog();
                            break;
                        case "CashLoan":
                            var cashLoanEntryWindow = new ExpenseEntry(PaymentTypes.CashIn);
                            cashLoanEntryWindow.ShowDialog();
                            break;
                        case "Edit":
                            var editWindow = new ExpenseEntry(SelectedPayment);
                            editWindow.ShowDialog();
                            break;
                    }
                }
                Load();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Add Payment"
                                  + Environment.NewLine + exception.Message, "Can't Add Payment", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeletePaymentCommand
        {
            get
            {
                return _deletePaymentCommand ?? (_deletePaymentCommand = new RelayCommand(ExcuteDeletePaymentCommand, CanSave));
            }
        }
        private void ExcuteDeletePaymentCommand()
        {
            try
            {
                SelectedPayment.Enabled = false;
                var stat = _paymentService.InsertOrUpdate(SelectedPayment);

                if (string.IsNullOrEmpty(stat))
                    Load();
                else
                    MessageBox.Show("Can't Delete Payment"
                                 + Environment.NewLine + stat, "Can't Delete Payment", MessageBoxButton.OK,
                     MessageBoxImage.Error);


            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Delete Payment"
                                  + Environment.NewLine + exception.Message, "Can't Delete Payment", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }
        #endregion

        public void GetLivePayments()
        {
            PaymentList = new List<PaymentDTO>();

            var criteria = new SearchCriteria<PaymentDTO>
            {
                CurrentUserId = Singleton.User.UserId,
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate
            };
            criteria.FiList.Add(p => p.Type == PaymentTypes.CashIn || p.Type == PaymentTypes.CashOut);

            //if (!string.IsNullOrEmpty(FilterByPerson))
            //    criteria.FiList.Add(pi => pi.PersonName.ToLower().Contains(FilterByPerson.ToLower()));

            //if (!string.IsNullOrEmpty(FilterByReason))
            //    criteria.FiList.Add(pi => pi.Reason.ToLower().Contains(FilterByReason.ToLower()));

            //if (SelectedPaymentType != null)
            //    criteria.Type = SelectedPaymentType.Value;

            if (SelectedWarehouse != null && SelectedWarehouse.Id != -1)
                criteria.SelectedWarehouseId = SelectedWarehouse.Id;

            PaymentList = _paymentService.GetAll(criteria).OrderBy(p=>p.PaymentDate).ToList();

            var sNo = 1;
            foreach (var paymentDTO in PaymentList)
            {
                paymentDTO.SerialNumber = sNo;
                sNo++;
            }

            Payments = new ObservableCollection<PaymentDTO>(PaymentList);

            TotalNumberOfPayments = Payments.Count;

            if (Payments.Count > 0)
                SelectedPayment = Payments.FirstOrDefault();
        }

        #endregion

        private ICommand _memberStartDateViewCommand, _memberEndDateViewCommand;
        private string _startDateText, _endDateText;

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
            var calConv = new CalendarConvertor(FilterStartDate);
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
            var calConv = new CalendarConvertor(FilterEndDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterEndDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        #region Warehouse
        private IEnumerable<WarehouseDTO> _warehouses;
        private WarehouseDTO _selectedWarehouse;

        public IEnumerable<WarehouseDTO> Warehouses
        {
            get { return _warehouses; }
            set
            {
                _warehouses = value;
                RaisePropertyChanged<IEnumerable<WarehouseDTO>>(() => Warehouses);
            }
        }
        public WarehouseDTO SelectedWarehouse
        {
            get { return _selectedWarehouse; }
            set
            {
                _selectedWarehouse = value;
                RaisePropertyChanged<WarehouseDTO>(() => SelectedWarehouse);

                if (SelectedWarehouse != null)
                {
                    AddNewPaymentCommandVisibility = SelectedWarehouse.Id != -1;
                    GetLivePayments();
                }
            }
        }
        public void GetWarehouses()
        {
            Warehouses = Singleton.WarehousesList;
        }
        #endregion

        #region Filter Header

        #region Fields
        private string _filterPeriod;
        private string _filterByPerson, _filterByReason;
        private IList<ListDataItem> _filterPeriods;
        private ListDataItem _selectedPeriod;
        private DateTime _filterStartDate, _filterEndDate;
        private List<ListDataItem> _paymentTypesFilter;
        private ListDataItem _selectedPaymentType;
        #endregion

        #region Public Properties
        public string FilterPeriod
        {
            get { return _filterPeriod; }
            set
            {
                _filterPeriod = value;
                RaisePropertyChanged<string>(() => FilterPeriod);
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
                    StartDateText = ReportUtility.GetEthCalendarFormated(FilterStartDate, "-") +
                                    "(" + FilterStartDate.ToString("dd-MM-yyyy") + ")";
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
                    EndDateText = ReportUtility.GetEthCalendarFormated(FilterEndDate, "-") + "(" +
                                  FilterEndDate.ToString("dd-MM-yyyy") + ")";
            }
        }

        public IList<ListDataItem> FilterPeriods
        {
            get { return _filterPeriods; }
            set
            {
                _filterPeriods = value;
                RaisePropertyChanged<IList<ListDataItem>>(() => FilterPeriods);
            }
        }
        public ListDataItem SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                _selectedPeriod = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedPeriod);
                if (SelectedPeriod == null) return;
                switch (SelectedPeriod.Value)
                {
                    case 0:
                        FilterStartDate = DateTime.Now.AddMonths(-3);
                        FilterEndDate = DateTime.Now;
                        break;
                    case 1:
                        FilterStartDate = DateTime.Now;
                        FilterEndDate = DateTime.Now;
                        break;
                    case 2:
                        FilterStartDate = DateTime.Now.AddDays(-1);
                        FilterEndDate = DateTime.Now.AddDays(-1);
                        break;
                    case 3:
                        FilterStartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                        FilterEndDate = DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek - 1);
                        break;
                    case 4:
                        FilterStartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 7);
                        FilterEndDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 1);
                        break;
                }
            }
        }
        public string FilterByPerson
        {
            get { return _filterByPerson; }
            set
            {
                _filterByPerson = value;
                RaisePropertyChanged<string>(() => FilterByPerson);
            }
        }
        public string FilterByReason
        {
            get { return _filterByReason; }
            set
            {
                _filterByReason = value;
                RaisePropertyChanged<string>(() => FilterByReason);
            }
        }

        public List<ListDataItem> PaymentTypesFilter
        {
            get { return _paymentTypesFilter; }
            set
            {
                _paymentTypesFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => PaymentTypesFilter);
            }
        }
        public ListDataItem SelectedPaymentType
        {
            get { return _selectedPaymentType; }
            set
            {
                _selectedPaymentType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedPaymentType);
            }
        }

        #endregion

        private void FillPaymentTypesCombo()
        {
            PaymentTypesFilter = new List<ListDataItem>
            {
                new ListDataItem {Display = "All", Value = 0},
                new ListDataItem {Display = "Expenses", Value = 2},
                new ListDataItem {Display = "Cash Loan", Value = 5}
            };
        }
        private void FillPeriodCombo()
        {
            FilterPeriods = new List<ListDataItem>
            {
                new ListDataItem {Display = "All/Custom", Value = 0},
                new ListDataItem {Display = "Today", Value = 1},
                new ListDataItem {Display = "Yesterday", Value = 2},
                new ListDataItem {Display = "This Week", Value = 3},
                new ListDataItem {Display = "Last Week", Value = 4}
            };
        }

        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave()
        {
            return Errors == 0;
        }
        #endregion

        #region Export To Excel
        private ICommand _exportToExcelCommand;
        public ICommand ExportToExcelCommand
        {
            get { return _exportToExcelCommand ?? (_exportToExcelCommand = new RelayCommand(ExecuteExportToExcelCommand)); }
        }

        private void ExecuteExportToExcelCommand()
        {
            string[] columnsHeader = { "Number", "On Date", "Reason", "Paid To", "Amount", "Receipt No.", "Check No." };


            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            var xlApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                Microsoft.Office.Interop.Excel.Workbook excelBook = xlApp.Workbooks.Add();
                var excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                int rowsTotal = Payments.Count;
                int colsTotal = columnsHeader.Count();

                var with1 = excelWorksheet;
                with1.Cells.Select();
                with1.Cells.Delete();

                var iC = 0;
                for (iC = 0; iC <= colsTotal - 1; iC++)
                {
                    with1.Cells[1, iC + 1].Value = columnsHeader[iC];
                }

                var I = 0;
                for (I = 0; I <= rowsTotal - 1; I++)
                {
                    //with1.Cells[I + 2, 0 + 1].value = Payments[I].Number;
                    with1.Cells[I + 2, 1 + 1].value = Payments[I].PaymentDateString;
                    with1.Cells[I + 2, 2 + 1].value = Payments[I].Reason;
                    with1.Cells[I + 2, 3 + 1].value = Payments[I].PersonName;
                    with1.Cells[I + 2, 4 + 1].value = Payments[I].Amount;
                    with1.Cells[I + 2, 5 + 1].value = Payments[I].ReceiptNumber;
                    with1.Cells[I + 2, 6 + 1].value = Payments[I].CheckNumber;
                }

                with1.Rows["1:1"].Font.FontStyle = "Bold";
                with1.Rows["1:1"].Font.Size = 12;

                with1.Cells.Columns.AutoFit();
                with1.Cells.Select();
                with1.Cells.EntireColumn.AutoFit();
                with1.Cells[1, 1].Select();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //RELEASE ALLOACTED RESOURCES
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                xlApp = null;
            }
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
