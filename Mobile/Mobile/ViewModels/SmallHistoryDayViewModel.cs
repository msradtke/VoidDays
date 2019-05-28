using System;
using System.Collections.Generic;
using System.Text;
using VoidDays.Contracts.Data;

namespace Mobile.ViewModels
{
    public class SmallHistoryDayViewModel
    {
        public SmallHistoryDayViewModel(DayDTO day)
        {
            Day = day;
            IsDayNull = true;
            if (Day != null)
                IsDayNull = false;
        }

        public DayDTO Day { get; set; }
        public bool IsDayNull { get; private set; }
    }
}
