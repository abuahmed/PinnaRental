using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class RentalPaymentMap : EntityTypeConfiguration<RentalPaymentDTO>
    {
        public RentalPaymentMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("RentalPayments");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Contrat)
                .WithMany(t => t.Payments)
                .HasForeignKey(t => new { t.ContratId })
                .WillCascadeOnDelete(false);

            //HasOptional(t=>t.ServicePayment)
            //    .WithMany()
            //    .HasForeignKey(t=>new {t.ServicePaymentId})
            //    .WillCascadeOnDelete(false);

        }
    }
}