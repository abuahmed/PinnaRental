using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum ContratStatusTypes
    {
        [Description("ሁሉንም")]
        All,
        [Description("ውል ጊዜ ያለው")]
        Active,
        [Description("ውል ጊዜ ያለፈበት")]
        Expired,
        [Description("ክፍያ ጊዜ ያለው")]
        PaymentActive,
        [Description("ክፍያ ጊዜ ያለፈበት")]
        PaymentExpired,
        [Description("ያልተያዘ/የተለቀቀ")]
        Vacant,
        //[Description("ትንሽ ጊዜ ያለውና ያለፈበት")]
        //FewDaysActiveExpired,
        //[Description("ትንሽ ጊዜ ያለው")]
        //FewDaysActive,
        //[Description("ትንሽ ጊዜ ያለፈበት")]
        //FewDaysExpired
    }
    public enum MembershipTypes
    {
        [Description("አዲስና ያደሱ")]
        All,
        [Description("አዲስ የተመዘገቡ")]
        New,
        [Description("ያደሱ")]
        Renewed
    }
    public enum ShiftTypes
    {
        [Description("ጠዋትና ከሰአት")]
        All,
        [Description("ጠዋት")]
        Morning,
        [Description("ከሰአት")]
        Afternoon
    }
}