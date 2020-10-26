using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PinnaRent.Core;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.DAL.Interfaces;
using PinnaRent.DAL.Mappings;

namespace PinnaRent.DAL
{
    public static class DbContextUtil
    {
        public static DbModelBuilder OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProductActivationMap());
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new RepresenteeMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new SettingMap());

            modelBuilder.Configurations.Add(new RoomMap());
            modelBuilder.Configurations.Add(new RoomResourceMap());
            modelBuilder.Configurations.Add(new RenteeMap());
            modelBuilder.Configurations.Add(new RentalContratMap());
            modelBuilder.Configurations.Add(new RentalPaymentMap());
            modelBuilder.Configurations.Add(new RentalPaymentRemarkMap());
            modelBuilder.Configurations.Add(new RentDepositMap());

            modelBuilder.Configurations.Add(new ChartofAccountMap());
            modelBuilder.Configurations.Add(new BankAccountMap());
            modelBuilder.Configurations.Add(new PaymentMap());
            modelBuilder.Configurations.Add(new CheckMap());
            //modelBuilder.Configurations.Add(new PaymentClearanceMap());

            modelBuilder.Configurations.Add(new WarehouseMap());
            modelBuilder.Configurations.Add(new ItemMap());
            modelBuilder.Configurations.Add(new ItemQuantityMap());
            modelBuilder.Configurations.Add(new TransactionHeaderMap());
            modelBuilder.Configurations.Add(new TransactionLineMap());
            modelBuilder.Configurations.Add(new BusinessPartnerMap());

            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new MembershipMap());
            modelBuilder.Configurations.Add(new RoleMap());
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            return modelBuilder;
        }

        public static IDbContext Seed(IDbContext context)
        {
            #region Setting Seeds
            context.Set<SettingDTO>().Add(new SettingDTO
            {
                Id = 1,
                EnableSeparteServicePayment = false,
                HandleBankTransaction = false,
                TaxType = TaxTypes.NoTax,
                TaxPercent = 0
            });
            #endregion

            #region List Seeds

            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "No Category" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.UnitMeasure, DisplayName = "Pcs" });

            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.TitleType, DisplayName = "አቶ" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.TitleType, DisplayName = "ወ/ሮ" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.TitleType, DisplayName = "ወ/ት" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.TitleType, DisplayName = "Mr." });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.TitleType, DisplayName = "Mrs." });

            #endregion
            
            return context;
        }

        public static IDbContext GetDbContextInstance()
        {
            switch (Singleton.Edition)
            {
                case PinnaRentEdition.CompactEdition:
                    return new DbContextFactory().Create();
                case PinnaRentEdition.ServerEdition:
                    return new DbContextFactory().Create();
            }
            return new DbContextFactory().Create();
        }
    }
}