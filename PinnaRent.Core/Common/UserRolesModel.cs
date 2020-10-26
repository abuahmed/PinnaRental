using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;

namespace PinnaRent.Core.Common
{
    public class UserRolesModel : EntityBase
    {
        public UserRolesModel()
        {
            #region Role Visibilities

            Settings = CommonUtility.UserHasRole(RoleTypes.Settings) ? "Visible" : "Collapsed";
            AdvancedSettings = CommonUtility.UserHasRole(RoleTypes.AdvancedSettings) ? "Visible" : "Collapsed";
            TaxSettings = CommonUtility.UserHasRole(RoleTypes.TaxSettings) ? "Visible" : "Collapsed";
            GeneralSettings = CommonUtility.UserHasRole(RoleTypes.GeneralSettings) ? "Visible" : "Collapsed";

            UsersMgmt = CommonUtility.UserHasRole(RoleTypes.UsersMgmt) ? "Visible" : "Collapsed";
            UsersPrivilegeMgmt = CommonUtility.UserHasRole(RoleTypes.UsersPrivilegeMgmt) ? "Visible" : "Collapsed";
            BackupRestore = CommonUtility.UserHasRole(RoleTypes.BackupRestore) ? "Visible" : "Collapsed";
            Company = CommonUtility.UserHasRole(RoleTypes.CompanyMgmt) ? "Visible" : "Collapsed";
            WarehouseMgmt = CommonUtility.UserHasRole(RoleTypes.WarehouseMgmt) ? "Visible" : "Collapsed";
            DashboardMgmt = CommonUtility.UserHasRole(RoleTypes.DashBoardMgmt) ? "Visible" : "Collapsed";
            PosReports = CommonUtility.UserHasRole(RoleTypes.PosReports) ? "Visible" : "Collapsed";

            RoomEntry = CommonUtility.UserHasRole(RoleTypes.RoomEntry) ? "Visible" : "Collapsed";
            RoomEdit = CommonUtility.UserHasRole(RoleTypes.RoomEdit) ? "Visible" : "Collapsed";
            RoomDelete = CommonUtility.UserHasRole(RoleTypes.RoomDelete) ? "Visible" : "Collapsed";

            RenteeEntry = CommonUtility.UserHasRole(RoleTypes.RenteeEntry) ? "Visible" : "Collapsed";
            RenteeEdit = CommonUtility.UserHasRole(RoleTypes.RenteeEdit) ? "Visible" : "Collapsed";
            RenteeDelete = CommonUtility.UserHasRole(RoleTypes.RenteeDelete) ? "Visible" : "Collapsed";

            ContratAdd = CommonUtility.UserHasRole(RoleTypes.ContratAdd) ? "Visible" : "Collapsed";
            ContratEdit = CommonUtility.UserHasRole(RoleTypes.ContratEdit) ? "Visible" : "Collapsed";
            ContratRenewal = CommonUtility.UserHasRole(RoleTypes.ContratRenewal) ? "Visible" : "Collapsed";
            ContratCancellation = CommonUtility.UserHasRole(RoleTypes.ContratCancellation) ? "Visible" : "Collapsed";

            PaymentAdd = CommonUtility.UserHasRole(RoleTypes.PaymentAdd) ? "Visible" : "Collapsed";
            PaymentEdit = CommonUtility.UserHasRole(RoleTypes.PaymentEdit) ? "Visible" : "Collapsed";
            PaymentDeletion = CommonUtility.UserHasRole(RoleTypes.PaymentDeletion) ? "Visible" : "Collapsed";
            PaymentRenewal = CommonUtility.UserHasRole(RoleTypes.PaymentRenewal) ? "Visible" : "Collapsed";

            ItemsMgmt = CommonUtility.UserHasRole(RoleTypes.ItemsMgmt) ? "Visible" : "Collapsed";
            OnHandMgmt = CommonUtility.UserHasRole(RoleTypes.OnHandMgmt) ? "Visible" : "Collapsed";

            ReceiveStock = CommonUtility.UserHasRole(RoleTypes.ReceiveStock) ? "Visible" : "Collapsed";
            SellStock = CommonUtility.UserHasRole(RoleTypes.SellStock) ? "Visible" : "Collapsed";
            TransferStock = CommonUtility.UserHasRole(RoleTypes.TransferStock) ? "Visible" : "Collapsed";

            TransferRequest = CommonUtility.UserHasRole(RoleTypes.TransferRequest) ? "Visible" : "Collapsed";
            TransferSend = CommonUtility.UserHasRole(RoleTypes.TransferSend) ? "Visible" : "Collapsed";
            TransferReceive = CommonUtility.UserHasRole(RoleTypes.TransferReceive) ? "Visible" : "Collapsed";

            PosReports = CommonUtility.UserHasRole(RoleTypes.PosReports) ? "Visible" : "Collapsed";

            ExpenseEntry = CommonUtility.UserHasRole(RoleTypes.ExpenseEntry) ? "Visible" : "Collapsed";
            ExpenseEdit = CommonUtility.UserHasRole(RoleTypes.ExpenseEdit) ? "Visible" : "Collapsed";
            ExpenseDelete = CommonUtility.UserHasRole(RoleTypes.ExpenseDelete) ? "Visible" : "Collapsed";

            SuppliersEntry = CommonUtility.UserHasRole(RoleTypes.SupplierEntry) ? "Visible" : "Collapsed";
            #endregion
        }

        #region Public Properties
        public string Settings
        {
            get { return GetValue(() => Settings); }
            set { SetValue(() => Settings, value); }
        }
        public string AdvancedSettings
        {
            get { return GetValue(() => AdvancedSettings); }
            set { SetValue(() => AdvancedSettings, value); }
        }
        public string TaxSettings
        {
            get { return GetValue(() => TaxSettings); }
            set { SetValue(() => TaxSettings, value); }
        }
        public string GeneralSettings
        {
            get { return GetValue(() => GeneralSettings); }
            set { SetValue(() => GeneralSettings, value); }
        }

        public string Admin
        {
            get { return GetValue(() => Admin); }
            set { SetValue(() => Admin, value); }
        }
        public string UsersMgmt
        {
            get { return GetValue(() => UsersMgmt); }
            set { SetValue(() => UsersMgmt, value); }
        }
        public string UsersPrivilegeMgmt
        {
            get { return GetValue(() => UsersPrivilegeMgmt); }
            set { SetValue(() => UsersPrivilegeMgmt, value); }
        }
        public string BackupRestore
        {
            get { return GetValue(() => BackupRestore); }
            set { SetValue(() => BackupRestore, value); }
        }
        public string Company
        {
            get { return GetValue(() => Company); }
            set { SetValue(() => Company, value); }
        }
        public string WarehouseMgmt
        {
            get { return GetValue(() => WarehouseMgmt); }
            set { SetValue(() => WarehouseMgmt, value); }
        }
        public string DashboardMgmt
        {
            get { return GetValue(() => DashboardMgmt); }
            set { SetValue(() => DashboardMgmt, value); }
        }
        public string PosReports
        {
            get { return GetValue(() => PosReports); }
            set { SetValue(() => PosReports, value); }
        }

        public string Files
        {
            get { return GetValue(() => Files); }
            set { SetValue(() => Files, value); }
        }
        
        public string RoomEntry
        {
            get { return GetValue(() => RoomEntry); }
            set { SetValue(() => RoomEntry, value); }
        }
        public string RoomEdit
        {
            get { return GetValue(() => RoomEdit); }
            set { SetValue(() => RoomEdit, value); }
        }
        public string RoomDelete
        {
            get { return GetValue(() => RoomDelete); }
            set { SetValue(() => RoomDelete, value); }
        }

        public string RenteeEntry
        {
            get { return GetValue(() => RenteeEntry); }
            set { SetValue(() => RenteeEntry, value); }
        }
        public string RenteeEdit
        {
            get { return GetValue(() => RenteeEdit); }
            set { SetValue(() => RenteeEdit, value); }
        }
        public string RenteeDelete
        {
            get { return GetValue(() => RenteeDelete); }
            set { SetValue(() => RenteeDelete, value); }
        }

        public string ContratAdd
        {
            get { return GetValue(() => ContratAdd); }
            set { SetValue(() => ContratAdd, value); }
        }
        public string ContratEdit
        {
            get { return GetValue(() => ContratEdit); }
            set { SetValue(() => ContratEdit, value); }
        }
        public string ContratRenewal
        {
            get { return GetValue(() => ContratRenewal); }
            set { SetValue(() => ContratRenewal, value); }
        }
        public string ContratCancellation
        {
            get { return GetValue(() => ContratCancellation); }
            set { SetValue(() => ContratCancellation, value); }
        }

        public string PaymentAdd
        {
            get { return GetValue(() => PaymentAdd); }
            set { SetValue(() => PaymentAdd, value); }
        }
        public string PaymentEdit
        {
            get { return GetValue(() => PaymentEdit); }
            set { SetValue(() => PaymentEdit, value); }
        }
        public string PaymentDeletion
        {
            get { return GetValue(() => PaymentDeletion); }
            set { SetValue(() => PaymentDeletion, value); }
        }
        public string PaymentRenewal
        {
            get { return GetValue(() => PaymentRenewal); }
            set { SetValue(() => PaymentRenewal, value); }
        }

        public string ItemsMgmt
        {
            get { return GetValue(() => ItemsMgmt); }
            set { SetValue(() => ItemsMgmt, value); }
        }
        public string OnHandMgmt
        {
            get { return GetValue(() => OnHandMgmt); }
            set { SetValue(() => OnHandMgmt, value); }
        }

        public string ReceiveStock
        {
            get { return GetValue(() => ReceiveStock); }
            set { SetValue(() => ReceiveStock, value); }
        }
        public string SellStock
        {
            get { return GetValue(() => SellStock); }
            set { SetValue(() => SellStock, value); }
        }
        public string TransferStock
        {
            get { return GetValue(() => TransferStock); }
            set { SetValue(() => TransferStock, value); }
        }

        public string TransferRequest
        {
            get { return GetValue(() => TransferRequest); }
            set { SetValue(() => TransferRequest, value); }
        }
        public string TransferSend
        {
            get { return GetValue(() => TransferSend); }
            set { SetValue(() => TransferSend, value); }
        }
        public string TransferReceive
        {
            get { return GetValue(() => TransferReceive); }
            set { SetValue(() => TransferReceive, value); }
        }
        
        public string ExpenseEntry
        {
            get { return GetValue(() => ExpenseEntry); }
            set { SetValue(() => ExpenseEntry, value); }
        }
        public string ExpenseEdit
        {
            get { return GetValue(() => ExpenseEdit); }
            set { SetValue(() => ExpenseEdit, value); }
        }
        public string ExpenseDelete
        {
            get { return GetValue(() => ExpenseDelete); }
            set { SetValue(() => ExpenseDelete, value); }
        }

        public string SuppliersEntry
        {
            get { return GetValue(() => SuppliersEntry); }
            set { SetValue(() => SuppliersEntry, value); }
        }

        #endregion
    }
}