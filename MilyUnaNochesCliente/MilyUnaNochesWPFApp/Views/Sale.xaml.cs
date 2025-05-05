using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using static MilyUnaNochesWPFApp.Views.CustomDialog;

namespace MilyUnaNochesWPFApp.Views {
    public partial class Sale : Page {
        private List<VentaProducto> _currentSaleDetails = new List<VentaProducto>();
        private decimal _totalAmount = 0;
        private int _currentEmployeeId;
        private int _currentClientId;
        private List<MilyUnaNochesWPFApp.MilyUnaNochesProxy.Product> _availableProducts = new List<MilyUnaNochesWPFApp.MilyUnaNochesProxy.Product>();

        public Sale(int employeeId) {
            InitializeComponent();
            _currentEmployeeId = employeeId;
            txtNumeroVendedor.Text = employeeId.ToString();
            InitializeUI();
        }

        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void InitializeUI() {
            ProductsPanel.Children.Clear();
            txtNumeroProductos.Text = "0";
            lblPrecio.Content = "$0.00";
            txtNumeroCliente.Text = "";
            gridSearchProduct.Visibility = Visibility.Collapsed;
            gridFormPay.Visibility = Visibility.Collapsed;
        }


        private void AddProductsButton_Click(object sender, RoutedEventArgs e) {
            gridSearchProduct.Visibility = Visibility.Visible;
            txtSearchProduct.Text = "";
            txtSearchProduct.Focus();
            DisableButtons();
        }

        private void DisableButtons() {
            lblPay.IsEnabled = false;
            lblCancelSale.IsEnabled = false;
            lblAddProduct.IsEnabled = false;
        }

        private void EnableButtons() {
            lblPay.IsEnabled = true;
            lblCancelSale.IsEnabled = true;
            lblAddProduct.IsEnabled = true;
        }

        private Dictionary<int, int> _productCounts = new Dictionary<int, int>();

        private async void AddSelectedProduct_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(txtSearchProduct.Text)) {
                ShowCustomMessage("Ingrese el código del producto.", DialogType.Warning);
                return;
            }

            string productCode = txtSearchProduct.Text.Trim();

            try {
                IProductsManager proxy = new ProductsManagerClient();
                var product = await proxy.GetProductByCodeAsync(productCode);

                if (product == null) {
                    
                    ShowCustomMessage($"No se encontró producto con código: {productCode}", DialogType.Warning);
                    return;
                }

                var stockResponse = await proxy.GetProductStockAsync(product.IdProducto);

                if (!stockResponse.Success) {
                    
                    ShowCustomMessage($"Error al consultar stock: {stockResponse.Message}", DialogType.Warning);
                    return;
                }

                int currentStock = stockResponse.Stock;
                int inCart = _productCounts.ContainsKey(product.IdProducto) ? _productCounts[product.IdProducto] : 0;

                if (currentStock <= 0) {
                    
                    ShowCustomMessage($"Producto agotado: {product.NombreProducto}\nCódigo: {product.CodigoProducto}", DialogType.Warning);
                    return;
                }

                if (inCart >= currentStock) {
                    MessageBox.Show($"No hay suficiente stock de {product.NombreProducto}\n" +
                                   $"En carrito: {inCart}, Disponible: {currentStock}",
                        "Stock insuficiente",
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }

                if (currentStock <= 10) {
                    MessageBox.Show($"Advertencia: Stock bajo de {product.NombreProducto}\n" +
                                  $"Unidades restantes: {currentStock}\n" +
                                  $"Considere reabastecer pronto.",
                        "Stock bajo",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                var existingProduct = _currentSaleDetails.FirstOrDefault(p => p.IdProducto == product.IdProducto);

                if (existingProduct != null) {
                    existingProduct.Cantidad += 1;
                    existingProduct.Subtotal = existingProduct.Cantidad * existingProduct.PrecioUnitario;
                } else {
                    var ventaProducto = new VentaProducto {
                        IdProducto = product.IdProducto,
                        NombreProducto = product.NombreProducto,
                        Cantidad = 1,
                        PrecioUnitario = product.PrecioVenta,
                        PrecioCompra = product.PrecioCompra, 
                        Subtotal = product.PrecioVenta
                        
                    };

                    _currentSaleDetails.Add(ventaProducto);
                }

                if (_productCounts.ContainsKey(product.IdProducto))
                    _productCounts[product.IdProducto]++;
                else
                    _productCounts.Add(product.IdProducto, 1);

                UpdateUI();
                gridSearchProduct.Visibility = Visibility.Collapsed;
                EnableButtons();
            } catch (Exception ex) {
                ShowCustomMessage($"Error al buscar producto: {ex.Message}", DialogType.Warning);
            }
        }

        private void UpdateUI() {
            txtNumeroProductos.Text = _productCounts.Values.Sum().ToString(); 
            _totalAmount = _currentSaleDetails.Sum(d => d.Subtotal);
            lblPrecio.Content = _totalAmount.ToString("C");
            UpdateProductsPanel();
        }

        private void UpdateProductsPanel() {
            ProductsPanel.Children.Clear();

            foreach (var kvp in _productCounts) {
                int productId = kvp.Key;
                int cantidad = kvp.Value;

                var product = _currentSaleDetails.FirstOrDefault(p => p.IdProducto == productId);
                if (product == null)
                    continue;

                for (int i = 0; i < cantidad; i++) {
                    var border = new Border {
                        CornerRadius = new CornerRadius(10),
                        Padding = new Thickness(10),
                        Width = 180,
                        Height = 234,
                        Background = Brushes.White,
                        Margin = new Thickness(5)
                    };

                    var stackPanel = new StackPanel();

                    var image = new Image {
                        Source = new BitmapImage(new Uri("/Images/Products/Product1.png", UriKind.Relative)),
                        Width = 146,
                        Height = 145,
                        Stretch = Stretch.UniformToFill
                    };

                    var nameText = new TextBlock {
                        Text = product.NombreProducto,
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        TextAlignment = TextAlignment.Center,
                        Foreground = (Brush)new BrushConverter().ConvertFrom("#FFC18D5C"),
                        FontFamily = new FontFamily("Bodoni MT Condensed")
                    };

                    var priceText = new TextBlock {
                        Text = $"Precio: {product.PrecioUnitario:C}",
                        FontSize = 16,
                        TextAlignment = TextAlignment.Center,
                        FontFamily = new FontFamily("Bodoni MT Condensed"),
                        Foreground = (Brush)new BrushConverter().ConvertFrom("#FFC18D5C")
                    };

                    var removeButton = new Button {
                        Content = "Quitar",
                        Background = (Brush)new BrushConverter().ConvertFrom("#FFC18D5C"),
                        Foreground = Brushes.White,
                        Margin = new Thickness(5),
                        BorderBrush = Brushes.Transparent,
                        FontWeight = FontWeights.Bold,
                        Tag = product 
                    };
                    removeButton.Click += RemoveProduct_Click;

                    stackPanel.Children.Add(image);
                    stackPanel.Children.Add(nameText);
                    stackPanel.Children.Add(priceText);
                    stackPanel.Children.Add(removeButton);

                    border.Child = stackPanel;
                    ProductsPanel.Children.Add(border);
                }
            }
        }

        private void RemoveProduct_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is VentaProducto product) {
                if (_productCounts.ContainsKey(product.IdProducto)) {
                    _productCounts[product.IdProducto]--;

                    if (_productCounts[product.IdProducto] <= 0) {
                        _productCounts.Remove(product.IdProducto);
                        var existingProduct = _currentSaleDetails.FirstOrDefault(p => p.IdProducto == product.IdProducto);
                        if (existingProduct != null) {
                            _currentSaleDetails.Remove(existingProduct);
                        }
                    } else {
                        var existingProduct = _currentSaleDetails.FirstOrDefault(p => p.IdProducto == product.IdProducto);
                        if (existingProduct != null) {
                            existingProduct.Cantidad--;
                            existingProduct.Subtotal = existingProduct.Cantidad * existingProduct.PrecioUnitario;
                        }
                    }

                    UpdateUI();
                }
            }
        }

        private async Task<bool> ValidateNumberPhoneClientAsync()
        {
            if (string.IsNullOrWhiteSpace(txtNumeroCliente.Text))
            {
                var dialog = new CustomDialog("No se ingresó número telefónico. ¿Desea continuar la venta sin asignar un cliente?", CustomDialog.DialogType.Confirmation);
                dialog.ShowDialog();

                if (dialog.UserConfirmed == true)
                {
                    _currentClientId = 0; 
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (txtNumeroCliente.Text.Length == 10)
            {
                try
                {
                    IUserManager userManager = new MilyUnaNochesProxy.UserManagerClient();
                    var clients = await userManager.GetUserProfileByNamePhoneAsync(txtNumeroCliente.Text);
                    

                    var exactMatch = clients?.FirstOrDefault(c => c.telefono == txtNumeroCliente.Text);

                    if (exactMatch != null)
                    {

                        int idCliente = userManager.GetClienteId(exactMatch.idUsuario);
                        _currentClientId = idCliente;
                       
                        return true;
                    }
                    else
                    {
                        var dialog = new CustomDialog("Cliente no registrado. ¿Desea registrarlo?", CustomDialog.DialogType.Confirmation);
                        dialog.ShowDialog();
                        if (dialog.UserConfirmed == true)
                        {
                            NavigationService.Navigate(new RegisterClient());
                        }
                        else
                        {
                            txtNumeroCliente.Text = string.Empty;
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ShowCustomMessage($"Error al buscar cliente: {ex.Message}", DialogType.Warning);
                    return false;
                }
            }
            else
            {
                ShowCustomMessage("Número de teléfono inválido.", DialogType.Warning);
                return false;
            }
        }



        private void CancelSaleButton_Click(object sender, RoutedEventArgs e) {
            _currentSaleDetails.Clear();
            InitializeUI();
        }

        private async void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSaleDetails.Count == 0)
            {
                ShowCustomMessage("No hay productos en la venta", DialogType.Warning);
                return;
            }

            bool clientExists = await ValidateNumberPhoneClientAsync();
            if (!clientExists)
            {
                return;
            }

            gridFormPay.Visibility = Visibility.Visible;
        }


        private async void ProcessPaymentButton_Click(object sender, RoutedEventArgs e) {
            string paymentMethod = (sender as Button)?.Content.ToString();
            try {
                ISaleManager proxy = new SaleManagerClient();
                var newSale = new Venta {
                    IdEmpleado = _currentEmployeeId,
                    IdCliente = _currentClientId,
                    MetodoPago = paymentMethod,
                    MontoTotal = _totalAmount
                };

                var validationResult = await proxy.ValidateSaleAsync(_currentSaleDetails.ToList());

                if (validationResult.Any()) {
                    var errorMessage = new StringBuilder("Problemas con el stock:\n");
                    foreach (var error in validationResult) {
                        errorMessage.AppendLine($"• {error}");
                    }

                    MessageBox.Show(errorMessage.ToString(), "Error de stock",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var saleResult = await proxy.ProcessSaleAsync(newSale, _currentSaleDetails.ToList());

                if (saleResult.Success) {
                    ShowReceipt(newSale, _currentEmployeeId);

                   
                    ShowCustomMessage("Venta registrada exitosamente", DialogType.Success);
                    _currentSaleDetails.Clear();
                    InitializeUI();
                } else {
                    var errorMessage = string.Join("\n", saleResult.Errors);
                    MessageBox.Show(errorMessage, "Error en la venta",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                }
            } catch (Exception ex) {
                
                ShowCustomMessage($"Error inesperado: {ex.Message}", DialogType.Error);
            } finally {
                gridFormPay.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowReceipt(Venta sale, int employeeId) {
            var receiptWindow = new Window {
                Title = $"Venta del empleado {employeeId}",
                Width = 400,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            stackPanel.Children.Add(new TextBlock {
                Text = $"Venta de empleado #{employeeId}",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            stackPanel.Children.Add(new TextBlock {
                Text = DateTime.Now.ToString("f"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 15)
            });

            foreach (var product in _currentSaleDetails) {
                var productPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };

                productPanel.Children.Add(new TextBlock {
                    Text = $"{product.NombreProducto}",
                    FontWeight = FontWeights.Bold
                });

                productPanel.Children.Add(new TextBlock {
                    Text = $"Cantidad: {product.Cantidad} x {product.PrecioUnitario:C}",
                    Margin = new Thickness(15, 0, 0, 0)
                });

                productPanel.Children.Add(new TextBlock {
                    Text = $"Subtotal: {product.Subtotal:C}",
                    Margin = new Thickness(15, 0, 0, 0)
                });

                productPanel.Children.Add(new TextBlock {
                    Text = $"Margen por producto: {(product.PrecioUnitario - product.PrecioCompra):C}",
                    Margin = new Thickness(15, 0, 0, 0),
                    Foreground = Brushes.Green
                });

                stackPanel.Children.Add(productPanel);
                stackPanel.Children.Add(new Separator());
            }

            stackPanel.Children.Add(new TextBlock {
                Text = $"Total: {_totalAmount:C}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 15, 0, 0)
            });

            receiptWindow.Content = stackPanel;
            receiptWindow.ShowDialog();
        }

        private void CloseSearchPanel_Click(object sender, RoutedEventArgs e) {
            gridSearchProduct.Visibility = Visibility.Collapsed;
            lblPay.IsEnabled = true;
            lblCancelSale.IsEnabled = true;
            lblAddProduct.IsEnabled = true;

        }

        private void ClosePaymentPanel_Click(object sender, RoutedEventArgs e) {
            gridFormPay.Visibility = Visibility.Collapsed;
        }

        private void Telephone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void Telephone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Space ||
                (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control) ||
                (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control) ||
                (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control))
            {
                e.Handled = true;
            }
        }
    }
}