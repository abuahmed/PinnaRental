using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.Service.Interfaces;
using PinnaRent.WPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaRent.WPF.ViewModel
{
    public class BankAccountViewModel : ViewModelBase
    {
        #region Fields
        private static IBankAccountService _bankAccountService;
        private BankAccountDTO _selectedbankAccount;
        private ObservableCollection<BankAccountDTO> _bankAccounts;
        private string _accountsVisibility;
        private bool _addNewAccountCommandVisibility;
        private ICommand _addNewAccountCommand, _saveAccountCommand, _deleteAccountCommand;
        #endregion

        #region Constructor
        public BankAccountViewModel()
        {
            CheckRoles();
            LoadBanks();
            SelectedCompany = new CompanyService(true).GetCompany();

            Load();
        }

        private void Load()
        {
            CleanUp();
            _bankAccountService = new BankAccountService();
            GetBankAccounts();
        }

        public static void CleanUp()
        {
            if (_bankAccountService != null)
                _bankAccountService.Dispose();
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
        public ObservableCollection<BankAccountDTO> BankAccounts
        {
            get { return _bankAccounts; }
            set
            {
                _bankAccounts = value;
                RaisePropertyChanged<ObservableCollection<BankAccountDTO>>(() => BankAccounts);
                ExecuteAddNewAccountCommand();
            }
        }
        public BankAccountDTO SelectedBankAccount
        {
            get { return _selectedbankAccount; }
            set
            {
                _selectedbankAccount = value;
                RaisePropertyChanged<BankAccountDTO>(() => SelectedBankAccount);
                if (SelectedBankAccount != null)
                {
                    SelectedBank = Banks.FirstOrDefault(b => b.DisplayName == SelectedBankAccount.BankName);
                }
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewBankAccountCommand
        {
            get { return _addNewAccountCommand ?? (_addNewAccountCommand = new RelayCommand(ExecuteAddNewAccountCommand)); }
        }
        private void ExecuteAddNewAccountCommand()
        {
            SelectedBankAccount = new BankAccountDTO
            {
                CompanyId = SelectedCompany.Id
            };
            if (Banks != null)
            {
                var comercialBank = Banks.FirstOrDefault(b => b.DisplayName.ToLower().Contains("commercial"));
                SelectedBank = comercialBank ?? Banks.FirstOrDefault();
            }
            AddNewAccountCommandVisibility = true;
        }

        public ICommand SaveBankAccountCommand
        {
            get { return _saveAccountCommand ?? (_saveAccountCommand = new RelayCommand(ExecuteSaveAccountCommand, CanSaveLine)); }
        }
        private void ExecuteSaveAccountCommand()
        {
            try
            {
                SelectedBankAccount.BankName = SelectedBank.DisplayName;

                var stat = _bankAccountService.InsertOrUpdate(SelectedBankAccount);

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

        public ICommand DeleteBankAccountCommand
        {
            get { return _deleteAccountCommand ?? (_deleteAccountCommand = new RelayCommand(ExecuteDeleteAccountCommand, CanSaveLine)); }
        }
        private void ExecuteDeleteAccountCommand()
        {
            if (MessageBox.Show("Are you Sure You want to delete?", "Delete Account",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                SelectedBankAccount.Enabled = false;
                _bankAccountService.Disable(SelectedBankAccount);
                Load();
            }
            catch
            {
                MessageBox.Show("Can't delete the account, may be the account is already in use...", "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetBankAccounts()
        {
            var criteria = new SearchCriteria<BankAccountDTO>
            {
                CurrentUserId = Singleton.User.UserId
            };
            

            var bankAccountsList = _bankAccountService.GetAll(criteria).OrderByDescending(f => f.Id).ToList();

            var sno = 1;
            foreach (var bankAccountDTO in bankAccountsList)
            {
                bankAccountDTO.SerialNumber = sno;
                sno++;
            }

            BankAccounts = new ObservableCollection<BankAccountDTO>(bankAccountsList);
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
        private CategoryDTO _selectedBank;
        private ObservableCollection<CategoryDTO> _banks;
        private ICommand _addNewBankCommand;

        public CategoryDTO SelectedBank
        {
            get { return _selectedBank; }
            set
            {
                _selectedBank = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedBank);
            }
        }
        public ObservableCollection<CategoryDTO> Banks
        {
            get { return _banks; }
            set
            {
                _banks = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => Banks);
            }
        }
        public ICommand AddNewBankCommand
        {
            get
            {
                return _addNewBankCommand ?? (_addNewBankCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.Bank);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadBanks();
                SelectedBank = Banks.FirstOrDefault(b => b.DisplayName.ToLower().Contains(category.TxtCategoryName.Text.ToLower()));
            }

        }

        public void LoadBanks()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.Bank);

            IEnumerable<CategoryDTO> banksList = new CategoryService(true).GetAll(criteria)
                .OrderBy(i => i.DisplayName)
                .ToList();
            Banks = new ObservableCollection<CategoryDTO>(banksList);
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