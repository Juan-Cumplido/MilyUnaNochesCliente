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
            ISaleManager proxy = null;
            try {
                proxy = new SaleManagerClient();
                var salesDb = await proxy.SearchSalesAsync(null, null) ?? Array.Empty<Venta>();

                return salesDb.Select(s => new SaleDisplayItem {
                    SaleId = s.idVenta,
                    EmployeeId = s.IdEmpleado,
                    ClientId = s.IdCliente ?? 0,
                    PaymentMethod = s.MetodoPago ?? "No especificado",
                    TotalAmount = s.MontoTotal,
                    SaleDate = s.fecha,
                    OriginalSale = s
                }).ToList();
            } catch (Exception ex) {
                await Dispatcher.InvokeAsync(() => {
                    MessageBox.Show($"Error al obtener ventas: {ex.Message}");
                });
                return new List<SaleDisplayItem>();
            } finally {
                try {
                    if (proxy != null) {
                        if (proxy is ICommunicationObject client) {
                            if (client.State == CommunicationState.Faulted) {
                                client.Abort();
                            } else {
                                client.Close();
                            }
                        }
                    }
                } catch {
                    // Ignorar errores al cerrar el proxy
                }
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
                    MessageBox.Show($"Mostrando {_allSales.Count} ventas");
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
                        (s.PaymentMethod?.ToLower()?.Contains(searchText) ?? false)
                    );
                }

                var result = filteredSales.ToList();
                SalesDataGrid.ItemsSource = result;

                MessageBox.Show(result.Any()
                    ? $"Mostrando {result.Count} ventas{(string.IsNullOrWhiteSpace(searchText) ? "" : " filtradas")}"
                    : "No hay ventas que coincidan con la búsqueda");
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
            public int ClientId { get; set; }
            public string PaymentMethod { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime SaleDate { get; set; }

            public string Cliente => ClientId == 0 ? "No registrado" : $"Cliente #{ClientId}";
            public string Monto => TotalAmount.ToString("C");
            public string Articulos => "Ver detalles";
            public string Fecha => SaleDate.ToString("dd/MM/yyyy");
            public string Empleado => $"Empleado #{EmployeeId}";

            public Venta OriginalSale { get; set; }
        }
    }
}