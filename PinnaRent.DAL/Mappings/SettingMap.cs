using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class SettingMap : EntityTypeConfiguration<SettingDTO>
    {
        public SettingMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            
            // Table & Column Mappings
            ToTable("Settings");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships 
        }
    }
}