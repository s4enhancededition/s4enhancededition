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
        public static readonly string S3HE_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs\11784", "InstallDir", null);
        //ToDo Find Me
        public static readonly string S3GE_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs\11784", "InstallDir", null);

        public static readonly string S4HE_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs\11785", "InstallDir", null);
        public static readonly string S4GE_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\BlueByte\Settlers", "Path", null);

        void App_Startup(object sender, StartupEventArgs e)
        {
            if (S4EE.Properties.Settings.Default.UpgradeRequired)
            {
                S4EE.Properties.Settings.Default.Upgrade();
                S4EE.Properties.Settings.Default.UpgradeRequired = false;
                S4EE.Properties.Settings.Default.Save();
            }

            switch (S4EE.Properties.Settings.Default.Language)
            {
                case ("de-DE"):
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
                        break;
                    }
                case ("en-US"):
                default:
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        break;
                    }
            }

            bool SilentUninstall = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/SilentUninstall")
                {
                    SilentUninstall = true;
                }
            }

            if (SilentUninstall)
            {
                //ToDo Implemententieren
                MessageBox.Show("NotImplement", "NotImplement", MessageBoxButton.OK, MessageBoxImage.Question);
                Environment.Exit(0);
            }
            AppWindow AppWindow = new();
            AppWindow.Show();
        }
    }
}