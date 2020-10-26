using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.Service.Interfaces;
using PinnaRent.WPF.Views;

namespace PinnaRent.WPF.ViewModel
{
    public class ChartofAccountViewModel : ViewModelBase
    {
        #region Fields
        private static IChartofAccountService _chartofAccountService;
        private ChartofAccountDTO _selectedchartofAccount;
        private ObservableCollection<ChartofAccountDTO> _chartofAccounts;
        private string _accountsVisibility;
        private bool _addNewAccountCommandVisibility;
        private ICommand _addNewAccountCommand, _saveAccountCommand, _deleteAccountCommand;
        #endregion

        #region Constructor
        public ChartofAccountViewModel()
        {
            CheckRoles();
            LoadAccountTypes();
            SelectedCompany = new CompanyService(true).GetCompany();

            Load();
        }

        private void Load()
        {
            CleanUp();
            _chartofAccountService = new ChartofAccountService();
            GetChartofAccounts();
        }

        public static void CleanUp()
        {
            if (_chartofAccountService != null)
                _chartofAccountService.Dispose();
        }
        #endregion

        #region Properties

        public string AccountsVisibility
        {
            get { return _accountsVisibility; }
            set
            {
                _accountsVisibility = value;
                RaisePropertyChanged<string>(() => AccountsVisibility);
            }
        }
        public bool AddNewAccountCommandVisibility
        {
            get { return _addNewAccountCommandVisibility; }
            set
            {
                _addNewAccountCommandVisibility = value;
                RaisePropertyChanged<bool>(() => AddNewAccountCommandVisibility);
            }
        }
        public ObservableCollection<ChartofAccountDTO> ChartofAccounts
        {
            get { return _chartofAccounts; }
            set
            {
                _chartofAccounts = value;
                RaisePropertyChanged<ObservableCollection<ChartofAccountDTO>>(() => ChartofAccounts);
                ExecuteAddNewAccountCommand();
            }
        }
        public ChartofAccountDTO SelectedChartofAccount
        {
            get { return _selectedchartofAccount; }
            set
            {
                _selectedchartofAccount = value;
                RaisePropertyChanged<ChartofAccountDTO>(() => SelectedChartofAccount);
                if (SelectedChartofAccount != null)
                {
                    SelectedAccountType = AccountTypes.FirstOrDefault(b => b.Id == SelectedChartofAccount.AccountTypeId);
                }
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewChartofAccountCommand
        {
            get { return _addNewAccountCommand ?? (_addNewAccountCommand = new RelayCommand(ExecuteAddNewAccountCommand)); }
        }
        private void ExecuteAddNewAccountCommand()
        {
            SelectedChartofAccount = new ChartofAccountDTO
            {
                
            };
            if (AccountTypes != null)
            {
                var comercialBank = AccountTypes.FirstOrDefault();
                SelectedAccountType = comercialBank ?? AccountTypes.FirstOrDefault();
            }
            AddNewAccountCommandVisibility = true;
        }

        public ICommand SaveChartofAccountCommand
        {
            get { return _saveAccountCommand ?? (_saveAccountCommand = new RelayCommand(ExecuteSaveAccountCommand, CanSaveLine)); }
        }
        private void ExecuteSaveAccountCommand()
        {
            try
            {
                SelectedChartofAccount.AccountTypeId = SelectedAccountType.Id;

                var stat = _chartofAccountService.InsertOrUpdate(SelectedChartofAccount);

                if (string.IsNullOrEmpty(stat))
                {
                    Load();
                }
                else
                {
                    MessageBox.Show("Got Problem while saving, try again..." + Environment.NewLine + stat, "save error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show("Got Problem while saving, try again..." + Environment.NewLine + exception.Message, "save error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public ICommand DeleteChartofAccountCommand
        {
            get { return _deleteAccountCommand ?? (_deleteAccountCommand = new RelayCommand(ExecuteDeleteAccountCommand, CanSaveLine)); }
        }
        private void ExecuteDeleteAccountCommand()
        {
            if (MessageBox.Show("Are you Sure You want to delete?", "Delete Account",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                SelectedChartofAccount.Enabled = false;
                _chartofAccountService.Disable(SelectedChartofAccount);
                Load();
            }
            catch
            {
                MessageBox.Show("Can't delete the account, may be the account is already in use...", "Can't Delete",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetChartofAccounts()
        {
            var criteria = new SearchCriteria<ChartofAccountDTO>
            {
                CurrentUserId = Singleton.User.UserId
            };


            var chartofAccountsList = _chartofAccountService.GetAll(criteria).OrderByDescending(f => f.Id).ToList();

            var sno = 1;
            foreach (var chartofAccountDTO in chartofAccountsList)
            {
                chartofAccountDTO.SerialNumber = sno;
                sno++;
            }

            ChartofAccounts = new ObservableCollection<ChartofAccountDTO>(chartofAccountsList);
        }
        #endregion

        #region Companys
        private ObservableCollection<CompanyDTO> _companys;
        private CompanyDTO _selectedCompany;

        public ObservableCollection<CompanyDTO> Companys
        {
            get { return _companys; }
            set
            {
                _companys = value;
                RaisePropertyChanged<ObservableCollection<CompanyDTO>>(() => Companys);
            }
        }

        public CompanyDTO SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;
                RaisePropertyChanged<CompanyDTO>(() => SelectedCompany);
                if (SelectedCompany != null)
                {
                    AddNewAccountCommandVisibility = SelectedCompany.Id != -1;

                }
            }
        }



        #endregion

        #region AccountTypes
        private CategoryDTO _selectedAccountType;
        private ObservableCollection<CategoryDTO> _accountTypes;
        private ICommand _addNewBankCommand;

        public CategoryDTO SelectedAccountType
        {
            get { return _selectedAccountType; }
            set
            {
                _selectedAccountType = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedAccountType);
            }
        }
        public ObservableCollection<CategoryDTO> AccountTypes
        {
            get { return _accountTypes; }
            set
            {
                _accountTypes = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => AccountTypes);
            }
        }
        public ICommand AddNewAccountTypeCommand
        {
            get
            {
                return _addNewBankCommand ?? (_addNewBankCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.ChartOfAccount);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadAccountTypes();
                SelectedAccountType = AccountTypes.FirstOrDefault(b => b.DisplayName.ToLower().Contains(category.TxtCategoryName.Text.ToLower()));
            }

        }

        public void LoadAccountTypes()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.ChartOfAccount);

            IEnumerable<CategoryDTO> banksList = new CategoryService(true).GetAll(criteria)
                .OrderBy(i => i.DisplayName)
                .ToList();
            AccountTypes = new ObservableCollection<CategoryDTO>(banksList);
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;

        }

        public static int LineErrors { get; set; }
        public bool CanSaveLine()
        {
            return LineErrors == 0;

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