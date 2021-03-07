using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;
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
        private static readonly string S4_AppPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher\Installs\11785", "InstallDir", null);
        /// <summary>
        /// Startmethode der Anwendung
        /// </summary>
        public AppWindow()
        {
            InitializeComponent();

            if (S4_AppPath == null)
            {
                Environment.Exit(1);
            }
            AppStart = new AppStart();
            AppWebView = new AppWebView();
            AppSettings = new AppSettings();
            VersionChange();
            FrameContent.Navigate(AppStart);

            // Versionsinfo der Assembly
            Versiontext.Content = "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        public void VersionChange()
        {
            if (Properties.Settings.Default.Language == "de-DE")
            {              
                Logo.Source = new BitmapImage(new Uri(@"/Resources/Logo_Enhanced_History_Edition_GER_200px.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                Logo.Source = new BitmapImage(new Uri(@"/Resources/Logo_Enhanced_History_Edition_ENG_200px.png", UriKind.RelativeOrAbsolute));
            }
            Title = Properties.Resources.App_Name;
        }

        public bool Installer()
        {
            return true;
        }
        /// <summary>
        /// Navigation zur Play Page
        /// </summary>
        private void Button_PlayClick(object sender, RoutedEventArgs e)
        {
            FrameContent.Navigate(AppStart);
            if (Installer())
            {
                Process.Start(S4_AppPath + @"\S4_Main.exe");
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
                                WorkingDirectory = S4_AppPath + @"\Editor\",
                                FileName = S4_AppPath + @"\Editor\" + @"RunEditorEN.bat",
                                CreateNoWindow = true
                            };
                            Process.Start(startInfo);
                            break;
                        }
                    case ("de-DE"):
                        {
                            var startInfo = new ProcessStartInfo
                            {
                                WorkingDirectory = S4_AppPath + @"\Editor\",
                                FileName = S4_AppPath + @"\Editor\" + @"RunEditorDE.bat",
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
                    WorkingDirectory = S4_AppPath + @"\Editor\",
                    FileName = S4_AppPath + @"\Editor\" + @"RunEditorFast.bat",
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
    }
}