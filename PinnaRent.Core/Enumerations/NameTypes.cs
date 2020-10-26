using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum NameTypes
    {
        [Description("Item Category")]
        Category = 0,
        [Description("Unit Of Measure")]
        UnitMeasure = 1,
        [Description("Client Category")]
        ClientCategory = 2,
        [Description("Equipment Category")]
        EquipmentCategory = 3,
        [Description("City")]
        City = 4,
        [Description("SubCity")]
        SubCity = 5,
        [Description("ፍሎር")]
        FloorCategory = 6,
        [Description("የእቃ አይነት")]
        RoomResourceCategory = 7,
        [Description("Account Type")]
        AccountType = 8,
        [Description("Title Type")]
        TitleType = 9,
        [Description("Bank")]
        Bank = 10,
        [Description("ChartOfAccount")]
        ChartOfAccount = 11
    }
}
