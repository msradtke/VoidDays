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
        static StreamWriter _streamWriter = null;
        static Log()
        {
            directory = Directory.GetCurrentDirectory();
        }
        public static void DBLog(string message)
        {

            using (var sw = OpenCreateLogFile("DBLog.txt"))
            {
                var msg = DateTime.Now.ToString() + ": " + message;
                sw.WriteLine(msg);
                _streamWriter.Dispose();
                _streamWriter = null;
            }
        }

        public static void GeneralLog(string message)
        {

            using (var sw = OpenCreateLogFile("GeneralLog.txt"))
            {
                var msg = DateTime.Now.ToString() + ": " + message;
                sw.WriteLine(msg);
                _streamWriter.Dispose();
                _streamWriter = null;
            }
        }
        static StreamWriter OpenCreateLogFile(string name)
        {
            if (_streamWriter != null)
                return _streamWriter;
            string path = directory + "\\Logs\\" + name;
            _streamWriter = new StreamWriter(path, true);
                return _streamWriter;


        }
    }
}
