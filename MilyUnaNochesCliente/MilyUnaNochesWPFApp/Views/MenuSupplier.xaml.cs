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

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para MenuProvider2.xaml
    /// </summary>
    public partial class MenuProvider : Page {
        public MenuProvider() {
            InitializeComponent();
        }

        private void ConsultarProveedores_Click(object sender, RoutedEventArgs e) {
            mainFrame.Source = new System.Uri("FindProvider.xaml", System.UriKind.Relative);
        }

        private void RegistrarProveedor_Click(object sender, RoutedEventArgs e) {
            mainFrame.Source = new System.Uri("RegisterProvider.xaml", System.UriKind.Relative);
        }
    }
}
