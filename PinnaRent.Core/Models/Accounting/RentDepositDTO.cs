using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaRent.Core.Models
{
    public class RentDepositDTO : CommonFieldsA
    {
        public RentDepositDTO()
        {
            Payments=new HashSet<PaymentDTO>();
        }

        [ForeignKey("Contrat")]
        public int ContratId { get; set; }
        public RentalContratDTO Contrat
        {
            get { return GetValue(() => Contrat); }
            set { SetValue(() => Contrat, value); }
        }
        
        //[ConcurrencyCheck]
        [Range(0, 12)]
        public int DepositMonths
        {
            get { return GetValue(() => DepositMonths); }
            set { SetValue(() => DepositMonths, value); }
        }

        public decimal TotalDepositAmount
        {
            get { return GetValue(() => TotalDepositAmount); }
            set { SetValue(() => TotalDepositAmount, value); }
        }

        //This is the same as Payment Date on PaymentDTO object
        [DataType(DataType.Date)]
        public DateTime DepositedDate
        {
            get { return GetValue(() => DepositedDate); }
            set { SetValue(() => DepositedDate, value); }
        }

        public DateTime? ReturnedDate
        {
            get { return GetValue(() => ReturnedDate); }
            set
            {
                SetValue(() => ReturnedDate, value);
                if (value != null) SetValue(() => ReturnedDateStringAmharic, value.Value.ToLongDateString());
                else SetValue(() => ReturnedDateStringAmharic, "");
            }
        }

        public DateTime? UsedDate
        {
            get { return GetValue(() => UsedDate); }
            set
            {
                SetValue(() => UsedDate, value);
                if (value != null) SetValue(() => UsedDateStringAmharic, value.Value.ToLongDateString());
                else SetValue(() => UsedDateStringAmharic, "");
            }
        }

        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }

        public ICollection<PaymentDTO> Payments
        {
            get { return GetValue(() => Payments); }
            set { SetValue(() => Payments, value); }
        }

        //[Timestamp]
        //public byte[] RowVersion { get; set; }

        [NotMapped]
        public string TotalDepositAmountString
        {
            get
            {
                if (TotalDepositAmount!=0)
                return TotalDepositAmount.ToString("N2");
                return "";
            }
            set { SetValue(() => TotalDepositAmountString, value); }
        }

        [NotMapped]
        public string DepositedDateStringAndAmharic
        {
            get
            {
                return DepositedDate.ToString("dd/MM/yyyy") + "(" + ReportUtility.GetEthCalendarFormated(DepositedDate, "/") + ")";
            }
            set { SetValue(() => DepositedDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string DepositedDateString
        {
            get
            {
                return DepositedDate.ToString("dd/MM/yyyy");
            }
            set { SetValue(() => DepositedDateString, value); }
        }
        [NotMapped]
        public string DepositedDateStringAmharic
        {
            get
            {
                return ReportUtility.GetEthCalendar(DepositedDate, true);
            }
            set { SetValue(() => DepositedDateStringAmharic, value); }
        }
        [NotMapped]
        public string DepositedDateStringAmharicFormatted
        {
            get
            {
                return ReportUtility.GetEthCalendarFormated(DepositedDate, "/");
            }
            set { SetValue(() => DepositedDateStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string ReturnedDateStringAndAmharic
        {
            get
            {
                if (ReturnedDate != null)
                    return ReturnedDate.Value.ToString("dd/MM/yyyy") + "(" + ReportUtility.GetEthCalendarFormated(ReturnedDate.Value, "/") + ")";
                return "";
            }
            set { SetValue(() => ReturnedDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string ReturnedDateString
        {
            get
            {
                if (ReturnedDate != null)
                    return ReturnedDate.Value.ToString("dd/MM/yyyy");
                return "";
            }
            set { SetValue(() => ReturnedDateString, value); }
        }
        [NotMapped]
        public string ReturnedDateStringAmharic
        {
            get
            {
                if (ReturnedDate != null)
                    return ReportUtility.GetEthCalendar(ReturnedDate.Value, true);
                return "";
            }
            set { SetValue(() => ReturnedDateStringAmharic, value); }
        }
        [NotMapped]
        public string ReturnedDateStringAmharicFormatted
        {
            get
            {
                if (ReturnedDate != null)
                    return ReportUtility.GetEthCalendarFormated(ReturnedDate.Value, "/");
                return "";
            }
            set { SetValue(() => ReturnedDateStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string UsedDateStringAndAmharic
        {
            get
            {
                if (UsedDate != null)
                    return UsedDate.Value.ToString("dd/MM/yyyy") + "(" + ReportUtility.GetEthCalendarFormated(UsedDate.Value, "/") + ")";
                return "";
            }
            set { SetValue(() => UsedDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string UsedDateString
        {
            get
            {
                if (UsedDate != null)
                    return UsedDate.Value.ToString("dd/MM/yyyy");
                return "";
            }
            set { SetValue(() => UsedDateString, value); }
        }
        [NotMapped]
        public string UsedDateStringAmharic
        {
            get
            {
                if (UsedDate != null)
                    return ReportUtility.GetEthCalendar(UsedDate.Value, true);
                return "";
            }
            set { SetValue(() => UsedDateStringAmharic, value); }
        }
        [NotMapped]
        public string UsedDateStringAmharicFormatted
        {
            get
            {
                if (UsedDate != null)
                    return ReportUtility.GetEthCalendarFormated(UsedDate.Value, "/");
                return "";
            }
            set { SetValue(() => UsedDateStringAmharicFormatted, value); }
        }
    }
}