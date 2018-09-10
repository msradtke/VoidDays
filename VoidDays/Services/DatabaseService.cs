using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.Services
{
    public class DatabaseService : IDatabaseService
    {
        public string ConnectionString { get; set; }
    }

    public interface IDatabaseService
    {
        string ConnectionString { get; set; }
    }
}
