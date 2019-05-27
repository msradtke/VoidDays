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
    public class Goal
    {
        [Column("goal_id")]
        public int GoalId { get; set; }
        [Column("message")]
        public string Message { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("is_complete")]
        public bool IsComplete { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
        [Column("delete_date")]
        public DateTime DeleteDate { get; set; }
        [Column("complete_date")]
        public DateTime CompleteDate { get; set; }
    }
}
