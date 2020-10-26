using System.ComponentModel.DataAnnotations.Schema;
using PinnaRent.Core.Models;

namespace PinnaRent.Core
{
    public class CommonFieldsC : CommonFieldsB
    {
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }
    }

    public class CommonFieldsC2 : CommonFieldsB2
    {
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }
    }
}
