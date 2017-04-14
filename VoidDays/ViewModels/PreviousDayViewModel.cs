using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using VoidDays.ViewModels.Events;
using VoidDays.ViewModels.Interfaces;
using PropertyChanged;
namespace VoidDays.ViewModels
{
    [ImplementPropertyChanged]
    public class PreviousDayViewModel : IPreviousDayViewModel
    {
        IEventAggregator _eventAggregator;
        public PreviousDayViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PreviousDayStatusEvent>().Subscribe(SetStatus);
        }
        public bool? PreviousDayIsComplete { get; set; }
        private void SetStatus(bool? previousDayIsComplete)
        {
            PreviousDayIsComplete = previousDayIsComplete;
        }
    }
}
