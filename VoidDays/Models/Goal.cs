using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
namespace VoidDays.Models
{
    [ImplementPropertyChanged]
    public class Goal
    {
        public string Message { get; set; }
        public DateTime Created { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }
        public bool isDeleted { get; set; }

    }
}
