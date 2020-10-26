﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AMActivation.OA;
using PinnaRent.Core.Enumerations;
using PinnaRent.DAL;
using PinnaRent.DAL.Interfaces;
using PinnaRent.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaRent.Core;
using PinnaRent.Core.Models;
using PinnaRent.Repository.Interfaces;
using PinnaRent.WPF.Views;

namespace PinnaRent.WPF.ViewModel
{
    public class ActivationViewModel : ViewModelBase
    {
        #region Fields
        private static EntitiesModel _dbContext;
        private IDbContext dbContext;
        private static IUnitOfWork _unitOfWork;
        private string _productKey, _progressBarVisibility, _biosNo;
        private bool _commandsEnability;
        private ICommand _activateCommand, _closeWindowCommand;
        private ProductActivationDTO _productActivation;
        private const string ConnectionStringName = @"Data Source=DevPc;Initial Catalog=AMActivationDb3;User ID=sa;pwd=amihan; Connect Timeout=2000; Pooling='true'; Max Pool Size=200";
        private object _obj;
        private bool _login;
        #endregion

        #region Constructor
        public ActivationViewModel()
        {
            CleanUp();
            //ConnectionStringName = @"data source=198.38.83.33;initial catalog=ibrahim11_amregkys;user id=ibrahim11_admin1;password=adminP@ssw0rd1";
            _dbContext = new EntitiesModel(ConnectionStringName);
            dbContext = DbContextUtil.GetDbContextInstance();
            _unitOfWork = new UnitOfWork(dbContext);

            ProductActivation = _unitOfWork.Repository<ProductActivationDTO>().Query().Get().FirstOrDefault() ??
                                new ProductActivationDTO();

            ProgressBarVisibility = "Collapsed";
            CommandsEnability = true;
            BiosNo = "Bios No:" + new ProductActivationDTO().BiosSn;
        }
        public static void CleanUp()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }
        #endregion

        #region Properties

        public ProductActivationDTO ProductActivation
        {
            get { return _productActivation; }
            set
            {
                _productActivation = value;
                RaisePropertyChanged<ProductActivationDTO>(() => ProductActivation);
            }
        }

        public string ProductKey
        {
            get { return _productKey; }
            set
            {
                _productKey = value;
                RaisePropertyChanged<string>(() => ProductKey);
            }
        }
        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged<string>(() => ProgressBarVisibility);
            }
        }
        public bool CommandsEnability
        {
            get { return _commandsEnability; }
            set
            {
                _commandsEnability = value;
                RaisePropertyChanged<bool>(() => this.CommandsEnability);
            }
        }
        public string BiosNo
        {
            get { return _biosNo; }
            set
            {
                _biosNo = value;
                RaisePropertyChanged<string>(() => BiosNo);
            }
        }
        #endregion

        #region Commands

        public ICommand ActivateCommand
        {
            get
            {
                return _activateCommand ?? (_activateCommand = new RelayCommand<Object>(ExcuteActivateCommand));
            }
        }
        private void ExcuteActivateCommand(object windowObject)
        {
            _obj = windowObject;
            ProgressBarVisibility = "Visible";
            CommandsEnability = false;
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();

        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarVisibility = "Collapsed";
            CommandsEnability = true;
            if (!_login) return;
            new Login().Show();
            CloseWindow(_obj);
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            ProductActivation.ProductKey = ProductKey;
            var act = _dbContext.ActivationKeys.FirstOrDefault(a => a.ProductKey == ProductActivation.ProductKey
                && a.KeyStatus == 0 && a.ProductType == 3);//O represents Active and 3 represents PinnaRent

            if (act != null)//
            {
                try
                {
                    if (string.IsNullOrEmpty(act.BIOS_SN))//ON PinnaRent Activation Server
                    {
                        act.BIOS_SN = ProductActivation.BiosSn;
                        act.FirstActivatedDate = DateTime.Now; //the time will be better if it is the server timer
                        act.ExpirationDate = act.FirstActivatedDate.Value.AddDays(act.ExpiryDuration);
                    }
                    else
                    {
                        if (!act.BIOS_SN.Contains(ProductActivation.BiosSn))
                        {
                            if (act.NoOfAllowedPcs == 1)
                            {
                                MessageBox.Show(
                                    "Can't Activate the product, check your product key and try again, or contact PinnaRent office!");
                                ProductKey = "";
                                CommandsEnability = true;
                                return;
                            }

                            act.BIOS_SN = act.BIOS_SN + "," + ProductActivation.BiosSn;
                            if (act.BIOS_SN.Split(',').Count() > act.NoOfAllowedPcs)
                            {
                                MessageBox.Show(
                                    "Can't Activate the product, check your product key and try again, or contact PinnaRent office!");
                                ProductKey = "";
                                CommandsEnability = true;
                                return;
                            }
                        }
                    }
                    act.NoOfActivations = act.NoOfActivations + 1;

                    _dbContext.Add(act);
                    _dbContext.SaveChanges();
                    
                    ProductActivation.RegisteredBiosSn = act.BIOS_SN;

                    if (ProductActivation.Id == 0)
                    {
                        ProductActivation.LicensedTo = act.CustomerName;

                        ProductActivation.DateLastModified = DateTime.Now;
                        ProductActivation.ModifiedByUserId = 1;
                        ProductActivation.CreatedByUserId = 1;
                        ProductActivation.DateRecordCreated = DateTime.Now;

                        ProductActivation.ActivatedDate = DateTime.Now;
                        ProductActivation.ExpirationDate = DateTime.Now.AddDays(act.ExpiryDuration);

                        if (Singleton.Edition != PinnaRentEdition.OnlineEdition)
                        {
                            #region Company

                            var company = new CompanyDTO
                            {
                                DisplayName = act.CustomerName,
                                CompanyAddress = new AddressDTO
                                {
                                    Country = "ኢትዮጲያ",
                                    City = "አዲስ አበባ",
                                    Telephone = act.Telephone,
                                    PrimaryEmail = act.Email
                                },
                                Address = new AddressDTO
                                {
                                    Country = "ኢትዮጲያ",
                                    City = "አዲስ አበባ",
                                    Telephone = act.Telephone,
                                    PrimaryEmail = act.Email
                                }
                            };

                            #endregion

                            #region Warehouse

                            var warehouse = new WarehouseDTO
                            {
                                Id = 1,
                                DisplayName = "_Default Store",
                                WarehouseNumber = 11,
                                IsDefault = true,
                                Company = company,
                                Address = new AddressDTO
                                {
                                    Country = "Ethiopia",
                                    City = "Addis Abeba"
                                }
                            };

                            #endregion

                            _unitOfWork.Repository<CompanyDTO>().Insert(company);
                            _unitOfWork.Repository<WarehouseDTO>().Insert(warehouse);
                        }
                        _unitOfWork.Repository<ProductActivationDTO>().Insert(ProductActivation);
                    }
                    else
                    {
                        _unitOfWork.Repository<ProductActivationDTO>().Update(ProductActivation);
                    }

                    _unitOfWork.Commit();

                    Singleton.ProductActivation = ProductActivation;
                    
                    _login = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + Environment.NewLine + " There may be no Internet connection." +
                        Environment.NewLine + "Check your connection and try again." + Environment.NewLine + ex.Message);
                    CommandsEnability = true;
                }
            }
            else
            {
                MessageBox.Show("Can't Activate the product, check your product key and try again, or contact PinnaRent office!");
                ProductKey = "";
                CommandsEnability = true;
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
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion

    }
}
