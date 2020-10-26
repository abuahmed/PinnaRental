using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum RemarkTypes
    {
        [Description("Phone Out Of Service Area")]
        PhoneOutOfServiceArea,
        [Description("Phone Switchedoff")]
        PhoneSwitchedof,
        [Description("Phone Not Working")]
        PhoneNotWorking,
        [Description("Phone Not Answering")]
        PhoneNotAnswering,
        [Description("Out Of The City Or Country")]
        OutOfTheCityOrCountry,
        [Description("Aware Of The Time")]
        AwareOfTheTime,
        [Description("Will Give Quick Response")]
        WIllGiveQuickResponse,
        [Description("Other")]
        Other
    }
}