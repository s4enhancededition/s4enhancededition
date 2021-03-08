using System;
using System.IO;

namespace S4EE
{
    struct Writer
    {
        public static void LogWriter(string logModule, string logMessage)
        {
            using StreamWriter w = File.AppendText(@"log.txt");
            w.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fffffff", System.Globalization.CultureInfo.InvariantCulture)} #{logModule} {logMessage}");
        }
    }
}