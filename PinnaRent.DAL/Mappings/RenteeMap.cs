using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class RenteeMap : EntityTypeConfiguration<RenteeDTO>
    {
        public RenteeMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Rentees");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}