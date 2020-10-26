using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaRent.Core.Models
{
    public class RoomResourceDTO : CommonFieldsA
    {
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public RoomDTO Room
        {
            get { return GetValue(() => Room); }
            set { SetValue(() => Room, value); }
        }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public CategoryDTO Category
        {
            get { return GetValue(() => Category); }
            set { SetValue(() => Category, value); }
        }
        
        public string Description
        {
            get { return GetValue(() => Description); }
            set { SetValue(() => Description, value); }
        }

        [Range(0.0,100000.0)]
        public decimal? Quantity
        {
            get { return GetValue(() => Quantity); }
            set { SetValue(() => Quantity, value); }
        }
        
    }
}