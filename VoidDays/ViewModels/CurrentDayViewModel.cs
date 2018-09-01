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
    public class CurrentDayViewModel : ViewModelBase,ICurrentDayViewModel
    {        
        public CurrentDayViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<CurrentDayStatusEvent>().Subscribe(UpdateCurrentDayStatus);
        }
        public bool IsCurrentDayComplete { get; set; }
        private void UpdateCurrentDayStatus(bool isComplete)
        {
            IsCurrentDayComplete = isComplete;
        }
    }
}
