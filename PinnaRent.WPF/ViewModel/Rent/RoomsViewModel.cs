#region

//using PinnaRent.WPF.Reports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Extensions;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.Service.Interfaces;
using PinnaRent.WPF.Reports;
using PinnaRent.WPF.Views;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class RoomsViewModel : ViewModelBase
    {
        #region Fields

        private static IRoomService _roomService;
        private ICommand _refreshWindowCommand, _itemViewCommand;
        private string _subTotalValueOfRooms, _totalTaxOfRooms, _totalValueOfRooms;
        private string _searchText, _totalNumberOfRooms, _servicePaymentVisibility, _depositVisibility;
        private bool _itemDetailEnability;
        private bool _loadData;

        #endregion

        #region Constructor

        public RoomsViewModel()
        {
            Load();
        }

        private void Load()
        {
            CleanUp();
            _roomService = new RoomService();

            LoadCategories();
            ItemDetailEnability = false;
            ProgressBarVisibility = "Collapsed";
            PaymentHistoryVisibility = "Collapsed";
            DepositVisibility = "Collapsed";

            GetRooms();
            RentalPaymentHistory = new ObservableCollection<RentalPaymentDTO>();

            CheckRoles();
        }

        public static void CleanUp()
        {
            if (_roomService != null)
                _roomService.Dispose();
        }

        public bool LoadData
        {
            get { return _loadData; }
            set
            {
                _loadData = value;
                RaisePropertyChanged<bool>(() => LoadData);
                if (LoadData)
                    Load();
            }
        }

        #endregion

        #region Properties

        public string TotalNumberOfRooms
        {
            get { return _totalNumberOfRooms; }
            set
            {
                _totalNumberOfRooms = value;
                RaisePropertyChanged<string>(() => TotalNumberOfRooms);
            }
        }

        public string SubTotalValueOfRooms
        {
            get { return _subTotalValueOfRooms; }
            set
            {
                _subTotalValueOfRooms = value;
                RaisePropertyChanged<string>(() => SubTotalValueOfRooms);
            }
        }

        public string TotalValueOfRooms
        {
            get { return _totalValueOfRooms; }
            set
            {
                _totalValueOfRooms = value;
                RaisePropertyChanged<string>(() => TotalValueOfRooms);
            }
        }

        public string TotalTaxOfRooms
        {
            get { return _totalTaxOfRooms; }
            set
            {
                _totalTaxOfRooms = value;
                RaisePropertyChanged<string>(() => TotalTaxOfRooms);
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged<string>(() => SearchText);
                if (RoomList != null)
                {
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        Rooms = new ObservableCollection<RoomDTO>
                            (RoomList.Where(c => c.Number.ToLower().Contains(value.ToLower()))
                                .ToList());
                    }
                    else
                        Rooms = new ObservableCollection<RoomDTO>(RoomList);
                }
                SetSummary();
            }
        }

        public bool ItemDetailEnability
        {
            get { return _itemDetailEnability; }
            set
            {
                _itemDetailEnability = value;
                RaisePropertyChanged<bool>(() => ItemDetailEnability);
            }
        }

        public string ServicePaymentVisibility
        {
            get { return _servicePaymentVisibility; }
            set
            {
                _servicePaymentVisibility = value;
                RaisePropertyChanged<string>(() => ServicePaymentVisibility);
            }
        }

        //public string ServicePaymentVisibility
        //{
        //    get { return _servicePaymentVisibility; }
        //    set
        //    {
        //        _servicePaymentVisibility = value;
        //        RaisePropertyChanged<string>(() => ServicePaymentVisibility);
        //    }
        //}
        public string DepositVisibility
        {
            get { return _depositVisibility; }
            set
            {
                _depositVisibility = value;
                RaisePropertyChanged<string>(() => DepositVisibility);
            }
        }

        #endregion

        #region Categories

        private CategoryDTO _selectedCategory;
        private ObservableCollection<CategoryDTO> _categories;

        public void LoadCategories()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.FloorCategory);

            IList<CategoryDTO> categoriesList = new CategoryService(true)
                .GetAll(criteria)
                .OrderBy(i => i.Id)
                .ToList();

            if (categoriesList.Count > 1)
                categoriesList.Insert(0, new CategoryDTO
                {
                    DisplayName = "All",
                    Id = -1
                });

            Categories = new ObservableCollection<CategoryDTO>(categoriesList);
        }

        public CategoryDTO SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedCategory);

                if (SelectedCategory == null) return;
                GetRooms();
                //if (SelectedCategory.DisplayName == "All")
                //{
                //    GetRooms();
                //    return;
                //}

                //try
                //{
                //    if (RoomList != null)
                //    {
                //        RoomList = RoomList
                //            .Where(iq => iq.Floor.DisplayName == SelectedCategory.DisplayName);
                //        Rooms = new ObservableCollection<RoomDTO>(RoomList);
                //    }
                //}
                //catch (Exception exception)
                //{
                //    MessageBox.Show("Can't Filter By Category"
                //                    + Environment.NewLine + exception.Message, "Can't Filter By Category",
                //        MessageBoxButton.OK,
                //        MessageBoxImage.Error);
                //}
            }
        }

        public ObservableCollection<CategoryDTO> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => Categories);
            }
        }

        #endregion

        #region Filter by DaysLeft

        #region Public Properties

        private bool _allChecked, _activeChecked, _warningChecked, _expiredChecked, _emptyChecked;

        public bool AllChecked
        {
            get { return _allChecked; }
            set
            {
                _allChecked = value;
                RaisePropertyChanged<bool>(() => AllChecked);
                FilterByDaysLeft();
            }
        }

        public bool ActiveChecked
        {
            get { return _activeChecked; }
            set
            {
                _activeChecked = value;
                RaisePropertyChanged<bool>(() => ActiveChecked);
                FilterByDaysLeft();
            }
        }

        public bool WarningChecked
        {
            get { return _warningChecked; }
            set
            {
                _warningChecked = value;
                RaisePropertyChanged<bool>(() => WarningChecked);
                FilterByDaysLeft();
            }
        }

        public bool ExpiredChecked
        {
            get { return _expiredChecked; }
            set
            {
                _expiredChecked = value;
                RaisePropertyChanged<bool>(() => ExpiredChecked);
                FilterByDaysLeft();
            }
        }

        public bool EmptyChecked
        {
            get { return _emptyChecked; }
            set
            {
                _emptyChecked = value;
                RaisePropertyChanged<bool>(() => EmptyChecked);
                FilterByDaysLeft();
            }
        }

        #endregion

        public void FilterByDaysLeft()
        {
            GetRooms();
        }

        #endregion

        #region Payment/Remarks History

        private ObservableCollection<RentalPaymentDTO> _rentalPaymentHistory;
        private RentalPaymentDTO _selectedRentalPaymentHistory;
        private ObservableCollection<RentalPaymentRemarkDTO> _rentalPaymentRemarks;
        private RentalPaymentRemarkDTO _selectedRentalPaymentRemark;
        private bool _paymentDetailEnability;

        public ObservableCollection<RentalPaymentDTO> RentalPaymentHistory
        {
            get { return _rentalPaymentHistory; }
            set
            {
                _rentalPaymentHistory = value;
                RaisePropertyChanged<ObservableCollection<RentalPaymentDTO>>(() => RentalPaymentHistory);
            }
        }

        public RentalPaymentDTO SelectedRentalPaymentHistory
        {
            get { return _selectedRentalPaymentHistory; }
            set
            {
                _selectedRentalPaymentHistory = value;
                RaisePropertyChanged<RentalPaymentDTO>(() => SelectedRentalPaymentHistory);

                PaymentDetailEnability = SelectedRentalPaymentHistory != null;
            }
        }

        public void ShowRentalPaymentsHistory()
        {
            if (SelectedRoom != null)
            {
                var cri = new SearchCriteria<RentalPaymentDTO>
                {
                    CurrentUserId = Singleton.User.UserId
                };

                cri.FiList.Add(
                    r =>
                        r.Contrat.RoomId == SelectedRoom.Id &&
                        (r.Type == PaymentTypes.Rent || r.Type == PaymentTypes.Service));
                cri.FiList.Add(r => r.Id != SelectedRoom.LastRentalPaymentId);
                var rentalpays = new RentalPaymentService(true).GetAll(cri).ToList();
                var sNo = 1;
                foreach (var rentalPaymentDTO in rentalpays)
                {
                    rentalPaymentDTO.SerialNumber = sNo;
                    sNo++;
                }
                RentalPaymentHistory = new ObservableCollection<RentalPaymentDTO>(rentalpays);
            }
        }

        public bool PaymentDetailEnability
        {
            get { return _paymentDetailEnability; }
            set
            {
                _paymentDetailEnability = value;
                RaisePropertyChanged<bool>(() => PaymentDetailEnability);
            }
        }

        public ObservableCollection<RentalPaymentRemarkDTO> RentalPaymentRemarks
        {
            get { return _rentalPaymentRemarks; }
            set
            {
                _rentalPaymentRemarks = value;
                RaisePropertyChanged<ObservableCollection<RentalPaymentRemarkDTO>>(() => RentalPaymentRemarks);
            }
        }

        public RentalPaymentRemarkDTO SelectedRentalPaymentRemark
        {
            get { return _selectedRentalPaymentRemark; }
            set
            {
                _selectedRentalPaymentRemark = value;
                RaisePropertyChanged<RentalPaymentRemarkDTO>(() => SelectedRentalPaymentRemark);
            }
        }

        public void ShowRentalPaymentRemarks()
        {
            if (SelectedRoom != null)
            {
                var cri = new SearchCriteria<RentalPaymentRemarkDTO>
                {
                    CurrentUserId = Singleton.User.UserId
                };

                cri.FiList.Add(r => r.RentalPaymentId == SelectedRoom.LastRentalPaymentId);

                var rentalPaymentRemarkDtos = new RentalPaymentRemarkService(true).GetAll(cri).ToList();
                var sNo = 1;
                foreach (var rentalPaymentRemarkDTO in rentalPaymentRemarkDtos)
                {
                    rentalPaymentRemarkDTO.SerialNumber = sNo;
                    sNo++;
                }
                RentalPaymentRemarks = new ObservableCollection<RentalPaymentRemarkDTO>(rentalPaymentRemarkDtos);
            }
        }

        #endregion

        #region Rooms

        #region Fields

        private RoomDTO _selectedRoom;
        private RoomDTO _selectedRoomForSearch;
        private IEnumerable<RoomDTO> _roomList, _roomLi;
        private ObservableCollection<RoomDTO> _rooms;
        private string _progressBarVisibility, _paymentHistoryVisibility, _paymentAddEditVisibility;

        #endregion

        #region Public Properties

        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged<string>(() => ProgressBarVisibility);
            }
        }

        public string PaymentHistoryVisibility
        {
            get { return _paymentHistoryVisibility; }
            set
            {
                _paymentHistoryVisibility = value;
                RaisePropertyChanged<string>(() => PaymentHistoryVisibility);
            }
        }

        public string PaymentAddEditVisibility
        {
            get { return _paymentAddEditVisibility; }
            set
            {
                _paymentAddEditVisibility = value;
                RaisePropertyChanged<string>(() => PaymentAddEditVisibility);
            }
        }

        public RoomDTO SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                RaisePropertyChanged<RoomDTO>(() => SelectedRoom);
                if (SelectedRoom != null)
                {
                    ItemDetailEnability = SelectedRoom != null;
                    PaymentHistoryVisibility = SelectedRoom != null ? "Visible" : "Collapsed";
                    ServicePaymentVisibility = SelectedRoom != null && SelectedRoom.ServiceFee > 0
                        ? "Visible"
                        : "Collapsed";
                    DepositVisibility = SelectedRoom != null && SelectedRoom.LastRentalPaymentId > 0
                        ? "Visible"
                        : "Collapsed";

                    if (SelectedRoom.LastRentalPaymentId > 0)
                        PaymentAddEditVisibility = UserRoles.PaymentEdit == "Visible" ? "Visible" : "Collapsed";
                    else
                        PaymentAddEditVisibility = "Visible";


                    ShowRentalPaymentsHistory();
                    ShowRentalPaymentRemarks();
                }
            }
        }

        public RoomDTO SelectedRoomForSearch
        {
            get { return _selectedRoomForSearch; }
            set
            {
                _selectedRoomForSearch = value;
                RaisePropertyChanged<RoomDTO>(() => SelectedRoomForSearch);

                if (SelectedRoomForSearch != null &&
                    !string.IsNullOrEmpty(SelectedRoomForSearch.Number))
                {
                    SelectedRoom = SelectedRoomForSearch;
                    SelectedRoomForSearch.Number = "";
                }
            }
        }

        public ObservableCollection<RoomDTO> Rooms
        {
            get { return _rooms; }
            set
            {
                _rooms = value;
                RaisePropertyChanged<ObservableCollection<RoomDTO>>(() => Rooms);

                SetSummary();
            }
        }

        public IEnumerable<RoomDTO> RoomList
        {
            get { return _roomList; }
            set
            {
                _roomList = value;
                RaisePropertyChanged<IEnumerable<RoomDTO>>(() => RoomList);
            }
        }

        public IEnumerable<RoomDTO> RoomLi
        {
            get { return _roomLi; }
            set
            {
                _roomLi = value;
                RaisePropertyChanged<IEnumerable<RoomDTO>>(() => RoomLi);
            }
        }

        #endregion

        //public void GetRooms()
        //{
        //    ProgressBarVisibility = "Visible";

        //    var worker = new BackgroundWorker();
        //    worker.DoWork += DoWork;
        //    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        //    worker.RunWorkerAsync();
        //}

        //private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    try
        //    {
        //        ProgressBarVisibility = "Collapsed";
        //        Rooms = new ObservableCollection<RoomDTO>(RoomLi.ToList().OrderBy(i => i.FloorId).ThenBy(i => i.Number));
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        GetRoomsBk();
        //    }
        //    catch
        //    {
        //    }
        //}

        public void GetRooms()
        {
            try
            {
                RoomList = new List<RoomDTO>();

                var criteria = new SearchCriteria<RoomDTO>()
                {
                    CurrentUserId = Singleton.User.UserId,
                    //PageSize = 15,
                    //Page = 1
                };

                if (SelectedCategory != null && SelectedCategory.DisplayName != "All")
                {
                    criteria.FiList.Add(r => r.FloorId == SelectedCategory.Id);
                }

                RoomList = _roomService.GetAll(criteria).ToList();

                foreach (var roomDTO in RoomList)
                {
                    if (roomDTO.LastRentalPayment == null)
                    {
                        roomDTO.LastRentalPaymentTemp = new RentalPaymentDTO()
                        {
                            DaysLeft = 1000,
                            Contrat = new RentalContratDTO()
                            {
                                Rentee = new RenteeDTO()
                                {
                                    DisplayName = "",
                                    Address = new AddressDTO()
                                    {
                                        Mobile = ""
                                    }
                                }
                            }
                        };
                    }
                    else
                        roomDTO.LastRentalPaymentTemp = roomDTO.LastRentalPayment;

                    if (roomDTO.LastServicePayment == null)
                    {
                        roomDTO.LastServicePaymentTemp = new RentalPaymentDTO()
                        {
                            DaysLeft = 1000,
                            Contrat = new RentalContratDTO()
                            {
                                Rentee = new RenteeDTO()
                                {
                                    DisplayName = "",
                                    Address = new AddressDTO()
                                    {
                                        Mobile = ""
                                    }
                                }
                            }
                        };
                    }
                    else
                        roomDTO.LastServicePaymentTemp = roomDTO.LastServicePayment;


                    if (roomDTO.LastRentDeposit == null)
                    {
                        roomDTO.LastRenDepositTemp = new RentDepositDTO()
                        {
                            TotalDepositAmountString = ""
                        };
                    }
                    else
                        roomDTO.LastRenDepositTemp = roomDTO.LastRentDeposit;
                }

                RoomLi = new List<RoomDTO>();

                if (ActiveChecked)
                    RoomLi = RoomLi.Union(RoomList.Where(iq => iq.LastRentalPayment != null &&
                                                               iq.LastRentalPayment.DaysLeft > 10).ToList()).ToList();
                if (WarningChecked)
                    RoomLi = RoomLi.Union(RoomList.Where(iq => iq.LastRentalPayment != null &&
                                                               iq.LastRentalPayment.DaysLeft <= 10 &&
                                                               iq.LastRentalPayment.DaysLeft >= 0)
                        .ToList()).ToList();
                if (ExpiredChecked)
                    RoomLi = RoomLi.Union(RoomList.Where(iq => iq.LastRentalPayment != null &&
                                                               iq.LastRentalPayment.DaysLeft < 0).ToList()).ToList();
                if (EmptyChecked)
                    RoomLi = RoomLi.Union(RoomList.Where(iq => iq.LastRentalPayment == null).ToList()).ToList();

                if (!RoomLi.Any() && !ActiveChecked && !WarningChecked && !ExpiredChecked && !EmptyChecked)
                    RoomLi = (List<RoomDTO>) RoomList;

                RoomLi=RoomLi.ToList().OrderBy(i => i.FloorId).ThenBy(i => i.Number);
                
                var sNo = 1;
                foreach (var roomDTO in RoomLi)
                {
                    roomDTO.SerialNumber = sNo;
                    sNo++;
                }

                Rooms =
                    new ObservableCollection<RoomDTO>(RoomLi);//.ToList().OrderBy(i => i.FloorId).ThenBy(i => i.Number));
                PaymentHistoryVisibility = "Collapsed";
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't load rooms, please refresh window again..."
                                + Environment.NewLine + exception.Message, "Can't Load rooms", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SetSummary()
        {
            if (Rooms != null)
            {
                try
                {
                    TotalNumberOfRooms =
                        Rooms.Count().ToString("N0");

                    var valueOfRooms = Rooms.Sum(
                        iq =>
                            iq != null &&
                            iq.RentalFee != null
                                ? Convert.ToDecimal(iq.RentalFee)
                                : 0);
                    valueOfRooms += Rooms.Sum(
                        iq =>
                            iq != null &&
                            iq.ServiceFee != null
                                ? Convert.ToDecimal(iq.ServiceFee)
                                : 0);
                    TotalValueOfRooms = valueOfRooms.ToString("N2");

                    SubTotalValueOfRooms = (Convert.ToDecimal(TotalValueOfRooms)/(decimal) 1.15).ToString("N2");

                    var tax = Convert.ToDecimal(SubTotalValueOfRooms)*(decimal) 0.15;
                    TotalTaxOfRooms = tax.ToString("N2");
                    if (TotalNumberOfRooms == "0")
                    {
                        TotalNumberOfRooms = "";
                        SubTotalValueOfRooms = "";
                        TotalTaxOfRooms = "";
                        TotalValueOfRooms = "";
                    }
                }
                catch
                {
                    TotalNumberOfRooms = "";
                    SubTotalValueOfRooms = "";
                    TotalTaxOfRooms = "";
                    TotalValueOfRooms = "";
                }
            }
        }

        private void SetSummaryForUnpaidRooms()
        {
            if (Rooms != null)
            {
                try
                {
                    TotalNumberOfRooms =
                        Rooms.Count(
                            r =>
                                (r.LastRentalPayment != null && r.LastRentalPayment.UnPaidAmount > 0) ||
                                (r.LastServicePayment != null && r.LastServicePayment.UnPaidAmount > 0)).ToString("N0");

                    var valueOfRooms = Rooms.Sum(
                        iq =>
                            iq != null &&
                            iq.LastRentalPayment != null
                                ? Convert.ToDecimal(iq.LastRentalPayment.UnPaidAmount)
                                : 0);
                    valueOfRooms += Rooms.Sum(
                        iq =>
                            iq != null &&
                            iq.LastServicePayment != null
                                ? Convert.ToDecimal(iq.LastServicePayment.UnPaidAmount)
                                : 0);
                    TotalValueOfRooms = valueOfRooms.ToString("N2");

                    SubTotalValueOfRooms = (Convert.ToDecimal(TotalValueOfRooms) / (decimal)1.15).ToString("N2");

                    var tax = Convert.ToDecimal(SubTotalValueOfRooms) * (decimal)0.15;
                    TotalTaxOfRooms = tax.ToString("N2");
                    if (TotalNumberOfRooms == "0")
                    {
                        TotalNumberOfRooms = "1";
                        SubTotalValueOfRooms = "1";
                        TotalTaxOfRooms = "1";
                        TotalValueOfRooms = "1";
                    }
                }
                catch
                {
                    TotalNumberOfRooms = "";
                    SubTotalValueOfRooms = "";
                    TotalTaxOfRooms = "";
                    TotalValueOfRooms = "";
                }
            }
        }

        #endregion

        #region Commands

        public ICommand RefreshWindowCommand
        {
            get { return _refreshWindowCommand ?? (_refreshWindowCommand = new RelayCommand(ExcuteRefreshWIndow)); }
        }

        private void ExcuteRefreshWIndow()
        {
            Load();
        }

        public ICommand ItemViewCommand
        {
            get { return _itemViewCommand ?? (_itemViewCommand = new RelayCommand<Object>(ExecuteItemViewCommand)); }
        }

        private void ExecuteItemViewCommand(object button)
        {
            try
            {
                var clickedbutton = button as RadButton;

                if (clickedbutton == null) return;
                var buttenTag = clickedbutton.Tag.ToString();
                switch (buttenTag)
                {
                    case "Room":
                        Window roomWindow = SelectedRoom != null ? new RoomEntry(SelectedRoom) : new RoomEntry();
                        roomWindow.ShowDialog();
                        Load();
                        break;
                    case "Deposit":
                        if (SelectedRoom.LastRentalPayment != null)
                        {
                            var renteeWindow = new RentDepositEntry(SelectedRoom.LastRentalPayment.Contrat);
                            renteeWindow.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("You have to first add rental contract and payment");
                        }
                        break;

                    case "PaymentEntry":
                        SelectedRoom.PaymentType = PaymentTypes.Rent;
                        var paymentWindow2 = new RentalPaymentEntry(SelectedRoom);
                        paymentWindow2.ShowDialog();
                        Load();
                        break;
                    case "PaymentRenew":
                        SelectedRoom.PaymentType = PaymentTypes.Rent;
                        Window contratRenewWindow = SelectedRoom.LastRentalPayment != null
                            ? new RentalPaymentEntry(SelectedRoom, true)
                            : new RentalPaymentEntry(SelectedRoom);
                        contratRenewWindow.ShowDialog();
                        Load();
                        break;
                    case "ServiceEntry":
                        var servP = SelectedRoom.LastServicePayment;
                        if (servP != null && servP.Type == PaymentTypes.Service)
                            SelectedRoom.PaymentType = PaymentTypes.Service;
                        else
                        {
                            SelectedRoom.PaymentType = PaymentTypes.Rent;
                        }
                        var serviceWindow = new RentalPaymentEntry(SelectedRoom);
                        serviceWindow.ShowDialog();
                        Load();
                        break;
                    case "ServiceRenew":
                        SelectedRoom.PaymentType = PaymentTypes.Service;
                        Window serviceWindow2 = SelectedRoom.LastServicePayment != null
                            ? new RentalPaymentEntry(SelectedRoom, true)
                            : new RentalPaymentEntry(SelectedRoom);
                        serviceWindow2.ShowDialog();
                        Load();
                        break;

                    case "ContratCancel":
                        DiscontinueContract();
                        Load();
                        break;
                    case "Attachment":
                        GetAttachmentCommand();
                        break;
                    case "ContractAgreement":
                        GetContractAgreementCommand();
                        break;
                    case "AddPayment":
                        SelectedRoom.IsArchived = true;
                        SelectedRoom.PaymentType = PaymentTypes.Rent;
                        var addPaymentWindow = new RentalPaymentEntry(SelectedRoom);
                        addPaymentWindow.ShowDialog();
                        ShowRentalPaymentsHistory();
                        break;
                    case "AddServicePayment":
                        SelectedRoom.IsArchived = true;
                        SelectedRoom.PaymentType = PaymentTypes.Service;
                        var addservicePaymentWindow = new RentalPaymentEntry(SelectedRoom);
                        addservicePaymentWindow.ShowDialog();
                        ShowRentalPaymentsHistory();
                        break;
                    case "EditPayment":
                        SelectedRentalPaymentHistory.IsArchived = true;
                        SelectedRoom.PaymentType = SelectedRentalPaymentHistory.Type;
                        var paymentWindow3 = new RentalPaymentEntry(SelectedRentalPaymentHistory);
                        paymentWindow3.ShowDialog();
                        ShowRentalPaymentsHistory();
                        break;
                    case "DefaultPayment":
                        var payService = new RentalPaymentService(false);
                        SelectedRoom.PaymentType = PaymentTypes.Rent;
                        var ser = payService.Find(SelectedRentalPaymentHistory.Id.ToString());
                        ser.IsArchived = false;
                        payService.InsertOrUpdateWithPayment(ser, null);
                        payService.Dispose();
                        Load();
                        break;
                    case "DeletePayment":
                        if (MessageBox.Show("Are you Sure You want to Delete this Payment History?",
                            "Delete Payment History",
                            MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            new RentalPaymentService(true).Delete(SelectedRentalPaymentHistory.Id.ToString());
                        }
                        ShowRentalPaymentsHistory();
                        break;
                    case "AddEditRemarks":
                        if (SelectedRoom.LastRentalPaymentId != null)
                        {
                            if (SelectedRentalPaymentRemark == null)
                            {
                                SelectedRentalPaymentRemark = new RentalPaymentRemarkDTO()
                                {
                                    RentalPaymentId = (int) SelectedRoom.LastRentalPaymentId,
                                    RemarkDate = DateTime.Now,
                                    Type = RemarkTypes.WIllGiveQuickResponse
                                };
                            }

                            var remarkWindow = new RentalPaymentRemarkEntry(SelectedRentalPaymentRemark);
                            remarkWindow.ShowDialog();
                            ShowRentalPaymentRemarks();
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't open window"
                                + Environment.NewLine + exception.Message, "Can't open window", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void DiscontinueContract()
        {
            if (SelectedRoom.LastRentalPayment != null &&
                MessageBox.Show("Are you sure you want to discontinue this contract?", "Discontinue Contract",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedRoom.LastRoomReleasedDate = SelectedRoom.LastRentalPayment.EndDate;
                    SelectedRoom.ContractDiscontinuedId = SelectedRoom.LastRentalPayment.ContratId;

                    SelectedRoom.LastRentalPayment = null;
                    SelectedRoom.LastRentee = null;
                    SelectedRoom.LastRentDeposit = null;

                    var stat = _roomService.InsertOrUpdate(SelectedRoom);
                    if (stat == string.Empty)
                    {
                        Load();
                    }
                    else
                    {
                        MessageBox.Show("Can't Cancel, may be the data is already in use..."
                                        + Environment.NewLine + stat, "Can't Cancel",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't Cancel, may be the data is already in use..."
                                    + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException,
                        "Can't Cancel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Attachment

        private void GetAttachmentCommand()
        {
            try
            {
                if (SelectedRoom != null && SelectedRoom.LastRentalPayment != null)
                {
                    var myReport = new SingleTransaction();
                    myReport.SetDataSource(GetAttachmentDataSet());

                    var report = new ReportViewerCommon(myReport);
                    report.Show();
                }
            }
            catch
            {
                MessageBox.Show("Got problem while getting Attachment!", "Attachment Problem");
            }
        }

        public TransactionDataSet GetAttachmentDataSet()
        {
            var myDataSet = new TransactionDataSet();

            try
            {
                #region Fields  

                var lastRentalPayment = SelectedRoom.LastRentalPayment;

                var cri = new SearchCriteria<PaymentDTO>
                {
                    CurrentUserId = Singleton.User.UserId
                };
                cri.FiList.Add(p => p.RentalPaymentId == lastRentalPayment.Id);
                var payments = new PaymentService(true).GetAll(cri).ToList();
                if (payments.Count == 0) return null;

                var rentee = lastRentalPayment.Contrat.Rentee;
                var client = new CompanyService(true).GetCompany();
                if (client == null) return null;

                var brCode = new BarcodeProcess();
                var tranNumberbarcode =
                    ImageToByteArray(brCode.GetBarcode(SelectedRoom.LastRentalPayment.PaymentNumber, 320, 40, false),
                        ImageFormat.Bmp);

                var subTotal = lastRentalPayment.TotalAmountRequired/(decimal) 1.15;
                var tax = subTotal*(decimal) 0.15;
                var total = lastRentalPayment.TotalAmountRequired;

                #endregion

                #region Header

                myDataSet.TransactionHeader.Rows.Add(
                    lastRentalPayment.ReceiptNumber,
                    tranNumberbarcode,
                    "",
                    rentee.DisplayName,
                    rentee.TinNumber,
                    rentee.VatNumber,
                    lastRentalPayment.ReceiptDateStringAndAmharic,
                    CommonUtility.GetNumberInWords(total.ToString(), true),
                    subTotal,
                    "VAT 15%",
                    tax,
                    total,
                    "linknumber1"
                    );

                #endregion

                #region Client Address

                myDataSet.ClientDetail.Rows.Add(
                    client.Header,
                    client.Footer,
                    client.Address.AddressDetail,
                    client.Address.SubCity,
                    client.Address.Kebele,
                    client.Address.HouseNumber,
                    client.Address.Telephone,
                    client.Address.Mobile,
                    client.Address.Fax,
                    client.Address.PrimaryEmail,
                    Singleton.User.FullName,
                    client.TinNumber,
                    client.VatNumber,
                    "CASH", "CASH", "", "linknumber1");

                #endregion

                #region BPAddress

                myDataSet.BPAddress.Rows.Add(
                    rentee.Address.AddressDetail,
                    rentee.Address.SubCity,
                    rentee.Address.Kebele,
                    rentee.Address.HouseNumber,
                    rentee.Address.Telephone,
                    rentee.Address.Mobile,
                    rentee.Address.Fax,
                    rentee.Address.PrimaryEmail,
                    rentee.Address.AlternateEmail,
                    "linknumber1");

                #endregion

                #region Lines

                var serNo = 1;
                foreach (var payment in payments)
                {
                    myDataSet.TransactionLine.Rows.Add(
                        serNo,
                        "",
                        payment.Reason,
                        "",
                        "",
                        payment.UnitOfMeasure,
                        payment.EachPrice,
                        payment.UnitQty,
                        payment.Amount,
                        0,
                        "linknumber1");

                    serNo++;
                }

                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't get data for the report"
                                + Environment.NewLine + exception.Message, "Can't get data", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return myDataSet;
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn, ImageFormat format)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ms.ToArray();
        }

        #endregion

        #region Get Contract Agreement

        private void GetContractAgreementCommand()
        {
            try
            {
                if (SelectedRoom != null && SelectedRoom.LastRentalPayment != null)
                {
                    var myReport = new ContratDiscontinueAgreement();

                    myReport.SetDataSource(GetContractAgreementDataSet());

                    var report = new ReportViewerCommon(myReport);
                    report.Show();
                }
            }
            catch
            {
                MessageBox.Show("Got problem while getting contract agreement!", "Contract Agreement Problem");
            }
        }

        public ReportDataSet GetContractAgreementDataSet()
        {
            var myDataSet = new ReportDataSet();
            var client = new CompanyService(true).GetCompany();

            try
            {
                var set = Singleton.Setting;
                var room = SelectedRoom.LastRentalPayment.Contrat.Room;
                var roomNumber = room.Number;

                var onDate = ReportUtility.GetEthCalendar(SelectedRoom.LastRentalPayment.Contrat.StartDate, true, " ቀን ");
                var conEndDate = ReportUtility.GetEthCalendar(SelectedRoom.LastRentalPayment.Contrat.EndDate, true,
                    " ቀን ");
                var conPeriod =
                    CommonUtility.GetNumberInWordsWithNumber(
                        SelectedRoom.LastRentalPayment.Contrat.ContratPeriod.ToString(), false);

                var monthlyPay =
                    CommonUtility.GetNumberInWordsWithNumber(SelectedRoom.LastRentalPayment.RentAmount.ToString(), false,
                        true);

                var payPeriod =
                    CommonUtility.GetNumberInWordsWithNumber(SelectedRoom.LastRentalPayment.PaymentPeriod.ToString(),
                        false,false);
                //var payStartDate = ReportUtility.GetEthCalendar(SelectedRoom.LastRentalPayment.StartDate, true, " ቀን ");
                //var payEndDate = ReportUtility.GetEthCalendar(SelectedRoom.LastRentalPayment.EndDate, true," ቀን ");
               
                var tot = SelectedRoom.LastRentalPayment.PaymentPeriod*SelectedRoom.LastRentalPayment.RentAmount;
                var totalPay =
                    CommonUtility.GetNumberInWordsWithNumber(tot.ToString(),
                        false, true);

                var penality = set.PenalityPercent.ToString();
                penality += "%/" + CommonUtility.GetNumberInWords(penality, false).Replace(" ብር", "").Replace(" ሣንቲም", "") + " በመቶ/";

                var renter = GetRenter();
                var rentee = GetRentee();

                #region typeofwork
                var renteeDetail = new RenteeService(true).GetAll().FirstOrDefault(r => r.Id == SelectedRoom.LastRenteeId);
                var typeOfWork = "";
                if (renteeDetail != null)
                {
                    typeOfWork = renteeDetail.TypeOfWork;
                }

                #endregion

                #region Is Renter Representative
                var company = new CompanyService(true).GetCompany();
                var isrenteeRep = "";
                if (company.Representee!=null)
                    isrenteeRep = "በወኪል አድራጊዎቼ ስም";
                #endregion

                myDataSet.ContractHeader.Rows.Add(
                    onDate,
                    set.Ministry,
                    set.Office,
                    set.City,
                    renter.Split('?')[0],
                    renter.Split('?')[1],
                    rentee.Split('?')[0],
                    rentee.Split('?')[1]);

                myDataSet.ContractFooter.Rows.Add(
                    onDate,
                    set.GoverningArticleCode,
                    set.TerminationArticleCode,
                    set.TerminationAmount.ToString(),
                    CommonUtility.GetNumberInWords(set.TerminationAmount.ToString(), false),
                    "", "", "", "");

               
                myDataSet.RenterRenteeParagraph.Rows.Add(
                    onDate,
                    CommonUtility.GetNumberInWordsWithNumber(set.DueDaysToDiscontinueContrat,false,false),
                    CommonUtility.GetNumberInWordsWithNumber(set.PaymentWithoutPenalityDays,false,false),
                    penality,
                    CommonUtility.GetNumberInWordsWithNumber(set.AdditionalPenalityDays.ToString(),false,false),
                    CommonUtility.GetNumberInWordsWithNumber(set.AdditionalPenalityDays.ToString(),false,false),
                    set.TerminationArticleCode, "", "", "", "");

                myDataSet.RenteeParagraph.Rows.Add(
                    onDate,
                    EnumUtil.GetEnumDesc(room.Service)+" ",
                    roomNumber,
                    conPeriod, onDate, conEndDate, monthlyPay,
                    "", "", typeOfWork, "", "", "", "", "");
                //client.CompanyAddress.SubCity
                //client.CompanyAddress.HouseNumber
                myDataSet.RenterParagraph.Rows.Add(
                    onDate,
                    GetByName(),
                    client.CompanyAddress.City,
                    client.BusinessAddressPrevious,
                    GetAddress2(client.CompanyAddress),
                    "",
                    client.TradeName,
                    client.PlotNumber,
                    client.TotalSqrFeet,
                    "", EnumUtil.GetEnumDesc(room.Service) + " ",
                    roomNumber,
                    conPeriod, onDate, conEndDate, monthlyPay,
                    payPeriod, totalPay,
                    typeOfWork, isrenteeRep, isrenteeRep, "", "", "");
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't get data for the report"
                                + Environment.NewLine + exception.Message, "Can't get data", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return myDataSet;
        }

        private static string GetByName()
        {
            var company = new CompanyService(true).GetCompany();
            var name = "";
            if (company.Type == CompanyTypes.Personal)
            {
                if (company.Representee == null)
                    name = "በስሜ ";
                else if(!string.IsNullOrEmpty(company.OtherOwnerCountry))
                    name = "የወኪል አድራጊዎቼ የጋራ ንብረት የሆነውን በአንደኛ ወኪል አድራጊዬ " + company.Title.DisplayName + " " + company.DisplayName + " ስም";
                else name = "በወኪል አድራጊዬ " + company.Title.DisplayName + " " + company.DisplayName + " ስም";
            }
            if (company.Type == CompanyTypes.Organization)
            {
                if (company.Representee == null)
                    name = "በ"+company.DisplayName+" ድርጅት ስም";
                else name = "በወኪል አድራጊዬ " + company.DisplayName + " ድርጅት ስም";
            }
            return name;
        }

        private string GetRenter()
        {
            var company = new CompanyService(true).GetCompany();
            var owner = company.DisplayName;
            var addrs = GetAddress(company.Address);
            if (company.Type == CompanyTypes.Organization)
            {
                owner += " ዋና ስራ አስኪያጅ " + company.Title.DisplayName + " " + company.ManagerName;
                
            }
            else
            {
                owner = company.Title.DisplayName + " " + owner;
            }

            owner += "/ዜግነት " + company.Address.Country + "/" + company.OtherOwnerName;

            if (company.Representee!=null)
            {
                var representee = company.Representee;
                owner = "የ" + owner + " ህጋዊ ወኪል " + representee.Title.DisplayName + " " +
                        representee.FullName + "/ዜግነት " + representee.Address.Country + "/" +
                        " የውክልና ስልጣን ቁጥር " + representee.AuthorizationNumber + " በቀን " +
                        representee.AuthorizationDateStringAmharicFormatted + " ዓ.ም በተሰጠኝ ውክልና ሥልጣን መሰረት";
                addrs = GetAddress(representee.Address);
            }

            return owner +"?"+ addrs;
        }

        private string GetRentee()
        {
            var rentee = new RenteeService(true).GetAll().FirstOrDefault(r=>r.Id==SelectedRoom.LastRenteeId);
            if (rentee != null)
            {
                var owner = rentee.DisplayName;
                var addrs = GetAddress(rentee.Address);
                if (rentee.Type == RenteeTypes.Organization)
                {
                    owner += " ዋና ስራ አስኪያጅ " + rentee.Title.DisplayName + " " + rentee.ManagerName;
                }
                else
                {
                    owner = rentee.Title.DisplayName + " " + owner;
                }

                owner += "/ዜግነት " + rentee.Address.Country + "/";

                if (rentee.Representee != null)
                {
                    var representee = rentee.Representee;
                    owner = "የ" + owner + " ህጋዊ ወኪል " + representee.Title.DisplayName + " " +
                            representee.FullName + "/ዜግነት " + representee.Address.Country + "/" +
                            " የውክልና ስልጣን ቁጥር " + representee.AuthorizationNumber + " በቀን " +
                            representee.AuthorizationDateStringAmharicFormatted + " ዓ.ም በተሰጠኝ ውክልና ሥልጣን መሰረት";
                    addrs = GetAddress(representee.Address);
                }

                return owner + "?" + addrs;
            }
            return " ? ";
        }

        private static string GetAddress(AddressDTO address)
        {
            return "አድራሻ፡- " + GetAddress2(address);
        }

        private static string GetAddress2(AddressDTO address)
        {
            if (!string.IsNullOrEmpty(address.PassportNumber))
                return " የፓስፖርት ቁ. " + address.PassportNumber;

            var addr = "";
            if (!string.IsNullOrEmpty(address.City))
                addr += address.City + " ከተማ ";
            if (!string.IsNullOrEmpty(address.SubCity))
                addr += address.SubCity + " ክፍለ ከተማ";
            if (!string.IsNullOrEmpty(address.Woreda))
                addr += " ወረዳ " + address.Woreda;
            if (!string.IsNullOrEmpty(address.Kebele))
                addr += " ቀበሌ " + address.Kebele;
            if (!string.IsNullOrEmpty(address.HouseNumber))
                addr += " የቤት ቁጥር " + address.HouseNumber;
            if (!string.IsNullOrEmpty(address.DrivingLicenseNumber))
                addr += " መንጃ ፈቃድ ቁ. " + address.DrivingLicenseNumber;

            return addr;
        }
        

        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave()
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