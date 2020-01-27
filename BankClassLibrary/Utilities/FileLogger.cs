using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BankClassLibrary.Utilities
{
    public static class FileLogger
    {
        static string fileName = @"Logs\Logfil.txt";
        public static void WriteToLog(string logMessage)
        {
            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory("Logs");
            }
            if (File.Exists(fileName))
            {
                using (StreamWriter swm = File.AppendText(fileName))
                {
                    swm.WriteLine(logMessage);
                }
            }
            else
            {
                using (StreamWriter swm = File.CreateText(fileName))
                {
                    swm.WriteLine(logMessage);
                }
            }
        }
        public static string ReadFromLog()
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName).ToString();
            }
            else
            {
                return "Der er ikke nogen log at vise";
            }
        }
    }
}
