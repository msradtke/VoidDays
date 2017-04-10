using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
namespace VoidDays.Models
{
    [ImplementPropertyChanged]
    public class GoalItem
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
