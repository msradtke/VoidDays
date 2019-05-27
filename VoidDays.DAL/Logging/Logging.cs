using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace VoidDays.DAL.Logging
{
    public static class Log
    {
        static string _directory;
        static StreamWriter _streamWriter = null;
        static Log()
        {
            _directory = Directory.GetCurrentDirectory();
        }
        public static void DBLog(string message)
        {
            string path = _directory + "\\Logs\\" + "DBLog.txt";
            using (var sw = new StreamWriter(path, true))
            {
                var msg = DateTime.Now.ToString() + ": " + message;
                sw.WriteLine(msg);
            }
        }

        public static void GeneralLog(string message)
        {
            string path = _directory + "\\Logs\\" + "GeneralLog.txt";
            using (var sw = new StreamWriter(path, true))
            {
                var msg = DateTime.Now.ToString() + ": " + message;
                sw.WriteLine(msg);
            }
        }
        public static void DebugLog(string message)
        {
            string path = _directory + "\\Logs\\" + "GeneralLog.txt";
            using (var sw = new StreamWriter(path, true))
            {
                var msg = DateTime.Now.ToString() + ": " + message;
                sw.WriteLine(msg);
            }
        }

        //do not use
        static StreamWriter OpenCreateLogFile(string name)
        {
            if (_streamWriter != null)
                return _streamWriter;
            string path = _directory + "\\Logs\\" + name;
            _streamWriter = new StreamWriter(path, true);
            return _streamWriter;


        }
    }
}
