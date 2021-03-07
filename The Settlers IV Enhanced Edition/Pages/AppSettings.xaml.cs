using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace S4EE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppSettings : Page
    {
        private static readonly string S3_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs\11784", "InstallDir", null);
        private static readonly string S4_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs\11785", "InstallDir", null);
        public AppSettings()
        {
            InitializeComponent();
        }
        private readonly SHA256 Sha256 = SHA256.Create();
        private byte[] GetHashSha256(string filename)
        {
            using FileStream stream = File.OpenRead(filename);
            return Sha256.ComputeHash(stream);
        }
        private void Button_Maps(object sender, RoutedEventArgs e)
        {
            var runExplorer = new System.Diagnostics.ProcessStartInfo();
            runExplorer.FileName = "explorer.exe";
            runExplorer.Arguments = @"/N," + S4_AppPath.Replace(@"/", @"\") + @"Map\User"; // @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\games\thesettlers4\Map\User\";
            System.Diagnostics.Process.Start(runExplorer);
        }

        private void Button_S3Test(object sender, RoutedEventArgs e)
        {
            //ToDo Import

            if (S3_AppPath != null)
            {
                MessageBox.Show(Properties.Resources.MSB_S3_Text, Properties.Resources.MSB_S3, MessageBoxButton.OK, MessageBoxImage.Question);

            }
            else
            {
                MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void Button_Settings(object sender, RoutedEventArgs e)
        {
            //Video
            IniFile s4video = new(S4_AppPath + @"Config\video.cfg");
            MessageBoxResult result = MessageBox.Show(Properties.Resources.MSB_Videos_Text, Properties.Resources.MSB_Videos, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    s4video.Write("ShowVideos ", "1", "ADVGAMESETTINGS");
                    break;
                case MessageBoxResult.No:
                    s4video.Write("ShowVideos ", "0", "ADVGAMESETTINGS");
                    break;
            }
            //Spielstände
            IniFile s4settings = new(S4_AppPath + @"Config\GAMESETTINGS.cfg");
            MessageBoxResult resultsettings = MessageBox.Show(Properties.Resources.MSB_Missionen_Text, Properties.Resources.MSB_Missionen, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (resultsettings)
            {
                case MessageBoxResult.Yes:
                    s4settings.Write("MsgLevelMask ", "-83886081", "GAMESETTINGS");
                    string[] lines = {  @"//",
                                        @"// Automatically generated file. Do not edit!",
                                        @"// ",
                                        @"",
                                        @"[MISCDATA2]",
                                        @"{",
                                        @"    Data01 = 237338634",
                                        @"    Data02 = -467991751",
                                        @"    Data03 = 2005954587",
                                        @"    Data04 = -399514398",
                                        @"    Data05 = -2147273387",
                                        @"    Data06 = 803908",
                                        @"    Data07 = 808276",
                                        @"}"
                                      };
                    File.WriteAllLines(S4_AppPath + @"Config\MiscData2.cfg", lines);
                    break;
                case MessageBoxResult.No:
                    break;
            }
            //Config
            var startInfo = new ProcessStartInfo
            {
                WorkingDirectory = S4_AppPath + @"\Config\",
                FileName = S4_AppPath + @"\Config\" + @"Settlers4Config.exe"
            };
            Process.Start(startInfo);
        }

        private void Button_LangChangeClick(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Language == "de-DE")
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Properties.Settings.Default.Language = "en-US";              
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
                Properties.Settings.Default.Language = "de-DE";
            }
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).VersionChange();
                }
            }

            LangChange();
            Properties.Settings.Default.EditorInstalled = false;
            Properties.Settings.Default.Save();
        }
        private void LangChange()
        {
            App_Edition_Title.Content = Properties.Resources.App_Edition_Title;
            App_Edition_EHE.Content = Properties.Resources.App_Edition_EHE;
            App_Edition_EGE.Content = Properties.Resources.App_Edition_EGE;
            App_Edition_HE.Content = Properties.Resources.App_Edition_HE;
            App_Edition_GE.Content = Properties.Resources.App_Edition_GE;
            App_Textures_Title.Content = Properties.Resources.App_Textures_Title;
            App_Textures_ORG.Content = Properties.Resources.App_Textures_ORG;
            App_Textures_NW.Content = Properties.Resources.App_Textures_NW;

        }

    } 
}