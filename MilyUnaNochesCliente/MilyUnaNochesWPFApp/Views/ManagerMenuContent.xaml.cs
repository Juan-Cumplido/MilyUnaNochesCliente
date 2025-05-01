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
    /// Lógica de interacción para ManagerMenuContent.xaml
    /// </summary>
    public partial class ManagerMenuContent : Page {
        public ManagerMenuContent() {
            InitializeComponent();
        }

        private void FrameSale_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("/Views/MenuSupplier.xaml", UriKind.Relative));
        }

        private void FrameSuppliers_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("Views/MenuSupplier.xaml", UriKind.Relative));
        }

        private void FrameProducts_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("Views/ConsultProductsView.xaml", UriKind.Relative));
        }
    }
}
