using System;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaRent.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _headerText, _titleText;
        readonly static DashBoardViewModel DashBoardViewModel = new ViewModelLocator().DashBoard;
        readonly static RoomsViewModel RoomsViewModel = new ViewModelLocator().Rooms;
       
        private ViewModelBase _currentViewModel;

        public MainViewModel()
        {
            CheckRoles();
            TitleText = "PinnaRent V1.0.0.1, Rental Management System - " +
                        Singleton.User.UserName + " - " +
                        DateTime.Now.ToString("dd/MM/yyyy") + " - " +
                        ReportUtility.GetEthCalendar(DateTime.Now, true);
            
            //if (UserRoles.RoomEntry == "Visible")
            //{
                HeaderText = "የሚከራዩ ክፍሎች";
                CurrentViewModel = RoomsViewModel;
            //}
    
            //else if (UserRoles.DashboardMgmt == "Visible")
            //{
            //    HeaderText = "DashBoard";
            //    RenteeAddressViewModel = DashBoardViewModel;
            //}
            

            DashboardViewCommand = new RelayCommand(ExecuteDashboardViewCommand);
            RoomsViewCommand = new RelayCommand(ExecuteRoomsViewCommand);
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
        
        public RelayCommand DashboardViewCommand { get; private set; }
        private void ExecuteDashboardViewCommand()
        {
            HeaderText = "Dashboard";
            CurrentViewModel = DashBoardViewModel;
        }

    
        public RelayCommand RoomsViewCommand { get; private set; }
        private void ExecuteRoomsViewCommand()
        {
            HeaderText = "የሚከራዩ ክፍሎች";
            CurrentViewModel = RoomsViewModel;
        }
        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                if (_headerText == value)
                    return;
                _headerText = value;
                RaisePropertyChanged("HeaderText");
            }
        }

        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                if (_titleText == value)
                    return;
                _titleText = value;
                RaisePropertyChanged("TitleText");
            }
        }

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
            UserRoles.Settings = UserRoles.GeneralSettings == "Visible" ||
                                UserRoles.TaxSettings == "Visible" ||
                                UserRoles.AdvancedSettings == "Visible"
                                    ? "Visible" : "Collapsed";
            UserRoles.Admin = UserRoles.Company == "Visible" ||
                                UserRoles.Settings == "Visible" ||
                                UserRoles.UsersMgmt == "Visible" ||
                                UserRoles.BackupRestore == "Visible"
                                ? "Visible" : "Collapsed";
  
        }

        #endregion
    }
}