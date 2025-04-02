using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;

namespace MilyUnaNochesWPFApp.Views {
    public partial class ConsultSale : Page {
        private List<SaleDisplayItem> _allSales = new List<SaleDisplayItem>();
        private DispatcherTimer _filterTimer;
        private bool _isActive = true;

        public ConsultSale() {
            InitializeComponent();
            InitializeFilterTimer();
            LoadSales();
        }

        private void InitializeFilterTimer() {
            _filterTimer = new DispatcherTimer {
                Interval = TimeSpan.FromMilliseconds(350)
            };
            _filterTimer.Tick += OnFilterTimerTick;
        }

        public async Task<List<SaleDisplayItem>> GetSalesFromServerAsync() {
            try {
                ISaleManager proxy = new SaleManagerClient();
                var salesDb = await proxy.SearchSalesAsync(null, null);

                // Mapeo de ventas a la clase de visualización
                List<SaleDisplayItem> sales = salesDb.Select(s => new SaleDisplayItem {
                    SaleId = s.idVenta,
                    EmployeeId = s.IdEmpleado,
                    ClientId = s.IdCliente ?? 0,
                    PaymentMethod = s.MetodoPago,
                    TotalAmount = s.MontoTotal,
                    SaleDate = s.fecha,
                    // Asignamos los detalles del producto
                    ProductDetails = s.Detalles?.Select(d => new VentaProducto {
                        IdProducto = d.IdProducto,
                        NombreProducto = d.NombreProducto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList(),
                    OriginalSale = s
                }).ToList();

                return sales;
            } catch (Exception ex) {
                MessageBox.Show($"Error al obtener ventas: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<SaleDisplayItem>();
            }
        }

        private async void LoadSales() {
            try {
                _allSales = await GetSalesFromServerAsync();

                await Dispatcher.InvokeAsync(() => {
                    if (!_isActive) return;

                    if (!_allSales.Any()) {
                        ShowNoSalesMessage();
                        return;
                    }

                    SalesDataGrid.ItemsSource = _allSales;
                });
            } catch (Exception ex) {
                await Dispatcher.InvokeAsync(() => {
                    MessageBox.Show($"Error al cargar ventas: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            _filterTimer.Stop();
            _filterTimer.Start();
        }

        private void OnFilterTimerTick(object sender, EventArgs e) {
            _filterTimer.Stop();
            ApplyFilter();
        }

        private void ApplyFilter() {
            try {
                var searchText = SearchTextBox?.Text?.ToLower() ?? string.Empty;

                if (!_allSales.Any()) {
                    ShowNoSalesMessage();
                    return;
                }

                IEnumerable<SaleDisplayItem> filteredSales = _allSales;

                if (!string.IsNullOrWhiteSpace(searchText)) {
                    filteredSales = _allSales.Where(s =>
                        (s.ClientId.ToString().Contains(searchText)) ||
                        (s.TotalAmount.ToString("0.00").Contains(searchText)) ||
                        (s.EmployeeId.ToString().Contains(searchText)) ||
                        (s.PaymentMethod?.ToLower()?.Contains(searchText) ?? false) ||
                        (s.Articulos.ToLower().Contains(searchText)) // Búsqueda también en los artículos
                    );
                }

                var result = filteredSales.ToList();
                SalesDataGrid.ItemsSource = result;
            } catch (Exception ex) {
                MessageBox.Show($"Error al filtrar ventas: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowNoSalesMessage() {
            SalesDataGrid.ItemsSource = new List<SaleDisplayItem>();
            MessageBox.Show("No hay ventas registradas");
        }

        private void SalesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (SalesDataGrid.SelectedItem is SaleDisplayItem selectedSale) {
                // Implementar lógica de selección aquí
            }
        }

        public class SaleDisplayItem {
            public int SaleId { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public int ClientId { get; set; }
            public string PaymentMethod { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime SaleDate { get; set; }
            public List<VentaProducto> ProductDetails { get; set; }

            public string Cliente => ClientId == 0 ? "No registrado" : $"Cliente #{ClientId}";
            public string Monto => TotalAmount.ToString("C");
            public string Articulos => GetFormattedProducts();
            public string Fecha => SaleDate.ToString("dd/MM/yyyy");
            public string Empleado => string.IsNullOrEmpty(EmployeeName) ? $"Empleado #{EmployeeId}" : EmployeeName;

            public Venta OriginalSale { get; set; }

            private string GetFormattedProducts() {
                if (ProductDetails == null || !ProductDetails.Any())
                    return "Sin artículos";

                return string.Join(Environment.NewLine,
                    ProductDetails.Select(p =>
                        $"({p.Cantidad}) {p.NombreProducto} x {p.PrecioUnitario.ToString("0.00")} = {(p.Cantidad * p.PrecioUnitario).ToString("0.00")}"));
            }
        }
    }
}
