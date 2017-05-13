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
using VoidDays.Services.Interfaces;
using VoidDays.Services;
using Prism.Events;
using VoidDays.Resources;
namespace VoidDays
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel container;
        private MainContainer MainContainer;
        protected override void OnStartup(StartupEventArgs e)
        {

            Database.SetInitializer<DbContext>(null);
            base.OnStartup(e);
            ConfigureContainer();

            //Current.MainWindow = this.container.Get<Splash>();
            //Current.MainWindow.Show();


            ComposeObjects();
            Current.MainWindow.Show();
            
            Task.Factory.StartNew(() =>
           {
               //MainContainer.MainContainerViewModel.Initialize();
               var _startupService = this.container.Get<IStartupService>();
               _startupService.Initialize();
           }
            );
            
           


        }

        private void ConfigureContainer()
        {
            this.container = new StandardKernel();

            container.Bind<IUnitOfWork>().To<UnitOfWork>().InTransientScope();
            container.Bind<IRepositoryBaseFactory>().ToFactory();
            container.Bind<IViewModelFactory>().ToFactory();

            container.Bind<IDbContext>().To<EFDbContext>().InTransientScope();
            container.Bind<ICurrentListViewModel>().To<CurrentListViewModel>().InTransientScope();
            container.Bind<IMainViewContainerViewModel>().To<MainViewContainerViewModel>().InTransientScope();
            container.Bind<IMainContainerViewModel>().To<MainContainerViewModel>().InTransientScope();
            container.Bind<IGoalItemViewModel>().To<GoalItemViewModel>().InTransientScope();
            container.Bind<IPreviousDayViewModel>().To<PreviousDayViewModel>().InTransientScope();
            container.Bind<ICurrentDayViewModel>().To<CurrentDayViewModel>().InTransientScope();
            container.Bind<ISmallHistoryDayViewModelContainer>().To<SmallHistoryDayViewModelContainer>().InTransientScope();
            container.Bind<IDayHistoryViewModel>().To<DayHistoryViewModel>().InTransientScope();

            container.Bind<IGoalService>().To<GoalService>().InTransientScope();
            container.Bind<IDialogService>().To<DialogService>().InSingletonScope();
            container.Bind<IStartupService>().To<StartupService>().InTransientScope();
            container.Bind<IAdminService>().To<AdminService>().InTransientScope();
            container.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            container.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>)).InTransientScope();
        }

        private void ComposeObjects()
        {
            MainContainer = this.container.Get<MainContainer>();
            Current.MainWindow = MainContainer;
            Current.MainWindow.Title = "VoidDays";
        }
    }
}
