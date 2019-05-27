using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
namespace VoidDays.DAL.Models
{
    [AddINotifyPropertyChangedInterface]
    public class GoalHistory
    {
        public List<History> MessageHistory { get; set; }
        public List<History> TitleHistory { get; set; }
    }
}
