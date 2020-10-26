using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Models;

namespace PinnaRent.Core
{
    public class CommonFieldsA : EntityBase
    {
        [NotMapped]
        [DisplayName("No.")]
        public int SerialNumber { get; set; }
        
    }
}
