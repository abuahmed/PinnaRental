using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class RoomResourceMap : EntityTypeConfiguration<RoomResourceDTO>
    {
        public RoomResourceMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("RoomResources");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Room)
                .WithMany(t => t.Resources)
                .HasForeignKey(t => new { t.RoomId })
                .WillCascadeOnDelete(false);
        }
    }
}