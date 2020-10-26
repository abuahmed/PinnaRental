using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Extensions;

namespace PinnaRent.Core.Models
{
    public class PaymentDTO : CommonFieldsA
    {
        [ForeignKey("Account")]
        public int? AccountId { get; set; }
        public ChartofAccountDTO Account
        {
            get { return GetValue(() => Account); }
            set { SetValue(() => Account, value); }
        }

        [ForeignKey("RentalPayment")]
        public int? RentalPaymentId { get; set; }
        public RentalPaymentDTO RentalPayment
        {
            get { return GetValue(() => RentalPayment); }
            set { SetValue(() => RentalPayment, value); }
        }

        [ForeignKey("RentDeposit")]
        public int? RentDepositId { get; set; }
        public RentDepositDTO RentDeposit
        {
            get { return GetValue(() => RentDeposit); }
            set { SetValue(() => RentDeposit, value); }
        }

        [Required]
        public PaymentTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        [Required]
        public DateTime PaymentDate
        {
            get { return GetValue(() => PaymentDate); }
            set { SetValue(() => PaymentDate, value); }
        }

        [Required]
        public decimal Amount
        {
            get { return GetValue(() => Amount); }
            set { SetValue(() => Amount, value); }
        }

        public ReceiptTypes ReceiptType
        {
            get { return GetValue(() => ReceiptType); }
            set { SetValue(() => ReceiptType, value); }
        }

        //[Required]
        [MaxLength(10, ErrorMessage = "Name Exceeded 10 letters")]
        [DisplayName("Receipt Number")]
        public string ReceiptNumber
        {
            get { return GetValue(() => ReceiptNumber); }
            set { SetValue(() => ReceiptNumber, value); }
        }

        public DateTime ReceiptDate
        {
            get { return GetValue(() => ReceiptDate); }
            set { SetValue(() => ReceiptDate, value); }
        }

        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }

        public bool IncludesTax
        {
            get { return GetValue(() => IncludesTax); }
            set { SetValue(() => IncludesTax, value); }
        }
        
        public decimal? WithholdAmount
        {
            get
            {
                return Amount * (decimal)0.02;
            }
            set
            {
                SetValue(() => WithholdAmount, value);
            }
        }

        public string CheckNumber
        {
            get
            {
                return GetValue(() => CheckNumber);
            }
            set { SetValue(() => CheckNumber, value); }
        }

        [ForeignKey("Check")]
        public int? CheckId { get; set; }
        public CheckDTO Check
        {
            get { return GetValue(() => Check); }
            set { SetValue(() => Check, value); }
        }
        
        //public ICollection<PaymentClearanceDTO> Clearances
        //{
        //    get { return GetValue(() => Clearances); }
        //    set { SetValue(() => Clearances, value); }
        //}

        //[NotMapped]
        //public string CheckNumber
        //{
        //    get
        //    {
        //        return Check != null ? Check.CheckNumber : "";
        //    }
        //    set { SetValue(() => CheckNumber, value); }
        //}

        [Required]//Or Served as Description
        public string Reason
        {
            get { return GetValue(() => Reason); }
            set { SetValue(() => Reason, value); }
        }

        [MaxLength(50, ErrorMessage = "Exceeded 50 letters")]//Or Served as UOM
        public string UnitOfMeasure
        {
            get { return GetValue(() => UnitOfMeasure); }
            set { SetValue(() => UnitOfMeasure, value); }
        }

        //[MaxLength(50, ErrorMessage = "Exceeded 50 letters")]//Or Served as Quantity
        public int? UnitQty
        {
            get { return GetValue(() => UnitQty); }
            set { SetValue(() => UnitQty, value); }
        }
        //[MaxLength(50, ErrorMessage = "Exceeded 50 letters")]//Or Served as EachPrice
        public decimal? EachPrice
        {
            get { return GetValue(() => EachPrice); }
            set { SetValue(() => EachPrice, value); }
        }

        [MaxLength(50, ErrorMessage = "Exceeded 50 letters")]
        public string PersonName
        {
            get { return GetValue(() => PersonName); }
            set { SetValue(() => PersonName, value); }
        }

        public PaymentMethods PaymentMethod
        {
            get { return GetValue(() => PaymentMethod); }
            set { SetValue(() => PaymentMethod, value); }
        }

        public PaymentStatus Status
        {
            get { return GetValue(() => Status); }
            set { SetValue(() => Status, value); }
        }

        [ForeignKey("BusinessPartner")]
        public int? BusinessPartnerId { get; set; }
        public BusinessPartnerDTO BusinessPartner
        {
            get { return GetValue(() => BusinessPartner); }
            set { SetValue(() => BusinessPartner, value); }
        }

        [ForeignKey("Transaction")]
        public int? TransactionId { get; set; }
        public TransactionHeaderDTO Transaction
        {
            get { return GetValue(() => Transaction); }
            set { SetValue(() => Transaction, value); }
        }
     
        [ForeignKey("Warehouse")]
        public int? WarehouseId { get; set; }
        public WarehouseDTO Warehouse
        {
            get { return GetValue(() => Warehouse); }
            set { SetValue(() => Warehouse, value); }
        }
         /***/
        
        #region Not Mapped Attributes
        [NotMapped]
        public string ReceiverName
        {
            get
            {
                if (!String.IsNullOrEmpty(PersonName))
                    return PersonName;
                else if (BusinessPartner != null)
                    return BusinessPartner.DisplayName;

                return "";
            }
            set { SetValue(() => ReceiverName, value); }
        }

        [NotMapped]
        public string Number
        {
            get
            {
                int id = 10000 + Id;

                return "DP0" + id.ToString().Substring(1);
            }
            set { SetValue(() => Number, value); }
        }

        [NotMapped]
        public string PaymentDateStringAndAmharic
        {
            get
            {
                return PaymentDate.ToString("dd/MM/yyyy") + "(" + ReportUtility.GetEthCalendarFormated(PaymentDate, "/") + ")";
            }
            set { SetValue(() => PaymentDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string PaymentDateString
        {
            get
            {
                return PaymentDate.ToString("dd/MM/yyyy");
            }
            set { SetValue(() => PaymentDateString, value); }
        }
        [NotMapped]
        public string PaymentDateStringAmharic
        {
            get
            {
                return ReportUtility.GetEthCalendar(PaymentDate, true);
            }
            set { SetValue(() => PaymentDateStringAmharic, value); }
        }
        [NotMapped]
        public string PaymentDateStringAmharicFormatted
        {
            get
            {
                return ReportUtility.GetEthCalendarFormated(PaymentDate, "/");
            }
            set { SetValue(() => PaymentDateStringAmharicFormatted, value); }
        }


        [NotMapped]
        public string AmountString
        {
            get
            {
                if (Amount != 0)
                    return Amount.ToString("N2");
                return "";
            }
            set { SetValue(() => AmountString, value); }
        }
        [NotMapped]
        public string AmountAfterVat
        {
            get
            {
                if (Amount != 0)
                    return (Amount * (decimal)1.15).ToString("N2");
                return "";
            }
            set { SetValue(() => AmountAfterVat, value); }
        }
        [NotMapped]
        public string VatAmount
        {
            get
            {
                if (Amount != 0)
                    return (Amount * (decimal)0.15).ToString("N2");
                return "";
            }
            set { SetValue(() => VatAmount, value); }
        }

        [NotMapped]
        [DisplayName("Payment Type")]
        public string PaymentTypeString
        {
            get
            {
                return EnumUtil.GetEnumDesc(Type);
            }
            set { SetValue(() => PaymentTypeString, value); }
        }

        [NotMapped]
        public string ReceiptDateStringAndAmharic
        {
            get
            {
                if (ReceiptDate.Year < 2000)
                    return "";
                return ReceiptDate.ToString("dd/MM/yyyy") + " (" + ReportUtility.GetEthCalendarFormated(ReceiptDate, "/") + ")";
            }
            set { SetValue(() => ReceiptDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string ReceiptDateString
        {
            get
            {
                if (ReceiptDate.Year < 2000)
                    return "";
                return ReceiptDate.ToString("dd/MM/yyyy");
            }
            set { SetValue(() => ReceiptDateString, value); }
        }
        [NotMapped]
        public string ReceiptDateStringAmharic
        {
            get
            {
                if (ReceiptDate.Year < 2000)
                    return "";
                return ReportUtility.GetEthCalendar(ReceiptDate, true);
            }
            set { SetValue(() => ReceiptDateStringAmharic, value); }
        }
        [NotMapped]
        public string ReceiptDateStringAmharicFormatted
        {
            get
            {
                if (ReceiptDate.Year < 2000)
                    return "";
                return ReportUtility.GetEthCalendarFormated(ReceiptDate, "/");
            }
            set { SetValue(() => ReceiptDateStringAmharicFormatted, value); }
        } 
        #endregion
    }
}
