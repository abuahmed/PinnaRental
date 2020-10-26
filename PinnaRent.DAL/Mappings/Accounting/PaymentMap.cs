using System.Data.Entity.ModelConfiguration;
using PinnaRent.Core.Models;

namespace PinnaRent.DAL.Mappings
{
    public class PaymentMap : EntityTypeConfiguration<PaymentDTO>
    {
        public PaymentMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.PaymentDate)
               .IsRequired();

            Property(t => t.Amount)
                .IsRequired();
            
            Property(t => t.Type)
                .IsRequired();

            // Table & Column Mappings
            ToTable("Payments");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.RentalPayment)
                .WithMany(t=>t.Payments)
                .HasForeignKey(t => t.RentalPaymentId)
                .WillCascadeOnDelete(true);

            HasOptional(t => t.Check)
                .WithMany()
                .HasForeignKey(t => t.CheckId);
        }
    }
}
