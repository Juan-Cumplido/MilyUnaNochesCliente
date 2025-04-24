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
    /// Lógica de interacción para ManagerMenu.xaml
    /// </summary>
    public partial class ManagerMenu : Page {
        public ManagerMenu() {
            InitializeComponent();
        }

        private void Sales_Click(object sender, RoutedEventArgs e) {

        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }
        private void ImgLogo1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            fra_mainFrame.Source = new System.Uri("ManagerMenuContent.xaml", System.UriKind.Relative);
        }
        private void ImgLogo2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            fra_mainFrame.Source = new System.Uri("ManagerMenuContent.xaml", System.UriKind.Relative);
        }
        private void Users_Click(object sender, RoutedEventArgs e) {
            fra_mainFrame.Source = new System.Uri("ConsultClient.xaml", System.UriKind.Relative);
        }

        private void Products_Click(object sender, RoutedEventArgs e) {
            fra_mainFrame.Source = new System.Uri("ConsultProductsView.xaml", System.UriKind.Relative);
        }

        private void Suppliers_Click(object sender, RoutedEventArgs e) {
            fra_mainFrame.Source = new System.Uri("MenuSupplier.xaml", System.UriKind.Relative);
        }

        private void Reports_Click(object sender, RoutedEventArgs e) {
            fra_mainFrame.Source = new System.Uri("GenerateReport.xaml", System.UriKind.Relative);
        }

        private void Purchases_Click(object sender, RoutedEventArgs e) {
            fra_mainFrame.Source = new System.Uri("PurchasesMenu.xaml", System.UriKind.Relative);
        }
    }
}
