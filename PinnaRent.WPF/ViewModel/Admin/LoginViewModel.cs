using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PinnaRent.Core.Common;
using PinnaRent.DAL;
using PinnaRent.Repository;
using PinnaRent.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaRent.Core;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.Repository.Interfaces;
using PinnaRent.WPF.Views;
using WebMatrix.WebData;

namespace PinnaRent.WPF.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields
        private static IUnitOfWork _unitOfWork;
        private UserDTO _user;
        private ICommand _loginCommand, _closeLoginView;
        private string _lockedVisibility, _unLockedVisibility;
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            LockedVisibility = "Visible";
            UnLockedVisibility = "Collapsed";
            
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            User = new UserDTO();
        }
        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }
        #endregion

        #region Properties

        public UserDTO User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged<UserDTO>(() => User);
            }
        }
        public string LockedVisibility
        {
            get { return _lockedVisibility; }
            set
            {
                _lockedVisibility = value;
                RaisePropertyChanged<string>(() => LockedVisibility);
            }
        }
        public string UnLockedVisibility
        {
            get { return _unLockedVisibility; }
            set
            {
                _unLockedVisibility = value;
                RaisePropertyChanged<string>(() => UnLockedVisibility);
            }
        }
        
        #endregion

        #region Commands
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand<object>(ExcuteLoginCommand, CanSave));
            }
        }
        private void ExcuteLoginCommand(object obj)
        {
            var values = (object[])obj;
            var psdBox = values[0] as PasswordBox;

            //Do Validation if not handled on the UI
            if (psdBox != null && psdBox.Password == "")
            {
                psdBox.Focus();
                return;
            }

            if (psdBox != null)
            {
                var us = Membership.ValidateUser(User.UserName, psdBox.Password);

                if (!us)
                {
                    MessageBox.Show("IncorrectUserId", "Error Logging",
                                                            MessageBoxButton.OK,
                                                            MessageBoxImage.Error);
                    User.Password = "";
                    return;
                }

                int userId = WebSecurity.GetUserId(User.UserName);
                var user = new UserService().GetUser(userId);

                if (user == null)
                {
                    MessageBox.Show("Incorrect UserId", "Error Logging",
                                                            MessageBoxButton.OK,
                                                            MessageBoxImage.Error);
                    User.Password = "";
                }
                else
                {
                    //LockedVisibility = "Collapsed";
                    //UnLockedVisibility = "Visible";
                    //Thread.Sleep(1000);

                    Singleton.User = user;
                    Singleton.User.Password = psdBox.Password;
                    Singleton.UserRoles = new UserRolesModel();
                    Singleton.Setting = _unitOfWork.Repository<SettingDTO>()
                                                    .Query()
                                                    .Get()
                                                    .FirstOrDefault();

                    #region Warehouse Filter
                    var warehouseList = new WarehouseService(true).GetWarehousesPrevilegedToUser(user.UserId).ToList();

                    if (warehouseList.Count > 1)
                        warehouseList.Insert(warehouseList.Count, new WarehouseDTO
                        {
                            DisplayName = "All",
                            Id = -1
                        });

                    Singleton.WarehousesList = warehouseList;
                    #endregion
              
                    switch (user.Status)
                    {
                        case UserTypes.Waiting:
                            new ChangePassword(psdBox.Password).Show();
                            break;
                        case UserTypes.Active:
                            new MainWindow().Show();
                            break;
                        default:
                            MessageBox.Show("Login Failed");
                            break;
                    }

                    CloseWindow(values[1]);
                }
            }
        }

        public ICommand CloseLoginView
        {
            get
            {
                return _closeLoginView ?? (_closeLoginView = new RelayCommand<Object>(CloseWindow));
            }
        }
        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
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
    }
}
