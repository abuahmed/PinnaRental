using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class RoomMap : EntityTypeConfiguration<RoomDTO>
    {
        public RoomMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Rooms");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}