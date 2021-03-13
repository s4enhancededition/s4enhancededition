using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace S4EE
{
    class Worker
    {
        private static readonly string LogName = "Worker.cs";

        public static async Task ZipInstallerAsync(string zipFilePath)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).DownlaodPanel.Visibility = Visibility.Visible;
                    (window as AppWindow).DownlaodLabel.Content = "Installiere";
                    Thread.Sleep(100);
                }
            }

            string InstallPath = "";
            switch (Properties.Settings.Default.EditionInstalled)
            {
                case ("HE"):
                case ("EHE"):
                    {
                        Log.LogWriter(LogName, "HE Installationspfad");
                        InstallPath = App.S4HE_AppPath;
                        break;
                    }
                case ("GE"):
                case ("EGE"):
                    {
                        Log.LogWriter(LogName, "GE Installationspfad");
                        InstallPath = App.S4GE_AppPath;
                        break;
                    }
            }

            if (InstallPath == "")
            {
                Log.LogWriter(LogName, "ERROR: Kein Installationspfad gefunden");
                return;
            }

            using (ZipArchive archive = await Task.Run(() => ZipFile.OpenRead(zipFilePath)))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string completeFileName = Path.Combine(InstallPath, entry.FullName);
                    string directory = Path.GetDirectoryName(completeFileName);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    try
                    {
                        if (entry.FullName.EndsWith(@"Uninstall.txt", StringComparison.OrdinalIgnoreCase))
                        {
                            string line;
                            using StreamReader file = new(entry.Open());
                            while ((line = file.ReadLine()) != null)
                            {
                                System.Console.WriteLine(line);
                                if(line.EndsWith(@"\"))
                                {
                                    if (Directory.Exists(InstallPath+ line[..^1]))
                                    {
                                        Directory.Delete(InstallPath +line[..^1], true);
                                    }
                                }
                                Log.LogWriter(LogName, "Delete File" + InstallPath + line);

                            }
                            await Task.Run(() => entry.ExtractToFile(completeFileName, true));
                        }
                        if (!entry.FullName.EndsWith(@"/", StringComparison.OrdinalIgnoreCase))
                        {
                            await Task.Run(() => entry.ExtractToFile(completeFileName, true));
                        }
                    }
                    catch (Exception)
                    {
                        Log.LogWriter(LogName, "Fehler beim Installieren der Datei " + completeFileName);
                    }
                }
            }


            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(AppWindow))
                {
                    (window as AppWindow).DownlaodLabel.Content = "Abgeschlossen";
                    Thread.Sleep(100);
                    (window as AppWindow).DownlaodPanel.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
