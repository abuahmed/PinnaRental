using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
    public class RenteeEntryViewModel : ViewModelBase
    {
        #region Fields
        readonly static RepresenteeUCViewModel RepresenteeUC = new ViewModelLocator().RepresenteeUC;
        private ViewModelBase _renteeRepresenteeViewModel;

        private static IRenteeService _renteeService;
        private IEnumerable<RenteeDTO> _rentees;
        private ObservableCollection<RenteeDTO> _filteredRentees;
        private RenteeDTO _selectedRentee;
        private RenteeTypes _selectedRenteeType;

        private ICommand _addNewRenteeViewCommand,
            _saveRenteeViewCommand,
            _deleteRenteeViewCommand,
            _renteeAddressViewCommand, _contactPersonAddressViewCommand;
        private string _renteeText, _ownerTitleVisibility, _managerVisibility, _searchText;
        private bool _representeeEnability;
        #endregion

        #region Constructor
        public RenteeEntryViewModel()
        {
            CleanUp();
            _renteeService = new RenteeService();

            CheckRoles();

            RenteeRepresenteeViewModel = RepresenteeUC;

            LoadTitles();
            GetLiveRentees();

            RenteeText = "ተከራይ ማስገቢያ";
        }
        public static void CleanUp()
        {
            if (_renteeService != null)
                _renteeService.Dispose();
        }
        #endregion

        #region Public Properties
     
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
        
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged<string>(() => SearchText);
                if (RenteeList != null)
                {
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        Rentees = new ObservableCollection<RenteeDTO>
                            (RenteeList.Where(c => c.RenteeDetail.ToLower().Contains(value.ToLower()))
                                .ToList());
                    }
                    else
                        Rentees = new ObservableCollection<RenteeDTO>(RenteeList);
                }
                
            }
        }
        public string RenteeText
        {
            get { return _renteeText; }
            set
            {
                _renteeText = value;
                RaisePropertyChanged<string>(() => RenteeText);
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

        public RenteeDTO SelectedRentee
        {
            get { return _selectedRentee; }
            set
            {
                _selectedRentee = value;
                RaisePropertyChanged<RenteeDTO>(() => SelectedRentee);

                if (SelectedRentee != null)
                {
                    RepresenteeEnability = SelectedRentee.Representee != null;
                    SelectedRenteeType = SelectedRentee.Type;

                    if (SelectedRentee.Title != null && Titles != null)
                        SelectedTitle = Titles.FirstOrDefault(t => t.Id == SelectedRentee.TitleId);
                    else
                        SelectedTitle = null;
                    
                }
            }
        }
        public IEnumerable<RenteeDTO> RenteeList
        {
            get { return _rentees; }
            set
            {
                _rentees = value;
                RaisePropertyChanged<IEnumerable<RenteeDTO>>(() => RenteeList);
            }
        }
        public ObservableCollection<RenteeDTO> Rentees
        {
            get { return _filteredRentees; }
            set
            {
                _filteredRentees = value;
                RaisePropertyChanged<ObservableCollection<RenteeDTO>>(() => Rentees);

                //if (Rentees != null && Rentees.Any())
                //    SelectedRentee = Rentees.FirstOrDefault();
                //else
                    AddNewRentee();
            }
        }
        public RenteeTypes SelectedRenteeType
        {
            get { return _selectedRenteeType; }
            set
            {
                _selectedRenteeType = value;
                RaisePropertyChanged<RenteeTypes>(() => SelectedRenteeType);
                if (SelectedRenteeType == RenteeTypes.Organization)
                {
                    ManagerVisibility = "Visible";
                    OwnerTitleVisibility = "Collapsed";
                }
                else if (SelectedRenteeType == RenteeTypes.Person)
                {
                    ManagerVisibility = "Collapsed";
                    OwnerTitleVisibility = "Visible";
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
                    if (SelectedRentee.Representee != null)
                        RepresenteeUC.SelectedRepresentee = SelectedRentee.Representee;
                    else RepresenteeUC.SelectedRepresentee = new RepresenteeDTO()
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
        #endregion

        #region Filter List
        private List<ListDataItem> _renteeTypeList;
        private ListDataItem _selectedRenteeTypeFilter;

        public List<ListDataItem> RenteeTypeList
        {
            get { return _renteeTypeList; }
            set
            {
                _renteeTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => RenteeTypeList);
            }
        }
        public ListDataItem SelectedRenteeTypeFilter
        {
            get { return _selectedRenteeTypeFilter; }
            set
            {
                _selectedRenteeTypeFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedRenteeTypeFilter);
            }
        }

        private List<ListDataItem> _renteeTypeListForFilter;
        private ListDataItem _selectedRenteeTypeForFilter;

        public List<ListDataItem> RenteeTypeListForFilter
        {
            get { return _renteeTypeListForFilter; }
            set
            {
                _renteeTypeListForFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => RenteeTypeListForFilter);
            }
        }
        public ListDataItem SelectedRenteeTypeForFilter
        {
            get { return _selectedRenteeTypeForFilter; }
            set
            {
                _selectedRenteeTypeForFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedRenteeTypeForFilter);
                GetLiveRentees();
            }
        }

        //public void FillRenteeTypes()
        //{
        //    var renteeTypes = (List<ListDataItem>)CommonUtility.GetList(typeof(RenteeTypes));

        //    RenteeTypeListForFilter = renteeTypes.ToList();
        //    SelectedRenteeTypeForFilter = RenteeTypeListForFilter.FirstOrDefault();

        //    if (renteeTypes != null && renteeTypes.Count > 1)
        //    {
        //        RenteeTypeList = renteeTypes.Skip(1).ToList();
        //        SelectedCompanyType = RenteeTypeList.FirstOrDefault();
        //    }
        //}


        #endregion

        #region Commands
        public ICommand AddNewRenteeViewCommand
        {
            get { return _addNewRenteeViewCommand ?? (_addNewRenteeViewCommand = new RelayCommand(AddNewRentee)); }
        }
        private void AddNewRentee()
        {
            try
            {
                SelectedRentee = new RenteeDTO
                {
                    Type = RenteeTypes.Organization,
                    Address = new AddressDTO
                    {
                        Country = "ኢትዮጲያዊ",
                        Region = "አዲስ አበባ",
                        City = "አዲስ አበባ"
                    },
                };
                
            }
            catch (Exception)
            {
                //MessageBox.Show("Problem on adding new Rentee");
            }
        }

        public ICommand SaveRenteeViewCommand
        {
            get { return _saveRenteeViewCommand ?? (_saveRenteeViewCommand = new RelayCommand<Object>(SaveRentee, CanSave)); }
        }
        private void SaveRentee(object obj)
        {
            try
            {
                SelectedRentee.Type = SelectedRenteeType;
                SelectedRentee.Representee = RepresenteeUC.SelectedRepresentee;

                if (SelectedTitle != null)
                    SelectedRentee.TitleId = SelectedTitle.Id;

                var stat = _renteeService.InsertOrUpdate(SelectedRentee);

                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else
                {
                    CloseWindow(obj);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        public ICommand DeleteRenteeViewCommand
        {
            get { return _deleteRenteeViewCommand ?? (_deleteRenteeViewCommand = new RelayCommand<Object>(DeleteRentee, CanSave)); }
        }
        private void DeleteRentee(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Rentee?", "Delete Rentee", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    //SelectedRentee.Enabled = false;
                    var stat = _renteeService.Delete(SelectedRentee.Id.ToString());
                    if (stat == 0)
                    {
                        Rentees.Remove(SelectedRentee);
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
                                    + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ICommand RenteeAddressViewCommand
        {
            get { return _renteeAddressViewCommand ?? (_renteeAddressViewCommand = new RelayCommand(RenteeAddress)); }
        }
        public void RenteeAddress()
        {
            new AddressEntry(SelectedRentee.Address).ShowDialog();
        }

        public ObservableCollection<AddressDTO> ContactPersonAdressDetail
        {
            get { return _contactPersonAddressDetail; }
            set
            {
                _contactPersonAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ContactPersonAdressDetail);
            }
        }

        public ICommand ContactPersonAddressViewCommand
        {
            get { return _contactPersonAddressViewCommand ?? (_contactPersonAddressViewCommand = new RelayCommand(ContactPersonAddress)); }
        }

        public void ContactPersonAddress()
        {
            new AddressEntry(SelectedRentee.Address).ShowDialog();
        }

        public void CloseWindow(object obj)
        {
            try
            {
                if (obj == null) return;
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch { }
        }
        #endregion

        public void GetLiveRentees()
        {
            var criteria = new SearchCriteria<RenteeDTO>();

            RenteeList = _renteeService.GetAll(criteria).OrderBy(i => i.Id).ToList();
            var sNo = 1;
            foreach (var renteeDTO in RenteeList)
            {
                renteeDTO.SerialNumber = sNo;
                sNo++;
            }
            Rentees = new ObservableCollection<RenteeDTO>(RenteeList);
        }

        #region Titles
        private ICommand  _addNewTitleCommand;
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
                if (SelectedTitle != null) SelectedRentee.TitleId = SelectedTitle.Id;
            }
        }
        #endregion

        #region Print List
        private ICommand _printListCommandView;

        public ICommand PrintListCommandView
        {
            get
            {
                return _printListCommandView ?? (_printListCommandView = new RelayCommand<Object>(PrintList));
            }
        }
        public void PrintList(object obj)
        {

            //var myReport4 = new PinnaRent.WPF.Reports.RenteeID();
            //myReport4.SetDataSource(GetAttachmentDataSet());

            ////MenuItem menu = obj as MenuItem;
            ////if (menu != null)
            ////    new ReportUtility().DirectPrinter(myReport4);
            ////else
            ////{
            //var report = new ReportViewerCommon(myReport4);
            //report.ShowDialog();
            ////}
        }
        //public FitnessDataSet GetAttachmentDataSet()
        //{
        //    var myDataSet = new FitnessDataSet();

        //    //var brCode = new BarcodeProcess();
        //    //var renteeNumberbarcode = ImageToByteArray(brCode.GetBarcode(SelectedRentee.Id.ToString() + "_ABC850", 620, 50, false), ImageFormat.Bmp);

        //    //var renteeSub = RenteeSubscriptionDetail.FirstOrDefault();
        //    ////if (SelectedRentee.LastSubscription != null)
        //    //myDataSet.RenteeDetail.Rows.Add(
        //    //    SelectedRentee.DisplayName,
        //    //    SelectedRentee.ShortPhoto,
        //    //    SelectedRentee.Number,
        //    //    renteeNumberbarcode,
        //    //    SelectedRentee.Sex,
        //    //    SelectedRentee.Address.AddressDetail,
        //    //    "", "", "", "",
        //    //    SelectedRentee.Address.Mobile,
        //    //    "", "", "", "",
        //    //    renteeSub != null ? renteeSub.SubscribedDateStringAndAmharic : null,
        //    //     renteeSub != null ? renteeSub.EndDateString : null,
        //    //     renteeSub != null ? renteeSub.FacilitySubscription.PackageName : null,
        //    //    "");

        //    ////SelectedRentee.LastSubscription.SubscribedDateStringAndAmharic,
        //    ////        SelectedRentee.LastSubscription.EndDateString,
        //    ////        SelectedRentee.LastSubscription.FacilitySubscription.PackageName,

        //    return myDataSet;
        //}
        //public byte[] ImageToByteArray(Image imageIn, ImageFormat format)
        //{
        //    var ms = new MemoryStream();
        //    imageIn.Save(ms, format);
        //    return ms.ToArray();
        //}

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
        private ObservableCollection<AddressDTO> _contactPersonAddressDetail;

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