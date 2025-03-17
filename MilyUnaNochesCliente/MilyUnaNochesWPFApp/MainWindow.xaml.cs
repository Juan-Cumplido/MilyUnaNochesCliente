using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MilyUnaNochesWPFApp
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fra_NavigationFrame.Navigate(new Views.LoginView());
        }
        private void NavigationFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var storyb_FadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1.5));
            fra_NavigationFrame.BeginAnimation(Frame.OpacityProperty, storyb_FadeOutAnimation);
        }

        private void NavigationFrame_Navigated(object sender, NavigationEventArgs e) {
            var storyb_FadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2.5));
            fra_NavigationFrame.BeginAnimation(Frame.OpacityProperty, storyb_FadeInAnimation);
        }
    }
}   