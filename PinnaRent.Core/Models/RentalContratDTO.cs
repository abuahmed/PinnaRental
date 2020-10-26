using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Extensions;

namespace PinnaRent.Core.Models
{
    public class RentalContratDTO : CommonFieldsA
    {     
        [NotMapped]
        public string RentalContratNumber
        {
            get { return "C001"+Id; }
            set { SetValue(() => RentalContratNumber, value); }
        }

        [ForeignKey("Rentee")]
        public int RenteeId { get; set; }
        public RenteeDTO Rentee
        {
            get { return GetValue(() => Rentee); }
            set { SetValue(() => Rentee, value); }
        }

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public RoomDTO Room
        {
            get { return GetValue(() => Room); }
            set { SetValue(() => Room, value); }
        }
        
        public int? PreviousContratId//To Handle New or is Renewed
        {
            get { return GetValue(() => PreviousContratId); }
            set { SetValue(() => PreviousContratId, value); }
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
        
        [Required]
        [Range(1,12)]
        public int? ContratPeriod//Number of Months
        {
            get { return GetValue(() => ContratPeriod); }
            set { SetValue(() => ContratPeriod, value); }
        }
        
        public bool Ended
        {
            get { return GetValue(() => Ended); }
            set { SetValue(() => Ended, value); }
        }

        public bool Discontinued
        {
            get { return GetValue(() => Discontinued); }
            set { SetValue(() => Discontinued, value); }
        }

        public DateTime? LastContractDiscontinuedDate
        {
            get { return GetValue(() => LastContractDiscontinuedDate); }
            set { SetValue(() => LastContractDiscontinuedDate, value); }
        }

        [StringLength(255)]
        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }

        [StringLength(255)]
        public string AgreementNumber
        {
            get { return GetValue(() => AgreementNumber); }
            set { SetValue(() => AgreementNumber, value); }
        }

        public DateTime? AgreementDate
        {
            get { return GetValue(() => AgreementDate); }
            set { SetValue(() => AgreementDate, value); }
        }
        
        [ForeignKey("LastRentalPayment")]
        public int? LastRentalPaymentId { get; set; }
        public RentalPaymentDTO LastRentalPayment
        {
            get { return GetValue(() => LastRentalPayment); }
            set { SetValue(() => LastRentalPayment, value); }
        }

        [ForeignKey("LastRentDeposit")]
        public int? LastRentDepositId { get; set; }
        public RentDepositDTO LastRentDeposit
        {
            get { return GetValue(() => LastRentDeposit); }
            set { SetValue(() => LastRentDeposit, value); }
        }

        public ICollection<RentalPaymentDTO> Payments
        {
            get { return GetValue(() => Payments); }
            set
            {
                SetValue(() => Payments, value);
            }
        }
        
        public ICollection<RentDepositDTO> RentDeposits
        {
            get { return GetValue(() => RentDeposits); }
            set
            {
                SetValue(() => RentDeposits, value);
            }
        }

        #region Not Mapped Attributes
        [NotMapped]
        public bool IsNew//OR is Renewed
        {
            get
            {
                return PreviousContratId == null;
            }
            set { SetValue(() => IsNew, value); }
        }
        [NotMapped]
        public bool RentalContratExpired
        {
            get
            {
                return DaysLeft < 0;
            }
            set { SetValue(() => RentalContratExpired, value); }
        }
        [NotMapped]
        public int DaysLeft
        {
            get
            {
                if (EndDate.Year < 2000)
                    return 0;

                var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                return EndDate.Subtract(today).Days;
            }
            set { SetValue(() => DaysLeft, value); }
        }
        [NotMapped]
        public string DaysLeftString
        {
            get
            {
                if (EndDate.Year < 2000)
                    return "";
                return DaysLeft.ToString();
            }
            set { SetValue(() => DaysLeftString, value); }
        }
        [NotMapped]
        public string ContratStatus
        {
            get
            {
                return DateTime.Now >= EndDate ?
                    EnumUtil.GetEnumDesc(ContratStatusTypes.Expired) :
                    EnumUtil.GetEnumDesc(ContratStatusTypes.Active);
            }
            set { SetValue(() => ContratStatus, value); }
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
        public string LastContractDiscontinuedDateStringAndAmharic
        {
            get
            {
                if (LastContractDiscontinuedDate==null || LastContractDiscontinuedDate.Value.Year < 2000)
                    return "";
                return LastContractDiscontinuedDate.Value.ToString("dd/MM/yyyy") + " (" + ReportUtility.GetEthCalendarFormated(LastContractDiscontinuedDate.Value, "/") + ")";
            }
            set { SetValue(() => LastContractDiscontinuedDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string LastContractDiscontinuedDateString
        {
            get
            {
                if (LastContractDiscontinuedDate == null || LastContractDiscontinuedDate.Value.Year < 2000)
                    return "";
                return LastContractDiscontinuedDate.Value.ToString("dd/MM/yyyy");
            }
            set { SetValue(() => LastContractDiscontinuedDateString, value); }
        }
        [NotMapped]
        public string LastContractDiscontinuedDateStringAmharic
        {
            get
            {
                if (LastContractDiscontinuedDate == null || LastContractDiscontinuedDate.Value.Year < 2000)
                    return "";
                return ReportUtility.GetEthCalendar(LastContractDiscontinuedDate.Value, true);
            }
            set { SetValue(() => LastContractDiscontinuedDateStringAmharic, value); }
        }
        [NotMapped]
        public string LastContractDiscontinuedDateStringAmharicFormatted
        {
            get
            {
                if (LastContractDiscontinuedDate == null || LastContractDiscontinuedDate.Value.Year < 2000)
                    return "";
                return ReportUtility.GetEthCalendarFormated(LastContractDiscontinuedDate.Value, "/");
            }
            set { SetValue(() => LastContractDiscontinuedDateStringAmharicFormatted, value); }
        } 
        #endregion
    }
}