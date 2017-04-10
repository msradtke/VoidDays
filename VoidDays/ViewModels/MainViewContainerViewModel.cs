using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.ViewModels.Interfaces;
namespace VoidDays.ViewModels
{
    public class MainViewContainerViewModel : IMainViewContainerViewModel
    {
        public MainViewContainerViewModel(ICurrentListViewModel currentListViewModel)
        {
            CurrentListViewModel = currentListViewModel;
            CurrentDayViewModel = new CurrentDayViewModel();
            PreviousDayViewModel = new PreviousDayViewModel();
        }
        public object CurrentListViewModel { get; private set; }
        public object CurrentDayViewModel { get; private set; }
        public object PreviousDayViewModel { get; private set; }
    }
}
