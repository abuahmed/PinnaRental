using System;
using System.ComponentModel.DataAnnotations;
using PinnaRent.Core.Enumerations;

namespace PinnaRent.Core.Models
{
    public class SettingDTO : ContratSettingDTO
    {
        

        #region Advanced Settings
        public bool EnableSeparteServicePayment
        {
            get { return GetValue(() => EnableSeparteServicePayment); }
            set { SetValue(() => EnableSeparteServicePayment, value); }
        }

        public bool EnablePenality
        {
            get { return GetValue(() => EnablePenality); }
            set { SetValue(() => EnablePenality, value); }
        }

        public string PaymentWithoutPenalityDays
        {
            get { return GetValue(() => PaymentWithoutPenalityDays); }
            set { SetValue(() => PaymentWithoutPenalityDays, value); }
        }
        
        public decimal? PenalityPercent
        {
            get { return GetValue(() => PenalityPercent); }
            set { SetValue(() => PenalityPercent, value); }
        }
        
        public bool EnablePaymentsWithoutReceipt
        {
            get { return GetValue(() => EnablePaymentsWithoutReceipt); }
            set { SetValue(() => EnablePaymentsWithoutReceipt, value); }
        }

        public bool EnableAdditionalDays
        {
            get { return GetValue(() => EnableAdditionalDays); }
            set { SetValue(() => EnableAdditionalDays, value); }
        }
        #endregion

        #region Tax Settings
        public TaxTypes TaxType
        {
            get { return GetValue(() => TaxType); }
            set { SetValue(() => TaxType, value); }
        }
        [Range(0, 100)]
        public decimal TaxPercent
        {
            get { return GetValue(() => TaxPercent); }
            set { SetValue(() => TaxPercent, value); }
        }
        public bool ByDefaultItemsHaveThisTaxRate
        {
            get { return GetValue(() => ByDefaultItemsHaveThisTaxRate); }
            set { SetValue(() => ByDefaultItemsHaveThisTaxRate, value); }
        }
        public bool ItemPricesAreTaxInclusive
        {
            get { return GetValue(() => ItemPricesAreTaxInclusive); }
            set { SetValue(() => ItemPricesAreTaxInclusive, value); }
        }
        #endregion

        #region More Advanced Settings
        public bool HandleBankTransaction
        {
            get { return GetValue(() => HandleBankTransaction); }
            set { SetValue(() => HandleBankTransaction, value); }
        }
        public bool EnableExpenses
        {
            get { return GetValue(() => EnableExpenses); }
            set { SetValue(() => EnableExpenses, value); }
        }

        public bool EnableCheckEntry
        {
            get { return GetValue(() => EnableCheckEntry); }
            set { SetValue(() => EnableCheckEntry, value); }
        }
        public bool EnableInventory
        {
            get { return GetValue(() => EnableInventory); }
            set { SetValue(() => EnableInventory, value); }
        }
        #endregion

        #region Sync Status
        public DateTime? LastToServerSyncDate
        {
            get { return GetValue(() => LastToServerSyncDate); }
            set { SetValue(() => LastToServerSyncDate, value); }
        }
        public DateTime? LastFromServerSyncDate
        {
            get { return GetValue(() => LastFromServerSyncDate); }
            set { SetValue(() => LastFromServerSyncDate, value); }
        }
        #endregion
    }

}
