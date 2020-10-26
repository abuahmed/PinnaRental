using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.Core;
using System.Data.Common;

namespace PinnaRent.DAL
{
    public class PinnaRentDBContext : DbContextBase
    {
        public PinnaRentDBContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PinnaRentDBContext, Configuration>());
            Configuration.ProxyCreationEnabled = false;
        }
        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DbContextUtil.OnModelCreating(modelBuilder);
        }
    }

    public class DbContextFactory : IDbContextFactory<PinnaRentDBContext>
    {
        public PinnaRentDBContext Create()
        {
            switch (Singleton.Edition)
            {
                case PinnaRentEdition.CompactEdition:
                    var sqlCeConString = "Data Source=" + Singleton.SqlceFileName + ";" +
                                         "Max Database Size=4091;Password=pinnaRentP@ssw0rd";
                    Singleton.ConnectionStringName = sqlCeConString;
                    Singleton.ProviderName = "System.Data.SqlServerCe.4.0";
                    var sqlce = new SqlCeConnectionFactory(Singleton.ProviderName);
                    return new PinnaRentDBContext(sqlce.CreateConnection(sqlCeConString), true);

                case PinnaRentEdition.ServerEdition:
                    const string serverName = "DevPC"; // "SRVRPC";
                    var sQlServConString = "data source=" + serverName + ";initial catalog=" + Singleton.SqlceFileName +
                                              ";user id=sa;password=amihan";
                    Singleton.ConnectionStringName = sQlServConString;
                    Singleton.ProviderName = "System.Data.SqlClient";
                    var sql = new SqlConnectionFactory(sQlServConString);
                    return new PinnaRentDBContext(sql.CreateConnection(sQlServConString), true);
            }
            return null;
        }
    }

    public class Configuration : DbMigrationsConfiguration<PinnaRentDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            
        }

        protected override void Seed(PinnaRentDBContext context)
        {
            if (Singleton.SeedDefaults)
            {
                var setting = context.Set<SettingDTO>().Find(1);
                if (setting == null)
                {
                    context = (PinnaRentDBContext)DbContextUtil.Seed(context);
                }
            }
            base.Seed(context);
        }
    }
}
