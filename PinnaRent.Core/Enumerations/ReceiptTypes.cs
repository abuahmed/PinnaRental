using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum ReceiptTypes
    {
        [Description("ካሽ ሬጅስተር")]
        FiscalPrinter = 0,
        [Description("ደረሰኝ")]
        Receipt = 1
    }
}