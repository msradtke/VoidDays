using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Prism.Events;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace VoidDays.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IViewModelBase
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

        public ObservableCollection<IViewModelBase> TabItems { get; set; }
        public IViewModelBase SelectedTabItem { get; set; }
        
        public string TabHeader { get; set; }

        public void AddTabItem(IViewModelBase viewModel)
        {
            TabItems.Add(viewModel);
        }

        public virtual void Cleanup()
        {
            PropertyChanged = null;
        }
        public virtual void Initialize()
        {

        }
    }

    public interface IViewModelBase
    {
        string TabHeader { get; set; }
        void AddTabItem(IViewModelBase viewModel);
        ObservableCollection<IViewModelBase> TabItems { get; set; }
        void Cleanup();
    }
}
