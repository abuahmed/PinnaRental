using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum RentalContractTypes
    {
        [Description("Number Of Days")]
        Daily,
        [Description("Number of Months")]
        Monthy
    }
}