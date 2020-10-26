using PinnaRent.Core.Enumerations;

namespace PinnaRent.Core.Models
{
    public class CategoryDTO : CommonFieldsB
    {
        public NameTypes NameType
        {
            get { return GetValue(() => NameType); }
            set { SetValue(() => NameType, value); }
        }
    }
}
