using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.WPF.Views;

namespace PinnaRent.WPF.ViewModel
{
    public class CheckEntryViewModel : ViewModelBase
    {
        #region Fields
        private static IUnitOfWork _unitOfWork;
        private CheckDTO _selectedCheck;
        private ICommand _addCheckCommand;
        #endregion
        
        #region Constructor
        public CheckEntryViewModel()
        {
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());

            LoadAcounts();
            
            Messenger.Default.Register<CheckDTO>(this, (message) =>
            {
                SelectedCheck = message;
            });
        }
        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }
        #endregion

        #region Properties

        
        public CheckDTO SelectedCheck
        {
            get { return _selectedCheck; }
            set
            {
                _selectedCheck = value;
                RaisePropertyChanged<CheckDTO>(() => SelectedCheck);
                if (SelectedCheck != null)
                {
                    SelectedBankAccount = BankAcounts.FirstOrDefault(a => a.Id == SelectedCheck.BankAccountId) ??
                                          BankAcounts.FirstOrDefault();
                    //CheckDateMax = SelectedCheck.CheckDueDate;
                }
            }
        }

        //public DateTime CheckDateMax
        //{
        //    get { return _checkDateMax; }
        //    set
        //    {
        //        _checkDateMax = value;
        //        RaisePropertyChanged<DateTime>(() => CheckDateMax);
        //    }
        //}
        #endregion

        #region BankAcounts
        private BankAccountDTO _selectedBankAccount;
        private ObservableCollection<BankAccountDTO> _clientAccounts;
        public void LoadAcounts()
        {
            IEnumerable<BankAccountDTO> categoriesList = _unitOfWork.Repository<BankAccountDTO>()
                .Query()
                .Get()
                .OrderBy(i => i.Id).ToList();

            BankAcounts = new ObservableCollection<BankAccountDTO>(categoriesList);

            if (BankAcounts.Any())
                SelectedBankAccount = BankAcounts.FirstOrDefault();
        }

        public BankAccountDTO SelectedBankAccount
        {
            get { return _selectedBankAccount; }
            set
            {
                _selectedBankAccount = value;
                RaisePropertyChanged<BankAccountDTO>(() => SelectedBankAccount);
            }
        }
        public ObservableCollection<BankAccountDTO> BankAcounts
        {
            get { return _clientAccounts; }
            set
            {
                _clientAccounts = value;
                RaisePropertyChanged<ObservableCollection<BankAccountDTO>>(() => BankAcounts);
            }
        }
        #endregion


        #region Commands
        public ICommand AddCheckCommand
        {
            get { return _addCheckCommand ?? (_addCheckCommand = new RelayCommand<object>(ExecuteAddCheckCommand, CanSave)); }
        }
        private void ExecuteAddCheckCommand(object obj)
        {
            try
            {
                SelectedCheck.BankAccountId = SelectedBankAccount.Id;
                
                CloseWindow(obj);
            }
            catch
            {
                MessageBox.Show("Got problem while getting check!", "Check Problem");
            }
        }
        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.DialogResult = true;
            window.Close();
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