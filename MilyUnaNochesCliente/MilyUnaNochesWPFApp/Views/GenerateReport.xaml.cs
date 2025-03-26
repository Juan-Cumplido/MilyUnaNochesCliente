using System.Windows;
using System.Windows.Controls;

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para GenerateReport.xaml
    /// </summary>
    public partial class GenerateReport : Page {

        private string tipoReporte = "";
        public GenerateReport() {
            InitializeComponent();
            ConfigureInterfaz();
        }

        private void ConfigureInterfaz() {
            HideAllGrids();
        }

        private void HideAllGrids() {
            gridCategoria.Visibility = Visibility.Collapsed;
            gridInventarioGanancias.Visibility = Visibility.Collapsed;
            gridProducto.Visibility = Visibility.Collapsed;
        }

        private void ShowGrid(Grid gridSeleccionado) {
            HideAllGrids();
            gridSeleccionado.Visibility = Visibility.Visible;
        }

        private void BtnClick_Categoria(object sender, RoutedEventArgs e) {
            tipoReporte = "Categoria";
            ShowGrid(gridCategoria);
            AsignarEventosCategoria();
        }

        private void BtnClick_Producto(object sender, RoutedEventArgs e) {
            tipoReporte = "Producto";
            ShowGrid(gridProducto);
            AsignarEventos();
        }

        private void btnClick_Inventario(object sender, RoutedEventArgs e) {
            tipoReporte = "Inventario";
            ShowGrid(gridInventarioGanancias);
            AsignarEventosIventarioGanancias();
        }
        private void btnClick_Ganancias(object sender, RoutedEventArgs e) {
            tipoReporte = "Ganancias";
            ShowGrid(gridInventarioGanancias);
            AsignarEventosIventarioGanancias();
        }
        private void btnClick_Descargar(object sender, RoutedEventArgs e) {

        }


        //Logica para la creacion de reporte producto
        private void AsignarEventos() {
            txtCodigoProducto.TextChanged += TxtCodigoProducto_TextChanged;
            btnDiarioProducto.MouseDown += (s, e) => SeleccionarPeriodo(btnDiarioProducto);
            btnSemanalProducto.MouseDown += (s, e) => SeleccionarPeriodo(btnSemanalProducto);
            btnMensualProducto.MouseDown += (s, e) => SeleccionarPeriodo(btnMensualProducto);
            btnAnualProducto.MouseDown += (s, e) => SeleccionarPeriodo(btnAnualProducto);
            btnDiarioProducto.Opacity = 0.5;
            btnSemanalProducto.Opacity = 0.5;
            btnMensualProducto.Opacity = 0.5;
            btnAnualProducto.Opacity = 0.5;
            btnGenerarProducto.Opacity = 0.5;
            btnDiarioProducto.IsEnabled = false;
            btnSemanalProducto.IsEnabled = false;
            btnMensualProducto.IsEnabled = false;
            btnAnualProducto.IsEnabled = false;
            btnGenerarProducto.IsEnabled = false;

        }

        private void TxtCodigoProducto_TextChanged(object sender, TextChangedEventArgs e) {
            bool hayCodigo = !string.IsNullOrWhiteSpace(txtCodigoProducto.Text);
            btnDiarioProducto.IsEnabled = hayCodigo;
            btnSemanalProducto.IsEnabled = hayCodigo;
            btnMensualProducto.IsEnabled = hayCodigo;
            btnAnualProducto.IsEnabled = hayCodigo;
            btnGenerarProducto.IsEnabled = hayCodigo;

            if (!hayCodigo) {
                btnGenerarProducto.IsEnabled = false;
                RestaurarOpacidadBotones();
            }
        }

        private void SeleccionarPeriodo(Label botonSeleccionado) {
            btnDiarioProducto.Opacity = 0.5;
            btnSemanalProducto.Opacity = 0.5;
            btnMensualProducto.Opacity = 0.5;
            btnAnualProducto.Opacity = 0.5;
            botonSeleccionado.Opacity = 1.0;
            btnGenerarProducto.IsEnabled = true;
            btnGenerarProducto.Opacity = 1.0;

        }

        private void RestaurarOpacidadBotones() {
            btnDiarioProducto.Opacity = 1.0;
            btnSemanalProducto.Opacity = 1.0;
            btnMensualProducto.Opacity = 1.0;
            btnAnualProducto.Opacity = 1.0;
            btnGenerarProducto.Opacity = 1.0;
        }

        private void btnClickGenerarProducto(object sender, RoutedEventArgs e) {

        }

        private void btnClickCancelarProducto(object sender, RoutedEventArgs e) {
            RestablecerFormulario();
        }

        //Logica para la creacion de reporte categoria
        private void AsignarEventosCategoria() {
            txtCategoriaDeProducto.TextChanged += TxtCategoriaDeProducto_TextChanged;
            btnDiarioCategoria.MouseDown += (s, e) => SeleccionarPeriodoCategoria(btnDiarioCategoria);
            btnSemanalCategoria.MouseDown += (s, e) => SeleccionarPeriodoCategoria(btnSemanalCategoria);
            btnMensualCategoria.MouseDown += (s, e) => SeleccionarPeriodoCategoria(btnMensualCategoria);
            btnAnualCategoria.MouseDown += (s, e) => SeleccionarPeriodoCategoria(btnAnualCategoria);
            btnDiarioCategoria.Opacity = 0.5;
            btnSemanalCategoria.Opacity = 0.5;
            btnMensualCategoria.Opacity = 0.5;
            btnAnualCategoria.Opacity = 0.5;
            btnGenerarCategoria.Opacity = 0.5;
            btnDiarioCategoria.IsEnabled = false;
            btnSemanalCategoria.IsEnabled = false;
            btnMensualCategoria.IsEnabled = false;
            btnAnualCategoria.IsEnabled = false;
            btnGenerarCategoria.IsEnabled = false;
        }

        private void TxtCategoriaDeProducto_TextChanged(object sender, TextChangedEventArgs e) {
            bool hayCategoria = !string.IsNullOrWhiteSpace(txtCategoriaDeProducto.Text);
            btnDiarioCategoria.IsEnabled = hayCategoria;
            btnSemanalCategoria.IsEnabled = hayCategoria;
            btnMensualCategoria.IsEnabled = hayCategoria;
            btnAnualCategoria.IsEnabled = hayCategoria;
            btnGenerarCategoria.IsEnabled = hayCategoria;

            if (!hayCategoria) {
                btnGenerarCategoria.IsEnabled = false;
                RestaurarOpacidadBotonesCategoria();
            }
        }

        private void SeleccionarPeriodoCategoria(Label botonSeleccionado) {
            btnDiarioCategoria.Opacity = 0.5;
            btnSemanalCategoria.Opacity = 0.5;
            btnMensualCategoria.Opacity = 0.5;
            btnAnualCategoria.Opacity = 0.5;
            botonSeleccionado.Opacity = 1.0;
            btnGenerarCategoria.IsEnabled = true;
            btnGenerarCategoria.Opacity = 1.0;

        }

        private void RestaurarOpacidadBotonesCategoria() {
            btnDiarioCategoria.Opacity = 1.0;
            btnSemanalCategoria.Opacity = 1.0;
            btnMensualCategoria.Opacity = 1.0;
            btnAnualCategoria.Opacity = 1.0;
            btnGenerarCategoria.Opacity = 1.0;
        }

        private void btnClickGenerarCategoria(object sender, RoutedEventArgs e) {

        }

        private void btnClickCancelarCategoria(object sender, RoutedEventArgs e) {
            RestablecerFormulario();
        }

        //Logica para la creacion de reporte ganancias o inventario
        private void AsignarEventosIventarioGanancias() {
            btnDiarioInventarioGanancias.MouseDown += (s, e) => SeleccionarPeriodoInventarioGanancias(btnDiarioInventarioGanancias);
            btnSemanalInventarioGanancias.MouseDown += (s, e) => SeleccionarPeriodoInventarioGanancias(btnSemanalInventarioGanancias);
            btnMensualInventarioGanancias.MouseDown += (s, e) => SeleccionarPeriodoInventarioGanancias(btnMensualInventarioGanancias);
            btnAnualInventarioGanancias.MouseDown += (s, e) => SeleccionarPeriodoInventarioGanancias(btnAnualInventarioGanancias);
            btnDiarioInventarioGanancias.Opacity = 0.5;
            btnSemanalInventarioGanancias.Opacity = 0.5;
            btnMensualInventarioGanancias.Opacity = 0.5;
            btnAnualInventarioGanancias.Opacity = 0.5;
            btnGenerarInventarioGanancias.Opacity = 0.5;
            btnGenerarInventarioGanancias.IsEnabled = false;
        }

        private void SeleccionarPeriodoInventarioGanancias(Label botonSeleccionado) {
            btnDiarioInventarioGanancias.Opacity = 0.5;
            btnSemanalInventarioGanancias.Opacity = 0.5;
            btnMensualInventarioGanancias.Opacity = 0.5;
            btnAnualInventarioGanancias.Opacity = 0.5;
            botonSeleccionado.Opacity = 1.0;
            btnGenerarInventarioGanancias.IsEnabled = true;
            btnGenerarInventarioGanancias.Opacity = 1.0;

        }
        private void btnClickCancelarInventarioGanancias(object sender, RoutedEventArgs e) {
            RestablecerFormulario();
        }

        private void btnClickGenerarInventarioGanancias(object sender, RoutedEventArgs e) {
            GenerarReporteInventarioGanancias();
        }

        private void RestaurarOpacidadBotonesInventarioGanancias() {
            btnDiarioInventarioGanancias.Opacity = 1.0;
            btnSemanalInventarioGanancias.Opacity = 1.0;
            btnMensualInventarioGanancias.Opacity = 1.0;
            btnAnualInventarioGanancias.Opacity = 1.0;
            btnGenerarInventarioGanancias.Opacity = 1.0;
        }

        private void GenerarReporteInventarioGanancias() {

            if(tipoReporte == "Inventario") {
                //Logica de inventario
            }
            else if(tipoReporte== "Ganancias") {
                //Logica de ganancias
            }
        }

        private void ReportsDataGrid_SelectionChanged(object sender, RoutedEventArgs e) {

        }


        private void RestablecerFormulario() {
            // Limpiar los campos de texto
            txtCodigoProducto.Text = string.Empty;
            txtCategoriaDeProducto.Text = string.Empty;

            // Restaurar opacidad y deshabilitar botones de periodo
            RestaurarOpacidadBotones();
            RestaurarOpacidadBotonesCategoria();
            RestaurarOpacidadBotonesInventarioGanancias();

            // Deshabilitar botones de generación
            btnGenerarProducto.IsEnabled = false;
            btnGenerarCategoria.IsEnabled = false;
            btnGenerarInventarioGanancias.IsEnabled = false;

            // Ocultar grids
            HideAllGrids();
        }


    }
}
