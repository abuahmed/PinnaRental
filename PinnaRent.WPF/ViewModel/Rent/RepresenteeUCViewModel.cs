using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.WPF.Views;

namespace PinnaRent.WPF.ViewModel
{
    public class RepresenteeUCViewModel : ViewModelBase
    {
        #region Fields
        private RepresenteeDTO _selectedRepresentee;
        private ICommand _authorizationDateViewCommand, _representeeAddressViewCommand;

        #endregion

        #region Constructor
        public RepresenteeUCViewModel()
        {
            CheckRoles();
            LoadTitles();
            if (Titles != null) SelectedTitle = Titles.FirstOrDefault();
        }

        #endregion

        #region Public Properties

        public RepresenteeDTO SelectedRepresentee
        {
            get { return _selectedRepresentee; }
            set
            {
                _selectedRepresentee = value;
                RaisePropertyChanged<RepresenteeDTO>(() => SelectedRepresentee);
                if (SelectedRepresentee != null && SelectedRepresentee.Title != null)
                    SelectedTitle = Titles.FirstOrDefault(t => t.Id == SelectedRepresentee.TitleId);
            }
        }

        #endregion

        #region Commands
        
        public ICommand AuthorizationDateViewCommand
        {
            get
            {
                return _authorizationDateViewCommand ??
                       (_authorizationDateViewCommand = new RelayCommand(AuthorizationDate));
            }
        }
        public void AuthorizationDate()
        {
            var calConv = new CalendarConvertor(SelectedRepresentee.AuthorizationDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedRepresentee.AuthorizationDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand RepresenteeAddressViewCommand
        {
            get
            {
                return _representeeAddressViewCommand ??
                       (_representeeAddressViewCommand = new RelayCommand(RepresenteeAddress));
            }
        }
        public void RepresenteeAddress()
        {
            SelectedRepresentee.Address = SelectedRepresentee.Address ?? new AddressDTO
            {
                Country = "ኢትዮጲያ",
                City = "አዲስ አበባ"
            };
            new AddressEntry(SelectedRepresentee.Address).ShowDialog();
        } 
        #endregion
        
        #region Titles
        private ICommand _addNewTitleCommand;
        private ObservableCollection<CategoryDTO> _titles;
        private CategoryDTO _selectedTitle;

        public void LoadTitles()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.TitleType);
            IEnumerable<CategoryDTO> titleList = new CategoryService(true)
                .GetAll(criteria)
                .ToList();

            Titles = new ObservableCollection<CategoryDTO>(titleList);
        }

        public CategoryDTO SelectedTitle
        {
            get { return _selectedTitle; }
            set
            {
                _selectedTitle = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedTitle);
                if (SelectedTitle != null)
                    if (SelectedRepresentee != null) SelectedRepresentee.TitleId = SelectedTitle.Id;
            }
        }
        public ObservableCollection<CategoryDTO> Titles
        {
            get { return _titles; }
            set
            {
                _titles = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => Titles);
                
            }
        }
        public ICommand AddNewTitleCommand
        {
            get
            {
                return _addNewTitleCommand ?? (_addNewTitleCommand = new RelayCommand(ExcuteAddNewTitleCommand));
            }
        }
        private void ExcuteAddNewTitleCommand()
        {
            var category = new Categories(NameTypes.TitleType);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadTitles();//should also get the latest updates in each row
                SelectedTitle = Titles.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedTitle != null) SelectedRepresentee.TitleId = SelectedTitle.Id;
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
        }

        #endregion
    }
}