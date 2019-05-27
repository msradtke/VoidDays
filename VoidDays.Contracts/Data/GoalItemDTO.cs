using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Core;

namespace VoidDays.Contracts.Data
{
    public class GoalItemDTO : BaseDTO
    {
        public int GoalItemId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsComplete { get; set; }
        public bool IsVoid { get; set; }
        public int GoalId { get; set; }
        public int DayNumber { get; set; }
        public string CompleteMessage { get; set; }
        public int SatisfyScale { get; set; }
    }
}
