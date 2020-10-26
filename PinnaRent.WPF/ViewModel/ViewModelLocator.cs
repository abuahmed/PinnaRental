#region

using PinnaRent.Core;
using PinnaRent.Core.Enumerations;

#endregion

namespace PinnaRent.WPF.ViewModel
{
    public class ViewModelLocator
    {
        private static Bootstrapper _bootStrapper;

        public ViewModelLocator()
        {
            //Add Code to choose the server/database the user wants to connect to, the line below depends on it
            Singleton.Edition = PinnaRentEdition.CompactEdition;
            if (_bootStrapper == null)
                _bootStrapper = new Bootstrapper();
        }

        public DashBoardViewModel DashBoard
        {
            get { return _bootStrapper.Container.Resolve<DashBoardViewModel>(); }
        }

        public RentalContratEntryViewModel RentalContratEntry
        {
            get { return _bootStrapper.Container.Resolve<RentalContratEntryViewModel>(); }
        }

        public RentalPaymentEntryViewModel RentalPaymentEntry
        {
            get { return _bootStrapper.Container.Resolve<RentalPaymentEntryViewModel>(); }
        }

        public RemarkEntryViewModel RemarkEntry
        {
            get { return _bootStrapper.Container.Resolve<RemarkEntryViewModel>(); }
        }

        public ServicePaymentEntryViewModel ServicePaymentEntry
        {
            get { return _bootStrapper.Container.Resolve<ServicePaymentEntryViewModel>(); }
        }

        public ServicePaymentUCViewModel ServicePaymentUC
        {
            get { return _bootStrapper.Container.Resolve<ServicePaymentUCViewModel>(); }
        }

        public RentDepositEntryViewModel RentDepositEntry
        {
            get { return _bootStrapper.Container.Resolve<RentDepositEntryViewModel>(); }
        }

        public SplashScreenViewModel Splash
        {
            get { return _bootStrapper.Container.Resolve<SplashScreenViewModel>(); }
        }

        public ActivationViewModel Activation
        {
            get { return _bootStrapper.Container.Resolve<ActivationViewModel>(); }
        }

        public DurationViewModel Duration
        {
            get { return _bootStrapper.Container.Resolve<DurationViewModel>(); }
        }

        public CategoryViewModel Categories
        {
            get { return _bootStrapper.Container.Resolve<CategoryViewModel>(); }
        }

        public SettingViewModel Setting
        {
            get { return _bootStrapper.Container.Resolve<SettingViewModel>(); }
        }

        public CalendarConvertorViewModel CalendarConvertor
        {
            get { return _bootStrapper.Container.Resolve<CalendarConvertorViewModel>(); }
        }

        public CompanyViewModel Company
        {
            get { return _bootStrapper.Container.Resolve<CompanyViewModel>(); }
        }

        public AddressViewModel AddressVm
        {
            get { return _bootStrapper.Container.Resolve<AddressViewModel>(); }
        }

        public AddressUCViewModel AddressUC
        {
            get { return _bootStrapper.Container.Resolve<AddressUCViewModel>(); }
        }

        public RenteeEntryViewModel RenteeEntry
        {
            get { return _bootStrapper.Container.Resolve<RenteeEntryViewModel>(); }
        }

        public RepresenteeUCViewModel RepresenteeUC
        {
            get { return _bootStrapper.Container.Resolve<RepresenteeUCViewModel>(); }
        }

        public RoomsViewModel Rooms
        {
            get { return _bootStrapper.Container.Resolve<RoomsViewModel>(); }
        }

        public RoomEntryViewModel RoomEntry
        {
            get { return _bootStrapper.Container.Resolve<RoomEntryViewModel>(); }
        }

        public RoomResourceEntryViewModel RoomResourceEntry
        {
            get { return _bootStrapper.Container.Resolve<RoomResourceEntryViewModel>(); }
        }

        public MainViewModel Main
        {
            get { return _bootStrapper.Container.Resolve<MainViewModel>(); }
        }

        public UserViewModel User
        {
            get { return _bootStrapper.Container.Resolve<UserViewModel>(); }
        }

        public LoginViewModel Login
        {
            get { return _bootStrapper.Container.Resolve<LoginViewModel>(); }
        }

        public ChangePasswordViewModel ChangePassword
        {
            get { return _bootStrapper.Container.Resolve<ChangePasswordViewModel>(); }
        }

        public BackupRestoreViewModel BackupRestore
        {
            get { return _bootStrapper.Container.Resolve<BackupRestoreViewModel>(); }
        }

        public ReportViewerViewModel ReportViewerCommon
        {
            get { return _bootStrapper.Container.Resolve<ReportViewerViewModel>(); }
        }

        public BusinessPartnerEntryViewModel BusinessPartnerEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<BusinessPartnerEntryViewModel>();
            }
        }
        public ItemEntryViewModel ItemEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<ItemEntryViewModel>();
            }
        }
        public ItemViewModel Item
        {
            get
            {
                return _bootStrapper.Container.Resolve<ItemViewModel>();
            }
        }
        public BankAccountViewModel BankAccount
        {
            get
            {
                return _bootStrapper.Container.Resolve<BankAccountViewModel>();
            }
        }
        public ChartofAccountViewModel ChartofAccount
        {
            get
            {
                return _bootStrapper.Container.Resolve<ChartofAccountViewModel>();
            }
        }
        public SellItemEntryViewModel SellItemEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<SellItemEntryViewModel>();
            }
        }
        public WarehouseViewModel Warehouse
        {
            get
            {
                return _bootStrapper.Container.Resolve<WarehouseViewModel>();
            }
        }
        public OnHandInventoryViewModel OnHandInventory
        {
            get
            {
                return _bootStrapper.Container.Resolve<OnHandInventoryViewModel>();
            }
        }
        public ReceiveStockViewModel ReceiveStock
        {
            get
            {
                return _bootStrapper.Container.Resolve<ReceiveStockViewModel>();
            }
        }
        public ExpensesViewModel Expenses
        {
            get
            {
                return _bootStrapper.Container.Resolve<ExpensesViewModel>();
            }
        }
        public ExpenseEntryViewModel ExpenseEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<ExpenseEntryViewModel>();
            }
        }
        public CheckEntryViewModel CheckEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<CheckEntryViewModel>();
            }
        }

        public static void Cleanup()
        {
        }
    }
}