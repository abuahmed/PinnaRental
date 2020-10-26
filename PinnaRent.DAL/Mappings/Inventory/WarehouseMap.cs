using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class WarehouseMap : EntityTypeConfiguration<WarehouseDTO>
    {
        public WarehouseMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            //Property(t => t.DisplayName)
            //   .IsRequired()
            //   .HasMaxLength(45);

            // Table & Column Mappings
            ToTable("Warehouses");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.Address)
              .WithMany()
              .HasForeignKey(t => new { t.AddressId });


        }
    }
}
