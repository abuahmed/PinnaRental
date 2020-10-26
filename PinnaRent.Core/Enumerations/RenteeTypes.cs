using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum RenteeTypes
    {
        [Description("የግል")]
        Person,
        [Description("ድርጅት")]
        Organization
    }
    public enum CompanyTypes
    {
        [Description("የግል")]
        Personal,
        [Description("ድርጅት")]
        Organization
    }

}