using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Extensions;

namespace PinnaRent.Core.Models
{
    public partial class RentalPaymentDTO : CommonFieldsA
    {
        public RentalPaymentDTO()
        {
            Payments = new HashSet<PaymentDTO>();
            PaymentRemarks=new HashSet<RentalPaymentRemarkDTO>();
            DaysLeft = 1000;
        }

        [ForeignKey("Contrat")]
        public int ContratId { get; set; }
        public RentalContratDTO Contrat
        {
            get { return GetValue(() => Contrat); }
            set { SetValue(() => Contrat, value); }
        }

        public int? PreviousPaymentId//To Handle New or is Renewed
        {
            get { return GetValue(() => PreviousPaymentId); }
            set { SetValue(() => PreviousPaymentId, value); }
        }

        ////Service Or Rent Or Other
        [Required]
        public PaymentTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        //public int? PreviousPaymentId //To Handle New/IsRenewed
        //{
        //    get { return GetValue(() => PreviousPaymentId); }
        //    set { SetValue(() => PreviousPaymentId, value); }
        //}

        //Will serve as the day the customer pays in cash/deposited to bank... 
        //Penality will be calculated based on the difference between this date and 
        //The previous payment enddate using PreviousPaymentId attribute
        public DateTime PaymentDate
        {
            get { return GetValue(() => PaymentDate); }
            set
            {
                SetValue(() => PaymentDate, value);
                SetValue<string>(() => PaymentDateStringAmharic, value.ToLongDateString());
                //SetValue<string>(() => PaymentDateStringAndAmharic, value.ToLongDateString());
            }
        }

        //Should be automatic for renewal, no one should be able to change
        public DateTime StartDate
        {
            get { return GetValue(() => StartDate); }
            set
            {
                SetValue(() => StartDate, value);
                SetValue<string>(() => StartDateStringAmharic, value.ToLongDateString());
                SetValue<string>(() => StartDateStringAndAmharic, value.ToLongDateString());
            }
        }

        //Should be automatic for both new and renewal, no one should be able to change
        public DateTime EndDate
        {
            get { return GetValue(() => EndDate); }
            set
            {
                SetValue(() => EndDate, value);
                SetValue<string>(() => EndDateStringAmharic, value.ToLongDateString());
                SetValue<string>(() => EndDateStringAndAmharic, value.ToLongDateString());
            }
        }

        [Required]
        [Range(1, 12)]
        public int? PaymentPeriod //Number of Months
        {
            get { return GetValue(() => PaymentPeriod); }
            set { SetValue(() => PaymentPeriod, value); }
        }

        //[Range(1, 30)]
        public int? AdditionalDays
        {
            get { return GetValue(() => AdditionalDays); }
            set { SetValue(() => AdditionalDays, value); }
        }

        [Range(1.0, 10000000.0)]
        public decimal AmountRequired
        {
            get { return GetValue(() => AmountRequired); }
            set { SetValue(() => AmountRequired, value); }
        }

        public int OverDueDays
        {
            get { return GetValue(() => OverDueDays); }
            set { SetValue(() => OverDueDays, value); }
        }

        public decimal? Penality
        {
            get { return GetValue(() => Penality); }
            set { SetValue(() => Penality, value); }
        }

        public decimal TotalAmountRequired
        {
            get { return GetValue(() => TotalAmountRequired); }
            set { SetValue(() => TotalAmountRequired, value); }
        }

        public decimal? TotalAmountPaid
        {
            get { return GetValue(() => TotalAmountPaid); }
            set { SetValue(() => TotalAmountPaid, value); }
        }

        public ICollection<PaymentDTO> Payments
        {
            get { return GetValue(() => Payments); }
            set { SetValue(() => Payments, value); }
        }

        public ICollection<RentalPaymentRemarkDTO> PaymentRemarks
        {
            get { return GetValue(() => PaymentRemarks); }
            set { SetValue(() => PaymentRemarks, value); }
        }

        [NotMapped]
        public string NoOfRemarks
        {
            get
            {
                if (PaymentRemarks != null)
                {
                   if (PaymentRemarks.Count > 0)
                   return PaymentRemarks.Count.ToString("N0"); 
                }
                return "";
            }
            set { SetValue(() => NoOfRemarks, value); }
        }
    }
    
    public partial class RentalPaymentDTO
    {
        public ReceiptTypes ReceiptType
        {
            get { return GetValue(() => ReceiptType); }
            set { SetValue(() => ReceiptType, value); }
        }

        [Required]
        [MaxLength(10, ErrorMessage = "Name Exceeded 10 letters")]
        [DisplayName("Receipt Number")]
        //[Index(IsUnique = true)]
        public string ReceiptNumber
        {
            get { return GetValue(() => ReceiptNumber); }
            set { SetValue(() => ReceiptNumber, value); }
        }

        public DateTime ReceiptDate
        {
            get { return GetValue(() => ReceiptDate); }
            set
            {
                SetValue(() => ReceiptDate, value);
                SetValue<string>(() => ReceiptDateStringAmharic, value.ToLongDateString());
            }
        }
        
        [StringLength(255)]
        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }

        [NotMapped]
        public bool IsArchived
        {
            get { return GetValue(() => IsArchived); }
            set { SetValue(() => IsArchived, value); }
        }

        public decimal? RentAmount
        {
            get{ return GetValue(() => RentAmount); }
            set{ SetValue(() => RentAmount, value); }
        }
        
        public decimal? ServiceAmount
        {
            get { return GetValue(() => ServiceAmount); }
            set { SetValue(() => ServiceAmount, value); }
        }
        
        public decimal? WithholdAmount
        {
            get
            {
                return GetValue(() => WithholdAmount);//RentAmount*(decimal)0.02;  
            }
            set{ SetValue(() => WithholdAmount, value); }
        }

        public decimal? ExtraPaymentAmount
        {
            get
            {
                return GetValue(() => ExtraPaymentAmount);
            }
            set
            {
                SetValue(() => ExtraPaymentAmount, value);
            }
        }

        public decimal? TotalPenalityAmount
        {
            get
            {
                return GetValue(() => TotalPenalityAmount);
            }
            set
            {
                SetValue(() => TotalPenalityAmount, value);
            }
        }

        [StringLength(255)]
        public string ExtraPaymentComments
        {
            get { return GetValue(() => ExtraPaymentComments); }
            set { SetValue(() => ExtraPaymentComments, value); }
        }

        [ForeignKey("ServicePayment")]
        public int? ServicePaymentId { get; set; }
        public RentalPaymentDTO ServicePayment
        {
            get { return GetValue(() => ServicePayment); }
            set { SetValue(() => ServicePayment, value); }
        }

        #region NotMapped Attributes
        [NotMapped]
        public string PaymentMadeFor
        {
            get
            {
                string madefor="";
                if(Contrat.Room==null)
                    return "";
                if (Type == PaymentTypes.Service)
                {
                    madefor = "የአገልግሎት";
                    if (TotalPenalityAmount != null && TotalPenalityAmount > 0)
                        madefor += ", የቅጣት";
                }
                if (Type == PaymentTypes.Rent)
                {
                    madefor = "የኪራይ";
                    if (ServicePayment != null && ServicePayment.TotalAmountRequired > 0)
                        madefor += ", የአገልግሎት";
                    if (TotalPenalityAmount != null && TotalPenalityAmount > 0)
                        madefor += ", የቅጣት";
                }

                return madefor;
            }
            set { SetValue(() => PaymentMadeFor, value); }
        }
        [NotMapped]
        public string PaymentNumber
        {
            get
            {
                return "P00" + Id;
            }
            set { SetValue(() => PaymentNumber, value); }
        }

        [NotMapped]
        public string TotalAmountRequiredString
        {
            get
            {
                if (TotalAmountRequired != 0)
                    return TotalAmountRequired.ToString("N2");
                return "";
            }
            set { SetValue(() => TotalAmountRequiredString, value); }
        }
        [NotMapped]
        public decimal TotalAmountRequiredBeforeVat
        {
            get
            {
                if (TotalAmountRequired != 0)
                    return (TotalAmountRequired / (decimal)1.15);
                return 0;
            }
            set { SetValue(() => TotalAmountRequiredBeforeVat, value); }
        }
        [NotMapped]
        public decimal TotalAmountRequiredVatAmount
        {
            get
            {
                if (TotalAmountRequired != 0)
                    return ((TotalAmountRequired / (decimal)1.15) * (decimal)0.15);
                return 0;
            }
            set { SetValue(() => TotalAmountRequiredVatAmount, value); }
        }

        [NotMapped]
        public string AmountRequiredString
        {
            get
            {
                if (AmountRequired != 0)
                    return AmountRequired.ToString("N2");
                return "";
            }
            set { SetValue(() => AmountRequiredString, value); }
        }
        [NotMapped]
        public decimal AmountRequiredBeforeVat
        {
            get
            {
                if (AmountRequired != 0)
                    return (AmountRequired / (decimal)1.15);
                return 0;
            }
            set { SetValue(() => AmountRequiredBeforeVat, value); }
        }
        [NotMapped]
        public decimal AmountRequiredVatAmount
        {
            get
            {
                if (AmountRequired != 0)
                    return ((AmountRequired / (decimal)1.15) * (decimal)0.15);
                return 0;
            }
            set { SetValue(() => AmountRequiredVatAmount, value); }
        }

        [NotMapped]
        public string TotalPenalityAmountString
        {
            get
            {
                if (TotalPenalityAmount!=null && TotalPenalityAmount != 0)
                    return TotalPenalityAmount.ToString();
                return "";
            }
            set { SetValue(() => TotalPenalityAmountString, value); }
        }
        [NotMapped]
        public decimal TotalPenalityAmountBeforeVat
        {
            get
            {
                if (TotalPenalityAmount!=null && TotalPenalityAmount != 0)
                    return (decimal) (TotalPenalityAmount / (decimal)1.15);
                return 0;
            }
            set { SetValue(() => TotalPenalityAmountBeforeVat, value); }
        }
        [NotMapped]
        public decimal TotalPenalityAmountVatAmount
        {
            get
            {
                if (TotalPenalityAmount!=null && TotalPenalityAmount != 0)
                    return (decimal) ((TotalPenalityAmount / (decimal)1.15) * (decimal)0.15);
                return 0;
            }
            set { SetValue(() => TotalPenalityAmountVatAmount, value); }
        }
        
        [NotMapped]
        public string PaymentPeriodString
        {
            get
            {
                if (PaymentPeriod!=null && PaymentPeriod != 0)
                    return PaymentPeriod.ToString() + " ወር";
                return "";
            }
            set { SetValue(() => PaymentPeriodString, value); }
        }
        
        [NotMapped]
        public decimal UnPaidAmount
        {
            get
            {
                var overdueMonths = CommonUtility.GetMonthsFromDays(EndDate, DateTime.Now);
                var fee = (decimal) 0;
                if (Contrat != null)
                {
                    if (Contrat.Room != null)
                    {
                        var @decimal = Type == PaymentTypes.Rent ? Contrat.Room.RentalFee : Contrat.Room.ServiceFee;
                        if (@decimal != null)
                            fee = (decimal) @decimal;
                    }
                }
                    
                return overdueMonths*fee;
            }
            set { SetValue(() => UnPaidAmount, value); }
        }
        [NotMapped]
        public string UnPaidAmountString
        {
            get
            {
                var tot = UnPaidAmount;
                if (tot > 0)
                    return "-"+tot.ToString("N2");
                else return "";// "የለበትም";
            }
            set { SetValue(() => UnPaidAmountString, value); }
        }
        
        [NotMapped]
        public string AmountPaidString
        {
            get
            {
                if (TotalAmountPaid!=null && TotalAmountPaid != 0)
                    return ((decimal)TotalAmountPaid).ToString("N2");
                return "";
            }
            set { SetValue(() => AmountPaidString, value); }
        }
        [NotMapped]
        public string AmountPaidBeforeVat
        {
            get
            {
                if (TotalAmountPaid != null && TotalAmountPaid != 0)
                    return ((decimal)TotalAmountPaid / (decimal)1.15).ToString("N2");
                return "";
            }
            set { SetValue(() => AmountPaidBeforeVat, value); }
        }
        [NotMapped]
        public string VatAmount
        {
            get
            {
                if (TotalAmountPaid != null && TotalAmountPaid != 0)
                    return (((decimal)TotalAmountPaid / (decimal)1.15) * (decimal)0.15).ToString("N2");
                return "";
            }
            set { SetValue(() => VatAmount, value); }
        }
        
        [NotMapped]
        public bool PaymentExpired
        {
            get
            {
                return DaysLeft < 0;
            }
            set { SetValue(() => PaymentExpired, value); }
        }
        [NotMapped]
        public int DaysLeft
        {
            get
            {
                if (EndDate.Year < 2000)
                    return 0;

                var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                var dl=EndDate.Subtract(today).Days;
                return dl;
            }
            set
            {
                SetValue(() => DaysLeft, value);
                SetValue(() => DaysLeftString, value.ToString());
            }
        }
        [NotMapped]
        public string DaysLeftString
        {
            get
            {
                if (EndDate.Year < 2000 || DaysLeft>366)
                    return "";
                return DaysLeft.ToString();
            }
            set { SetValue(() => DaysLeftString, value); }
        }
        [NotMapped]
        public string PaymentStatus
        {
            get
            {
                return DateTime.Now >= EndDate ?
                    EnumUtil.GetEnumDesc(ContratStatusTypes.Expired) :
                    EnumUtil.GetEnumDesc(ContratStatusTypes.Active);
            }
            set { SetValue(() => PaymentStatus, value); }
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
        public string StartDateStringAndAmharic
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return StartDate.ToString("dd/MM/yyyy") + " (" + ReportUtility.GetEthCalendarFormated(StartDate, "/") + ")";

            }
            set { SetValue(() => StartDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string StartDateString
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return StartDate.ToString("dd/MM/yyyy");

            }
            set { SetValue(() => StartDateString, value); }
        }
        [NotMapped]
        public string StartDateStringAmharic
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return ReportUtility.GetEthCalendar(StartDate, true);//"-"

            }
            set { SetValue(() => StartDateStringAmharic, value); }
        }
        [NotMapped]
        public string StartDateStringAmharicFormatted
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return ReportUtility.GetEthCalendarFormated(StartDate, "/");
            }
            set { SetValue(() => StartDateStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string EndDateStringAndAmharic
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return EndDate.ToString("dd/MM/yyyy") + " (" + ReportUtility.GetEthCalendarFormated(EndDate, "/") + ")";
            }
            set { SetValue(() => EndDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string EndDateString
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return EndDate.ToString("dd/MM/yyyy");
            }
            set { SetValue(() => EndDateString, value); }
        }
        [NotMapped]
        public string EndDateStringAmharic
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return ReportUtility.GetEthCalendar(EndDate, true);
            }
            set { SetValue(() => EndDateStringAmharic, value); }
        }
        [NotMapped]
        public string EndDateStringAmharicFormatted
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return ReportUtility.GetEthCalendarFormated(EndDate, "/");
            }
            set { SetValue(() => EndDateStringAmharicFormatted, value); }
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

        //[NotMapped]
        //public int OverDueDays
        //{
        //    get
        //    {
        //        if (Contrat != null && (Contrat.Room != null && Contrat.Room.LastRentalPayment != null))
        //        {
        //            var lastEndDate=Contrat.Room.LastRentalPayment.EndDate;
        //            lastEndDate=new DateTime(lastEndDate.Year,lastEndDate.Month,lastEndDate.Day,23,59,59);
        //            var paymentDay = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day, 23, 59, 59);
        //            return paymentDay.Subtract(lastEndDate).Days;
        //        }

        //        return 15;
        //    }
        //    set { SetValue(() => OverDueDays, value); }
        //}
        //[NotMapped]
        //public decimal PenalityAmount
        //{
        //    get
        //    {
        //        decimal overdueMonths = OverDueDays/30;
        //        overdueMonths = overdueMonths + 1;
        //        //if (Contrat != null && (Contrat.Room != null && Contrat.Room.RentalFee != null))
        //        //{
        //        var roomAmount = 15000;// (decimal) Contrat.Room.RentalFee;
        //            return (decimal)0.1 * roomAmount * overdueMonths;
        //        //}

        //        //return 0;
        //    }
        //    set { SetValue(() => PenalityAmount, value); }
        //}
        //[ForeignKey("PenalityPayment")]
        //public int? PenalityPaymentId { get; set; }
        //public PaymentDTO PenalityPayment
        //{
        //    get { return GetValue(() => PenalityPayment); }
        //    set { SetValue(() => PenalityPayment, value); }
        //}

        ////Coverted to AmountRequired and Should be NotMapped to easily get the difference
        //[Range(1.0, 10000000.0)]
        //public decimal AmountPaid
        //{
        //    get { return GetValue(() => AmountPaid); }
        //    set { SetValue(() => AmountPaid, value); }
        //}

        //[ForeignKey("ServicePayment")]
        //public int RentOrServicePaymentId { get; set; }
        //public PaymentDTO ServicePayment
        //{
        //    get { return GetValue(() => ServicePayment); }
        //    set { SetValue(() => ServicePayment, value); }
        //}

        //[Range(0, 1000000.0)]
        //public decimal? Penality//will be deleted after new paymentDTO tested
        //{
        //    get { return GetValue(() => Penality); }
        //    set { SetValue(() => Penality, value); }
        //}
    }
}