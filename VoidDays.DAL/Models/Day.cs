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
    public class Day
    {
        [Column("day_id")]
        public int DayId { get; set; }
        [Column("day_number")]
        public int DayNumber { get; set; }
        [Column("start")]
        public DateTime Start { get; set; }
        [Column("end")]
        public DateTime End { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("is_void")]
        public bool IsVoid { get; set; }

    }
}
