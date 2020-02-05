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
        //Stien til logfilen
        private static string _fileName = @"Logs\Logfil.txt";
        public static void WriteToLog(string logMessage)
        {
            //Hvis mappen ikke findes på stien laves den
            if (!Directory.Exists(_fileName))
            {
                Directory.CreateDirectory("Logs");
            }
            //Skriver til Logfilen. Bruger using for at lukke forbindelsen til filen når den er færdig
            if (File.Exists(_fileName))
            {
                using (StreamWriter swm = File.AppendText(_fileName))
                {
                    swm.WriteLine(logMessage);
                }
            }
            //Hvis logfilen ikke eksitere laves den og skriver til den. Bruger using for at lukke forbindelsen igen når den er færdig
            else
            {
                using (StreamWriter swm = File.CreateText(_fileName))
                {
                    swm.WriteLine(logMessage);
                }
            }
        }
        public static string ReadFromLog()
        {
            //Checker om filen eksitere og læser loggen hvis den gør
            if (File.Exists(_fileName))
            {
                return File.ReadAllText(_fileName).ToString();
            }
            //eksitere filen ikke eksistere sendes nedenstående besked tilbage
            else
            {
                return "Der er ikke nogen log at vise";
            }
        }
    }
}
