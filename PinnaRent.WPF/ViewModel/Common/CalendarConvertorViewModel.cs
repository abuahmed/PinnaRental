﻿#region

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaRent.Core;
using PinnaRent.Core.Common;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class CalendarConvertorViewModel : ViewModelBase
    {
        #region Fields

        private string _durationHeader;
        private DateTime _gregorDayFrom, _gregorDayTo;
        private bool _toGregor = false, _toEthio = false;

        #endregion

        #region Constructors

        public CalendarConvertorViewModel()
        {
            DurationHeader = "Calendar Convertor";

            #region Initialize

            _gregorDays = new ObservableCollection<ListDataItem>();
            _gregorMonths = new ObservableCollection<ListDataItem>();
            _gregorYears = new ObservableCollection<ListDataItem>();
            _selectedGregorDay = new ListDataItem();
            _selectedGregorMonth = new ListDataItem();
            _selectedGregorYear = new ListDataItem();
            _ethioDays = new ObservableCollection<ListDataItem>();
            _ethioMonths = new ObservableCollection<ListDataItem>();
            _ethioYears = new ObservableCollection<ListDataItem>();
            _selectedEthioDay = new ListDataItem();
            _selectedEthioMonth = new ListDataItem();
            _selectedEthioYear = new ListDataItem();

            #endregion

            #region Load Properties

            for (var i = 1; i <= 30; i++)
            {
                GregorDays.Add(new ListDataItem {Display = i.ToString(), Value = i});
                EthioDays.Add(new ListDataItem {Display = i.ToString(), Value = i});
            }
            GregorDays.Add(new ListDataItem {Display = "31", Value = 31});

            for (var i = 1; i <= 12; i++)
            {
                string monthNo = " (" + i + ")"; //""; 
                GregorMonths.Add(new ListDataItem {Display = ReportUtility.GetEngMonth(i - 1) + monthNo, Value = i});
                EthioMonths.Add(new ListDataItem {Display = ReportUtility.GetAmhMonth(i - 1) + monthNo, Value = i});
            }
            EthioMonths.Add(new ListDataItem {Display = ReportUtility.GetAmhMonth(12) + " (13)", Value = 13});

            for (var i = 1900; i <= 2020; i++)
            {
                GregorYears.Add(new ListDataItem {Display = i.ToString(), Value = i});
            }
            for (var i = 1900; i <= 2020; i++)
            {
                EthioYears.Add(new ListDataItem {Display = i.ToString(), Value = i});
            }


            //try
            //{
            //    SelectedGregorDay = GregorDays[DateTime.Now.Day - 1];
            //    SelectedGregorMonth = GregorMonths[DateTime.Now.Month - 1];
            //    SelectedGregorYear = GregorYears[DateTime.Now.Year - 1900];

            //    SetEthioValues();
            //}
            //catch
            //{
            //}

            #endregion

            Messenger.Default.Register<DateTime>(this, (message) => { SelectedDateParam = message; });
            //SetGregor();
        }

        #endregion

        #region Properties

        public string DurationHeader
        {
            get { return _durationHeader; }
            set
            {
                _durationHeader = value;
                RaisePropertyChanged<string>(() => DurationHeader);
            }
        }

        public DateTime GregorDayFrom
        {
            get { return _gregorDayFrom; }
            set
            {
                _gregorDayFrom = value;
                RaisePropertyChanged<DateTime>(() => GregorDayFrom);
            }
        }

        public DateTime GregorDayTo
        {
            get { return _gregorDayTo; }
            set
            {
                _gregorDayTo = value;
                RaisePropertyChanged<DateTime>(() => GregorDayTo);
            }
        }

        #endregion

        #region Gregor Properties

        private ObservableCollection<ListDataItem> _gregorDays, _gregorMonths, _gregorYears;
        private ListDataItem _selectedGregorDay, _selectedGregorMonth, _selectedGregorYear;

        public ObservableCollection<ListDataItem> GregorDays
        {
            get { return _gregorDays; }
            set
            {
                _gregorDays = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => GregorDays);
            }
        }

        public ListDataItem SelectedGregorDay
        {
            get { return _selectedGregorDay; }
            set
            {
                _selectedGregorDay = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedGregorDay);
                if (!_toGregor)
                    SetEthioValues();
            }
        }

        public ObservableCollection<ListDataItem> GregorMonths
        {
            get { return _gregorMonths; }
            set
            {
                _gregorMonths = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => GregorMonths);
            }
        }

        public ListDataItem SelectedGregorMonth
        {
            get { return _selectedGregorMonth; }
            set
            {
                _selectedGregorMonth = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedGregorMonth);
                if (!_toGregor)
                    SetEthioValues();
            }
        }

        public ObservableCollection<ListDataItem> GregorYears
        {
            get { return _gregorYears; }
            set
            {
                _gregorYears = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => GregorYears);
            }
        }

        public ListDataItem SelectedGregorYear
        {
            get { return _selectedGregorYear; }
            set
            {
                _selectedGregorYear = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedGregorYear);
                if (!_toGregor)
                    SetEthioValues();
            }
        }

        #endregion

        #region Ethio Properties

        private ObservableCollection<ListDataItem> _ethioDays, _ethioMonths, _ethioYears;
        private ListDataItem _selectedEthioDay, _selectedEthioMonth, _selectedEthioYear;
        private DateTime _selectedDate, _selectedDateParam;

        public ObservableCollection<ListDataItem> EthioDays
        {
            get { return _ethioDays; }
            set
            {
                _ethioDays = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => EthioDays);
            }
        }

        public ListDataItem SelectedEthioDay
        {
            get { return _selectedEthioDay; }
            set
            {
                _selectedEthioDay = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedEthioDay);
                if(!_toEthio)
                SetGregorValues();
            }
        }

        public ObservableCollection<ListDataItem> EthioMonths
        {
            get { return _ethioMonths; }
            set
            {
                _ethioMonths = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => EthioMonths);
            }
        }

        public ListDataItem SelectedEthioMonth
        {
            get { return _selectedEthioMonth; }
            set
            {
                _selectedEthioMonth = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedEthioMonth);
                if (!_toEthio)
                SetGregorValues();
            }
        }

        public ObservableCollection<ListDataItem> EthioYears
        {
            get { return _ethioYears; }
            set
            {
                _ethioYears = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => EthioYears);
            }
        }

        public ListDataItem SelectedEthioYear
        {
            get { return _selectedEthioYear; }
            set
            {
                _selectedEthioYear = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedEthioYear);
                if (!_toEthio)
                SetGregorValues();
            }
        }

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged<DateTime>(() => SelectedDate);
            }
        }

        public DateTime SelectedDateParam
        {
            get { return _selectedDateParam; }
            set
            {
                _selectedDateParam = value;
                RaisePropertyChanged<DateTime>(() => SelectedDateParam);
                SetGregor();
            }
        }

        #endregion

        #region Commands

        private ICommand _convertDateToEthioCommand, _convertDateToGregorCommand, _okCommand;

        public ICommand ConvertDateToEthioCommand
        {
            get
            {
                return _convertDateToEthioCommand ?? (_convertDateToEthioCommand = new RelayCommand(SetEthioValues));
            }
        }

        public void SetEthioValues()
        {
            SetEthioValues(true);
        }

        public void SetEthioValues(bool toEthio)
        {
            try
            {
                _toEthio = toEthio;
                var gregorDayFrom = new DateTime(SelectedGregorYear.Value, SelectedGregorMonth.Value,
                    SelectedGregorDay.Value);
                var ethioDayFrom = ReportUtility.GetEthCalendar(gregorDayFrom, false);
                int dayf = Convert.ToInt32(ethioDayFrom.Substring(0, 2)),
                    monthf = Convert.ToInt32(ethioDayFrom.Substring(2, 2)),
                    yearf = Convert.ToInt32(ethioDayFrom.Substring(4, 4));
                SelectedEthioDay = EthioDays[dayf - 1];
                SelectedEthioMonth = EthioMonths[monthf - 1];
                SelectedEthioYear = EthioYears[yearf - 1900];
                _toEthio = false;
            }
            catch
            {
                //MessageBox.Show("Can't convert, may be out side of the scope!");
            }
        }

        public ICommand ConvertDateToGregorCommand
        {
            get
            {
                return _convertDateToGregorCommand ?? (_convertDateToGregorCommand = new RelayCommand(SetGregorValues));
            }
        }

        public void SetGregor()
        {
            try
            {
                SelectedDate = SelectedDateParam;
                if (SelectedDateParam.Date.Year < 1900)
                    SelectedDate = DateTime.Now;

                SelectedGregorDay = GregorDays[SelectedDate.Day - 1];
                SelectedGregorMonth = GregorMonths[SelectedDate.Month - 1];
                SelectedGregorYear = GregorYears[SelectedDate.Year - 1900];

                SetEthioValues(false);
            }
            catch
            {
            }
        }

        public void SetGregorValues()
        {
            try
            {
                _toGregor = true;
                var gregorDayFrom = ReportUtility.GetGregorCalendar(SelectedEthioYear.Value, SelectedEthioMonth.Value,
                    SelectedEthioDay.Value);
                SelectedDate = gregorDayFrom;

                int dayf = Convert.ToInt32(gregorDayFrom.Day),
                    monthf = Convert.ToInt32(gregorDayFrom.Month),
                    yearf = Convert.ToInt32(gregorDayFrom.Year);
                SelectedGregorDay = GregorDays[dayf - 1];
                SelectedGregorMonth = GregorMonths[monthf - 1];
                SelectedGregorYear = GregorYears[yearf - 1900];
                _toGregor = false;
            }
            catch
            {
                //MessageBox.Show("Can't convert, may be out side of the scope!");
            }
        }


        public ICommand OkCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand<Object>(ExcuteOkCommand)); }
        }

        public void ExcuteOkCommand(object obj)
        {
            SelectedDateParam = SelectedDate;
            CloseWindow(obj);
        }

        public void CloseWindow(object obj)
        {
            try
            {
                if (obj == null) return;
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch {}
        }

        #endregion
    }
}