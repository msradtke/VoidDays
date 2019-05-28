using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Mobile.Services;
using Mobile.Views;
using Ninject;

namespace Mobile
{
    public partial class App : Application
    {
        IKernel _container;
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();

            var settings = new Ninject.NinjectSettings() { LoadExtensions = false };
            _container = new StandardKernel(settings);
            _container.Load<VoidNinjectModule>();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
