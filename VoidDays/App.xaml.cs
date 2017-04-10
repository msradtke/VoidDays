using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
using Ninject.Extensions.Factory;
using System.Data.Entity;
using VoidDays.Views;
using VoidDays.Models.Interfaces;
using VoidDays.Models;
using VoidDays.ViewModels;
using VoidDays.ViewModels.Interfaces;
namespace VoidDays
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel _container;
        protected override void OnStartup(StartupEventArgs e)
        {
            
            Database.SetInitializer<DbContext>(null);
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            //Current.MainWindow.Show();            
        }

        private void ConfigureContainer()
        {
            this._container = new StandardKernel();

            _container.Bind<IRepositoryBaseFactory>().ToFactory();
            _container.Bind<IDbContext>().To<EFDbContext>().InTransientScope();
            _container.Bind<ICurrentListViewModel>().To<CurrentListViewModel>().InTransientScope();
            _container.Bind<IMainViewContainerViewModel>().To<MainViewContainerViewModel>().InTransientScope();
            _container.Bind<IMainContainerViewModel>().To<MainContainerViewModel>().InTransientScope();
            _container.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>)).InTransientScope();
        }

        private void ComposeObjects()
        {
            //Current.MainWindow = this._container.Get<MainContainer>();
            //Current.MainWindow.Title = "VoidDays";
        }
    }
}
