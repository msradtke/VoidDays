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
        public MainViewContainerViewModel(IPreviousDayViewModel previousDayViewModel,ICurrentDayViewModel currentDayViewModel, ICurrentListViewModel currentListViewModel)
        {
            PreviousDayViewModel = previousDayViewModel;
            CurrentListViewModel = currentListViewModel;
            CurrentDayViewModel = currentDayViewModel;


        }
        public object CurrentListViewModel { get; private set; }
        public object CurrentDayViewModel { get; private set; }
        public object PreviousDayViewModel { get; private set; }
    }
}
