using Microsoft.Win32;
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
        bool load;
        public AppSettings()
        {
            InitializeComponent();
            Load(true);
            load = false;
        }

        public void Load(bool lload)
        {
            load = lload;
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("EHE"):
                    App_Edition_EHE_Button.IsChecked = true;
                    break;
                case ("EGE"):
                    App_Edition_EGE_Button.IsChecked = true;
                    break;
                case ("HE"):
                    App_Edition_HE_Button.IsChecked = true;
                    break;
                case ("GE"):
                    App_Edition_GE_Button.IsChecked = true;
                    break;
            }
            switch (Properties.Settings.Default.TexturesInstalled)
            {
                case ("ORG"):
                    App_Textures_ORG_Button.IsChecked = true;
                    break;
                case ("NW"):
                    App_Textures_NW_Button.IsChecked = true;
                    break;
            }
            load = false;
        }
        #region App_Edition
        private void App_Edition_EHE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Edition_EHE_Button.IsChecked = true;
            //EHE
        }
        private void App_Edition_EHE_Button_Checked(object sender, RoutedEventArgs e)
        {
            //EHE
            App_Edition_EGE_Button.IsChecked = false;
            App_Edition_HE_Button.IsChecked = false;
            App_Edition_GE_Button.IsChecked = false;
            Properties.Settings.Default.EditionNew = "EHE";
            SomeThingChange();
            Log.LogWriter("???", "App_Edition_EHE_Button_Checked");

        }

        private void App_Edition_EGE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Edition_EGE_Button.IsChecked = true;
            //EGE
        }
        private void App_Edition_EGE_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Edition_EHE_Button.IsChecked = false;
            //EGE
            App_Edition_HE_Button.IsChecked = false;
            App_Edition_GE_Button.IsChecked = false;
            Properties.Settings.Default.EditionNew = "EGE";
            SomeThingChange();
            Log.LogWriter("???", "App_Edition_EGE_Button_Checked");

        }
        private void App_Edition_HE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Edition_HE_Button.IsChecked = true;
            //HE
        }
        private void App_Edition_HE_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Edition_EHE_Button.IsChecked = false;
            App_Edition_EGE_Button.IsChecked = false;
            //HE
            App_Edition_GE_Button.IsChecked = false;
            Properties.Settings.Default.EditionNew = "HE";
            SomeThingChange();
            Log.LogWriter("???", "App_Edition_HE_Button_Checked");

        }
        private void App_Edition_GE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Edition_GE_Button.IsChecked = true;
            //GE
        }

        private void App_Edition_GE_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Edition_EHE_Button.IsChecked = false;
            App_Edition_EGE_Button.IsChecked = false;
            App_Edition_HE_Button.IsChecked = false;
            Properties.Settings.Default.EditionNew = "GE";
            SomeThingChange();
            Log.LogWriter("???", "App_Edition_GE_Button_Checked");

            //GE
        }
        #endregion


        #region App_Textures
        private void App_Textures_ORG_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Textures_ORG_Button.IsChecked = true;
        }
        private void App_Textures_ORG_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Textures_NW_Button.IsChecked = false;
            Properties.Settings.Default.TexturesNew = "ORG";
            SomeThingChange();
            Log.LogWriter("???", "App_Textures_ORG_Button_Checked");

        }

        private void App_Textures_NW_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Textures_NW_Button.IsChecked = true;
        }
        private void App_Textures_NW_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Textures_ORG_Button.IsChecked = false;
            Properties.Settings.Default.TexturesNew = "NW";
            SomeThingChange();
            Log.LogWriter("???", "App_Textures_NW_Button_Checked");
        }

        #endregion
        private readonly SHA256 Sha256 = SHA256.Create();
        public byte[] GetHashSha256(string filename)
        {
            using FileStream stream = File.OpenRead(filename);
            return Sha256.ComputeHash(stream);
        }
        private void Button_Maps(object sender, RoutedEventArgs e)
        {
            var runExplorer = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = @"/N," + App.S4HE_AppPath.Replace(@"/", @"\") + @"Map\User"
            };
            Process.Start(runExplorer);
        }

        private void Button_S3Test(object sender, RoutedEventArgs e)
        {
            //ToDo Import

            if (App.S3HE_AppPath != null)
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
            IniFile s4video = new(App.S4HE_AppPath + @"Config\video.cfg");
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
            IniFile s4settings = new(App.S4HE_AppPath + @"Config\GAMESETTINGS.cfg");
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
                    File.WriteAllLines(App.S4HE_AppPath + @"Config\MiscData2.cfg", lines);
                    break;
                case MessageBoxResult.No:
                    break;
            }
            //Config
            var startInfo = new ProcessStartInfo
            {
                WorkingDirectory = App.S4HE_AppPath + @"\Config\",
                FileName = App.S4HE_AppPath + @"\Config\" + @"Settlers4Config.exe"
            };
            Process.Start(startInfo);
        }

        private void Button_LangChangeClick(object sender, RoutedEventArgs e)
        {
            LangChange();
        }

        public void LangChange()
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
            SomeThingChange();
            Properties.Settings.Default.EditorInstalled = false;
            Properties.Settings.Default.Save();
        }

        public void LangSet()
        {
            if (Properties.Settings.Default.Language == "de-DE")
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            }
            SomeThingChange();
        }
        private void SomeThingChange()
        {

            App_Edition_Enhanced_Title.Content = Properties.Resources.App_Edition_Enhanced_Title;
            App_Edition_EHE.Content = Properties.Resources.App_Edition_EHE;
            App_Edition_EGE.Content = Properties.Resources.App_Edition_EGE;
            App_Edition_Classic_Title.Content = Properties.Resources.App_Edition_Classic_Title;
            App_Edition_HE.Content = Properties.Resources.App_Edition_HE;
            App_Edition_GE.Content = Properties.Resources.App_Edition_GE;
            App_Edition_Information_Title.Content = Properties.Resources.App_Edition_Information_Title;
            App_Edition_Information_Text.Content = Properties.Resources.App_Edition_Information_Text;
            App_Textures_Title.Content = Properties.Resources.App_Textures_Title;
            App_Textures_ORG.Content = Properties.Resources.App_Textures_ORG;
            App_Textures_NW.Content = Properties.Resources.App_Textures_NW;

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).VersionChange(load);
                }
            }
        }


    }
}