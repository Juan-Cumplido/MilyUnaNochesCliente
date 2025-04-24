using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using iText.Kernel.Pdf;
using Microsoft.Win32;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace MilyUnaNochesWPFApp.Views {
    public partial class GenerateReport : Page {
        private string _tipoReporte = "";
        private string _periodoSeleccionado = "";
        private string _parametroBusqueda = "";
        private List<ReportMetadata> _reportesDisponibles = new List<ReportMetadata>();
        private int _currentPage = 0;  // Página actual (base 0)
        private const int _pageSize = 5; // Items por página (ajusta según tu lógica)
        private int _totalItems = 0;
        private int _totalPages = 0;

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
            _tipoReporte = "Categoria";
            ShowGrid(grid_Categoria);
            AssignEventsCategory();
        }

        private void BtnClick_Producto(object sender, RoutedEventArgs e) {
            _tipoReporte = "Producto";
            ShowGrid(grid_Producto);
            AssignEvents();
        }

        private void BtnClick_Inventario(object sender, RoutedEventArgs e) {
            _tipoReporte = "Inventario";
            ShowGrid(grid_InventarioGanancias);
            AssignEventsInventoryProfit();
        }

        private void BtnClick_Ganancias(object sender, RoutedEventArgs e) {
            _tipoReporte = "Ganancias";
            ShowGrid(grid_InventarioGanancias);
            AssignEventsInventoryProfit();
        }

        private async void BtnClick_Descargar(object sender, RoutedEventArgs e) {
            if (dg_Reports.SelectedItem == null) {
                MessageBox.Show("Por favor seleccione un reporte para descargar", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IReportManager proxy = null;
            try {
                // Obtener el reporte seleccionado
                var selectedIndex = dg_Reports.SelectedIndex;
                var reportMetadata = _reportesDisponibles[selectedIndex + (_currentPage * _pageSize)];

                // Crear el proxy
                proxy = new ReportManagerClient();

                // Mostrar diálogo para guardar el archivo
                var saveFileDialog = new SaveFileDialog {
                    Filter = "Archivo PDF (*.pdf)|*.pdf",
                    Title = "Guardar reporte como PDF",
                    FileName = $"Reporte_{_tipoReporte}_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true) {
                    // Obtener los datos completos según el tipo de reporte
                    switch (_tipoReporte.ToLower()) {
                        case "producto":
                            var productReport = await proxy.GetProductReportDataAsync(reportMetadata.ReportId);
                            GenerarPDFProducto(saveFileDialog.FileName, productReport);
                            break;

                        case "categoria":
                            var categoryReport = await proxy.GetCategoryReportDataAsync(reportMetadata.ReportId);
                            GenerarPDFCategoria(saveFileDialog.FileName, categoryReport);
                            break;

                        case "inventario":
                            var inventoryReport = await proxy.GetInventoryReportDataAsync(reportMetadata.ReportId);
                            GenerarPDFInventario(saveFileDialog.FileName, inventoryReport);
                            break;

                        case "ganancias":
                            var profitReport = await proxy.GetProfitReportDataAsync(reportMetadata.ReportId);
                            GenerarPDFGanancias(saveFileDialog.FileName, profitReport);
                            break;
                    }

                    MessageBox.Show("Reporte generado exitosamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } catch (FaultException ex) {
                MessageBox.Show($"Error del servicio: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (TimeoutException) {
                MessageBox.Show("Tiempo de espera agotado al generar el reporte", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (CommunicationException ex) {
                MessageBox.Show($"Error de comunicación: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } finally {
                // Cerrar el proxy adecuadamente
                if (proxy != null && proxy is ICommunicationObject) {
                    try {
                        if (((ICommunicationObject)proxy).State == CommunicationState.Faulted) {
                            ((ICommunicationObject)proxy).Abort();
                        } else {
                            ((ICommunicationObject)proxy).Close();
                        }
                    } catch {
                        ((ICommunicationObject)proxy).Abort();
                    }
                }
            }
        }

        // Métodos para generar PDFs (implementación básica)
        private void GenerarPDFProducto(string filePath, ProductReportData reporte) {
            using (var fs = new FileStream(filePath, FileMode.Create)) {
                using (var doc = new Document(PageSize.A4, 30, 30, 30, 30)) {
                    iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Fuentes personalizadas
                    var tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                    var subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                    // Título
                    doc.Add(new Paragraph($"REPORTE DE PRODUCTO - {reporte.ProductInfo.ProductName}", tituloFont));
                    doc.Add(new Paragraph($"Generado el: {DateTime.Now.ToString("g")}", normalFont));

                    // Información básica
                    doc.Add(new Paragraph("INFORMACIÓN DEL PRODUCTO:", subtituloFont));
                    doc.Add(new Paragraph($"• Código: {reporte.ProductInfo.ProductCode}", normalFont));
                    doc.Add(new Paragraph($"• Categoría: {reporte.ProductInfo.Category}", normalFont));
                    doc.Add(new Paragraph($"• Descripción: {reporte.ProductInfo.Description}", normalFont));
                    doc.Add(new Paragraph($"• Período del reporte: {reporte.StartDate.ToString("d")} a {reporte.EndDate.ToString("d")}", normalFont));

                    // Resumen
                    doc.Add(new Paragraph("RESUMEN DE VENTAS:", subtituloFont));
                    doc.Add(new Paragraph($"• Total vendido: {reporte.Summary.TotalSales:C}", normalFont));
                    doc.Add(new Paragraph($"• Beneficio: {reporte.Summary.TotalProfit:C}", normalFont));
                    doc.Add(new Paragraph($"• Unidades vendidas: {reporte.Summary.TotalItemsSold}", normalFont));
                    doc.Add(new Paragraph($"• Transacciones: {reporte.Summary.TotalTransactions}", normalFont));

                    // Tabla de ventas detalladas
                    doc.Add(new Paragraph("DETALLE DE VENTAS:", subtituloFont));

                    // Ajustamos la tabla a 6 columnas (coincide con los datos que vamos a mostrar)
                    var table = new PdfPTable(6);
                    table.WidthPercentage = 100;

                    // Configuramos los anchos de las columnas (opcional)
                    float[] columnWidths = { 15f, 10f, 12f, 12f, 20f, 20f };
                    table.SetWidths(columnWidths);

                    // Encabezados
                    table.AddCell(new Phrase("Fecha", subtituloFont));
                    table.AddCell(new Phrase("Cantidad", subtituloFont));
                    table.AddCell(new Phrase("P. Unitario", subtituloFont));
                    table.AddCell(new Phrase("Total", subtituloFont));
                    table.AddCell(new Phrase("Vendedor", subtituloFont));
                    table.AddCell(new Phrase("Cliente", subtituloFont));

                    // Datos
                    foreach (var venta in reporte.SalesDetails) {
                        table.AddCell(new Phrase(venta.SaleDate.ToString("d"), normalFont));
                        table.AddCell(new Phrase(venta.QuantitySold.ToString(), normalFont));
                        table.AddCell(new Phrase(venta.UnitPrice.ToString("C"), normalFont));
                        table.AddCell(new Phrase((venta.QuantitySold * venta.UnitPrice).ToString("C"), normalFont));
                        table.AddCell(new Phrase(venta.SoldBy ?? "N/A", normalFont));
                        table.AddCell(new Phrase(venta.CustomerName ?? "Venta general", normalFont));
                    }

                    doc.Add(table);
                    doc.Close();
                }
            }
        }
        private void GenerarPDFCategoria(string filePath, CategoryReportData reporte) {
            using (var fs = new FileStream(filePath, FileMode.Create)) {
                using (var doc = new Document(PageSize.A4.Rotate())) // Horizontal para más espacio
                {
                    iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Encabezado
                    doc.Add(new Paragraph($"REPORTE DE CATEGORÍA: {reporte.CategoryName}",
                          new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                    doc.Add(new Paragraph($"Período: {reporte.StartDate:dd/MM/yyyy} - {reporte.EndDate:dd/MM/yyyy}"));
                    doc.Add(new Paragraph($"Generado el: {DateTime.Now:g}"));

                    // Resumen Estadístico
                    var summaryTable = new PdfPTable(4);
                    summaryTable.WidthPercentage = 100;
                    summaryTable.AddCell(new PdfPCell(new Phrase("RESUMEN ESTADÍSTICO",
                        new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { Colspan = 4, HorizontalAlignment = Element.ALIGN_CENTER });

                    summaryTable.AddCell("Transacciones");
                    summaryTable.AddCell("Unidades Vendidas");
                    summaryTable.AddCell("Ventas Totales");
                    summaryTable.AddCell("Beneficio");

                    summaryTable.AddCell(reporte.Summary.TotalTransactions.ToString());
                    summaryTable.AddCell(reporte.Summary.TotalItemsSold.ToString());
                    summaryTable.AddCell(reporte.Summary.TotalSales.ToString("C"));
                    summaryTable.AddCell(reporte.Summary.TotalProfit.ToString("C"));

                    doc.Add(summaryTable);

                    // Productos Más Vendidos
                    doc.Add(new Paragraph("TOP PRODUCTOS:",
                        new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));

                    var topProductsTable = new PdfPTable(4);
                    topProductsTable.WidthPercentage = 100;
                    topProductsTable.AddCell("Producto");
                    topProductsTable.AddCell("Código");
                    topProductsTable.AddCell("Unidades Vendidas");
                    topProductsTable.AddCell("Beneficio");

                    foreach (var product in reporte.TopProducts) {
                        topProductsTable.AddCell(product.ProductName);
                        topProductsTable.AddCell(product.ProductCode);
                        topProductsTable.AddCell(product.TotalSales.ToString());
                    }

                    doc.Add(topProductsTable);
                    doc.Close();
                }
            }
        }
        private void GenerarPDFInventario(string filePath, InventoryReportData reporte) {
            using (var fs = new FileStream(filePath, FileMode.Create)) {
                using (var doc = new Document(PageSize.A4.Rotate())) // Horizontal para mejor visualización
                {
                    iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Encabezado
                    doc.Add(new Paragraph("REPORTE DE INVENTARIO",
                        new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                    doc.Add(new Paragraph($"Fecha del reporte: {reporte.ReportDate:dd/MM/yyyy}"));

                    // Resumen General
                    var summaryTable = new PdfPTable(4);
                    summaryTable.WidthPercentage = 100;
                    summaryTable.AddCell(new PdfPCell(new Phrase("RESUMEN GENERAL",
                        new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { Colspan = 4, HorizontalAlignment = Element.ALIGN_CENTER });

                    summaryTable.AddCell("Total Productos");
                    summaryTable.AddCell("Fuera de Stock");
                    summaryTable.AddCell("Stock Bajo");
                    summaryTable.AddCell("Valor Total");

                    summaryTable.AddCell(reporte.Summary.TotalProducts.ToString());
                    summaryTable.AddCell(reporte.Summary.OutOfStock.ToString());
                    summaryTable.AddCell(reporte.Summary.LowStock.ToString());
                    summaryTable.AddCell(reporte.Summary.TotalInventoryValue.ToString("C"));

                    doc.Add(summaryTable);

                    // Detalle de Inventario
                    doc.Add(new Paragraph("DETALLE DE PRODUCTOS:",
                        new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));

                    var inventoryTable = new PdfPTable(8);
                    inventoryTable.WidthPercentage = 100;

                    // Encabezados
                    inventoryTable.AddCell("Código");
                    inventoryTable.AddCell("Nombre");
                    inventoryTable.AddCell("Categoría");
                    inventoryTable.AddCell("Stock");
                    inventoryTable.AddCell("P. Compra");
                    inventoryTable.AddCell("P. Venta");
                    inventoryTable.AddCell("Últ. Reabastecimiento");
                    inventoryTable.AddCell("Últ. Venta");

                    // Datos
                    foreach (var item in reporte.Items) {
                        inventoryTable.AddCell(item.ProductCode);
                        inventoryTable.AddCell(item.ProductName);
                        inventoryTable.AddCell(item.Category);
                        inventoryTable.AddCell(item.CurrentStock.ToString());
                        inventoryTable.AddCell(item.PurchasePrice.ToString("C"));
                        inventoryTable.AddCell(item.SalePrice.ToString("C"));
                        inventoryTable.AddCell(item.LastRestockDate.HasValue ? item.LastRestockDate.Value.ToString("d") : "N/A");
                        inventoryTable.AddCell(item.LastSaleDate.HasValue ? item.LastSaleDate.Value.ToString("d") : "N/A");
                    }

                    doc.Add(inventoryTable);
                    doc.Close();
                }
            }
        }
        private void GenerarPDFGanancias(string filePath, ProfitReportData reporte) {
            using (var fs = new FileStream(filePath, FileMode.Create)) {
                using (var doc = new Document()) {
                    iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Encabezado
                    doc.Add(new Paragraph("REPORTE DE GANANCIAS",
                        new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                    doc.Add(new Paragraph($"Período: {reporte.StartDate:dd/MM/yyyy} - {reporte.EndDate:dd/MM/yyyy}"));
                    doc.Add(new Paragraph($"Generado el: {DateTime.Now:g}"));

                    // Resumen Financiero
                    var financeTable = new PdfPTable(2);
                    financeTable.WidthPercentage = 100;
                    financeTable.AddCell(new PdfPCell(new Phrase("RESUMEN FINANCIERO",
                        new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))) { Colspan = 2, HorizontalAlignment = Element.ALIGN_CENTER });

                    AddFinanceRow(financeTable, "Ventas Totales", reporte.Financials.TotalSales);
                    AddFinanceRow(financeTable, "Costos Totales", reporte.Financials.TotalCosts);
                    AddFinanceRow(financeTable, "Ganancia Bruta", reporte.Financials.GrossProfit);
                    AddFinanceRow(financeTable, "Margen de Ganancia", reporte.Financials.ProfitMargin, true);
                    AddFinanceRow(financeTable, "Gastos Operativos", reporte.Financials.OperatingExpenses);
                    AddFinanceRow(financeTable, "Ganancia Neta", reporte.Financials.NetProfit);

                    doc.Add(financeTable);

                    // Desglose por Categoría
                    doc.Add(new Paragraph("GANANCIAS POR CATEGORÍA:",
                        new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));

                    var categoryTable = new PdfPTable(3);
                    categoryTable.WidthPercentage = 100;
                    categoryTable.AddCell("Categoría");
                    categoryTable.AddCell("Ventas");
                    categoryTable.AddCell("Ganancia");

                    foreach (var category in reporte.CategoryBreakdown) {
                        categoryTable.AddCell(category.CategoryName);
                        categoryTable.AddCell(category.SalesPercentage.ToString("C"));
                        categoryTable.AddCell(category.ProfitPercentage.ToString("C"));
                    }

                    doc.Add(categoryTable);

                    // Tendencia Mensual (Gráfico opcional)
                    doc.Add(new Paragraph("TENDENCIA MENSUAL:",
                        new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));

                    var trendTable = new PdfPTable(2);
                    trendTable.WidthPercentage = 50;
                    trendTable.AddCell("Mes");
                    trendTable.AddCell("Ganancia");

                    foreach (var month in reporte.MonthlyTrend) {
                        trendTable.AddCell(month.Profit.ToString("C"));
                    }

                    doc.Add(trendTable);
                    doc.Close();
                }
            }
        }

        // Método auxiliar para filas financieras
        private void AddFinanceRow(PdfPTable table, string label, decimal value, bool isPercentage = false) {
            table.AddCell(new PdfPCell(new Phrase(label,
                new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase(
                isPercentage ? $"{value:P}" : $"{value:C}",
                new Font(Font.FontFamily.HELVETICA, 10))));
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
            // Restablecer opacidad de todos los botones de periodo
            lbl_DiarioProducto.Opacity = 0.5;
            lbl_SemanalProducto.Opacity = 0.5;
            lbl_MensualProducto.Opacity = 0.5;
            lbl_AnualProducto.Opacity = 0.5;

            // Resaltar el botón seleccionado
            botonSeleccionado.Opacity = 1.0;

            // Habilitar botón de generar
            lbl_GenerarProducto.IsEnabled = true;
            lbl_GenerarProducto.Opacity = 1.0;

            // Guardar el periodo seleccionado
            _periodoSeleccionado = botonSeleccionado.Content.ToString().ToLower();
        }

        private void RestoreOpacityButtons() {
            lbl_DiarioProducto.Opacity = 1.0;
            lbl_SemanalProducto.Opacity = 1.0;
            lbl_MensualProducto.Opacity = 1.0;
            lbl_AnualProducto.Opacity = 1.0;
            lbl_GenerarProducto.Opacity = 1.0;
        }

        private async void BtnClickGenerarProducto(object sender, RoutedEventArgs e) {
            try {
                if (string.IsNullOrWhiteSpace(txtb_CodigoProducto.Text)) {
                    MessageBox.Show("Por favor introduzca un codigo de producto");
                }
                _parametroBusqueda = txtb_CodigoProducto.Text;
                await CargarReportesDisponibles();
                HideAllGrids();
            } catch (Exception ex) {
                MessageBox.Show($"Error al cargar reportes: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CargarReportesDisponibles() {
            try {
                IReportManager proxy = new ReportManagerClient();

                // Obtener los datos paginados
                switch (_tipoReporte) {
                    case "Producto":
                        _reportesDisponibles = await proxy.GetAvailableProductReportsAsync(
                            _parametroBusqueda, _periodoSeleccionado, _currentPage * _pageSize, _pageSize);
                        break;
                    case "Categoria":
                        _reportesDisponibles = await proxy.GetAvailableCategoryReportsAsync(
                            _parametroBusqueda, _periodoSeleccionado, _currentPage * _pageSize, _pageSize);
                        break;
                    case "Inventario":
                        _reportesDisponibles = await proxy.GetAvailableInventoryReportsAsync(
                            _periodoSeleccionado, _currentPage * _pageSize, _pageSize);
                        break;
                    case "Ganancias":
                        _reportesDisponibles = await proxy.GetAvailableProfitReportsAsync(
                            _periodoSeleccionado, _currentPage * _pageSize, _pageSize);
                        break;
                }

                // Asignar valores según el tipo de reporte
                dg_Reports.ItemsSource = _reportesDisponibles
                    .Select((r, index) => new {
                        No = (_currentPage * _pageSize) + index + 1,
                        Tipo = ObtenerTipoReporteDisplay(_tipoReporte),
                        Periodo = $"{r.StartDate:dd/MM/yyyy} - {r.EndDate:dd/MM/yyyy}",
                        Categoria = ObtenerCategoriaDisplay(_tipoReporte, r),
                        Descripcion = ObtenerDescripcionDisplay(_tipoReporte, r)
                    })
                    .ToList();

                ActualizarEtiquetaPagina();
                ActualizarEstadoBotonesPaginacion();
            } catch (FaultException ex) {
                MessageBox.Show($"Error del servicio: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (CommunicationException ex) {
                MessageBox.Show($"Error de comunicación: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarEstadoBotonesPaginacion() {
            btnPrimeraPagina.IsEnabled = _currentPage > 0;
            btnPaginaAnterior.IsEnabled = _currentPage > 0;
            btnPaginaSiguiente.IsEnabled = _currentPage < _totalPages - 1;
            btnUltimaPagina.IsEnabled = _currentPage < _totalPages - 1;
        }

        private void BtnClickCancelarProducto(object sender, RoutedEventArgs e) {
            ResetForm();
        }

        private string ObtenerTipoReporteDisplay(string tipoReporte) {
            switch (tipoReporte) {
                case "Producto": return "Reporte de Producto";
                case "Categoria": return "Reporte de Categoría";
                case "Inventario": return "Reporte de Inventario";
                case "Ganancias": return "Reporte de Ganancias";
                default: return tipoReporte;
            }
        }
        private string ObtenerCategoriaDisplay(string tipoReporte, ReportMetadata reporte) {
            switch (tipoReporte) {
                case "Producto":
                    return "Producto Específico"; // O podrías poner la categoría del producto si la tienes
                case "Categoria":
                    return string.IsNullOrEmpty(reporte.Category) ? "Todas las categorías" : reporte.Category;
                case "Inventario":
                case "Ganancias":
                    return "Todas las categorías";
                default:
                    return reporte.Category ?? "General";
            }
        }

        private string ObtenerDescripcionDisplay(string tipoReporte, ReportMetadata reporte) {
            switch (tipoReporte) {
                case "Producto":
                    return $"Producto: {_parametroBusqueda}";
                case "Categoria":
                    return string.IsNullOrEmpty(_parametroBusqueda) ?
                        "Todos los productos de la categoría" :
                        $"Productos de la categoría: {_parametroBusqueda}";
                case "Inventario":
                    return "Todos los productos en inventario";
                case "Ganancias":
                    return "Todos los productos con ventas";
                default:
                    return reporte.Description ?? "Reporte general";
            }
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

            _periodoSeleccionado = botonSeleccionado.Content.ToString().ToLower();
        }

        private void RestoreOpacityButtonsCategory() {
            lbl_DiarioCategoria.Opacity = 1.0;
            lbl_SemanalCategoria.Opacity = 1.0;
            lbl_MensualCategoria.Opacity = 1.0;
            lbl_AnualCategoria.Opacity = 1.0;
            lbl_GenerarCategoria.Opacity = 1.0;
        }

        private async void BtnClickGenerarCategoria(object sender, RoutedEventArgs e) {
            try {
                _parametroBusqueda = txtb_CategoriaProducto.Text;
                await CargarReportesDisponibles();
                HideAllGrids();
            } catch (Exception ex) {
                MessageBox.Show($"Error al cargar reportes: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            _periodoSeleccionado = botonSeleccionado.Content.ToString().ToLower();
        }

        private void BtnClickCancelarInventarioGanancias(object sender, RoutedEventArgs e) {
            ResetForm();
        }

        private async void BtnClickGenerarInventarioGanancias(object sender, RoutedEventArgs e) {
            try {
                await CargarReportesDisponibles();
                HideAllGrids();
            } catch (Exception ex) {
                MessageBox.Show($"Error al cargar reportes: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestoreOpacityButtonsInventoryGain() {
            lbl_DiarioInventarioGanancias.Opacity = 1.0;
            lbl_SemanalInventarioGanancias.Opacity = 1.0;
            lbl_MensualInventarioGanancias.Opacity = 1.0;
            lbl_AnualInventarioGanancias.Opacity = 1.0;
            lbl_GenerarInventarioGanancias.Opacity = 1.0;
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
        private void BtnPrimeraPagina_Click(object sender, RoutedEventArgs e) {
            _currentPage = 0;
            CargarReportesDisponibles().Wait();
        }

        private void BtnPaginaAnterior_Click(object sender, RoutedEventArgs e) {
            if (_currentPage > 0) {
                _currentPage--;
                CargarReportesDisponibles().Wait();
            }
        }

        private void BtnPaginaSiguiente_Click(object sender, RoutedEventArgs e) {
            if (_currentPage < _totalPages - 1) {
                _currentPage++;
                CargarReportesDisponibles().Wait();
            }
        }

        private void BtnUltimaPagina_Click(object sender, RoutedEventArgs e) {
            _currentPage = _totalPages - 1;
            CargarReportesDisponibles().Wait();
        }

        private void ActualizarEtiquetaPagina() {
            lblInfoPagina.Content = $"Página {_currentPage + 1} de {_totalPages}";
        }
    }
}