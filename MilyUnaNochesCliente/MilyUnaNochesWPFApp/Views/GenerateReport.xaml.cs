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
            grid_Categoria.Visibility = Visibility.Collapsed;
            grid_InventarioGanancias.Visibility = Visibility.Collapsed;
            grid_Producto.Visibility = Visibility.Collapsed;
        }

        private void ShowGrid(Grid gridSeleccionado) {
            HideAllGrids();
            gridSeleccionado.Visibility = Visibility.Visible;
        }

        private void BtnClick_Categoria(object sender, RoutedEventArgs e) {
            tipoReporte = "Categoria";
            ShowGrid(grid_Categoria);
            AssignEventsCategory();
        }

        private void BtnClick_Producto(object sender, RoutedEventArgs e) {
            tipoReporte = "Producto";
            ShowGrid(grid_Producto);
            AssignEvents();
        }

        private void BtnClick_Inventario(object sender, RoutedEventArgs e) {
            tipoReporte = "Inventario";
            ShowGrid(grid_InventarioGanancias);
            AssignEventsInventoryProfit();
        }

        private void BtnClick_Ganancias(object sender, RoutedEventArgs e) {
            tipoReporte = "Ganancias";
            ShowGrid(grid_InventarioGanancias);
            AssignEventsInventoryProfit();
        }

        private void BtnClick_Descargar(object sender, RoutedEventArgs e) {
            // Lógica para descargar reporte
        }

        // Lógica para la creación de reporte producto
        private void AssignEvents() {
            txtb_CodigoProducto.TextChanged += TxtCodigoProducto_TextChanged;
            lbl_DiarioProducto.MouseDown += (s, e) => SelectPeriod(lbl_DiarioProducto);
            lbl_SemanalProducto.MouseDown += (s, e) => SelectPeriod(lbl_SemanalProducto);
            lbl_MensualProducto.MouseDown += (s, e) => SelectPeriod(lbl_MensualProducto);
            lbl_AnualProducto.MouseDown += (s, e) => SelectPeriod(lbl_AnualProducto);

            lbl_DiarioProducto.Opacity = 0.5;
            lbl_SemanalProducto.Opacity = 0.5;
            lbl_MensualProducto.Opacity = 0.5;
            lbl_AnualProducto.Opacity = 0.5;
            lbl_GenerarProducto.Opacity = 0.5;

            lbl_DiarioProducto.IsEnabled = false;
            lbl_SemanalProducto.IsEnabled = false;
            lbl_MensualProducto.IsEnabled = false;
            lbl_AnualProducto.IsEnabled = false;
            lbl_GenerarProducto.IsEnabled = false;
        }

        private void TxtCodigoProducto_TextChanged(object sender, TextChangedEventArgs e) {
            bool hayCodigo = !string.IsNullOrWhiteSpace(txtb_CodigoProducto.Text);
            lbl_DiarioProducto.IsEnabled = hayCodigo;
            lbl_SemanalProducto.IsEnabled = hayCodigo;
            lbl_MensualProducto.IsEnabled = hayCodigo;
            lbl_AnualProducto.IsEnabled = hayCodigo;
            lbl_GenerarProducto.IsEnabled = hayCodigo;

            if (!hayCodigo) {
                lbl_GenerarProducto.IsEnabled = false;
                RestoreOpacityButtons();
            }
        }

        private void SelectPeriod(Label botonSeleccionado) {
            lbl_DiarioProducto.Opacity = 0.5;
            lbl_SemanalProducto.Opacity = 0.5;
            lbl_MensualProducto.Opacity = 0.5;
            lbl_AnualProducto.Opacity = 0.5;
            botonSeleccionado.Opacity = 1.0;
            lbl_GenerarProducto.IsEnabled = true;
            lbl_GenerarProducto.Opacity = 1.0;
        }

        private void RestoreOpacityButtons() {
            lbl_DiarioProducto.Opacity = 1.0;
            lbl_SemanalProducto.Opacity = 1.0;
            lbl_MensualProducto.Opacity = 1.0;
            lbl_AnualProducto.Opacity = 1.0;
            lbl_GenerarProducto.Opacity = 1.0;
        }

        private void BtnClickGenerarProducto(object sender, RoutedEventArgs e) {
            // Lógica para generar reporte de producto
        }

        private void BtnClickCancelarProducto(object sender, RoutedEventArgs e) {
            ResetForm();
        }

        // Lógica para la creación de reporte categoría
        private void AssignEventsCategory() {
            txtb_CategoriaProducto.TextChanged += TxtCategoriaDeProducto_TextChanged;
            lbl_DiarioCategoria.MouseDown += (s, e) => SelectPeriodCategory(lbl_DiarioCategoria);
            lbl_SemanalCategoria.MouseDown += (s, e) => SelectPeriodCategory(lbl_SemanalCategoria);
            lbl_MensualCategoria.MouseDown += (s, e) => SelectPeriodCategory(lbl_MensualCategoria);
            lbl_AnualCategoria.MouseDown += (s, e) => SelectPeriodCategory(lbl_AnualCategoria);

            lbl_DiarioCategoria.Opacity = 0.5;
            lbl_SemanalCategoria.Opacity = 0.5;
            lbl_MensualCategoria.Opacity = 0.5;
            lbl_AnualCategoria.Opacity = 0.5;
            lbl_GenerarCategoria.Opacity = 0.5;

            lbl_DiarioCategoria.IsEnabled = false;
            lbl_SemanalCategoria.IsEnabled = false;
            lbl_MensualCategoria.IsEnabled = false;
            lbl_AnualCategoria.IsEnabled = false;
            lbl_GenerarCategoria.IsEnabled = false;
        }

        private void TxtCategoriaDeProducto_TextChanged(object sender, TextChangedEventArgs e) {
            bool hayCategoria = !string.IsNullOrWhiteSpace(txtb_CategoriaProducto.Text);
            lbl_DiarioCategoria.IsEnabled = hayCategoria;
            lbl_SemanalCategoria.IsEnabled = hayCategoria;
            lbl_MensualCategoria.IsEnabled = hayCategoria;
            lbl_AnualCategoria.IsEnabled = hayCategoria;
            lbl_GenerarCategoria.IsEnabled = hayCategoria;

            if (!hayCategoria) {
                lbl_GenerarCategoria.IsEnabled = false;
                RestoreOpacityButtonsCategory();
            }
        }

        private void SelectPeriodCategory(Label botonSeleccionado) {
            lbl_DiarioCategoria.Opacity = 0.5;
            lbl_SemanalCategoria.Opacity = 0.5;
            lbl_MensualCategoria.Opacity = 0.5;
            lbl_AnualCategoria.Opacity = 0.5;
            botonSeleccionado.Opacity = 1.0;
            lbl_GenerarCategoria.IsEnabled = true;
            lbl_GenerarCategoria.Opacity = 1.0;
        }

        private void RestoreOpacityButtonsCategory() {
            lbl_DiarioCategoria.Opacity = 1.0;
            lbl_SemanalCategoria.Opacity = 1.0;
            lbl_MensualCategoria.Opacity = 1.0;
            lbl_AnualCategoria.Opacity = 1.0;
            lbl_GenerarCategoria.Opacity = 1.0;
        }

        private void BtnClickGenerarCategoria(object sender, RoutedEventArgs e) {
            // Lógica para generar reporte de categoría
        }

        private void BtnClickCancelarCategoria(object sender, RoutedEventArgs e) {
            ResetForm();
        }

        // Lógica para la creación de reporte ganancias o inventario
        private void AssignEventsInventoryProfit() {
            lbl_DiarioInventarioGanancias.MouseDown += (s, e) => SelectPeriodInventoryProfit(lbl_DiarioInventarioGanancias);
            lbl_SemanalInventarioGanancias.MouseDown += (s, e) => SelectPeriodInventoryProfit(lbl_SemanalInventarioGanancias);
            lbl_MensualInventarioGanancias.MouseDown += (s, e) => SelectPeriodInventoryProfit(lbl_MensualInventarioGanancias);
            lbl_AnualInventarioGanancias.MouseDown += (s, e) => SelectPeriodInventoryProfit(lbl_AnualInventarioGanancias);

            lbl_DiarioInventarioGanancias.Opacity = 0.5;
            lbl_SemanalInventarioGanancias.Opacity = 0.5;
            lbl_MensualInventarioGanancias.Opacity = 0.5;
            lbl_AnualInventarioGanancias.Opacity = 0.5;
            lbl_GenerarInventarioGanancias.Opacity = 0.5;
            lbl_GenerarInventarioGanancias.IsEnabled = false;
        }

        private void SelectPeriodInventoryProfit(Label botonSeleccionado) {
            lbl_DiarioInventarioGanancias.Opacity = 0.5;
            lbl_SemanalInventarioGanancias.Opacity = 0.5;
            lbl_MensualInventarioGanancias.Opacity = 0.5;
            lbl_AnualInventarioGanancias.Opacity = 0.5;
            botonSeleccionado.Opacity = 1.0;
            lbl_GenerarInventarioGanancias.IsEnabled = true;
            lbl_GenerarInventarioGanancias.Opacity = 1.0;
        }

        private void BtnClickCancelarInventarioGanancias(object sender, RoutedEventArgs e) {
            ResetForm();
        }

        private void BtnClickGenerarInventarioGanancias(object sender, RoutedEventArgs e) {
            GenerateReportInventoryProfit();
        }

        private void RestoreOpacityButtonsInventoryGain() {
            lbl_DiarioInventarioGanancias.Opacity = 1.0;
            lbl_SemanalInventarioGanancias.Opacity = 1.0;
            lbl_MensualInventarioGanancias.Opacity = 1.0;
            lbl_AnualInventarioGanancias.Opacity = 1.0;
            lbl_GenerarInventarioGanancias.Opacity = 1.0;
        }

        private void GenerateReportInventoryProfit() {
            if (tipoReporte == "Inventario") {
                // Lógica de inventario
            } else if (tipoReporte == "Ganancias") {
                // Lógica de ganancias
            }
        }

        private void Dg_Reports_SelectionChanged(object sender, RoutedEventArgs e) {
            // Lógica para cambio de selección en el DataGrid
        }

        private void ResetForm() {
            // Limpiar los campos de texto
            txtb_CodigoProducto.Text = string.Empty;
            txtb_CategoriaProducto.Text = string.Empty;

            // Restaurar opacidad y deshabilitar botones de periodo
            RestoreOpacityButtons();
            RestoreOpacityButtonsCategory();
            RestoreOpacityButtonsInventoryGain();

            // Deshabilitar botones de generación
            lbl_GenerarProducto.IsEnabled = false;
            lbl_GenerarCategoria.IsEnabled = false;
            lbl_GenerarInventarioGanancias.IsEnabled = false;

            // Ocultar grids
            HideAllGrids();
        }
    }
}