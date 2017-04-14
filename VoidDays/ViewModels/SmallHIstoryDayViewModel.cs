using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using VoidDays.Models;

namespace VoidDays.ViewModels
{
    [ImplementPropertyChanged]
    public class SmallHistoryDayViewModel 
    {
        public SmallHistoryDayViewModel(Day day)
        {
            Day = day;
            IsDayNull = true;
            if (Day != null)
                IsDayNull = false;            
        }

        public Day Day { get; set; }
        public bool IsDayNull { get; private set; }
    }
}
