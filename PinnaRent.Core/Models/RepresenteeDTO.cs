using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaRent.Core.Models
{
    public class RepresenteeDTO : CommonFieldsA
    {
        [ForeignKey("Title")]
        public int TitleId { get; set; }
        public CategoryDTO Title
        {
            get { return GetValue(() => Title); }
            set { SetValue(() => Title, value); }
        }

        [Required]
        public string FullName
        {
            get { return GetValue(() => FullName); }
            set { SetValue(() => FullName, value); }
        }
        
        [Required]
        public string AuthorizationNumber
        {
            get { return GetValue(() => AuthorizationNumber); }
            set { SetValue(() => AuthorizationNumber, value); }
        }

        public DateTime AuthorizationDate
        {
            get { return GetValue(() => AuthorizationDate); }
            set
            {
                SetValue(() => AuthorizationDate, value);
                SetValue(() => AuthorizationDateStringAmharic, value.ToLongDateString());
            }
        }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }

        [NotMapped]
        public string AuthorizationDateStringAndAmharic
        {
            get
            {
                return AuthorizationDate.ToString("dd/MM/yyyy") + "(" + ReportUtility.GetEthCalendarFormated(AuthorizationDate, "/") + ")";
            }
            set { SetValue(() => AuthorizationDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string AuthorizationDateString
        {
            get
            {
                return AuthorizationDate.ToString("dd/MM/yyyy");
            }
            set { SetValue(() => AuthorizationDateString, value); }
        }
        [NotMapped]
        public string AuthorizationDateStringAmharic
        {
            get
            {
                return ReportUtility.GetEthCalendar(AuthorizationDate, true);
            }
            set { SetValue(() => AuthorizationDateStringAmharic, value); }
        }
        [NotMapped]
        public string AuthorizationDateStringAmharicFormatted
        {
            get
            {
                return ReportUtility.GetEthCalendarFormated(AuthorizationDate, "/");
            }
            set { SetValue(() => AuthorizationDateStringAmharicFormatted, value); }
        }
        
    }
}