using PropertyChanged;
using System.ComponentModel.DataAnnotations.Schema;
namespace VoidDays.DAL.Models
{
    [AddINotifyPropertyChangedInterface]
    public class GoalItemsCreated
    {
        [Column("goal_items_created_id")]
        public int GoalItemsCreatedId { get; set; }

        [Column("day_id")]
        public int DayId { get; set; }
        public Day Day { get; set; }
    }
}
