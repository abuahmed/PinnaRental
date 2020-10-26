using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class RentalPaymentRemarkMap : EntityTypeConfiguration<RentalPaymentRemarkDTO>
    {
        public RentalPaymentRemarkMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("PaymentRemarks");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.RentalPayment)
                .WithMany(t => t.PaymentRemarks)
                .HasForeignKey(t => new { t.RentalPaymentId })
                .WillCascadeOnDelete(false);

            //HasOptional(t=>t.ServicePayment)
            //    .WithMany()
            //    .HasForeignKey(t=>new {t.ServicePaymentId})
            //    .WillCascadeOnDelete(false);

        }
    }
}