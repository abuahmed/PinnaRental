using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class ChartofAccountMap : EntityTypeConfiguration<ChartofAccountDTO>
    {
        public ChartofAccountMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            
            // Table & Column Mappings
            ToTable("ChartofAccounts");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships 
        }
    }
}