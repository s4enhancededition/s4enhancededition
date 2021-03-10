using System;
using System.IO;

namespace S4EE
{
    /// <summary>
    /// Struktur um einen Log zu schreiben
    /// </summary>
    struct Log
    {
        /// <summary>
        /// LogWriter schreibt in eine Datei "Log.txt" in das Hauptverzeichnis der Anwendung 
        /// </summary>
        /// <param name="logModule">Aufrufende Klassen- oder Strukturmethode</param>
        /// <param name="logMessage">Beschreibender Text über das geloggte Ereignis</param>
        public static void LogWriter(string logModule, string logMessage = "")
        {
            if (App.DebugFlag)
            {
                using StreamWriter w = File.AppendText("log.txt");
                w.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffffff zzzz", System.Globalization.CultureInfo.InvariantCulture)} {logModule.PadRight(25)[..25]} {(logMessage.Length > 100 ? logMessage[..100] : logMessage)}");
            }
        }
    }
}