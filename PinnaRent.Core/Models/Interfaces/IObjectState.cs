using System.ComponentModel.DataAnnotations.Schema;


namespace PinnaRent.Core.Models.Interfaces
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}
