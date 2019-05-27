using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using System.ComponentModel.DataAnnotations.Schema;
namespace VoidDays.DAL.Models
{
    [AddINotifyPropertyChangedInterface]
    public class GoalItem
    {
        [Column("goal_item_id")]
        public int GoalItemId { get; set; }

        public string Title { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("created")]
        public DateTime DateTime { get; set; }

        [Column("is_complete")]
        public bool IsComplete { get; set; }
        [Column("is_void")]
        public bool IsVoid { get; set; }
        [Column("goal_id")]
        public int GoalId { get; set; }
        [Column("day_number")]
        public int DayNumber { get; set; }
        [Column("complete_message")]
        public string CompleteMessage { get; set; }

        [Column("satisfy_scale")]
        public int SatisfyScale { get; set; }

        public virtual Goal Goal { get; set; }
    }
}