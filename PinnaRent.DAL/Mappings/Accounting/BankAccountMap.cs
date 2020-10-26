using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class BankAccountMap : EntityTypeConfiguration<BankAccountDTO>
    {
        public BankAccountMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.AccountNumber)
               .IsRequired();

            Property(t => t.BankName)
                .IsRequired();

            Property(t => t.BankBranch)
                .IsRequired();

            // Table & Column Mappings
            ToTable("BankAccounts");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasRequired(t => t.Company)
                .WithMany(t => t.BankAccounts)
                .HasForeignKey(t => new { t.CompanyId })
                .WillCascadeOnDelete(false);

        }
    }
}
