using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaRent.Core
{
    public class CommonFieldsG : CommonFieldsA
    { 
        public DateTime PaymentDate
        {
            get { return GetValue(() => PaymentDate); }
            set
            {
                SetValue(() => PaymentDate, value);
                SetValue<string>(() => PaymentDateStringAmharic, value.ToLongDateString());

            }
        }
        public string PaymentNumber
        {
            get { return GetValue(() => PaymentNumber); }
            set { SetValue(() => PaymentNumber, value); }
        }

        public DateTime StartDate
        {
            get { return GetValue(() => StartDate); }
            set
            {
                SetValue(() => StartDate, value);
                SetValue<string>(() => StartDateStringAmharic, value.ToLongDateString());
            }
        }
        public DateTime EndDate
        {
            get { return GetValue(() => EndDate); }
            set
            {
                SetValue(() => EndDate, value);
                SetValue<string>(() => EndDateStringAmharic, value.ToLongDateString());
            }
        }
        public int ContractPeriod//Number of Months
        {
            get { return GetValue(() => ContractPeriod); }
            set { SetValue(() => ContractPeriod, value); }
        }
        
        public decimal AmountPaid
        {
            get { return GetValue(() => AmountPaid); }
            set { SetValue(() => AmountPaid, value); }
        }

        [Required]
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
        
        public bool IsBlocked
        {
            get { return GetValue(() => IsBlocked); }
            set { SetValue(() => IsBlocked, value); }
        }

        [StringLength(255)]
        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }

        [NotMapped]
        public string PaymentDateStringAndAmharic
        {
            get
            {
                return PaymentDate.ToString("dd-MM-yyyy") + "(" + ReportUtility.GetEthCalendarFormated(PaymentDate, "-") + ")";
            }
            set { SetValue(() => PaymentDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string PaymentDateString
        {
            get
            {
                return PaymentDate.ToString("dd-MM-yyyy");
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
              return ReportUtility.GetEthCalendarFormated(PaymentDate, "-");
            }
            set { SetValue(() => PaymentDateStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string StartDateStringAndAmharic
        {
            get
            {
                return StartDate.ToString("dd-MM-yyyy") + " (" + ReportUtility.GetEthCalendarFormated(StartDate, "-") + ")";
                
            }
            set { SetValue(() => StartDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string StartDateString
        {
            get
            {
                    return StartDate.ToString("dd-MM-yyyy");
                
            }
            set { SetValue(() => StartDateString, value); }
        }
        [NotMapped]
        public string StartDateStringAmharic
        {
            get
            {
                if (StartDate != null)
                    return ReportUtility.GetEthCalendar(StartDate, true);//"-"
                return "";
            }
            set { SetValue(() => StartDateStringAmharic, value); }
        }
        [NotMapped]
        public string StartDateStringAmharicFormatted
        {
            get
            {
                if (StartDate != null)
                    return ReportUtility.GetEthCalendarFormated(StartDate, "-");//"-"
                return "";
            }
            set { SetValue(() => StartDateStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string EndDateStringAndAmharic
        {
            get
            {
                if (EndDate != null)
                    return EndDate.ToString("dd-MM-yyyy") + " (" + ReportUtility.GetEthCalendarFormated(EndDate, "-") + ")";
                return "";
            }
            set { SetValue(() => EndDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string EndDateString
        {
            get
            {
                if (EndDate != null)
                    return EndDate.ToString("dd-MM-yyyy");
                return "";
            }
            set { SetValue(() => EndDateString, value); }
        }
        [NotMapped]
        public string EndDateStringAmharic
        {
            get
            {
                if (EndDate != null)
                    return ReportUtility.GetEthCalendar(EndDate, true);
                return "";
            }
            set { SetValue(() => EndDateStringAmharic, value); }
        }
        [NotMapped]
        public string EndDateStringAmharicFormatted
        {
            get
            {
                if (EndDate != null)
                    return ReportUtility.GetEthCalendarFormated(EndDate, "-");
                return "";
            }
            set { SetValue(() => EndDateStringAmharicFormatted, value); }
        }
    }
}