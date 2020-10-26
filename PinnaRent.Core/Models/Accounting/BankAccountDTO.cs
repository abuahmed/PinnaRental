using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaRent.Core.Models
{
    public class BankAccountDTO : CommonFieldsA
    {
        [ForeignKey("Account")]
        public int? AccountId { get; set; }
        public ChartofAccountDTO Account
        {
            get { return GetValue(() => Account); }
            set { SetValue(() => Account, value); }
        }

        [Required]
        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string BankName
        {
            get { return GetValue(() => BankName); }
            set { SetValue(() => BankName, value); }
        }

        [Required]
        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string BankBranch
        {
            get { return GetValue(() => BankBranch); }
            set { SetValue(() => BankBranch, value); }
        }

        [Required]
        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string AccountNumber
        {
            get { return GetValue(() => AccountNumber); }
            set { SetValue(() => AccountNumber, value); }
        }

        [NotMapped]
        public string AccountDetail
        {
            get { return BankName + "(" + AccountNumber + ")"; }
            set { SetValue(() => AccountDetail, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string AccountFormat
        {
            get { return GetValue(() => AccountFormat); }
            set { SetValue(() => AccountFormat, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Iban
        {
            get { return GetValue(() => Iban); }
            set { SetValue(() => Iban, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string SwiftCode
        {
            get { return GetValue(() => SwiftCode); }
            set { SetValue(() => SwiftCode, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Country
        {
            get { return GetValue(() => Country); }
            set { SetValue(() => Country, value); }
        }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public CompanyDTO Company
        {
            get { return GetValue(() => Company); }
            set { SetValue(() => Company, value); }
        }

        public ICollection<CheckDTO> Checks
        {
            get { return GetValue(() => Checks); }
            set { SetValue(() => Checks, value); }
        }
    }
}