using System;
using System.Linq;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.DAL;
using PinnaRent.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaRent.Core;
using PinnaRent.Core.Models;
using PinnaRent.Repository.Interfaces;
using MessageBox = System.Windows.MessageBox;

namespace PinnaRent.WPF.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        #region Fields
        private static IUnitOfWork _unitOfWork;
        private SettingDTO _currentSetting;
        //private TaxTypes _selectedTaxType;
        private ICommand _saveSettingViewCommand;
        private ICommand _closeWindowCommand;

        #endregion

        #region Constructor
        public SettingViewModel()
        {
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            CheckRoles();
            CurrentSetting = _unitOfWork.Repository<SettingDTO>().Query().Get().FirstOrDefault() ?? new SettingDTO();

            Messenger.Default.Register<object>(this, (message) =>
            {
                MainWindow = message;
            });
        }
        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }
        #endregion

        #region Properties
        private object _mainWindow;
        public object MainWindow
        {
            get { return _mainWindow; }
            set
            {
                _mainWindow = value;
                RaisePropertyChanged<object>(() => MainWindow);
            }
        }
        public SettingDTO CurrentSetting
        {
            get { return _currentSetting; }
            set
            {
                _currentSetting = value;
                RaisePropertyChanged<SettingDTO>(() => CurrentSetting);
                if (CurrentSetting != null)
                {
                }
            }
        }
 
        #endregion

        #region Commands
        public ICommand SaveSettingCommand
        {
            get { return _saveSettingViewCommand ?? (_saveSettingViewCommand = new RelayCommand<Object>(ExecuteSaveSettingViewCommand, CanSave)); }
        }
        private void ExecuteSaveSettingViewCommand(object obj)
        {
            try
            {
                CurrentSetting.DateLastModified = DateTime.Now;
                CurrentSetting.ModifiedByUserId = Singleton.User.UserId;

                _unitOfWork.Repository<SettingDTO>().InsertUpdate(CurrentSetting);
                _unitOfWork.Commit();
               
                MessageBox.Show("Setting Successfully Saved!" + Environment.NewLine +
                                        "The System will be closed, you have to reopen the system to see the changes made!",
                            "Saved Successfully", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                CloseWindow(obj);
                CloseWindow(MainWindow);
            }
            catch
            {
                MessageBox.Show("Can't Save Setting!");
            }
        }
        public ICommand CloseWindowCommand
        {
            get
            {
                return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand<Object>(CloseWindow));
            }
        }
        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as System.Windows.Window;
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
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
