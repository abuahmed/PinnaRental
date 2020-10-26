using System.Collections.Generic;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;

namespace PinnaRent.Core
{
    public class Singleton
    {
        private static Singleton _instance;
        private Singleton() { }

        public static Singleton Instance
        {
            get { return _instance ?? (_instance = new Singleton()); }
        }

        public static ProductActivationDTO ProductActivation { get; set; }

        public static UserDTO User { get; set; }

        public static PinnaRentEdition Edition { get; set; }

        public static string SqlceFileName { get; set; }

        public static bool SeedDefaults { get; set; }

        public static UserRolesModel UserRoles { get; set; }

        public static SettingDTO Setting { get; set; }

        public static IList<WarehouseDTO> WarehousesList { get; set; }

        public static string ConnectionStringName { get; set; }
        public static string ProviderName { get; set; }
    }
}
