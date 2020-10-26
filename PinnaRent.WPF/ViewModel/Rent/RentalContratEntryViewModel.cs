#region

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.Service.Interfaces;
using PinnaRent.WPF.Views;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class RentalContratEntryViewModel : ViewModelBase
    {
        #region Fields

        private static IRentalContratService _rentalContratService;
        private static IRoomService _roomService;
        private RentalContratDTO _selectedRentalContrat, _selectedRentalContratTemp;
        
        private ICommand
            _saveRentalContratViewCommand,
            _deleteRentalContratViewCommand, 
            _memberStartDateViewCommand,
            _memberEndDateViewCommand,
            _addNewRenteeCommand;

        private string _subscriptionText;
        private int? _contratPeriod;
        private int _conId;
        private RoomDTO _selectedRoom;
        private bool _subscriptionDateEnability, _renteeEditEnability;
        private ObservableCollection<RenteeDTO> _rentees;
        private RenteeDTO _selectedRentee;
        private DateTime _startDate;

        #endregion

        #region Constructor

        public RentalContratEntryViewModel()
        {
            CleanUp();
            _rentalContratService = new RentalContratService();
            _roomService = new RoomService();

            LoadRentees();
            CheckRoles();

            RentalContratText = "ውል ማስገቢያ/ማስተካከያ";
            
            Messenger.Default.Register<RentalContratDTO>(this, (message) => { SelectedRentalContratTemp = message; });
        }

        public static void CleanUp()
        {
            if (_rentalContratService != null)
                _rentalContratService.Dispose();
            if (_roomService != null)
                _roomService.Dispose();
        }

        #endregion

        #region Public Properties

        #region Rentees

        public ObservableCollection<RenteeDTO> Rentees
        {
            get { return _rentees; }
            set
            {
                _rentees = value;
                RaisePropertyChanged<ObservableCollection<RenteeDTO>>(() => Rentees);
            }
        }
        
        public RenteeDTO SelectedRentee
        {
            get { return _selectedRentee; }
            set
            {
                _selectedRentee = value;
                RaisePropertyChanged<RenteeDTO>(() => SelectedRentee);
            }
        }

        public void LoadRentees()
        {
            var criteria = new SearchCriteria<RenteeDTO>();
            var subs = new RenteeService(true).GetAll(criteria).ToList();
            Rentees = new ObservableCollection<RenteeDTO>(subs);
        }

        #endregion

        public RoomDTO SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                RaisePropertyChanged<RoomDTO>(() => SelectedRoom);
            }
        }
        
        public int? ContratPeriod
        {
            get { return _contratPeriod; }
            set
            {
                _contratPeriod = value;
                RaisePropertyChanged<int?>(() => ContratPeriod);
                if(ContratPeriod!=null && ContratPeriod!=0)
                if(SelectedRentalContrat.Id==0)
                    StartDate = DateTime.Now;
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged<DateTime>(() => StartDate);

                SelectedRentalContrat.StartDate = StartDate;
                if (ContratPeriod != null && ContratPeriod != 0)
                {
                    var days = (int)(ContratPeriod * 30);
                    SelectedRentalContrat.EndDate = StartDate.AddDays(days).AddDays(-1);
                }

            }
        }
        
        public int ConId
        {
            get { return _conId; }
            set
            {
                _conId = value;
                RaisePropertyChanged<int>(() => ConId);

            }
        }
        
        public string RentalContratText
        {
            get { return _subscriptionText; }
            set
            {
                _subscriptionText = value;
                RaisePropertyChanged<string>(() => RentalContratText);
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
        
        public bool RenteeEditEnability
        {
            get { return _renteeEditEnability; }
            set
            {
                _renteeEditEnability = value;
                RaisePropertyChanged<bool>(() => RenteeEditEnability);
            }
        }

        
        public RentalContratDTO SelectedRentalContratTemp
        {
            get { return _selectedRentalContratTemp; }
            set
            {
                _selectedRentalContratTemp = value;
                RaisePropertyChanged<RentalContratDTO>(() => SelectedRentalContratTemp);
                if (SelectedRentalContratTemp != null)
                {
                    SelectedRoom = new RoomService(true).GetAll().FirstOrDefault(r=>r.Id==SelectedRentalContratTemp.RoomId);
                    if(SelectedRoom==null)
                        return;
                    RenteeEditEnability = false;

                    if (SelectedRentalContratTemp.RenteeId != 0)
                        SelectedRentee = Rentees.FirstOrDefault(r => r.Id == SelectedRentalContratTemp.RenteeId);
                    
                    if (SelectedRentalContratTemp.Id != 0)
                        SelectedRentalContrat = _rentalContratService.GetAll().FirstOrDefault(c => c.Id == SelectedRentalContratTemp.Id);
                    else
                    {
                        RenteeEditEnability = true;
                        SelectedRentalContrat = new RentalContratDTO
                        {
                            ContratPeriod = 1,
                            LastRentDepositId = SelectedRoom.LastRentDepositId,//????? ????? Needs Checking
                            RoomId = SelectedRoom.Id,
                            EndDate = DateTime.Now.AddMonths(1),
                            StartDate = DateTime.Now
                        };
                    }
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
                if (SelectedRentalContrat != null)
                {
                    ContratPeriod = SelectedRentalContrat.ContratPeriod;
                    StartDate = SelectedRentalContrat.StartDate;
                }
            }
        }
        
        #endregion

        #region Commands
        
        public ICommand SaveRentalContratViewCommand
        {
            get
            {
                return _saveRentalContratViewCommand ??
                       (_saveRentalContratViewCommand = new RelayCommand<Object>(SaveRentalContrat, CanSave));
            }
        }

        private void SaveRentalContrat(object obj)
        {
            if (SaveRentalContrat())
                CloseWindow(obj);
        }

        private bool SaveRentalContrat()
        {
            try
            {
                if (SelectedRentee != null)
                {
                    SelectedRentalContrat.RenteeId = SelectedRentee.Id;
                    SelectedRentalContrat.StartDate = StartDate;
                    SelectedRentalContrat.ContratPeriod = ContratPeriod;
                }

                var stat = _rentalContratService.InsertOrUpdate(SelectedRentalContrat);
                ConId = SelectedRentalContrat.Id;
                //MessageBox.Show(SelectedRentalContrat.Id.ToString());
                if (stat != string.Empty)
                {
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
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

        public ICommand DeleteRentalContratViewCommand
        {
            get
            {
                return _deleteRentalContratViewCommand ??
                       (_deleteRentalContratViewCommand =
                           new RelayCommand<Object>(DeleteRentalContrat, CanSave));
            }
        }

        private void DeleteRentalContrat(object obj)
        {
            if (
                MessageBox.Show("Are you Sure You want to Delete this RentalContrat?", "Delete RentalContrat",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedRentalContrat.Enabled = false;
                    var stat = _rentalContratService.Disable(SelectedRentalContrat);
                    if (stat == string.Empty)
                    {
                        //RentalContrats.Remove(SelectedRentalContrat);
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
            var calConv = new CalendarConvertor(SelectedRentalContrat.StartDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    StartDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
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
            var calConv = new CalendarConvertor(SelectedRentalContrat.EndDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedRentalContrat.EndDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }
        
        public ICommand AddNewRenteeCommand
        {
            get { return _addNewRenteeCommand ?? (_addNewRenteeCommand = new RelayCommand(ExcuteAddNewRenteeCommand)); }
        }

        private void ExcuteAddNewRenteeCommand()
        {
            var category = new RenteeEntry();
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                LoadRentees(); //should also get the latest updates in each row
                SelectedRentee = Rentees.FirstOrDefault(c => c.DisplayName == category.TXtCustName.Text);
                if (SelectedRentee != null) SelectedRentalContrat.RenteeId = SelectedRentee.Id;
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