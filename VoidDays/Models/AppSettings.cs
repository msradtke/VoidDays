using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            Port = "3306";
        }
        public string ServerAddress { get; set; }
        public string Port { get; set; }
        public string LastUser { get; set; }
    }
}
