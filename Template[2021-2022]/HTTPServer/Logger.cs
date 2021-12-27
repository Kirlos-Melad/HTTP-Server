using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            if (!File.Exists("log.txt"))
            {
                FileStream fs = new FileStream("log.txt", FileMode.CreateNew);
                fs.Close();
            }
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine("Datetime: " + ex.Message);
                sw.WriteLine("message: " + DateTime.Now.ToString());
                sw.Close();
            }
        }
    }
}
