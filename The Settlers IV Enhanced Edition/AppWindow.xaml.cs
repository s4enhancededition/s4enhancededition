using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Windows;
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


        /// <summary>
        /// Startmethode der Anwendung
        /// </summary>
        public AppWindow()
        {
            InitializeComponent();
            Log.LogWriter("START APP", "-------------------------------------");

            AppStart = new AppStart();
            AppWebView = new AppWebView();
            AppSettings = new AppSettings();
            VersionChange(true);
            FrameContent.Navigate(AppStart);

            // Versionsinfo der Assembly
            Versiontext.Content = "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }



        public void VersionChange(bool load = false)
        {
            Title = Properties.Resources.App_Name;

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
                    Properties.Settings.Default.EditionNew = "EHE";
                    Log.LogWriter("VersionChange", "Editionsinstallation in Warteschlange " + Properties.Settings.Default.EditionNew);
                    load = false;
                    Properties.Settings.Default.Save();
                }
                else if (App.S4GE_AppPath != null)
                {
                    Properties.Settings.Default.EditionNew = "EGE";
                    Log.LogWriter("VersionChange", "Editionsinstallation in Warteschlange " + Properties.Settings.Default.EditionNew);
                    load = false;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }

            if (Properties.Settings.Default.EditionNew != "" && !load)
            {
                //ToDo Install Code
                Properties.Settings.Default.EditionInstalled = Properties.Settings.Default.EditionNew;
                Properties.Settings.Default.EditionNew = "";
                Properties.Settings.Default.Save();
                Log.LogWriter("VersionChange", "Neue Edition installiert " + Properties.Settings.Default.EditionInstalled);
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
                    load = false;
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
                    load = false;
                }
                else
                {
                    Log.LogWriter("VersionChange", "Keine Texturen gefunden: Suche nach Texturen abgeschlossen mit Fehler");
                }

            }
            if (Properties.Settings.Default.TexturesNew != "" && !load)
            {
                //ToDo Install Code
                Properties.Settings.Default.TexturesInstalled = Properties.Settings.Default.TexturesNew;
                Properties.Settings.Default.TexturesNew = "";
                Properties.Settings.Default.Save();
                Log.LogWriter("VersionChange", "Neue Texturen installiert " + Properties.Settings.Default.TexturesInstalled);
            }
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
            if (AppSettings != null)
            {
                AppSettings.Load(true);
            }
        }

        public static string GetMD5Hash(string filename)
        {
            using FileStream stream = File.OpenRead(filename);
            return BitConverter.ToString(Sha256.ComputeHash(stream)).Replace("-", "");
        }

        public bool Installer()
        {
            return true;
        }

        #region Downloader&ZIP

        private void DownloadFileEventCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownlaodPanel.Visibility = Visibility.Hidden;
            //switch (Patch)
            //{
            //    case 0:
            //        PatchLauncher();
            //        break;
            //    case 1:
            //        PatchDLC();
            //        CheckNewUpdateDLC();
            //        break;
            //    case 2:
            //        PatchEdition();
            //        Patch = -1;
            //        CheckVersion();
            //        break;
            //}


        }
        private static void DownloadFileSync(string URI, string File)
        {
            try
            {
                using WebClient wc = new();
                wc.DownloadFile(new Uri(URI), File);
            }
            catch (Exception)
            {
                Log.LogWriter("???","Failed to download File");
            }
        }
        private void DownloadFileAsync(string URI, string File, string Name)
        {
            DownlaodPanel.Visibility = Visibility.Visible;
            try
            {
                using WebClient wc = new();
                DownlaodLabel.Content = Name;
                wc.DownloadProgressChanged += DownloadProgressChanged;
                wc.DownloadFileCompleted += DownloadFileEventCompleted;
                wc.DownloadFileAsync(new Uri(URI), File);
            }
            catch (Exception)
            {
                Log.LogWriter("???","Failed to download File");
            }
        }
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownlaodPanel.Visibility = Visibility.Visible;
            ProgressBar.Value = e.ProgressPercentage;
        }
        private void ZIPInstallieren(string filename)
        {

            using ZipArchive archive = ZipFile.OpenRead(filename + ".zip");
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string completeFileName = Path.Combine(App.S4HE_AppPath, entry.FullName);
                string directory = Path.GetDirectoryName(completeFileName);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                try
                {
                    entry.ExtractToFile(completeFileName, true);
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion

        #region Buttons
        /// <summary>
        /// Navigation zur Play Page
        /// </summary>
        private void Button_PlayClick(object sender, RoutedEventArgs e)
        {
            FrameContent.Navigate(AppStart);
            if (Installer())
            {
                Process.Start(App.S4HE_AppPath + @"\S4_Main.exe");
            }
            else
            {
                //ToDo Error message
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
        }
        /// <summary>
        /// Startmethode vom Editor 
        /// Zunächst wird dafür überprüft ob der Editor bereits einmal gestartet wurde. 
        /// </summary>
        private void Button_Editor(object sender, RoutedEventArgs e)
        {
            if (!Properties.Settings.Default.EditorInstalled)
            {

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
                Properties.Settings.Default.EditorInstalled = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = App.S4HE_AppPath + @"\Editor\",
                    FileName = App.S4HE_AppPath + @"\Editor\" + @"RunEditorFast.bat",
                    CreateNoWindow = true
                };
                Process.Start(startInfo);
            }
        }
        private void Button_Settings(object sender, RoutedEventArgs e)
        {
            FrameContent.Navigate(AppSettings);
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
    }
}