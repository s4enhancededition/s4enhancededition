using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;

namespace S4EE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // LogName für den LogWriter
        private static readonly string LogName = "App.xaml.cs";
        // DebugFlag für Log Schreiben
        public static bool DebugFlag = false;
        // Verzeichnisse der Spiele auslesen
        public static readonly string S3HE_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs\11784", "InstallDir", null);
        public static readonly string S4HE_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs\11785", "InstallDir", null);
        public static readonly string S4GE_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\BlueByte\Settlers4", "Path", null);
        /// <summary>
        /// Einstiegspunkt der Anwendung
        /// </summary>
        void App_Startup(object sender, StartupEventArgs e)
        {
            for (int i = 0; i != e.Args.Length; ++i)
            {
                // Debug für Log Schreiben
                if (e.Args[i] == "/Debug")
                {
                    DebugFlag = true;
                    Log.LogWriter("Neuer Log");
                    Log.LogWriter(LogName, "Log schreiben wurde durch StartupArgs '/Debug' aktiviert");
                }
                // SilentUninstall für Rollback auf Standardeinstellungen bei Deinstallation durch Setup
                if (e.Args[i] == "/SilentUninstall")
                {
                    //ToDo RC03: "SilentUninstall-Implementation für Rollback auf Standardeinstellungen bei Deinstallation durch Setup"
                    Log.LogWriter(LogName, "SilentUninstall");

                    MessageBox.Show("Keine Implementierung Vorhanden", "Keine Implementierung Vorhanden - Bitte Neuinstallieren aller Siedler IV Installatione", MessageBoxButton.OK, MessageBoxImage.Information);
                    Environment.Exit(0);
                    return;
                }
            }
            // Upgrade der Userbezogenen Einstellungsdatei bei neuer Versionnummer
            if (S4EE.Properties.Settings.Default.UpgradeRequired)
            {
                S4EE.Properties.Settings.Default.Upgrade();
                S4EE.Properties.Settings.Default.UpgradeRequired = false;
                S4EE.Properties.Settings.Default.Save();
                Log.LogWriter(LogName, "Upgrade der Userbezogenen Einstellungsdatei bei neuer Versionnummer");
            }
            // Festlegung der Anwendungssprache durch auslesen der Default Language (Automatisches Fallback auf en-US)
            switch (S4EE.Properties.Settings.Default.Language)
            {
                case ("de-DE"):
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
                        Log.LogWriter(LogName, "Festlegung der Anwendungssprache auf de-DE");
                        break;
                    }
                case ("en-US"):
                default:
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        Log.LogWriter(LogName, "Festlegung der Anwendungssprache auf en-US");
                        break;
                    }
            }
            //Hauptfenster der Anwendung starten
            AppWindow AppWindow = new();
            Log.LogWriter(LogName, "Hauptfenster der Anwendung starten");
            AppWindow.Show();
            Log.LogWriter(LogName, "Hauptfenster der Anwendung zeigen");
        }
    }
}