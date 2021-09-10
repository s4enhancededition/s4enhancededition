using System;
using System.Diagnostics;
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
        private static readonly string LogName = "AppSettings.xaml.cs";
        public AppSettings()
        {

            InitializeComponent();
            SpracheFestlegen();
            Load();
            //ToDo Reaktivern sobald wieder eine Weltmeisterschaft oder Turnier ist
            Button_MapsTR.Visibility = Visibility.Hidden;
        }
        public void Load()
        {
            if (App.S4HE_AppPath == null)
            {
                App_Edition_EHE_Button.IsEnabled = false;
                App_Edition_HE_Button.IsEnabled = false;
                switch (Properties.Settings.Default.EditionInstalled)
                {
                    case ("EHE"):
                    case ("HE"):
                        Properties.Settings.Default.EditionInstalled = "";
                        Properties.Settings.Default.Save();
                        Log.LogWriter(LogName, "HE Fehlt unerwartet");
                        break;
                }
            }
            if (App.S4GE_AppPath == null)
            {
                App_Edition_EGE_Button.IsEnabled = false;
                App_Edition_GE_Button.IsEnabled = false;
                switch (Properties.Settings.Default.EditionInstalled)
                {
                    case ("EGE"):
                    case ("GE"):
                        Properties.Settings.Default.EditionInstalled = "";
                        Properties.Settings.Default.Save();
                        Log.LogWriter(LogName, "GE Fehlt unerwartet");
                        break;
                }
            }
            if (App.S3HE_AppPath == null)
            {
                App_Music_S3_CheckBox.IsEnabled = false;
                if (Properties.Settings.Default.Music_S3 == "1")
                {
                    Properties.Settings.Default.Music_S3 = "";
                    Properties.Settings.Default.Save();
                    Log.LogWriter(LogName, "Music_S3 Fehlt unerwartet");
                }
            }
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("EHE"):
                default:
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
                default:
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
            App_Mod_Zoom_CheckBox.IsChecked = Properties.Settings.Default.Mod_Zoom switch
            {
                ("1") => true,
                _ => false,
            };
            App_Mod_Hotkeys_CheckBox.IsChecked = Properties.Settings.Default.Mod_HotKeys switch
            {
                ("1") => true,
                _ => false,
            };
            App_ModCustom_UBO_CheckBox.IsChecked = Properties.Settings.Default.Mod_UBO switch
            {
                ("1") => true,
                _ => false,
            };
            App_Music_S3_CheckBox.IsChecked = Properties.Settings.Default.Music_S3 switch
            {
                ("1") => true,
                _ => false,
            };
            App_Maps_O01_CheckBox.IsChecked = Properties.Settings.Default.Map_O01 switch
            {
                ("0") => false,
                _ => true,
            };
            App_Maps_S01_CheckBox.IsChecked = Properties.Settings.Default.Map_S01 switch
            {
                ("1") => true,
                _ => false,
            };
            App_Maps_T01_CheckBox.IsChecked = Properties.Settings.Default.Map_T01 switch
            {
                ("1") => true,
                _ => false,
            };
            App_Maps_T02_CheckBox.IsChecked = Properties.Settings.Default.Map_T02 switch
            {
                ("1") => true,
                _ => false,
            };
            App_Maps_C01_CheckBox.IsChecked = Properties.Settings.Default.Map_C01 switch
            {
                ("1") => true,
                _ => false,
            };
            App_Maps_B01_CheckBox.IsChecked = Properties.Settings.Default.Map_B01 switch
            {
                ("1") => true,
                _ => false,
            };
            App_Maps_M01_CheckBox.IsChecked = Properties.Settings.Default.Map_M01 switch
            {
                ("1") => true,
                _ => false,
            };

            App_Maps_TR01_CheckBox.IsChecked = Properties.Settings.Default.Map_TR01 switch
            {
                ("1") => true,
                _ => false,
            };

            App_Maps_R01_CheckBox.IsChecked = Properties.Settings.Default.Map_R01 switch
            {
                ("1") => true,
                _ => false,
            };
            App_Maps_MS01_CheckBox.IsChecked = Properties.Settings.Default.Map_MS01 switch
            {
                ("1") => true,
                _ => false,
            };
            App_Misc_SAVE_AUTOSAVE_CheckBox.IsChecked = Properties.Settings.Default.AutoSave switch
            {
                ("0") => false,
                _ => true,
            };

            App_Misc_SAVE_SAVECLEANER_CheckBox.IsChecked = Properties.Settings.Default.SaveCleaner switch
            {
                ("48") => true,
                _ => false,
            };

            App_Misc_VIDEO_CheckBox.IsChecked = Properties.Settings.Default.VideosShow switch
            {
                ("0") => false,
                _ => true,
            };

            App_Misc_MISSIONS_CheckBox.IsChecked = Properties.Settings.Default.Missions switch
            {
                ("1") => true,
                _ => false,
            };

            App_Misc_MINE_CheckBox.IsChecked = Properties.Settings.Default.Minen switch
            {
                ("1") => true,
                _ => false,
            };

            App_Misc_LEGACYCONTROLS_CheckBox.IsChecked = Properties.Settings.Default.LegacyControls switch
            {
                ("1") => true,
                _ => false,
            };

            App_Misc_FIXES_EDITOR_CheckBox.IsChecked = Properties.Settings.Default.EditorFix switch
            {
                ("1") => true,
                _ => false,
            };
        }
        #region App_Edition
        private void App_Edition_EHE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (App.S4HE_AppPath != null)
            {
                App_Edition_EHE_Button.IsChecked = true;
            }
            //EHE
        }
        private void App_Edition_EHE_Button_Checked(object sender, RoutedEventArgs e)
        {
            //EHE
            App_Edition_EGE_Button.IsChecked = false;
            App_Edition_HE_Button.IsChecked = false;
            App_Edition_GE_Button.IsChecked = false;
            Properties.Settings.Default.EditionInstalled = "EHE";
            Properties.Settings.Default.Save();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).SpracheFestlegen();
                }
            }
        }

        private void App_Edition_EGE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (App.S4GE_AppPath != null)
            {
                App_Edition_EGE_Button.IsChecked = true;
            }
        }
        private void App_Edition_EGE_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Edition_EHE_Button.IsChecked = false;
            //EGE
            App_Edition_HE_Button.IsChecked = false;
            App_Edition_GE_Button.IsChecked = false;
            Properties.Settings.Default.EditionInstalled = "EGE";
            Properties.Settings.Default.Save();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).SpracheFestlegen();
                }
            }
        }
        private void App_Edition_HE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (App.S4HE_AppPath != null)
            {
                App_Edition_HE_Button.IsChecked = true;
            }
            //HE
        }
        private void App_Edition_HE_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Edition_EHE_Button.IsChecked = false;
            App_Edition_EGE_Button.IsChecked = false;
            //HE
            App_Edition_GE_Button.IsChecked = false;
            Properties.Settings.Default.EditionInstalled = "HE";
            Properties.Settings.Default.Save();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).SpracheFestlegen();
                }
            }

        }
        private void App_Edition_GE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (App.S4GE_AppPath != null)
            {
                App_Edition_GE_Button.IsChecked = true;
            }
            //GE
        }

        private void App_Edition_GE_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Edition_EHE_Button.IsChecked = false;
            App_Edition_EGE_Button.IsChecked = false;
            App_Edition_HE_Button.IsChecked = false;
            //GE
            Properties.Settings.Default.EditionInstalled = "GE";
            Properties.Settings.Default.Save();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).SpracheFestlegen();
                }
            }
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
            Properties.Settings.Default.TexturesInstalled = "ORG";
            Properties.Settings.Default.Save();
        }
        private void App_Textures_NW_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App_Textures_NW_Button.IsChecked = true;
        }
        private void App_Textures_NW_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Textures_ORG_Button.IsChecked = false;
            Properties.Settings.Default.TexturesInstalled = "NW";
            Properties.Settings.Default.Save();
        }
        #endregion
        #region Mods
        private void App_Mod_Zoom_CheckBox_Check(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Mod_Zoom = App_Mod_Zoom_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Mod_Hotkeys_CheckBox_Check(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Mod_HotKeys = App_Mod_Hotkeys_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }

        private void App_ModCustom_UBO_CheckBox_Check(object sender, RoutedEventArgs e)
        {
            if ((bool)App_ModCustom_UBO_CheckBox.IsChecked && Properties.Settings.Default.Mod_UBO == "0")
            {
                MessageBox.Show(Properties.Resources.MSB_ModCustom_UBO_Text, Properties.Resources.MSB_ModCustom_UBO, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Properties.Settings.Default.Mod_UBO = App_ModCustom_UBO_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        #endregion
        #region Music
        private void App_Music_S3_CheckBox_Check(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Music_S3 = App_Music_S3_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        #endregion
        #region App_Maps
        private void Button_Maps(object sender, RoutedEventArgs e)
        {
            switch (Properties.Settings.Default.EditionInstalled)
            {
                default:
                case ("HE"):
                case ("EHE"):
                    {
                        var runExplorer = new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = @"/N," + App.S4HE_AppPath.Replace(@"/", @"\") + @"Map"
                        };
                        Process.Start(runExplorer);
                        break;
                    }
                case ("GE"):
                case ("EGE"):
                    {
                        var runExplorer = new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = @"/N," + App.S4GE_AppPath.Replace(@"/", @"\") + @"\Map"
                        };
                        Process.Start(runExplorer);
                        break;
                    }
            }
        }
        private void Button_TM2021Maps(object sender, RoutedEventArgs e)
        {
            switch (Properties.Settings.Default.EditionInstalled)
            {
                default:
                case ("HE"):
                case ("EHE"):
                    {
                        Window Maps2021 = new S4EE.Windows.Maps2021();
                        Maps2021.ShowDialog();
                        break;
                    }
            }
        }

        private void Button_RND_Click(object sender, RoutedEventArgs e)
        {

            Window RND = new S4EE.Windows.RND();
            RND.ShowDialog();
        }

        private void App_Maps_O01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_O01 = App_Maps_O01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Maps_S01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_S01 = App_Maps_S01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Maps_T01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_T01 = App_Maps_T01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Maps_T02_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_T02 = App_Maps_T02_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Maps_C01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_C01 = App_Maps_C01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Maps_B01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_B01 = App_Maps_B01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();

        }
        private void App_Maps_M01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_M01 = App_Maps_M01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Maps_TR01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_TR01 = App_Maps_TR01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Maps_R01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_R01 = App_Maps_R01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Maps_MS01_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Map_MS01 = App_Maps_MS01_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        #endregion
        #region App_Lang
        private void App_Language_enUS_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Properties.Settings.Default.EditorInstalled = "0";
            App_Language_enUS_Button.IsChecked = true;
        }
        private void App_Language_enUS_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Language_deDE_Button.IsChecked = false;
            Properties.Settings.Default.Language = "en-US";
            Log.LogWriter(LogName, "App_Language_enUS_Button_Checked");
            LangChange();
            Log.LogWriter(LogName, "App_Language_enUS_Button_Checked_FIN");

        }
        private void App_Language_deDE_Button_Checked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Properties.Settings.Default.EditorInstalled = "0";
            App_Language_deDE_Button.IsChecked = true;
        }
        private void App_Language_deDE_Button_Checked(object sender, RoutedEventArgs e)
        {
            App_Language_enUS_Button.IsChecked = false;
            Properties.Settings.Default.Language = "de-DE";
            Log.LogWriter(LogName, "App_Language_deDE_Button_Checked");
            LangChange();
            Log.LogWriter(LogName, "App_Language_deDE_Button_Checked_FIN");
        }
        #endregion
        #region App_Misc
        private void App_Misc_SAVE_AUTOSAVE_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoSave = App_Misc_SAVE_AUTOSAVE_CheckBox.IsChecked switch
            {
                true => "5",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Misc_SAVE_SAVECLEANER_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SaveCleaner = App_Misc_SAVE_SAVECLEANER_CheckBox.IsChecked switch
            {
                true => "48",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Misc_VIDEO_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.VideosShow = App_Misc_VIDEO_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Misc_MISSIONS_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Missions = App_Misc_MISSIONS_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Misc_MINE_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Minen = App_Misc_MINE_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Misc_LEGACYCONTROLS_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LegacyControls = App_Misc_LEGACYCONTROLS_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        private void App_Misc_FIXES_EDITOR_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.EditorFix = App_Misc_FIXES_EDITOR_CheckBox.IsChecked switch
            {
                true => "1",
                false => "0",
            };
            Properties.Settings.Default.Save();
        }
        #endregion
        #region Sprachenmethoden
        public void LangSet()
        {
            try
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
                SpracheFestlegen();
                LangChange();
            }
            catch (Exception)
            {

            }
        }
        private void LangChange()
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
            SpracheFestlegen();
            Properties.Settings.Default.Save();
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
            App_ModCustom_Title.Content = Properties.Resources.App_ModCustom_Title;
            App_ModCustom_UBO.Content = Properties.Resources.App_ModCustom_UBO;
            App_Music_Title.Content = Properties.Resources.App_Music_Title;
            App_Music_S3.Content = Properties.Resources.App_Music_S3;
            App_Music_S3_Information_Text.Content = Properties.Resources.App_Music_S3_Information_Text;
            App_Maps_Title_Orignal.Content = Properties.Resources.App_Maps_Orignal;
            App_Maps_O01.Content = Properties.Resources.App_Maps_O01;
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
            App_Maps_Title_MISC.Content = Properties.Resources.App_Maps_Title_MISC;
            App_Maps_MS01.Content = Properties.Resources.App_Maps_MS01;
            App_Language_Title.Content = Properties.Resources.App_Language_Title;
            App_Language_enUS.Content = Properties.Resources.App_Language_enUS;
            App_Language_deDE.Content = Properties.Resources.App_Language_deDE;
            App_Misc_Tools.Content = Properties.Resources.App_Misc_Tools;
            App_Misc_RNG.Content = Properties.Resources.App_RND;
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
            if (Properties.Settings.Default.Language != "")
            {
                Style style = this.FindResource("Button_Settings_Maps_Folder_" + Properties.Settings.Default.Language) as Style;
                Button_Maps1.Style = style;
            }
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).SpracheFestlegen();
                }
            }
        }
        #endregion

        private void Button_Hotkeys(object sender, RoutedEventArgs e)
        {
            Window Info = new S4EE.Windows.Info();
            Info.ShowDialog();
        }


    }
}