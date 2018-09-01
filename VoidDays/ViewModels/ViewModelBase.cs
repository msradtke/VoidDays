using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Prism.Events;
using System.ComponentModel;

namespace VoidDays.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected IEventAggregator _eventAggregator; 
        public ViewModelBase()
        {
        }
        public void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
