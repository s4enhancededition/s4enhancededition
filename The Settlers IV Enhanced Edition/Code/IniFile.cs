using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace S4EE
{
    /// <summary>
    /// internal class IniFile ist dazu da eine ini-Datei auszulesen und / oder zu schreiben.
    /// </summary>
    internal class IniFile
    {
        //Private Private Methoden der internal class IniFile (ohne weitere Detailbeschreibungen)
        #region Private Methoden
        private readonly string Path;
        private readonly string AssemblyFileName = Assembly.GetExecutingAssembly().GetName().Name;
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
        #endregion
        //Konstruktor Methode der internal class IniFile
        #region Konstruktor
        /// <summary>
        /// Konstruktor Methode für die internal class IniFile <para/>
        /// Diese kann dazu genutzt werden um eine ini-Datei auszulesen und / oder zu schreiben.
        /// </summary>
        /// <param name="IniPath">Pfad zur ini Datei, ohne Parameter wird im Anwendungsverzeichnis eine 'Anwendungsname.ini' verwendet</param>
        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? AssemblyFileName + ".ini").FullName;
        }
        #endregion
        //Öffentliche Methoden der internal class IniFile
        #region Öffentliche Methoden
        /// <summary>
        /// Aus einer ini-Datei einen Wert auslesen
        ///  </summary>
        /// <param name="Key">Einstellungsname</param>
        /// <param name="Section">Selektion (üblicherweise in [Selektionsname] angeben) - Bei keiner Angabe wird der AssemblyFileName verwendet</param>
        /// <returns><code>string 'Einstellungswert'</code></returns>
        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            _ = GetPrivateProfileString(Section ?? AssemblyFileName, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }
        /// <summary>
        /// Wert in eine ini-Datei schreiben
        ///  </summary>
        /// <param name="Key">Einstellungsname</param>
        /// <param name="Value">Einstellungswert</param>
        /// <param name="Section">Selektion (üblicherweise in [Selektionsname] angeben) - Bei keiner Angabe wird der AssemblyFileName verwendet</param>
        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? AssemblyFileName, Key, Value, Path);
        }
        #endregion
    }
}
