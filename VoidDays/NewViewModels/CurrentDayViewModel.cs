using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Providers;
using VoidDays.ViewModels;

namespace VoidDays.NewViewModels
{
    public class CurrentDayViewModel : ViewModelBase
    {
        private readonly IVoidProvider _voidProvider;

        public CurrentDayViewModel(IEventAggregator eventAggregator,
            IVoidProvider voidProvider)
        {
            _eventAggregator = eventAggregator;
            _voidProvider = voidProvider;
        }


    }
}
