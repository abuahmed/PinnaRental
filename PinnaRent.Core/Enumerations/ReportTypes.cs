using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum ReportTypes
    {
        [Description("አጠቃላይ ክፍያ")]
        PaymentList = 0,
        [Description("ክፍያ መፈጸም ያለባቸው")]
        ToPay = 1,
        [Description("ክፍያ የሚጠበቅባቸው")]
        NotPaid = 2,
        [Description("የተለቀቁ ክፍሎች")]
        Discontinued = 3,
        [Description("ኪራይ")]
        RentOnly = 4,
        [Description("አገልግሎት ")]
        ServiceOnly = 5,
        [Description("ቅጣት ")]
        PenalityOnly = 6,
    }
}