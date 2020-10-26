using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class CompanyMap : EntityTypeConfiguration<CompanyDTO>
    {
        public CompanyMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties
            
            // Table & Column Mappings
            ToTable("Company");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
        }
    }
}