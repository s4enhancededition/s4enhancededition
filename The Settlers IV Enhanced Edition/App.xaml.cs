using System.Threading;
using System.Windows;

namespace S4EE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private App()
        {
            if (S4EE.Properties.Settings.Default.UpgradeRequired)
            {
                S4EE.Properties.Settings.Default.Upgrade();
                S4EE.Properties.Settings.Default.UpgradeRequired = false;
                S4EE.Properties.Settings.Default.Save();
            }

            switch (S4EE.Properties.Settings.Default.Language)
            {
                case ("de-DE"):
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
                        break;
                    }
                case ("en-US"):
                default:
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        break;
                    }
            }
        }
    }
}