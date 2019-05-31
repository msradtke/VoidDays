using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;

namespace Xamarin.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        public HistoryViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
        }
    }
}
