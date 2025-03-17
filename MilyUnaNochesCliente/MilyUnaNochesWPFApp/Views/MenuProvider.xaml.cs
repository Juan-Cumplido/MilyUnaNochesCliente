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
    /// Lógica de interacción para MenuProvider.xaml
    /// </summary>
    public partial class MenuProvider : Page {
        public MenuProvider() {
            InitializeComponent();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e) {
            PlaceholderText.Visibility = string.IsNullOrWhiteSpace(SearchBar.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ProviderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }

        private void Ventas_Click(object sender, RoutedEventArgs e) {

        }

        private void Usuarios_Click(object sender, RoutedEventArgs e) {

        }

        private void Productos_Click(object sender, RoutedEventArgs e) {

        }

        private void Proveedores_Click(object sender, RoutedEventArgs e) {

        }

        private void Reportes_Click(object sender, RoutedEventArgs e) {

        }

        private void Eliminar_Click(object sender, RoutedEventArgs e) {

        }

        private void Archivar_Click(object sender, RoutedEventArgs e) {

        }

        private void Editar_Click(object sender, RoutedEventArgs e) {

        }

        private void ConsultarProveedores_Click(object sender, RoutedEventArgs e) {

        }

        private void RegistrarProveedor_Click(object sender, RoutedEventArgs e) {

        }
    }
}
