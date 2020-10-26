using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class RepresenteeMap : EntityTypeConfiguration<RepresenteeDTO>
    {
        public RepresenteeMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Representees");
            Property(t => t.Id).HasColumnName("Id");
        }
    }
}