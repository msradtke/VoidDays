﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using System.ComponentModel.DataAnnotations.Schema;
namespace VoidDays.Models
{
    [ImplementPropertyChanged]
    public class Settings
    {
        [Column("settings_id")]
        public int SettingsId { get; set; }
        [Column("start_time")]
        public TimeSpan StartTime { get; set; }
        [Column("end_time")]
        public TimeSpan EndTime { get; set; }
        [Column("start_day")]
        public DateTime StartDay { get; set; }
    }
}