﻿using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace S4EE
{
    class Worker
    {
        private static readonly string LogName = "Worker.cs";
        public static async Task ZipInstallerAsync(string zipFilePath, string mode = "")
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
                            Log.LogWriter(LogName, "Delete File: " + InstallPath + line);
                        }
                    }
                    else if (entry.FullName.EndsWith(@"Rename_Map.txt", StringComparison.OrdinalIgnoreCase))
                    {
                        string line;
                        using StreamReader file = new(entry.Open());
                        if (mode == "Map")
                        {
                            while ((line = file.ReadLine()) != null)
                            {
                                if (File.Exists(InstallPath + line[..^4] + ".ma2"))
                                {
                                    File.Move(InstallPath + line[..^4] + ".ma2", InstallPath + line[..^4] + ".map");
                                }
                                Log.LogWriter(LogName, "Rename Ma2 to Map: " + InstallPath + line);
                            }
                        }
                        else if (mode == "Ma2")
                        {
                            while ((line = file.ReadLine()) != null)
                            {
                                if (File.Exists(InstallPath + line))
                                {
                                    File.Move(InstallPath + line, InstallPath + line[..^4] + ".ma2");
                                }
                                Log.LogWriter(LogName, "Rename Map to Ma2: " + InstallPath + line);
                            }
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
                    else if (entry.FullName.EndsWith(@".map", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!File.Exists(InstallPath + completeFileName))
                        {
                            if (!entry.FullName.EndsWith(@"/", StringComparison.OrdinalIgnoreCase))
                            {
                                await Task.Run(() => entry.ExtractToFile(completeFileName, true));
                            }
                        }
                        else if (!File.Equals(InstallPath + completeFileName, entry))
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
                            await Task.Run(() =>
                            {
                                try
                                {
                                    entry.ExtractToFile(completeFileName, true);
                                }
                                catch
                                {
                                    if (mode != "Textures")
                                    {
                                        MessageBox.Show(Properties.Resources.MSB_Error_Rechte_Text, Properties.Resources.MSB_Error_Rechte, MessageBoxButton.OK, MessageBoxImage.Error);
                                        Environment.Exit(0);
                                    }
                                }
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    Log.LogWriter(LogName, "Fehler " + completeFileName);
                }
            }
        }
        public static Task SaveCleaner(string Folder)
        {
            DateTime dt = DateTime.Now.Add(new TimeSpan(-48, 0, 0));
            try
            {
                foreach (var entry in Directory.GetFiles(Folder))
                {
                    DateTime entrydate = File.GetLastWriteTime(entry);
                    if (dt >= entrydate)
                    {
                        string FolderArchiv = Folder + @"\Archiv-" + entrydate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                        if (!Directory.Exists(FolderArchiv))
                        {
                            Directory.CreateDirectory(FolderArchiv);
                        }
                        if (!File.Exists(FolderArchiv + entry.Replace(Folder, "")))
                        {
                            File.Move(entry, FolderArchiv + entry.Replace(Folder, ""));
                        }
                        else
                        {
                            File.Delete(FolderArchiv + entry.Replace(Folder, ""));
                            File.Move(entry, FolderArchiv + entry.Replace(Folder, ""));
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return Task.CompletedTask;
        }
    }
}
