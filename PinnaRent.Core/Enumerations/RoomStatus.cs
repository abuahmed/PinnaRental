using System.ComponentModel;

namespace PinnaRent.Core.Enumerations
{
    public enum RoomStatus
    {
        [Description("አገልግሎት የሚሰጥ")]
        Active,
        [Description("የተዘጋ")]
        Closed,
        [Description("የታጠፈ")]
        MergedWith,
        [Description("የተከፈለ")]
        DividedInto
    }
    public enum RoomServices
    {
        [Description("ሱቅ")]
        Shop,
        [Description("ቢሮ")]
        Office,
        [Description("ሌላ")]
        Other,
       
    }
}