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

    public partial class PurchasesMenu : Page {
        public PurchasesMenu() {
            InitializeComponent();
        }

        private void ConsultarCompras_Click(object sender, RoutedEventArgs e) {
            mainFrame.Source = new System.Uri("ConsultPurchase.xaml", System.UriKind.Relative);
        }

        private void RegistrarCompra_Click(object sender, RoutedEventArgs e) {
            mainFrame.Source = new System.Uri("RegisterPurchase.xaml", System.UriKind.Relative);
        }
    }
}
