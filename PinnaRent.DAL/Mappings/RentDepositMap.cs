using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class RentDepositMap : EntityTypeConfiguration<RentDepositDTO>
    {
        public RentDepositMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("RentDeposits");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Contrat)
                .WithMany(t => t.RentDeposits)
                .HasForeignKey(t => new { t.ContratId })
                .WillCascadeOnDelete(false);

        }
    }
}