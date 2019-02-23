using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.Services
{
    public class DatabaseService : IDatabaseService
    {
        private static string _connectionString;
        public string ConnectionString { get => _connectionString; set => _connectionString = value; }
    }

    public interface IDatabaseService
    {
        string ConnectionString { get; set; }
    }
}
