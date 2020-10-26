#region MyRegion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PinnaRent.Core;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.Service;
using PinnaRent.Service.Interfaces;
using PinnaRent.WPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.MessageBox;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class RoomEntryViewModel : ViewModelBase
    {
        #region Fields
        private static IRoomService _roomService;
        private RoomDTO _selectedRoom, _selectedRoomParam;
        private ICommand _saveRoomViewCommand, _closeRoomViewCommand,
            _addNewRoomViewCommand, _addNewCategoryCommand, _addNewRoomResourceViewCommand;
        private ObservableCollection<CategoryDTO> _floors;
        private RoomResourceDTO _selectedRoomResource;
        private ObservableCollection<RoomResourceDTO> _roomResources;
        #endregion

        #region Constructor
        public RoomEntryViewModel()
        {
            CleanUp();
            _roomService = new RoomService();

            CheckRoles();

            LoadCategories();
            SelectedFloor = Floors.FirstOrDefault();

            AddNewRoom();

            Messenger.Default.Register<RoomDTO>(this, (message) => { SelectedRoomParam = message; });
        }

        public static void CleanUp()
        {
            if (_roomService != null)
                _roomService.Dispose();
        }
        #endregion

        #region Properties

        public RoomDTO SelectedRoomParam
        {
            get { return _selectedRoomParam; }
            set
            {
                _selectedRoomParam = value;
                RaisePropertyChanged<RoomDTO>(() => SelectedRoomParam);
                if (SelectedRoomParam != null)
                {
                    Messenger.Default.Unregister<RoomDTO>(this, (message) => { SelectedRoomParam = message; });
                    var cri = new SearchCriteria<RoomDTO>();
                    cri.FiList.Add(r=>r.Id==SelectedRoomParam.Id);
                    SelectedRoom = _roomService.GetAll(cri).FirstOrDefault();
                }
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
                    SelectedFloor = Floors.FirstOrDefault(c => c.Id == SelectedRoom.FloorId);
                    LoadRoomResources();
                }
            }
        }

        public ObservableCollection<RoomResourceDTO> RoomResources
        {
            get { return _roomResources; }
            set
            {
                _roomResources = value;
                RaisePropertyChanged<ObservableCollection<RoomResourceDTO>>(() => RoomResources);
                if (RoomResources != null)
                {
                    SelectedRoomResource = RoomResources.FirstOrDefault();
                }
            }
        }
        public RoomResourceDTO SelectedRoomResource
        {
            get { return _selectedRoomResource; }
            set
            {
                _selectedRoomResource = value;
                RaisePropertyChanged<RoomResourceDTO>(() => SelectedRoomResource);
                if (SelectedRoomResource != null)
                {

                }
            }
        }

        #endregion

        #region Commands
        public ICommand AddNewRoomViewCommand
        {
            get
            {
                return _addNewRoomViewCommand ?? (_addNewRoomViewCommand = new RelayCommand(AddNewRoom));
            }
        }
        public void AddNewRoom()
        {
            SelectedRoom = new RoomDTO
            {
                Type = RoomTypes.ByFixed,
                Status = RoomStatus.Active
            };
            if (Floors != null) SelectedFloor = Floors.FirstOrDefault();
        }

        public ICommand AddNewRoomResourceViewCommand
        {
            get
            {
                return _addNewRoomResourceViewCommand ??
                       (_addNewRoomResourceViewCommand = new RelayCommand<Object>(ResourceEntry));
            }
        }

        private void ResourceEntry(object obj)
        {
            var btn = obj as Button;
            if (btn != null)
                switch (btn.Tag.ToString())
                {
                    case "AddNew":
                        AddNewRoomResource(false);
                        break;
                    case "Edit":
                        AddNewRoomResource(true);
                        break;
                    case "Delete":
                        if (SelectedRoomResource != null)
                        {
                            new RoomResourceService(true).Delete(SelectedRoomResource.Id.ToString());
                            LoadRoomResources();
                        }
                        break;
                }
        }
        private void AddNewRoomResource(bool isEdit)
        {
            ExecuteSaveRoomViewCommand(null);
            var contratWindow = new RoomResourceEntry(SelectedRoom);
            if(isEdit && SelectedRoomResource!=null)
                contratWindow = new RoomResourceEntry(SelectedRoom,SelectedRoomResource);

            contratWindow.ShowDialog();
            var dialogueResult = contratWindow.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadRoomResources();
            }
        }

        private void LoadRoomResources()
        {
            var roomResourceDtos = new RoomResourceService(true)
                .GetAll()
                .Where(p => p.RoomId == SelectedRoom.Id)
                .OrderByDescending(p => p.Id)
                .ToList();
            RoomResources = new ObservableCollection<RoomResourceDTO>(roomResourceDtos);
        }

        public ICommand SaveCloseRoomViewCommand
        {
            get { return _saveRoomViewCommand ?? (_saveRoomViewCommand = new RelayCommand<Object>(ExecuteSaveRoomViewCommand, CanSave)); }
        }
        private void ExecuteSaveRoomViewCommand(object obj)
        {
            try
            {
                SelectedRoom.FloorId = SelectedFloor.Id;

                var stat = _roomService.InsertOrUpdate(SelectedRoom);
                if (stat != string.Empty)
                    MessageBox.Show("Got Problem while saving item, try again..." + Environment.NewLine + stat, "save error", MessageBoxButton.OK,
                       MessageBoxImage.Error);
                else CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem saving Room..." +
                            Environment.NewLine + exception.Message +
                            Environment.NewLine + exception.InnerException);
            }
        }

        public ICommand CloseRoomViewCommand
        {
            get
            {
                return _closeRoomViewCommand ?? (_closeRoomViewCommand = new RelayCommand<Object>(CloseWindow));
            }
        }
        private void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }
        #endregion
        
        #region Categories
        public void LoadCategories()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.FloorCategory);
            IEnumerable<CategoryDTO> categoriesList = new CategoryService(true)
                .GetAll(criteria)
                .ToList();

            Floors = new ObservableCollection<CategoryDTO>(categoriesList);
        }

        private CategoryDTO _selectedFloor;
        public CategoryDTO SelectedFloor
        {
            get { return _selectedFloor; }
            set
            {
                _selectedFloor = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedFloor);
            }
        }
        public ObservableCollection<CategoryDTO> Floors
        {
            get { return _floors; }
            set
            {
                _floors = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => Floors);
            }
        }
        public ICommand AddNewCategoryCommand
        {
            get
            {
                return _addNewCategoryCommand ?? (_addNewCategoryCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.FloorCategory);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadCategories();//should also get the latest updates in each row
                SelectedFloor = Floors.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedFloor != null) SelectedRoom.FloorId = SelectedFloor.Id;
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
