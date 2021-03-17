using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace S4EE
{
    class Worker
    {
        private static readonly string LogName = "Worker.cs";
        public static async Task ZipInstallerAsync(string zipFilePath)
        {
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

            using ZipArchive archive = await Task.Run(() => ZipFile.OpenRead(zipFilePath));
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
                    if (entry.FullName.EndsWith(@"Deinstallieren.txt", StringComparison.OrdinalIgnoreCase))
                    {
                        string line;
                        using StreamReader file = new(entry.Open());
                        while ((line = file.ReadLine()) != null)
                        {
                            if (line.EndsWith(@"\"))
                            {
                                if (Directory.Exists(InstallPath + line[..^1]))
                                {
                                    Directory.Delete(InstallPath + line[..^1], true);
                                }
                            }
                            else
                            {
                                if (File.Exists(InstallPath + line))
                                {
                                    File.Delete(InstallPath + line);
                                }
                            }
                            Log.LogWriter(LogName, "Delete File" + InstallPath + line);
                        }
                    }
                    else if (entry.FullName.EndsWith(@"ddraw.dll", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!File.Exists(InstallPath + @"\exe\ddraw.dll"))
                        {
                            if (!entry.FullName.EndsWith(@"/", StringComparison.OrdinalIgnoreCase))
                            {
                                await Task.Run(() => entry.ExtractToFile(completeFileName, true));
                            }
                        }
                    }
                    else
                    {
                        if (!entry.FullName.EndsWith(@"/", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        entry.ExtractToFile(completeFileName, true);
                                    }
                                    catch
                                    {
                                        MessageBox.Show(Properties.Resources.MSB_Error_Rechte_Text, Properties.Resources.MSB_Error_Rechte, MessageBoxButton.OK, MessageBoxImage.Error);
                                        Environment.Exit(0);
                                    }
                                });
                            }
                            catch
                            {
                                MessageBox.Show(Properties.Resources.MSB_Error_Rechte_Text, Properties.Resources.MSB_Error_Rechte, MessageBoxButton.OK, MessageBoxImage.Error);
                                Environment.Exit(0);

                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Log.LogWriter(LogName, "Fehler " + completeFileName);
                }
            }
        }
    }
}
