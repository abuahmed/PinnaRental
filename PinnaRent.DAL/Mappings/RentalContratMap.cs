using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class RentalContratMap : EntityTypeConfiguration<RentalContratDTO>
    {
        public RentalContratMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("RentalContrats");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Room)
              .WithMany(t => t.RoomContrats)
              .HasForeignKey(t => new { t.RoomId })
              .WillCascadeOnDelete(false);

            HasRequired(t => t.Rentee)
              .WithMany(t => t.RentalContrats)
              .HasForeignKey(t => new { t.RenteeId })
              .WillCascadeOnDelete(false);
        }
    }
}