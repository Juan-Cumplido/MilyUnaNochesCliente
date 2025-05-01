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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MilyUnaNochesWPFApp.Views
{
    public partial class CustomDialog : Window
    {
        public enum DialogType
        {
            Success,
            Error,
            Warning
        }
        public CustomDialog(string message, DialogType type)
        {
            InitializeComponent();
            MessageText.Text = message;
            SetStyle(type);
        }

        private void SetStyle(DialogType type)
        {
            switch (type)
            {
                case DialogType.Success:
                    MainBorder.Background = new SolidColorBrush(Color.FromRgb(220, 255, 220)); // verde claro
                    MainBorder.BorderBrush = Brushes.Green;
                    IconImage.Source = new BitmapImage(new Uri("/Images/Icons/success.png", UriKind.Relative));
                    break;
                case DialogType.Warning:
                    MainBorder.Background = new SolidColorBrush(Color.FromRgb(255, 245, 204)); // amarillo claro
                    MainBorder.BorderBrush = Brushes.Goldenrod;
                    IconImage.Source = new BitmapImage(new Uri("/Images/Icons/warning.png", UriKind.Relative));
                    break;

                case DialogType.Error:
                    MainBorder.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230)); // rojo claro
                    MainBorder.BorderBrush = Brushes.DarkRed;
                    IconImage.Source = new BitmapImage(new Uri("/Images/Icons/error.png", UriKind.Relative));
                    break;
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
