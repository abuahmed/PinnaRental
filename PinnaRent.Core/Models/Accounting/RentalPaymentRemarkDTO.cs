using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Extensions;

namespace PinnaRent.Core.Models
{
    public class RentalPaymentRemarkDTO : CommonFieldsA
    {
        [Required]
        [ForeignKey("RentalPayment")]
        public int RentalPaymentId { get; set; }
        public RentalPaymentDTO RentalPayment
        {
            get { return GetValue(() => RentalPayment); }
            set { SetValue(() => RentalPayment, value); }
        }

        public RemarkTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }
        
        [Required]
        public DateTime RemarkDate
        {
            get { return GetValue(() => RemarkDate); }
            set { SetValue(() => RemarkDate, value); }
        }
        
        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }
        [NotMapped]
        public string TypeString
        {
            get
            {
                return EnumUtil.GetEnumDesc(Type);
            }
            set { SetValue(() => TypeString, value); }
        }
        [NotMapped]
        public string RemarkDateStringAndAmharic
        {
            get
            {
                return RemarkDate.ToString("dd/MM/yyyy") + "(" + ReportUtility.GetEthCalendarFormated(RemarkDate, "/") + ")";
            }
            set { SetValue(() => RemarkDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string RemarkDateString
        {
            get
            {
                return RemarkDate.ToString("dd/MM/yyyy");
            }
            set { SetValue(() => RemarkDateString, value); }
        }
        [NotMapped]
        public string RemarkDateStringAmharic
        {
            get
            {
                return ReportUtility.GetEthCalendar(RemarkDate, true) +" "+ RemarkDate.ToShortTimeString();
            }
            set { SetValue(() => RemarkDateStringAmharic, value); }
        }
        [NotMapped]
        public string RemarkDateStringAmharicFormatted
        {
            get
            {
                return ReportUtility.GetEthCalendarFormated(RemarkDate, "/");
            }
            set { SetValue(() => RemarkDateStringAmharicFormatted, value); }
        }
    }
}