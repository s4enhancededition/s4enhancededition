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
            //SpracheFestlegen();

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
            switch (Properties.Settings.Default.Language)
            {
                case ("de-DE"):
                    App_Language_deDE_Button.IsChecked = true;
                    break;
                case ("en-US"):
                    App_Language_enUS_Button.IsChecked = true;
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
        #region App_Lang
        private void App_Language_enUS_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Language_enUS_Button.IsChecked = true;
        }
        private void App_Language_enUS_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Language_deDE_Button.IsChecked = false;
            Properties.Settings.Default.Language = "en-US";
            Log.LogWriter("???", "App_Language_enUS_Button_Checked");
            LangChange();
            Log.LogWriter("???", "App_Language_enUS_Button_Checked_FIN");

        }

        private void App_Language_deDE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Language_deDE_Button.IsChecked = true;
        }
        private void App_Language_deDE_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Language_enUS_Button.IsChecked = false;
            Properties.Settings.Default.Language = "de-DE";
            Log.LogWriter("???", "App_Language_deDE_Button_Checked");
            LangChange();
            Log.LogWriter("???", "App_Language_deDE_Button_Checked_FIN");

        }

        //private void App_Textures_NW_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    App_Textures_NW_Button.IsChecked = true;
        //}
        //private void App_Textures_NW_Button_Checked(object sender, RoutedEventArgs e)
        //{
        //    App_Textures_ORG_Button.IsChecked = false;
        //    Properties.Settings.Default.TexturesNew = "NW";
        //    SomeThingChange();
        //    Log.LogWriter("???", "App_Textures_NW_Button_Checked");
        //}


        #endregion
        private readonly SHA256 Sha256 = SHA256.Create();
        public byte[] GetHashSha256(string filename)
        {
            using FileStream stream = File.OpenRead(filename);
            return Sha256.ComputeHash(stream);
        }
        //KATEN ORDNER ÖFFNEN
        private void Button_Maps(object sender, RoutedEventArgs e)
        {
            var runExplorer = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = @"/N," + App.S4HE_AppPath.Replace(@"/", @"\") + @"Map\User"
            };
            Process.Start(runExplorer);
        }
        private void Button_Settings(object sender, RoutedEventArgs e)
        {
            //Video
            IniFile s4video = new(App.S4HE_AppPath + @"Config\video.cfg");
            //MessageBoxResult result = MessageBox.Show(Properties.Resources.MSB_Videos_Text, Properties.Resources.MSB_Videos, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (MessageBoxResult.Yes)
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
            //MessageBoxResult resultsettings = MessageBox.Show(Properties.Resources.MSB_Missionen_Text, Properties.Resources.MSB_Missionen, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (MessageBoxResult.Yes)
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
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
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

        private void SpracheFestlegen()
        {
            //Sprache Festlegen
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
            App_Mod_Title.Content = Properties.Resources.App_Mod_Title;
            App_Mod_Zoom.Content = Properties.Resources.App_Mod_Zoom;
            App_Mod_Hotkeys.Content = Properties.Resources.App_Mod_Hotkeys;
            App_Music_Title.Content = Properties.Resources.App_Music_Title;
            App_Music_S3.Content = Properties.Resources.App_Music_S3;
            App_Music_S3_Information_Text.Content = Properties.Resources.App_Music_S3_Information_Text;
            App_Maps_Title_SINGLEPLAYER.Content = Properties.Resources.App_Maps_Title_SINGLEPLAYER;
            App_Maps_S01.Content = Properties.Resources.App_Maps_S01;
            App_Maps_Title_TOURNAMENTS.Content = Properties.Resources.App_Maps_Title_TOURNAMENTS;
            App_Maps_T01.Content = Properties.Resources.App_Maps_T01;
            App_Maps_T02.Content = Properties.Resources.App_Maps_T02;
            App_Maps_Title_COOP.Content = Properties.Resources.App_Maps_Title_COOP;
            App_Maps_C01.Content = Properties.Resources.App_Maps_C01;
            App_Maps_Title_BUDDEL.Content = Properties.Resources.App_Maps_Title_BUDDEL;
            App_Maps_B01.Content = Properties.Resources.App_Maps_B01;
            App_Maps_Title_METZEL.Content = Properties.Resources.App_Maps_Title_METZEL;
            App_Maps_M01.Content = Properties.Resources.App_Maps_M01;
            App_Maps_Title_TRAINING.Content = Properties.Resources.App_Maps_Title_TRAINING;
            App_Maps_TR01.Content = Properties.Resources.App_Maps_TR01;
            App_Language_Title.Content = Properties.Resources.App_Language_Title;
            App_Language_enUS.Content = Properties.Resources.App_Language_enUS;
            App_Language_deDE.Content = Properties.Resources.App_Language_deDE;
            App_Misc_SAVE_Title.Content = Properties.Resources.App_Misc_SAVE_Title;
            App_Misc_SAVE_AUTOSAVE.Content = Properties.Resources.App_Misc_SAVE_AUTOSAVE;
            App_Misc_SAVE_SAVECLEANER.Content = Properties.Resources.App_Misc_SAVE_SAVECLEANER;
            App_Misc_SAVE_SAVECLEANER_Information_Text.Content = Properties.Resources.App_Misc_SAVE_SAVECLEANER_Information_Text;
            App_Misc_Title.Content = Properties.Resources.App_Misc_Title;
            App_Misc_VIDEO.Content = Properties.Resources.App_Misc_VIDEO;
            App_Misc_MISSIONS.Content = Properties.Resources.App_Misc_MISSIONS;
            App_Misc_MINE.Content = Properties.Resources.App_Misc_MINE;
            App_Misc_LEGACYCONTROLS.Content = Properties.Resources.App_Misc_LEGACYCONTROLS;
            App_Misc_FIXES_Title.Content = Properties.Resources.App_Misc_FIXES_Title;
            App_Misc_FIXES_EDITOR.Content = Properties.Resources.App_Misc_FIXES_EDITOR;
            //Sprache der Buttons Festlegen
            Style style = this.FindResource("Button_Settings_Maps_Folder_" + Properties.Settings.Default.Language) as Style;
            Button_Maps1.Style = style;

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).SpracheFestlegen();
                }
            }
        }

        private void SomeThingChange()
        {
            SpracheFestlegen();
            //ToDo Maps etc.
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