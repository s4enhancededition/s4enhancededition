using System;
using System.Windows;


namespace S4EE.Windows
{
    /// <summary>
    /// Interaktionslogik für Info.xaml
    /// </summary>
    public partial class RND : Window
    {
        public RND()
        {
            InitializeComponent();
        }
        private void Button_RND_Click(object sender, RoutedEventArgs e)
        {
            roll();
        }
        private void roll()
        {
            if (App_RND_R.IsChecked.Value || App_RND_W.IsChecked.Value || App_RND_M.IsChecked.Value || App_RND_T.IsChecked.Value)
            {
                var rand = new Random();
                int caseSwitch = rand.Next(1, 5);
                switch (caseSwitch)
                {
                    case 1:
                        if (App_RND_R.IsChecked.Value)
                        {
                            App_RND_Result.Content = Properties.Resources.App_RND_R;
                        }
                        else
                        {
                            roll();
                        }
                        break;
                    case 2:
                        if (App_RND_W.IsChecked.Value)
                        {
                            App_RND_Result.Content = Properties.Resources.App_RND_W;
                        }
                        else
                        {
                            roll();
                        }
                        break;
                    case 3:
                        if (App_RND_M.IsChecked.Value)
                        {
                            App_RND_Result.Content = Properties.Resources.App_RND_M;
                        }
                        else
                        {
                            roll();
                        }
                        break;
                    case 4:
                        if (App_RND_T.IsChecked.Value)
                        {
                            App_RND_Result.Content = Properties.Resources.App_RND_T;
                        }
                        else
                        {
                            roll();
                        }
                        break;
                    default:
                        App_RND_Result.Content = Properties.Resources.MSB_Error_Text;
                        break;
                }
            }
            else
            {
                App_RND_Result.Content = Properties.Resources.MSB_Error;
            }
        }
    }
}
