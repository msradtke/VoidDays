using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.ViewModels.Interfaces;
namespace VoidDays.ViewModels
{
    public class MainViewContainerViewModel : ViewModelBase, IMainViewContainerViewModel
    {
        public MainViewContainerViewModel(IPreviousDayViewModel previousDayViewModel,ICurrentDayViewModel currentDayViewModel, ICurrentListViewModel currentListViewModel)
        {
            PreviousDayViewModel = previousDayViewModel;
            CurrentListViewModel = currentListViewModel;
            CurrentDayViewModel = currentDayViewModel;


        }
        public override void Cleanup()
        {
            PreviousDayViewModel.Cleanup();
            CurrentListViewModel.Cleanup();
            CurrentDayViewModel.Cleanup();
            base.Cleanup();
        }
        public ICurrentListViewModel CurrentListViewModel { get; private set; }
        public ICurrentDayViewModel CurrentDayViewModel { get; private set; }
        public IPreviousDayViewModel PreviousDayViewModel { get; private set; }
    }
}
