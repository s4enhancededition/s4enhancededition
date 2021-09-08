using System.IO;
using System.Windows;

namespace S4EE.Windows
{
    /// <summary>
    /// Interaktionslogik für Info.xaml
    /// </summary>
    public partial class Maps2021 : Window
    {
        private static readonly string LogName = "Worker.cs";
        private string InstallPath = "";
        public Maps2021()
        {
            InitializeComponent();
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
        }
        private void Button_MAPSADD_Click(object sender, RoutedEventArgs e)

        {
            string zipFilePath = @"Artifacts\MapPack_TOURNAMENT_WC2021_MAP" + ID.Text + @".zip";
            try
            {
                if (Ionic.Zip.ZipFile.CheckZipPassword(zipFilePath, PW.Text))
                {
                    using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(zipFilePath))
                    {
                        zip.Password = PW.Text;
                        zip.ExtractExistingFile = Ionic.Zip.ExtractExistingFileAction.OverwriteSilently;
                        zip.ExtractAll(InstallPath);
                        MessageBox.Show(Properties.Resources.MSB_Info_Text, Properties.Resources.MSB_Info, MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show(Properties.Resources.MSB_Error_Text, Properties.Resources.MSB_Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void Button_REMOVEMAP_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(InstallPath + @"Map\User\WC2021-MAP" + ID.Text + ".map"))
            {
                File.Delete(InstallPath + @"Map\User\WC2021-MAP" + ID.Text + ".map");
                MessageBox.Show(Properties.Resources.MSB_Info_Text, Properties.Resources.MSB_Info, MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
       
    }
}
