using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Prism.Events;
namespace VoidDays.ViewModels
{
    [ImplementPropertyChanged]
    public class ViewModelBase
    {
        protected IEventAggregator _eventAggregator; 
        public ViewModelBase()
        {
        }
    }
}
