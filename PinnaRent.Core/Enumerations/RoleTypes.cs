using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum RoleTypes
    {
        #region Options
        [Description("Options")]
        Settings,
        [Description("Advanced Options")]
        AdvancedSettings,
        [Description("Tax Options")]
        TaxSettings,
        [Description("General Options")]
        GeneralSettings,
        #endregion

        [Description("Users Mgt")]
        UsersMgmt,
        [Description("Users Privilege Mgmt")]
        UsersPrivilegeMgmt,
        [Description("Backup and Restore Mgmt")]
        BackupRestore,
        [Description("Company Mgmt")]
        CompanyMgmt,
        [Description("Store Mgmt")]
        WarehouseMgmt,
      
        [Description("Rooms Mgmt")]
        RoomEntry,
        [Description("Rooms Edit")]
        RoomEdit,
        [Description("Rooms Delete")]
        RoomDelete,

        [Description("Rentees Mgmt")]
        RenteeEntry,
        [Description("Rentees Edit")]
        RenteeEdit,
        [Description("Rentees Delete")]
        RenteeDelete,

        [Description("Adding Contrats")]
        ContratAdd,
        [Description("Editing Contrats")]
        ContratEdit,
        [Description("Renewing Contrats")]
        ContratRenewal,
        [Description("Cancelling Contrats")]
        ContratCancellation,

        [Description("Adding Payments")]
        PaymentAdd,
        [Description("Renewing Payments")]
        PaymentRenewal,
        [Description("Editing Payments")]
        PaymentEdit,
        [Description("Deleting Payments")]
        PaymentDeletion,
        
        [Description("DashBoard Management")]
        DashBoardMgmt,
        
        [Description("Maraki Reports")]
        PosReports,

        [Description("Items Management")]
        ItemsMgmt,
        [Description("OnHand Management")]
        OnHandMgmt,

        [Description("Receive Stock ")]
        ReceiveStock,
        [Description("Sell Stock")]
        SellStock,
        [Description("Transfer Stock")]
        TransferStock,

        [Description("Transfer Request")]
        TransferRequest,
        [Description("Transfer Send")]
        TransferSend,
        [Description("Transfer Receive")]
        TransferReceive,

        [Description("Expense Entry")]
        ExpenseEntry,
        [Description("Expense Edit")]
        ExpenseEdit,
        [Description("Expense Delete")]
        ExpenseDelete,

        [Description("Suppliers Entry")]
        SupplierEntry,

    }
}
