using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
namespace VoidDays.Models
{
    [ImplementPropertyChanged]
    public class History
    {
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
    }
}
