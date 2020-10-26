using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PinnaRent.Core;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaRent.WPF.Views;

namespace PinnaRent.WPF.ViewModel
{
    public class CompanyViewModel : ViewModelBase
    {
        #region Fields
        readonly static RepresenteeUCViewModel RepresenteeUC = new ViewModelLocator().RepresenteeUC;
        private ViewModelBase _renteeRepresenteeViewModel;

        private static ICompanyService _companyService;
        private CompanyDTO _selectedCompany;
        private ICommand _saveCompanyViewCommand, _selectedCompanyAddressViewCommand, _selectedCompanyOwnerManagerAddressViewCommand;
        private bool _representeeEnability;

        private CompanyTypes _selectedCompanyType;
        private string _ownerTitleVisibility, _managerVisibility;
        #endregion

        #region Constructor

        public CompanyViewModel()
        {
            CleanUp();
            _companyService = new CompanyService();

            RenteeRepresenteeViewModel = RepresenteeUC;

            LoadTitles();

            SelectedCompany = _companyService.GetCompany() ?? new CompanyDTO()
            {
                CompanyAddress = new AddressDTO()
                {
                    Country = "ኢትዮጲያ",
                    City = "አዲስ አበባ"
                },
                Address = new AddressDTO()
                {
                    Country = "ኢትዮጲያ",
                    City = "አዲስ አበባ"
                }
            };
        }

        public static void CleanUp()
        {
            if (_companyService != null)
                _companyService.Dispose();
        }

        #endregion

        #region Properties
       
        public ViewModelBase RenteeRepresenteeViewModel
        {
            get
            {
                return _renteeRepresenteeViewModel;
            }
            set
            {
                if (_renteeRepresenteeViewModel == value)
                    return;
                _renteeRepresenteeViewModel = value;
                RaisePropertyChanged("RenteeRepresenteeViewModel");
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
                    LetterHeadImage = ImageUtil.ToImage(SelectedCompany.Header);
                    LetterFootImage = ImageUtil.ToImage(SelectedCompany.Footer);
                    
                    RepresenteeEnability = SelectedCompany.Representee != null;
                    SelectedCompanyType = SelectedCompany.Type;

                    if(SelectedCompany.Title!=null && Titles != null)
                        SelectedTitle = Titles.FirstOrDefault(t=>t.Id==SelectedCompany.TitleId);
                    else
                        SelectedTitle = null;
                }
            }
        }

        public bool RepresenteeEnability
        {
            get { return _representeeEnability; }
            set
            {
                _representeeEnability = value;
                RaisePropertyChanged<bool>(() => RepresenteeEnability);
                if (RepresenteeEnability)
                {
                    if (SelectedCompany.Representee != null)
                        RepresenteeUC.SelectedRepresentee = SelectedCompany.Representee;
                    else RepresenteeUC.SelectedRepresentee = new RepresenteeDTO
                    {
                        AuthorizationDate = DateTime.Now,
                        Address = new AddressDTO
                        {
                            Country = "ኢትዮጲያ",
                            City = "አዲስ አበባ"
                        }
                    };
                }
                else
                {
                    RepresenteeUC.SelectedRepresentee = null;
                }
            }
        }

        public string OwnerTitleVisibility
        {
            get { return _ownerTitleVisibility; }
            set
            {
                _ownerTitleVisibility = value;
                RaisePropertyChanged<string>(() => OwnerTitleVisibility);
            }
        }
        public string ManagerVisibility
        {
            get { return _managerVisibility; }
            set
            {
                _managerVisibility = value;
                RaisePropertyChanged<string>(() => ManagerVisibility);
            }
        }
        public CompanyTypes SelectedCompanyType
        {
            get { return _selectedCompanyType; }
            set
            {
                _selectedCompanyType = value;
                RaisePropertyChanged<CompanyTypes>(() => SelectedCompanyType);
                if (SelectedCompanyType == CompanyTypes.Organization)
                {
                    ManagerVisibility = "Visible";
                    OwnerTitleVisibility = "Collapsed";
                }
                else if (SelectedCompanyType == CompanyTypes.Personal)
                {
                    ManagerVisibility = "Collapsed";
                    OwnerTitleVisibility = "Visible";
                }
            }
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
                if (SelectedTitle != null) SelectedCompany.TitleId = SelectedTitle.Id;
            }
        }
        #endregion

        #region Commands
        public ICommand SaveCompanyViewCommand
        {
            get { return _saveCompanyViewCommand ?? (_saveCompanyViewCommand = new RelayCommand<Object>(ExecuteSaveCompanyViewCommand, CanSave)); }
        }
        private void ExecuteSaveCompanyViewCommand(object obj)
        {
            try
            {
                SelectedCompany.Type = SelectedCompanyType;
                SelectedCompany.Representee = RepresenteeUC.SelectedRepresentee;

                if (SelectedTitle != null)
                    SelectedCompany.TitleId = SelectedTitle.Id;

                if (LetterHeadImage.UriSource != null)
                    SelectedCompany.Header = ImageUtil.ToBytes(LetterHeadImage);
                if (LetterFootImage.UriSource != null)
                    SelectedCompany.Footer = ImageUtil.ToBytes(LetterFootImage);

                if (SelectedCompany != null && _companyService.InsertOrUpdate(SelectedCompany) == string.Empty)
                    CloseWindow(obj);
                else
                    MessageBox.Show("Got Problem while saving, try again...", "error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.InnerException.Message + Environment.NewLine + exception.Message, "error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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

        public ICommand SelectedCompanyAddressViewCommand
        {
            get { return _selectedCompanyAddressViewCommand ??(_selectedCompanyAddressViewCommand =new RelayCommand(ExcuteSelectedCompanyAddressViewCommand)); }
        }

        public void ExcuteSelectedCompanyAddressViewCommand()
        {
            SelectedCompany.CompanyAddress = SelectedCompany.CompanyAddress ?? new AddressDTO
            {
                Country = "ኢትዮጲያ",
                City = "አዲስ አበባ"
            };
     
            new AddressEntry(SelectedCompany.CompanyAddress).ShowDialog();
        }


        public ICommand SelectedCompanyOwnerManagerAddressViewCommand
        {
            get { return _selectedCompanyOwnerManagerAddressViewCommand ?? (_selectedCompanyOwnerManagerAddressViewCommand=new RelayCommand(ExcuteSelectedCompanyOwnerManagerAddressViewCommand)); }
        }

        public void ExcuteSelectedCompanyOwnerManagerAddressViewCommand()
        {
            new AddressEntry(SelectedCompany.Address).ShowDialog();
        }

        #endregion

        #region Letter Head
        private BitmapImage _letterHeadImage, _letterFootImage;
        private ICommand _showLetterHeaderImageCommand, _showLetterFooterImageCommand;

        public BitmapImage LetterHeadImage
        {
            get { return _letterHeadImage; }
            set
            {
                _letterHeadImage = value;
                RaisePropertyChanged<BitmapImage>(() => LetterHeadImage);
            }
        }
        public ICommand ShowLetterHeaderImageCommand
        {
            get { return _showLetterHeaderImageCommand ?? (_showLetterHeaderImageCommand = new RelayCommand(ExecuteShowLetterHeaderImageViewCommand)); }
        }
        private void ExecuteShowLetterHeaderImageViewCommand()
        {
            var file = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                LetterHeadImage = new BitmapImage(new Uri(file.FileName, true));// new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
            }
        }

        public BitmapImage LetterFootImage
        {
            get { return _letterFootImage; }
            set
            {
                _letterFootImage = value;
                RaisePropertyChanged<BitmapImage>(() => LetterFootImage);
            }
        }
        public ICommand ShowLetterFooterImageCommand
        {
            get { return _showLetterFooterImageCommand ?? (_showLetterFooterImageCommand = new RelayCommand(ExecuteShowLetterFooterImageViewCommand)); }
        }
        private void ExecuteShowLetterFooterImageViewCommand()
        {
            var file = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                LetterFootImage = new BitmapImage(new Uri(file.FileName, true));// new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
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
