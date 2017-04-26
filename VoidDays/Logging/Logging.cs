using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace VoidDays.Logging
{
    public static class Log
    {
        static string directory;

        static Log()
        {
            directory = Directory.GetCurrentDirectory();
        }
        public static void DBLog(string message)
        {
            
            using (var sw = OpenCreateLogFile("DBLog.txt"))
            {
                var msg = DateTime.Now.ToString()+ ": " + message;
                sw.WriteLine(msg);
            }
        }

        public static void GeneralLog(string message)
        {
            
            using (var sw = OpenCreateLogFile("GeneralLog.txt"))
            {
                var msg = DateTime.Now.ToString() + ": " + message;
                sw.WriteLine(msg);
            }
        }
        static StreamWriter OpenCreateLogFile(string name)
        {
            string path = directory + "\\Logs\\" + name;
            StreamWriter sw = new StreamWriter(path, true);

            return sw;


        }
    }
}
