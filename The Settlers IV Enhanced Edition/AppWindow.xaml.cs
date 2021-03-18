using Microsoft.Win32;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace S4EE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        readonly AppStart AppStart;
        readonly AppWebView AppWebView;
        readonly AppSettings AppSettings;
        private static readonly SHA256 Sha256 = SHA256.Create();
        private static readonly string LogName = "App.xaml.cs";
        /// <summary>
        /// Startmethode der Anwendung
        /// </summary>
        public AppWindow()
        {
            InitializeComponent();
            //INIT
            Log.LogWriter(LogName, "InitializeComponent");
            AppStart = new AppStart();
            Log.LogWriter(LogName, "AppStart");
            AppWebView = new AppWebView();
            Log.LogWriter(LogName, "AppWebView");
            AppSettings = new AppSettings();
            Log.LogWriter(LogName, "AppSettings");
            //NAV
            FrameContent.Navigate(AppStart);
            Log.LogWriter(LogName, "AppStart");
            //CHECKS
            Installationscheck();
            Log.LogWriter(LogName, "Installationscheck");
            CleanUp();
            Log.LogWriter(LogName, "CleanUp");
            CheckUpdate();
            Log.LogWriter(LogName, "CheckUpdate");
        }
        #region Updater
        private static void CleanUp()
        {
            if (File.Exists("TheSettlersIVEnhancedEditionSetup.exe"))
            {
                Log.LogWriter(LogName, "CleanUp Old Update File");
                File.Delete("TheSettlersIVEnhancedEditionSetup.exe");
            }
            Log.LogWriter(LogName, "No CleanUp");
        }
        public void Installationscheck()
        {
            Checksumme();
            Title = Properties.Resources.App_Name;
            // Versionsinfo der Assembly
            Versiontext.Content = "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Log.LogWriter(LogName, "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (Properties.Settings.Default.Map_O01 == "")
            {
                Log.LogWriter(LogName, "Maps Default Original");
                Properties.Settings.Default.Map_O01 = "1";
                Properties.Settings.Default.Save();
                Log.LogWriter(LogName, "Maps Default Original Set");
            }
            if (Properties.Settings.Default.Language == "")
            {
                Log.LogWriter("VersionChange", "Keine Sprache gefunden: Suche nach Standardsprache");
                if (App.S4HE_AppPath != null)
                {
                    string settings = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TheSettlers4\Config\GameSettings.cfg";
                    if (File.Exists(settings))
                    {
                        IniFile s4settings = new(settings);
                        Properties.Settings.Default.Language = s4settings.Read("Language", "GAMESETTINGS") switch
                        {
                            ("1") => "de-DE",
                            _ => "en-US",
                        };
                        Properties.Settings.Default.Save();
                        AppSettings.LangSet();
                        Log.LogWriter("VersionChange", "Sprache gesetzt " + Properties.Settings.Default.EditionInstalled + " " + Properties.Settings.Default.Language);
                        return;
                    }
                }
                else if (App.S4GE_AppPath != null)
                {
                    string S4GE_Lang = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\BlueByte\Settlers4", "Language", null);
                    if (S4GE_Lang != null)
                    {
                        Properties.Settings.Default.Language = S4GE_Lang switch
                        {
                            ("1") => "de-DE",
                            _ => "en-US",
                        };
                        Properties.Settings.Default.Save();
                        AppSettings.LangSet();
                        Log.LogWriter("VersionChange", "Sprache gesetzt " + Properties.Settings.Default.EditionInstalled + " " + Properties.Settings.Default.Language);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }
            if (Properties.Settings.Default.EditionInstalled == "")
            {
                Log.LogWriter("VersionChange", "Keine Installation gefunden: Suche nach Installationen");
                if (App.S4HE_AppPath != null)
                {
                    Properties.Settings.Default.EditionInstalled = "EHE";
                    AppSettings.App_Edition_EHE_Button.IsChecked = true;
                    Properties.Settings.Default.Save();
                }
                else if (App.S4GE_AppPath != null)
                {
                    Properties.Settings.Default.EditionInstalled = "EGE";
                    AppSettings.App_Edition_EGE_Button.IsChecked = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }
            if (Properties.Settings.Default.TexturesInstalled == "")
            {
                Log.LogWriter("VersionChange", "Keine Texturen gefunden: Suche nach Texturen");

                if (App.S4HE_AppPath != null && (Properties.Settings.Default.EditionInstalled == "HE" || Properties.Settings.Default.EditionInstalled == "EHE"))
                {

                    if (GetMD5Hash(App.S4HE_AppPath + @"\Gfx\2.gl5") == "A878998C135502053A5ED532C198CCACD15AC5F7331512D6FC73FE406FDB59CD")
                    {
                        Properties.Settings.Default.TexturesInstalled = "ORG";
                    }
                    else if (GetMD5Hash(App.S4HE_AppPath + @"\Gfx\2.gl5") == "2BECBFD70680D91BFFB29888EBA5D79A1E178C9F1A13AEB7598C83EA90481B9D")
                    {
                        Properties.Settings.Default.TexturesInstalled = "NW";
                    }
                    Properties.Settings.Default.Save();
                    Log.LogWriter("VersionChange", "Texturen Installed " + Properties.Settings.Default.TexturesInstalled);
                }
                else if (App.S4GE_AppPath != null && (Properties.Settings.Default.EditionInstalled == "GE" || Properties.Settings.Default.EditionInstalled == "EHE"))
                {
                    if (GetMD5Hash(App.S4GE_AppPath + @"\Gfx\2.gl5") == "A878998C135502053A5ED532C198CCACD15AC5F7331512D6FC73FE406FDB59CD")
                    {
                        Properties.Settings.Default.TexturesInstalled = "ORG";
                    }
                    else if (GetMD5Hash(App.S4GE_AppPath + @"\Gfx\2.gl5") == "2BECBFD70680D91BFFB29888EBA5D79A1E178C9F1A13AEB7598C83EA90481B9D")
                    {
                        Properties.Settings.Default.TexturesInstalled = "NW";
                    }
                    Properties.Settings.Default.Save();
                    Log.LogWriter("VersionChange", "Texturen Installed " + Properties.Settings.Default.TexturesInstalled);
                }
                else
                {
                    Log.LogWriter("VersionChange", "Keine Texturen gefunden: Suche nach Texturen abgeschlossen mit Fehler");
                }

            }
            SpracheFestlegen();
        }
        private void Checksumme()
        {
            //ToDo V1.2 AntiCheat
            if (App.S4HE_AppPath == null)
            {
                MessageBox.Show(Properties.Resources.MSB_Error_Mod, Properties.Resources.MSB_Error_Mod_Text, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                return;
            }
        }
        public void SpracheFestlegen()
        {
            //GUI
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("EHE"):
                    Logo.Source = new BitmapImage(new Uri((@"/Resources/Logo_Enhanced_History_Edition_" + Properties.Settings.Default.Language + @"_200px.png"), UriKind.RelativeOrAbsolute));
                    break;
                case ("EGE"):
                    Logo.Source = new BitmapImage(new Uri(@"/Resources/Logo_Enhanced_Gold_Edition_" + Properties.Settings.Default.Language + @"_200px.png", UriKind.RelativeOrAbsolute));
                    break;
                case ("HE"):
                    Logo.Source = new BitmapImage(new Uri(@"/Resources/Logo_History_Edition_" + Properties.Settings.Default.Language + @"_200px.png", UriKind.RelativeOrAbsolute));
                    break;
                case ("GE"):
                    Logo.Source = new BitmapImage(new Uri(@"/Resources/Logo_Gold_Edition_" + Properties.Settings.Default.Language + @"_200px.png", UriKind.RelativeOrAbsolute));
                    break;
            }
            //Sprache der Buttons Festlegen
            if (Properties.Settings.Default.Language != "")
            {
                Style style = this.FindResource("Button_PLAY_" + Properties.Settings.Default.Language) as Style;
                Button_PlayClick2.Style = style;
                style = this.FindResource("Button_Settings_" + Properties.Settings.Default.Language) as Style;
                Button_Settings2.Style = style;
            }
        }
        private static string GetMD5Hash(string filename)
        {
            using FileStream stream = File.OpenRead(filename);
            return BitConverter.ToString(Sha256.ComputeHash(stream)).Replace("-", "");
        }
        private void InstallprogressLogger(string module, string taskname, int Progress, int maxProgess)
        {
            DownlaodLabel.Content = module + ": " + taskname;
            ProgressBar.Value = (Progress / maxProgess) * 100;
            LogInfo.Inlines.Add("(" + Progress + @"/" + maxProgess + ") " + module + ": " + taskname + Environment.NewLine);
            LogInfoScroller.ScrollToBottom();
            Log.LogWriter(LogName, "(" + Progress + @"/" + maxProgess + ") " + module + ": " + taskname);
        }
        async Task InstallerAsync()
        {
            string Installieren = Properties.Resources.App_Install_Installiere;
            string Deinstallieren = Properties.Resources.App_Install_Deinstalliere;
            string Installiert = Properties.Resources.App_Install_Installiert;
            string Deinstalliert = Properties.Resources.App_Install_Deinstalliert;
            //ToDo V1.1: InstallerAsync maxProgess
            int maxProgess = 18;
            LogInfo.Inlines.Clear();
            DownlaodPanel.Visibility = Visibility.Visible;
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    string settings = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TheSettlers4\Config\GameSettings.cfg";
                    if (File.Exists(settings))
                    {
                        IniFile s4settings = new(settings);
                        switch (Properties.Settings.Default.Language)
                        {
                            case ("de-DE"):
                                s4settings.Write("Language", "1", "GAMESETTINGS");
                                break;
                            default:
                            case ("en-US"):
                                if (s4settings.Read("Language", "GAMESETTINGS") == "1")
                                {
                                    s4settings.Write("Language", "0", "GAMESETTINGS");
                                }
                                break;
                        }
                        AppSettings.LangSet();
                        Log.LogWriter("VersionChange", "Sprache gesetzt " + Properties.Settings.Default.EditionInstalled + " " + Properties.Settings.Default.Language);
                    }
                    break;
            }
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("EHE"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Edition_EHE, 1, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Edition_EHE.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Edition_EHE, 2, maxProgess);
                        break;
                    }
                case ("HE"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Edition_HE, 1, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Edition_HE.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Edition_HE, 2, maxProgess);

                        break;
                    }
                case ("EGE"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Edition_EGE, 1, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Edition_EGE.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Edition_EGE, 2, maxProgess);

                        break;
                    }
                case ("GE"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Edition_GE, 1, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Edition_GE.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Edition_GE, 2, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.TexturesInstalled)
            {
                case ("ORG"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Textures_ORG, 3, maxProgess);

                        await Worker.ZipInstallerAsync(@"Artifacts\Textures_OldWorld.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Textures_ORG, 4, maxProgess);
                        break;
                    }
                case ("NW"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Textures_NW, 3, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Textures_NewWorld.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Textures_NW, 4, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Mod_Zoom)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Mod_Zoom, 5, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Mod_Zoom.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Mod_Zoom, 6, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Mod_Zoom, 5, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Mod_Zoom_Deinstallieren.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Mod_Zoom, 6, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Mod_HotKeys)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Mod_Hotkeys, 7, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Mod_HotKey.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Mod_Hotkeys, 8, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Mod_Hotkeys, 7, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Mod_Hotkey_Deinstallieren.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Mod_Hotkeys, 8, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Music_S3)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Music_S3, 9, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Music_S3.zip");
                        CopyS3();
                        InstallprogressLogger(Installiert, Properties.Resources.App_Music_S3, 10, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Music_S3, 9, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Music_S3_Deinstallieren.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Music_S3, 10, maxProgess);
                        break;
                    }
                    {

                    }
            }
            switch (Properties.Settings.Default.Map_O01)
            {
                default:
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_O01, 11, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_ORGINAL.zip", "Map");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_O01, 12, maxProgess);
                        break;
                    }
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_O01, 11, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_ORGINAL.zip", "Ma2");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_O01, 12, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Map_S01)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_S01, 13, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_KAMPANGE_Pack01.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_S01, 14, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_S01, 13, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_KAMPANGE_Pack01_Un.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_S01, 14, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Map_T01)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_T01, 15, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_TOURNAMENT_TM2020.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_T01, 16, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_T01, 15, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_TOURNAMENT_TM2020_Un.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_T01, 16, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Map_T02)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_T02, 17, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_TOURNAMENT_WC2021.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_T02, 18, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_T02, 17, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_TOURNAMENT_WC2021_Un.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_T02, 18, maxProgess);
                        break;
                    }
            }
            //ToDo V1.1: InstallerAsync Maps
            //ToDo V1.2: Remastered Maps
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    {
                        string settings = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TheSettlers4\Config\GameSettings.cfg";
                        IniFile s4settings = new(settings);
                        switch (Properties.Settings.Default.AutoSave)
                        {
                            case ("5"):
                            default:
                                {
                                    s4settings.Write("AutoSaveInterval", "5", "GAMESETTINGS");
                                    break;
                                }
                            case ("0"):
                                {
                                    s4settings.Write("AutoSaveInterval", "0", "GAMESETTINGS");
                                    break;
                                }
                        }
                        break;
                    }
            }
            DownlaodPanel.Visibility = Visibility.Hidden;
        }
        private static void CopyS3()
        {
            DirectoryInfo dir = new(App.S3HE_AppPath + @"\Theme");

            if (!dir.Exists)
            {
                return;
            }

            //_ = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    // If the destination directory doesn't exist, create it.       
                    Directory.CreateDirectory(App.S4HE_AppPath + @"\Snd\S3\");

                    // Get the files in the directory and copy them to the new location.
                    foreach (FileInfo file in files)
                    {
                        string tempPath = Path.Combine(App.S4HE_AppPath + @"\Snd\S3\", file.Name);
                        file.CopyTo(tempPath, true);
                    }
                    break;
                case ("GE"):
                case ("EGE"):
                    // If the destination directory doesn't exist, create it.       
                    Directory.CreateDirectory(App.S4GE_AppPath + @"\Snd\S3\");

                    // Get the files in the directory and copy them to the new location.
                    foreach (FileInfo file in files)
                    {
                        string tempPath = Path.Combine(App.S4GE_AppPath + @"\Snd\S3\", file.Name);
                        file.CopyTo(tempPath, true);
                    }
                    break;
            }
        }
        #endregion
        #region Downloader&ZIP
        private void DownloadFileAsync(string URI, string File, string Name)
        {
            DownlaodPanel.Visibility = Visibility.Visible;
            try
            {
                using WebClient wc = new();
                DownlaodLabel.Content = Properties.Resources.App_Update_Downlaod + "\n" + Name;
                wc.DownloadProgressChanged += DownloadProgressChanged;
                wc.DownloadFileCompleted += DownloadFileEventCompleted;
                wc.DownloadFileAsync(new Uri(URI), File);
                Log.LogWriter(LogName, "Download New File Async");
            }
            catch (Exception)
            {
                Log.LogWriter(LogName, "Failed to download File");
            }
        }
        private void DownloadFileEventCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Log.LogWriter(LogName, "Download New File Abgeschlossen");
            DownlaodLabel.Content = Properties.Resources.App_Update_Abgeschlossen;
            Log.LogWriter(LogName, "Install Update");
            //Process.Start("TheSettlersIVEnhancedEditionSetup.exe", "/silent");
            var startInfo = new ProcessStartInfo
            {
                FileName = "TheSettlersIVEnhancedEditionSetup.exe",
                Arguments = @"/silent"
            };
            Process.Start(startInfo);
        }
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownlaodPanel.Visibility = Visibility.Visible;
            ProgressBar.Value = e.ProgressPercentage;
        }
        #endregion
        #region Buttons
        /// <summary>
        /// Navigation zur Play Page
        /// </summary>
        private void Button_PlayClick(object sender, RoutedEventArgs e)
        {
            Log.LogWriter(LogName, "CheckUpdate");

            if (CheckUpdate())
            {
                Log.LogWriter(LogName, "Kein Update gefunden");
                Play();
            }
            Log.LogWriter(LogName, "Update gefunden");
        }
        private async void Play()
        {
            FrameContent.Navigate(AppStart);
            Log.LogWriter(LogName, "Navigate AppStart");
            await InstallerAsync();
            Checksumme();
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    {
                        Log.LogWriter(LogName, "Start S4_Main.exe");
                        Process.Start(App.S4HE_AppPath + @"\S4_Main.exe");
                        break;
                    }
                case ("GE"):
                case ("EGE"):
                    {
                        var startInfo = new ProcessStartInfo
                        {
                            WorkingDirectory = App.S4GE_AppPath,
                            FileName = @"cmd.exe",
                            Arguments = "/C mklink /d \"Exe\\Plugins\" \"..\\plugins\""
                        };
                        Process.Start(startInfo);
                        Log.LogWriter(LogName, "Start S4.exe");
                        Process.Start(App.S4GE_AppPath + @"\S4.exe");
                        break;
                    }
            }
        }
        /// <summary>
        /// Navigation zur News Page
        /// </summary>
        private void Button_NewsClick(object sender, RoutedEventArgs e)
        {
            FrameContent.Navigate(AppWebView);
            switch (Properties.Settings.Default.Language)
            {
                default:
                case ("en-US"):
                    {
                        AppWebView.webView.Source = new Uri("https://www.zocker-lounge.com/s4-enhanced-edition/news-eng");
                        break;
                    }
                case ("de-DE"):
                    {
                        AppWebView.webView.Source = new Uri("https://www.zocker-lounge.com/s4-enhanced-edition/news");
                        break;
                    }
            }
            Log.LogWriter(LogName, "Navigate AppWebView");
        }
        /// <summary>
        /// Navigation zur ChangeLog
        /// </summary>
        private void Button_ChangeLogClick(object sender, RoutedEventArgs e)
        {
            FrameContent.Navigate(AppWebView);
            switch (Properties.Settings.Default.Language)
            {
                default:
                case ("en-US"):
                    {
                        AppWebView.webView.Source = new Uri("https://www.zocker-lounge.com/s4-enhanced-edition/changelog-eng");
                        break;
                    }
                case ("de-DE"):
                    {
                        AppWebView.webView.Source = new Uri("https://www.zocker-lounge.com/s4-enhanced-edition/changelog-de");
                        break;
                    }
            }
            Log.LogWriter(LogName, "Navigate AppWebView");
        }
        /// <summary>
        /// Startmethode vom Editor 
        /// Zunächst wird dafür überprüft ob der Editor bereits einmal gestartet wurde. 
        /// </summary>
        private async void Button_Editor(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.EditorInstalled != "1")
            {
                Log.LogWriter(LogName, "Editor Plus Installation gestartet");
                await Worker.ZipInstallerAsync(@"Artifacts\Editor_Plus.zip");
                switch (Properties.Settings.Default.Language)
                {
                    default:
                    case ("en-US"):
                        {
                            var startInfo = new ProcessStartInfo
                            {
                                WorkingDirectory = App.S4HE_AppPath + @"\Editor\",
                                FileName = App.S4HE_AppPath + @"\Editor\" + @"RunEditorEN_v2.bat",
                                CreateNoWindow = true
                            };
                            Process.Start(startInfo);
                            break;
                        }
                    case ("de-DE"):
                        {
                            var startInfo = new ProcessStartInfo
                            {
                                WorkingDirectory = App.S4HE_AppPath + @"\Editor\",
                                FileName = App.S4HE_AppPath + @"\Editor\" + @"RunEditorDE_v2.bat",
                                CreateNoWindow = true
                            };
                            Process.Start(startInfo);
                            break;
                        }
                }
                Properties.Settings.Default.EditorInstalled = "1";
                Properties.Settings.Default.Save();
                Log.LogWriter(LogName, "EditorInstalled True");
            }
            else
            {
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = App.S4HE_AppPath + @"\Editor\",
                    FileName = App.S4HE_AppPath + @"\Editor\" + @"RunEditor_Fast.bat",
                    CreateNoWindow = true
                };
                Process.Start(startInfo);
                Log.LogWriter(LogName, "EditorInstalled Start");
            }
        }
        private void Button_Settings(object sender, RoutedEventArgs e)
        {
            FrameContent.Navigate(AppSettings);
            Log.LogWriter(LogName, "Navigate AppSettings");
        }
        private void ButtonWebsite_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowser(@"https://www.diesiedler4.de");
        }
        private void ButtonZL_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowser(@"https://www.zocker-lounge.com/");
        }
        private void ButtonDC_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowser(@"https://discord.gg/NpSbDCZ");
        }
        private void ButtonYT_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowser(@"https://www.youtube.com/zockerlounge");
        }
        private void ButtonTW_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowser(@"https://www.twitch.tv/zockerlounger");
        }
        public static void OpenBrowser(string url)
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        #endregion
        #region Update
        public bool CheckUpdate()
        {
            LogInfo.Inlines.Clear();
            GitHubClient client = new(new ProductHeaderValue("s4enhancededition"));
            Task<IReadOnlyList<Release>> releaseTask;
            try
            {
                releaseTask = client.Repository.Release.GetAll("s4enhancededition", "s4enhancededition");
                releaseTask.Wait(1000);
            }
            catch (Exception)
            {
                Log.LogWriter(LogName, "Kein Update gefunden - Error Github");
                //MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            IReadOnlyList<Release> releases = releaseTask.Result;
            if (releases.Count == 0)
            {
                Log.LogWriter(LogName, "Kein Update gefunden - Error Github");
                //MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            Version version = new("0.0.0.0");
            Release lrelease = new("");
            foreach (Release release in releases)
            {
                Version lokal = new(release.TagName);
                if (release.Prerelease)
                {
                    if (!App.DebugFlag)
                    {
                        break;
                    }
                }
                if (lokal > version)
                {
                    version = lokal;
                    lrelease = release;
                }
            }
            if (version > System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)
            {
                foreach (ReleaseAsset Asset in lrelease.Assets)
                {
                    if (Asset.Name == "TheSettlersIVEnhancedEditionSetup.exe")
                    {
                        DownloadFileAsync(Asset.BrowserDownloadUrl, "TheSettlersIVEnhancedEditionSetup.exe", lrelease.TagName);
                        Log.LogWriter(LogName, "Update gefunden");
                        return false;
                    }
                }
            }
            else
            {
                Log.LogWriter(LogName, "Kein Update gefunden");
            }
            return true;
        }
        #endregion
    }
}
