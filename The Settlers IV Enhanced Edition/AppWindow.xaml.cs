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
        string Filename = "";
        bool Error = false;
        /// <summary>
        /// Startmethode der Anwendung
        /// </summary>
        public AppWindow()
        {
            InitializeComponent();
            if (App.DeinstallierenFlag)
            {
                Deinstaller();
            }
            else
            {
                //CHECKS
                Installationscheck();
                Log.LogWriter(LogName, "Installationscheck");
                //CHECKS
                CleanUp();
                Log.LogWriter(LogName, "CleanUp");
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
                CheckUpdate();
                Log.LogWriter(LogName, "CheckUpdate");
            }

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
            Title = Properties.Resources.App_Name;
            // Versionsinfo der Assembly
            Versiontext.Content = "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Log.LogWriter(LogName, "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (Properties.Settings.Default.Map_O01 == "")
            {
                Log.LogWriter("Maps", "Maps Default Original");
                Properties.Settings.Default.Map_O01 = "1";
                Properties.Settings.Default.Save();
                Log.LogWriter("Maps", "Maps Default Original Set");
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
                        //switch (Properties.Settings.Default.Language)
                        //{

                        //    case ("de-DE"):
                        //        {
                        //            AppSettings.App_Language_deDE_Button.IsChecked = true;
                        //            break;
                        //        }

                        //    case ("en-US"):
                        //        {
                        //            AppSettings.App_Language_enUS_Button.IsChecked = true;
                        //            break;
                        //        }

                        //}
                        //AppSettings.LangSet();
                        Log.LogWriter("VersionChange", "Sprache gesetzt " + Properties.Settings.Default.EditionInstalled + " " + Properties.Settings.Default.Language);
                    }
                    else
                    {
                        Properties.Settings.Default.Language = "en-US";
                        Properties.Settings.Default.Save();
                        //AppSettings.App_Language_enUS_Button.IsChecked = true;
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
                        switch (Properties.Settings.Default.Language)
                        {

                            case ("de-DE"):
                                {
                                    //AppSettings.App_Language_deDE_Button.IsChecked = true;
                                    break;
                                }

                            case ("en-US"):
                                {
                                    //AppSettings.App_Language_enUS_Button.IsChecked = true;
                                    break;
                                }

                        }
                        AppSettings.LangSet();
                        Log.LogWriter("VersionChange", "Sprache gesetzt " + Properties.Settings.Default.EditionInstalled + " " + Properties.Settings.Default.Language);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    //ToDo V1.4 Cleanup
                    //Environment.Exit(1);
                }
            }
            if (Properties.Settings.Default.EditionInstalled == "")
            {
                Log.LogWriter("Version", "Keine Installation gefunden: Suche nach Installationen");
                if (App.S4HE_AppPath != null)
                {
                    Properties.Settings.Default.EditionInstalled = "EHE";
                    //AppSettings.App_Edition_EHE_Button.IsChecked = true;
                    Properties.Settings.Default.Save();
                }
                else if (App.S4GE_AppPath != null)
                {
                    Properties.Settings.Default.EditionInstalled = "EGE";
                    //AppSettings.App_Edition_EGE_Button.IsChecked = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    //ToDo V1.4 Cleanup
                    //Environment.Exit(1);
                }
            }
            if (Properties.Settings.Default.TexturesInstalled == "")
            {
                Log.LogWriter("Texturen", "Keine Texturen gefunden: Suche nach Texturen");

                if (App.S4HE_AppPath != null)
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
                    Log.LogWriter("Texturen", "Texturen Installed " + Properties.Settings.Default.TexturesInstalled);
                }
                else if (App.S4GE_AppPath != null)
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
                    Log.LogWriter("Texturen", "Texturen Installed " + Properties.Settings.Default.TexturesInstalled);
                }
                else
                {
                    Log.LogWriter("Texturen", "Keine LegacyControls gefunden: Suche nach LegacyControls abgeschlossen mit Fehler");
                }
            }

            if (Properties.Settings.Default.LegacyControls == "")
            {
                Log.LogWriter("LegacyControls", "Keine LegacyControls gefunden: Suche nach LegacyControls");

                if (App.S4HE_AppPath != null)
                {
                    string settings = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TheSettlers4\Config\GameSettings.cfg";
                    IniFile s4settings = new(settings);
                    Properties.Settings.Default.LegacyControls = s4settings.Read("LegacyMouse", "GAMESETTINGS") switch
                    {
                        ("1") => "1",
                        _ => "0",
                    };
                    Properties.Settings.Default.Save();
                    Log.LogWriter("VersionChange", "LegacyControls Installed " + Properties.Settings.Default.LegacyControls);
                }
                else
                {
                    Log.LogWriter("VersionChange", "Keine LegacyControls gefunden: Suche nach LegacyControls abgeschlossen mit Fehler");
                }
            }
            SpracheFestlegen();
        }
        private void Checksumme()
        {
            Filename = "";
            Error = false;
            string Path = "";
            //Editionsglobal
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("EHE"):
                case ("HE"):
                    {
                        Path = App.S4HE_AppPath;
                        CheckMD5Hash(Path, @"binkw32.dll", "915afe5764d58fa9ee508948ae078f3673f1b052d81b365e15da6b85d9ae4d6b");
                        CheckMD5Hash(Path, @"binkw32hooked.dll", "b80c9c91cf3d354d38a89d136793a141c9cbd19e2a6118c8a3863af674e7cd8f");
                        CheckMD5Hash(Path, @"Config\AINames.cfg", "3b49bfcf7aa0754cfdd2336e6c19f8792e7492db1b663d335abbe5fc9789a2e9");
                        CheckMD5Hash(Path, @"Config\Animals.cfg", "860395c4ae455784f9e4ccb983e10afae015e476dacd7890275e7474ace0574d");
                        CheckMD5Hash(Path, @"Config\Balancing.cfg", "7c541207be13b986f275dfd00469e18ac9caded72e66a6ba5396420d46552a14");
                        CheckMD5Hash(Path, @"Config\Magic.cfg", "90bd35e3caea0718cce3be5a771a6abbd93946006e9a0c1ccd3f50f97f8419f3");
                        CheckMD5Hash(Path, @"Config\MainConfig.cfg", "db4e768730922d8982869c7277a16a9830e6d31c1cfaf0f851ed7a1f002a8f8a");
                        CheckMD5Hash(Path, @"Config\Network.cfg", "f9640edbc5d98e9d109847c71dc2da650fa3420292db6a35d187c8fc2497f64b");
                        CheckMD5Hash(Path, @"Config\Transport.cfg", "5a69deefc08d9670772f87638fafd64f54be355ba59517d893fe99d84bcc9539");
                        CheckMD5Hash(Path, @"GameData\buildingInfo.xml", "b1828250fb9a8794e98305458716ad3299d8a5d61583268d343da786abc6c0cb");
                        CheckMD5Hash(Path, @"GameData\BuildingTrigger.xml", "0430ef9758456c479f4686ebf5d8733986180706f6e2329700fbda73534a1dd9");
                        CheckMD5Hash(Path, @"GameData\effectsInfo.xml", "1ae2e51a7dc60c071c378fd311f1c13b9e945bd6c6153ba4e63821d20d911770");
                        CheckMD5Hash(Path, @"GameData\jobInfo.xml", "158a7397ff70922bbb4e0d0485eedc14aabc9fedb2510a0cf1af7178b16a56c2");
                        CheckMD5Hash(Path, @"GameData\jobSoundList.xml", "d11bb9879c003655da717e5ca860d70299019a95289c06bc18f26b6b924e4aaf");
                        CheckMD5Hash(Path, @"GameData\objectInfo.xml", "279b0d4226fb44ec9ff4c2566feb9c6f2a01ebbddd50c4c33d4d39df7338dec9");
                        CheckMD5Hash(Path, @"GameData\SettlerValuesMp.xml", "9dcfe0c4c2991591fa04233b2f90e2f0b0f590fad427f658418842790ff827d2");
                        CheckMD5Hash(Path, @"GameData\VehicleInfo.xml", "63aeec6ff03712d9052a48ae1416f2f3e57e500594e73ee9ea224bc0e6679aad");
                        CheckMD5Hash(Path, @"Mp3dec.asi", "51765b5a747bd2c5255b727557a7927c8318e09919bd9b4ef4b4019e801cd562");
                        CheckMD5Hash(Path, @"mss32.dll", "2a515199cc7a2e1ccd8d756efdaab07cedd5c3052309ef1c7499c2560e2097bd");
                        CheckMD5Hash(Path, @"S4_Main.exe", "f6dbb89072f3f0fefa35de23810a22317b8621e7a8a0aec2503ef28aaa25ea0b");
                        //CheckMD5Hash(Path, @"S4_MainD.exe", "f53bfe8335f824d44e75fa342ee79c1ef04051bb32865fd856083fc964510377");
                        //CheckMD5Hash(Path, @"S4_MainD.ilk", "514e57aca54e75b53d183460fe391262154903eb4f8213ec85ba00ae697b88ff");
                        CheckMD5Hash(Path, @"S4ModApi.dll", "add85ace2c8ac2fdfc47f0d0c01804923f97b1ab30c826589728862b3bf84a46");
                        CheckMD5Hash(Path, @"Script\Internal\CheckInstallation.txt", "c777dd5d254b14b90dae03fe32b18f11bbc86cce51d4eb8a9871379df6ac8f46");
                        CheckMD5Hash(Path, @"Script\Internal\StartResources.txt", "3c6e1668353ab642466f3061ad4ab1b4adee869f05158e21010849162074416d");
                        break;
                    }
                case ("GE"):
                case ("EGE"):
                    {
                        Path = App.S4GE_AppPath;
                        break;
                    }
            }

            //Spezifisches
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                    {
                        CheckMD5Hash(Path, @"GameData\SettlerValues.xml", "C50659FB64049D0B6C1550E0A089DB38FF8E7A3BE3AB3230BF4254E7AA20DA62");
                        break;
                    }
                case ("EHE"):
                    {
                        CheckMD5Hash(Path, @"GameData\SettlerValues.xml", "9dcfe0c4c2991591fa04233b2f90e2f0b0f590fad427f658418842790ff827d2");
                        CheckMD5Hash(Path, @"Plugins\RequestLimitExtender.asi", "a82cacd5b2fe8bc5d69ed23c86cd943de3c9b5e3f90f55672fcba0d2b1ff7191");
                        CheckMD5Hash(Path, @"Plugins\SettlerLimitExtender.asi", "b22c4d7f7544f777672f8622c38d5955ec0bccd90875c5cb0e637623c90147c3");
                    }
                    break;

                case ("GE"):
                    {
                        break;
                    }
                case ("EGE"):
                    {
                        CheckMD5Hash(Path, @"Plugins\RequestLimitExtender.asi", "a82cacd5b2fe8bc5d69ed23c86cd943de3c9b5e3f90f55672fcba0d2b1ff7191");
                        CheckMD5Hash(Path, @"Plugins\SettlerLimitExtender.asi", "b22c4d7f7544f777672f8622c38d5955ec0bccd90875c5cb0e637623c90147c3");
                        break;
                    }
            }
            if (Error)
            {
                MessageBox.Show(Properties.Resources.MSB_Error_Mod_Text + Filename, Properties.Resources.MSB_Error_Mod, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
        private void CheckMD5Hash(string Path, string File, string MD256)
        {
            try
            {
                if (GetMD5Hash(Path + File) != MD256.ToUpper())
                {
                    Filename += File + ",";
                    Error = true;
                }
            }
            catch (Exception)
            {

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
            int maxProgess = 40;
            LogInfo.Inlines.Clear();
            if (App.DebugFlag)
            {
                DownlaodPanel.Visibility = Visibility.Visible;
            }
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
                        await Worker.ZipInstallerAsync(@"Artifacts\Textures_OldWorld.zip", "Textures");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Textures_ORG, 4, maxProgess);
                        break;
                    }
                case ("NW"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Textures_NW, 3, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\Textures_NewWorld.zip", "Textures");
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
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_TOURNAMENT_WC2021_Part1.zip");
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_TOURNAMENT_WC2021_Part2.zip");
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
            switch (Properties.Settings.Default.Map_C01)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_C01, 19, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_COOP_Pack01.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_C01, 20, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_C01, 19, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_COOP_Pack01_Un.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_C01, 20, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Map_B01)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_B01, 21, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_BUDDEL_Pack01.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_B01, 22, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_B01, 21, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_BUDDEL_Pack01_Un.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_B01, 22, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Map_M01)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_M01, 23, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_METZEL_Pack01.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_M01, 24, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_M01, 23, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_METZEL_Pack01_Un.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_M01, 24, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Map_TR01)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_TR01, 25, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_TRAIN_Pack01.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_TR01, 26, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_TR01, 25, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_TRAIN_Pack01_Un.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_TR01, 26, maxProgess);
                        break;
                    }
            }
            switch (Properties.Settings.Default.Map_R01)
            {
                case ("1"):
                    {
                        InstallprogressLogger(Installieren, Properties.Resources.App_Maps_R01, 27, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_REMASTER_Pack01.zip");
                        InstallprogressLogger(Installiert, Properties.Resources.App_Maps_R01, 28, maxProgess);
                        break;
                    }
                default:
                case ("0"):
                    {
                        InstallprogressLogger(Deinstallieren, Properties.Resources.App_Maps_R01, 27, maxProgess);
                        await Worker.ZipInstallerAsync(@"Artifacts\MapPack_REMASTER_Pack01_Un.zip");
                        InstallprogressLogger(Deinstalliert, Properties.Resources.App_Maps_R01, 28, maxProgess);
                        break;
                    }
            }
            //AUTOSAVE
            InstallprogressLogger(Installieren, Properties.Resources.App_Misc_SAVE_AUTOSAVE, 29, maxProgess);
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
            InstallprogressLogger(Installiert, Properties.Resources.App_Misc_SAVE_AUTOSAVE, 30, maxProgess);
            //SAVECLEANER
            InstallprogressLogger(Installieren, Properties.Resources.App_Misc_SAVE_SAVECLEANER, 31, maxProgess);
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    {
                        switch (Properties.Settings.Default.SaveCleaner)
                        {
                            case ("48"):
                                string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TheSettlers4\Save";
                                if (Directory.Exists(folder))
                                {
                                    await Worker.SaveCleaner(folder);
                                }
                                break;
                        }
                        break;
                    }
                case ("GE"):
                case ("EGE"):
                    {
                        switch (Properties.Settings.Default.SaveCleaner)
                        {
                            case ("48"):
                                string folder = App.S4GE_AppPath + @"\Save";
                                if (Directory.Exists(folder))
                                {
                                    await Worker.SaveCleaner(folder);
                                }
                                break;
                        }
                        break;
                    }
            }
            InstallprogressLogger(Installiert, Properties.Resources.App_Misc_SAVE_SAVECLEANER, 32, maxProgess);
            //VIDEOS
            InstallprogressLogger(Installieren, Properties.Resources.App_Misc_VIDEO, 33, maxProgess);
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    {
                        string settings = App.S4HE_AppPath + @"Config\video.cfg";
                        IniFile s4settings = new(settings);
                        switch (Properties.Settings.Default.VideosShow)
                        {
                            case ("1"):
                            default:
                                {
                                    s4settings.Write("ShowVideos", "1", "ADVGAMESETTINGS");
                                    break;
                                }
                            case ("0"):
                                {
                                    s4settings.Write("ShowVideos", "0", "ADVGAMESETTINGS");
                                    break;
                                }
                        }
                        break;
                    }
                case ("GE"):
                case ("EGE"):
                    {
                        string settings = App.S4GE_AppPath + @"Config\video.cfg";
                        IniFile s4settings = new(settings);
                        switch (Properties.Settings.Default.VideosShow)
                        {
                            case ("1"):
                            default:
                                {
                                    s4settings.Write("ShowVideos", "1", "ADVGAMESETTINGS");
                                    break;
                                }
                            case ("0"):
                                {
                                    s4settings.Write("ShowVideos", "0", "ADVGAMESETTINGS");
                                    break;
                                }
                        }
                        break;
                    }
            }
            InstallprogressLogger(Installiert, Properties.Resources.App_Misc_VIDEO, 34, maxProgess);
            //MISSIONS
            InstallprogressLogger(Installieren, Properties.Resources.App_Misc_MISSIONS, 35, maxProgess);
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    {
                        switch (Properties.Settings.Default.Missions)
                        {
                            case ("1"):
                                {

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
                                }
                            default:
                            case ("0"):
                                {
                                    break;
                                }
                        }
                        break;
                    }
                case ("GE"):
                case ("EGE"):
                    {
                        switch (Properties.Settings.Default.Missions)
                        {
                            case ("1"):
                                {
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
                                    File.WriteAllLines(App.S4GE_AppPath + @"\Config\MiscData2.cfg", lines);
                                    break;
                                }
                            default:
                            case ("0"):
                                {
                                    break;
                                }
                        }
                        break;
                    }
            }
            InstallprogressLogger(Installiert, Properties.Resources.App_Misc_MISSIONS, 36, maxProgess);
            //MINEN
            InstallprogressLogger(Installieren, Properties.Resources.App_Misc_MINE, 37, maxProgess);
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    {
                        switch (Properties.Settings.Default.Minen)
                        {
                            case ("1"):
                                {
                                    IniFile s4settings = new(App.S4HE_AppPath + @"Config\WarningTypes.cfg");
                                    s4settings.Write("GUI_WARN_MINE_EMPTY", "<WARNING_ECO_MISSING_RESOURCES>", "WARNINGMSG_CLASSIFICATION");
                                    break;
                                }
                            default:
                            case ("0"):
                                {
                                    IniFile s4settings = new(App.S4HE_AppPath + @"Config\WarningTypes.cfg");
                                    s4settings.Write("GUI_WARN_MINE_EMPTY", "", "WARNINGMSG_CLASSIFICATION");
                                    break;
                                }
                        }
                        break;
                    }
            }
            InstallprogressLogger(Installiert, Properties.Resources.App_Misc_MINE, 38, maxProgess);
            //LegacyControls
            InstallprogressLogger(Installieren, Properties.Resources.App_Misc_LEGACYCONTROLS, 39, maxProgess);
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    {
                        string settings = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TheSettlers4\Config\GameSettings.cfg";
                        IniFile s4settings = new(settings);
                        switch (Properties.Settings.Default.LegacyControls)
                        {
                            case ("1"):
                                {
                                    s4settings.Write("LegacyMouse", "1", "GAMESETTINGS");
                                    break;
                                }
                            case ("0"):
                                {
                                    s4settings.Write("LegacyMouse", "0", "GAMESETTINGS");
                                    break;
                                }
                        }
                        break;
                    }
            }
            InstallprogressLogger(Installiert, Properties.Resources.App_Misc_LEGACYCONTROLS, 40, maxProgess);

            //ToDo V1.4 Cleanup "Ressourcen Gefunden abschalten"
            //IniFile s4settings = new(App.S4GE_AppPath + @"Config\GAMESETTINGS.cfg");
            //s4settings.Write("MsgLevelMask ", "-83886081", "GAMESETTINGS");

            DownlaodPanel.Visibility = Visibility.Hidden;
        }
        async static void Deinstaller()
        {
            if (App.S4HE_AppPath != null)
            {
                Properties.Settings.Default.EditionInstalled = "HE";
                await Worker.ZipInstallerAsync(@"Artifacts\Edition_HE_Deinstallieren.zip");
            }
            if (App.S4GE_AppPath != null)
            {
                Properties.Settings.Default.EditionInstalled = "GE";
                await Worker.ZipInstallerAsync(@"Artifacts\Edition_GE_Deinstallieren.zip");
            }

            Environment.Exit(0);
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
                FileName = "TheSettlersIVEnhancedEditionSetup.exe"
                //,
                //Arguments = @"/silent"
            };
            Process.Start(startInfo);
            Environment.Exit(0);
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
        public async void Play()
        {
            FrameContent.Navigate(AppStart);
            Log.LogWriter(LogName, "Navigate AppStart");
            await InstallerAsync();
            Checksumme();
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
            switch (Properties.Settings.Default.EditorFix)
            {
                case ("1"):
                    {
                        await Worker.ZipInstallerAsync(@"Artifacts\Editor_withFix.zip");
                        break;
                    }
                default:
                case ("0"):
                    {
                        await Worker.ZipInstallerAsync(@"Artifacts\Editor_withoutFix.zip");
                        break;
                    }
            }
            if (File.Exists(App.S4HE_AppPath + @"\Editor\" + @"RunEditorEN_v2.bat") &&
                File.Exists(App.S4HE_AppPath + @"\Editor\" + @"RunEditorDE_v2.bat") &&
                File.Exists(App.S4HE_AppPath + @"\Editor\" + @"InstallEditor_En.bat") &&
                File.Exists(App.S4HE_AppPath + @"\Editor\" + @"InstallEditor_De.bat") &&
                Properties.Settings.Default.EditorInstalled == "1")
            {
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = App.S4HE_AppPath + @"\Editor\",
                    FileName = App.S4HE_AppPath + @"\Editor\" + @"S4Editor.exe",
                    CreateNoWindow = true
                };
                Process.Start(startInfo);
                Log.LogWriter(LogName, "Editor Start");
            }
            else
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
