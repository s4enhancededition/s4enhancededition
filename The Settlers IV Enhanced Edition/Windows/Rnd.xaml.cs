using System;
using System.IO;
using System.Windows;


namespace S4EE.Windows
{
    /// <summary>
    /// Interaktionslogik für Info.xaml
    /// </summary>
    public partial class RND : Window
    {
        //private static readonly string LogName = "Worker.cs";
        int r = 0;
        int w = 0;
        int m = 0;
        int t = 0;

        public RND()
        {
            InitializeComponent();
        }
        private void Button_RND_Click(object sender, RoutedEventArgs e)
        {
            r = 0;
            m = 0;
            w = 0;
            t = 0;
            for (int i = 0; i < 4000000; i++)
            {
                var rand = new Random();
                int caseSwitch = rand.Next(1, 5);

                switch (caseSwitch)
                {
                    case 1:
                        App_RND_Result.Content = "Römer";
                        r++;
                        break;
                    case 2:
                        App_RND_Result.Content = "Wiki";
                        w++;
                        break;
                    case 3:
                        App_RND_Result.Content = "Maya";
                        m++;
                        break;
                    case 4:
                        App_RND_Result.Content = "Trojaner";
                        t++;
                        break;
                    default:
                        App_RND_Result.Content = "Error";
                        break;
                }
            }
            App_RND_Result_Stat.Content = "\n" + "R:    " + r + " W:    " + w + " M:    " + m + " T:    " + t;
        }

    }
}
