using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using System.ComponentModel.DataAnnotations.Schema;
namespace VoidDays.Models
{
    [ImplementPropertyChanged]
    public class GoalItemsCreated
    {
        [Column("goal_items_created_id")]
        public int GoalItemsCreatedId { get; set; }

        [Column("day_id")]
        public int DayId { get; set; }
        public Day Day { get; set; }
    }
}
