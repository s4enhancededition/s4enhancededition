using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace S4EE.Windows
{
    /// <summary>
    /// Interaktionslogik für Info.xaml
    /// </summary>
    public partial class Info : Window
    {
        public Info()
        {
            InitializeComponent();
            Infogrid.Source = new BitmapImage(new Uri(@"/Resources/App_Info_Hotkeys_" + Properties.Settings.Default.Language + @".png", UriKind.RelativeOrAbsolute));
        }
    }
}
