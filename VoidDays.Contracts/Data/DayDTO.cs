using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Core;

namespace VoidDays.Contracts.Data
{
    public class DayDTO : BaseDTO
    {
        public int DayId { get; set; }
        public int DayNumber { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsActive { get; set; }
        public bool IsVoid { get; set; }
    }
}
