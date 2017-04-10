using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.ViewModels.Interfaces;
namespace VoidDays.ViewModels
{
    public class MainContainerViewModel : ViewModelBase, IMainContainerViewModel
    {
        public MainContainerViewModel(IMainViewContainerViewModel mainViewContainerViewModel)
        {
            MainViewContainerViewModel = mainViewContainerViewModel;
        }
        public object MainViewContainerViewModel { get; private set; }
    }
}
