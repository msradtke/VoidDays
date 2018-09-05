using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodium;
namespace VoidDays.Models
{
    public class UserProfile
    {
        public User User { get; set; }
        public Settings Settings { get; set; }

    }
}
