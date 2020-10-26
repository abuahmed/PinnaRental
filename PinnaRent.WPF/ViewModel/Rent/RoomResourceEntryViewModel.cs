#region MyRegion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
    public class RoomResourceEntryViewModel : ViewModelBase
    {
        #region Fields
        private static IRoomResourceService _roomService;
        private RoomResourceDTO _selectedRoomResource, _selectedRoomResourceParam;
        private RoomDTO _selectedRoom;
        private ICommand _saveRoomResourceViewCommand, _closeRoomResourceViewCommand,_addNewRoomResourceViewCommand, _addNewCategoryCommand;
        private ObservableCollection<CategoryDTO> _categories;
        #endregion

        #region Constructor
        public RoomResourceEntryViewModel()
        {
            CleanUp();
            _roomService = new RoomResourceService();

            CheckRoles();

            LoadCategories();
            SelectedCategory = Categories.FirstOrDefault();

            Messenger.Default.Register<RoomDTO>(this, (message) => { SelectedRoom = message; });
            
            AddNewRoomResource();
            
            Messenger.Default.Register<RoomResourceDTO>(this, (message) => { SelectedRoomResourceParam = message; });
        }

        public static void CleanUp()
        {
            if (_roomService != null)
                _roomService.Dispose();
        }
        #endregion

        #region Properties

        public RoomResourceDTO SelectedRoomResourceParam
        {
            get { return _selectedRoomResourceParam; }
            set
            {
                _selectedRoomResourceParam = value;
                RaisePropertyChanged<RoomResourceDTO>(() => SelectedRoomResourceParam);
                if (SelectedRoomResourceParam != null)
                {
                    SelectedRoomResource = _roomService.Find(SelectedRoomResourceParam.Id.ToString());
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
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == SelectedRoomResource.CategoryId);
                }
            }
        }
        public RoomDTO SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                RaisePropertyChanged<RoomResourceDTO>(() => SelectedRoomResource);
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewRoomResourceViewCommand
        {
            get
            {
                return _addNewRoomResourceViewCommand ?? (_addNewRoomResourceViewCommand = new RelayCommand(AddNewRoomResource));
            }
        }
        public void AddNewRoomResource()
        {
            SelectedRoomResource = new RoomResourceDTO
            {
                 //RoomId = SelectedRoom.Id
            };
            if (Categories != null) SelectedCategory = Categories.FirstOrDefault();
        }

        public ICommand SaveCloseRoomResourceViewCommand
        {
            get { return _saveRoomResourceViewCommand ?? (_saveRoomResourceViewCommand = new RelayCommand<Object>(ExecuteSaveRoomResourceViewCommand, CanSave)); }
        }
        private void ExecuteSaveRoomResourceViewCommand(object obj)
        {
            try
            {
                if (SelectedCategory != null) 
                    SelectedRoomResource.CategoryId = SelectedCategory.Id;
                else return;

                if (SelectedRoom != null) SelectedRoomResource.RoomId = SelectedRoom.Id;
                else return;

                var stat = _roomService.InsertOrUpdate(SelectedRoomResource);
                if (stat != string.Empty)
                    MessageBox.Show("Got Problem while saving item, try again..." + Environment.NewLine + stat, "save error", MessageBoxButton.OK,
                       MessageBoxImage.Error);
                else CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem saving RoomResource..." +
                            Environment.NewLine + exception.Message +
                            Environment.NewLine + exception.InnerException);
            }
        }

        public ICommand CloseRoomResourceViewCommand
        {
            get
            {
                return _closeRoomResourceViewCommand ?? (_closeRoomResourceViewCommand = new RelayCommand<Object>(CloseWindow));
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
            criteria.FiList.Add(c => c.NameType == NameTypes.RoomResourceCategory);
            IEnumerable<CategoryDTO> categoriesList = new CategoryService(true)
                .GetAll(criteria)
                .ToList();

            Categories = new ObservableCollection<CategoryDTO>(categoriesList);
        }

        private CategoryDTO _selectedCategory;
        public CategoryDTO SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedCategory);
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
        public ICommand AddNewCategoryCommand
        {
            get
            {
                return _addNewCategoryCommand ?? (_addNewCategoryCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.RoomResourceCategory);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadCategories();//should also get the latest updates in each row
                SelectedCategory = Categories.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedCategory != null) SelectedRoomResource.CategoryId = SelectedCategory.Id;
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
