using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MilyUnaNochesWPFApp.Logic;

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para MenuSale.xaml
    /// </summary>
    public partial class MenuSale : Page {
        public MenuSale() {
            InitializeComponent();
            CargarVentas();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void SalesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }
        

        

        private void CargarVentas() {
            var ventas = new List<Sale>
            {
            new Sale { Cliente = "xxxx xxxx 1234", Monto = "$720.00", Articulos = "Collar antiguo, Gato de calavera" },
            new Sale { Cliente = "xxxx xxxx 1234", Monto = "$100,000.00", Articulos = "Juego de joyería de barro" },
            new Sale { Cliente = "xxxx xxxx 1234", Monto = "$80,000.00", Articulos = "Dije de perlas" }
        };

            SalesDataGrid.ItemsSource = ventas;
        }
    }
}

