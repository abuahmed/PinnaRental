using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum TaxTypes
    {
        [Description("VAT")]
        Vat,
        [Description("TOT")]
        Tot,
        [Description("No Tax")]
        NoTax
    }
}