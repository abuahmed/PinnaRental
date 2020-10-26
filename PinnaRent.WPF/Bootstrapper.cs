using System;
using System.IO;
using PinnaRent.Core.Enumerations;
using PinnaRent.DAL;
using PinnaRent.DAL.Interfaces;
using PinnaRent.WPF.ViewModel;
using Microsoft.Practices.Unity;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Repository;
using PinnaRent.Core;
using Telerik.Windows.Controls;

namespace PinnaRent.WPF
{
    public class Bootstrapper
    {
        public IUnityContainer Container { get; set; }

        public Bootstrapper()
        {
            //Theme theme=new Expression_DarkTheme();
            //StyleManager.ApplicationTheme = theme;

            Container = new UnityContainer();
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            switch (Singleton.Edition)
            {
                case PinnaRentEdition.CompactEdition:
                    var pcname = Environment.MachineName;
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) +
                               "\\PinnaRent\\";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var pathfile = Path.Combine(path, "PinnaRentDbOrdofo2.sdf");//PinnaRentDb 
                    Singleton.SqlceFileName = pathfile;
                    
                    break;

                case PinnaRentEdition.ServerEdition:
                    Singleton.SqlceFileName = "PinnaRentDb"; 
                    break;
            }

            Singleton.SeedDefaults = true;

            Container.RegisterType<IDbContext, PinnaRentDBContext>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IUnitOfWork, UnitOfWork>();

            Container.RegisterType<MainViewModel>();
        }
    }
}
