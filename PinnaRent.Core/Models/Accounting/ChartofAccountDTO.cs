using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaRent.Core.Models
{
    public class ChartofAccountDTO : CommonFieldsA
    {
        [Index(IsUnique = true)]
        [Required]
        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string AccountId
        {
            get { return GetValue(() => AccountId); }
            set { SetValue(() => AccountId, value); }
        }

        [Required]
        public string Description
        {
            get { return GetValue(() => Description); }
            set { SetValue(() => Description, value); }
        }


        [ForeignKey("AccountType")]
        public int? AccountTypeId { get; set; }
        public CategoryDTO AccountType
        {
            get { return GetValue(() => AccountType); }
            set { SetValue(() => AccountType, value); }
        }
    }
}