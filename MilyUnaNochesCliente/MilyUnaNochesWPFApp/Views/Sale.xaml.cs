using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;

namespace MilyUnaNochesWPFApp.Views
{
    public partial class Sale : Page
    {
        private List<VentaProducto> _currentSaleDetails = new List<VentaProducto>();
        private decimal _totalAmount = 0;
        private int _currentEmployeeId;
        private List<MilyUnaNochesWPFApp.MilyUnaNochesProxy.Product> _availableProducts = new List<MilyUnaNochesWPFApp.MilyUnaNochesProxy.Product>();

        public Sale(int employeeId)
        {
            InitializeComponent();
            _currentEmployeeId = employeeId;
            txtNumeroVendedor.Text = employeeId.ToString();
            InitializeUI();
        }

        private void InitializeUI()
        {
            ProductsPanel.Children.Clear();
            txtNumeroProductos.Text = "0";
            lblPrecio.Content = "$0.00";
            txtNumeroCliente.Text = "";
            gridSearchProduct.Visibility = Visibility.Collapsed;
            gridFormPay.Visibility = Visibility.Collapsed;
        }


        private void AddProductsButton_Click(object sender, RoutedEventArgs e)
        {
            gridSearchProduct.Visibility = Visibility.Visible;
            txtSearchProduct.Text = "";
            txtSearchProduct.Focus();
        }

        private Dictionary<int, int> _productCounts = new Dictionary<int, int>();

        private async void AddSelectedProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchProduct.Text))
            {
                MessageBox.Show("Ingrese el código del producto", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string productCode = txtSearchProduct.Text.Trim();

            try
            {
                IProductsManager proxy = new ProductsManagerClient();
                var product = await proxy.GetProductByCodeAsync(productCode);

                if (product == null)
                {
                    MessageBox.Show($"No se encontró producto con código: {productCode}",
                        "Producto no encontrado",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var stockResponse = await proxy.GetProductStockAsync(product.IdProducto);

                if (!stockResponse.Success)
                {
                    MessageBox.Show($"Error al consultar stock: {stockResponse.Message}",
                        "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int currentStock = stockResponse.Stock;
                int inCart = _productCounts.ContainsKey(product.IdProducto) ? _productCounts[product.IdProducto] : 0;

                if (currentStock <= 0)
                {
                    MessageBox.Show($"Producto agotado: {product.NombreProducto}\nCódigo: {product.CodigoProducto}",
                        "Stock agotado",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (inCart >= currentStock)
                {
                    MessageBox.Show($"No hay suficiente stock de {product.NombreProducto}\n" +
                                   $"En carrito: {inCart}, Disponible: {currentStock}",
                        "Stock insuficiente",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (currentStock <= 10)
                {
                    MessageBox.Show($"Advertencia: Stock bajo de {product.NombreProducto}\n" +
                                  $"Unidades restantes: {currentStock}\n" +
                                  $"Considere reabastecer pronto.",
                        "Stock bajo",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                var ventaProducto = new VentaProducto
                {
                    IdProducto = product.IdProducto,
                    NombreProducto = product.NombreProducto,
                    Cantidad = 1,
                    PrecioUnitario = product.PrecioVenta,
                    PrecioCompra = product.PrecioCompra, // Nuevo campo
                    Subtotal = product.PrecioVenta
                };

                _currentSaleDetails.Add(ventaProducto);

                if (_productCounts.ContainsKey(product.IdProducto))
                    _productCounts[product.IdProducto]++;
                else
                    _productCounts.Add(product.IdProducto, 1);

                UpdateUI();
                gridSearchProduct.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar producto: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateUI()
        {
            txtNumeroProductos.Text = _currentSaleDetails.Count.ToString();
            _totalAmount = _currentSaleDetails.Sum(d => d.Subtotal);
            lblPrecio.Content = _totalAmount.ToString("C");
            UpdateProductsPanel();
        }

        private void UpdateProductsPanel()
        {
            ProductsPanel.Children.Clear();
            foreach (var product in _currentSaleDetails)
            {
                var border = new Border
                {
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(10),
                    Width = 180,
                    Height = 234,
                    Background = Brushes.White,
                    Margin = new Thickness(5)
                };

                var stackPanel = new StackPanel();

                var image = new Image
                {
                    Source = new BitmapImage(new Uri("/Images/Products/Product1.png", UriKind.Relative)),
                    Width = 146,
                    Height = 145,
                    Stretch = Stretch.UniformToFill
                };

                var nameText = new TextBlock
                {
                    Text = product.NombreProducto,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#FFC18D5C"),
                    FontFamily = new FontFamily("Bodoni MT Condensed")
                };

                var priceText = new TextBlock
                {
                    Text = $"Precio: {product.PrecioUnitario:C}",
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center,
                    FontFamily = new FontFamily("Bodoni MT Condensed"),
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#FFC18D5C")
                };

                var removeButton = new Button
                {
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

        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is VentaProducto product)
            {
                _currentSaleDetails.Remove(product);

                if (_productCounts.ContainsKey(product.IdProducto))
                {
                    _productCounts[product.IdProducto]--;
                    if (_productCounts[product.IdProducto] <= 0)
                        _productCounts.Remove(product.IdProducto);
                }

                UpdateUI();
            }
        }

        private void CancelSaleButton_Click(object sender, RoutedEventArgs e)
        {
            _currentSaleDetails.Clear();
            InitializeUI();
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSaleDetails.Count == 0)
            {
                MessageBox.Show("No hay productos en la venta", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            gridFormPay.Visibility = Visibility.Visible;
        }

        private async void ProcessPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            string paymentMethod = (sender as Button)?.Content.ToString();

            try
            {
                ISaleManager proxy = new SaleManagerClient();
                var newSale = new Venta
                {
                    IdEmpleado = _currentEmployeeId,
                    IdCliente = int.TryParse(txtNumeroCliente.Text, out int temp) ? temp : (int?)null,
                    MetodoPago = paymentMethod,
                    MontoTotal = _totalAmount
                };

                var validationResult = await proxy.ValidateSaleAsync(_currentSaleDetails.ToArray());

                if (validationResult.Any())
                {
                    var errorMessage = new StringBuilder("Problemas con el stock:\n");
                    foreach (var error in validationResult)
                    {
                        errorMessage.AppendLine($"• {error}");
                    }

                    MessageBox.Show(errorMessage.ToString(), "Error de stock",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                var saleResult = await proxy.ProcessSaleAsync(newSale, _currentSaleDetails.ToList());

                if (saleResult.Success) {
                    // Mostrar ticket con los nuevos campos
                    ShowReceipt(newSale, saleResult.SaleId.Value);

                    MessageBox.Show("Venta registrada exitosamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    InitializeUI();
                } else {
                    var errorMessage = string.Join("\n", saleResult.Errors);
                    MessageBox.Show(errorMessage, "Error en la venta",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } catch (Exception ex) {

                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                gridFormPay.Visibility = Visibility.Collapsed;
            }
        }


        private void ShowReceipt(Venta sale, int saleId) {
            var receiptWindow = new Window {
                Title = $"Ticket de Venta # {saleId}",
                Width = 400,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            // Encabezado
            stackPanel.Children.Add(new TextBlock {
                Text = $"Venta #{saleId}",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            stackPanel.Children.Add(new TextBlock {
                Text = DateTime.Now.ToString("f"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 15)
            });

            // Detalles de productos
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

                // Nuevo: Mostrar margen de ganancia
                productPanel.Children.Add(new TextBlock {
                    Text = $"Margen: {(product.PrecioUnitario - product.PrecioCompra):C}",
                    Margin = new Thickness(15, 0, 0, 0),
                    Foreground = Brushes.Green
                });

                stackPanel.Children.Add(productPanel);
                stackPanel.Children.Add(new Separator());
            }

            // Total
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
        }

        private void ClosePaymentPanel_Click(object sender, RoutedEventArgs e)
        {
            gridFormPay.Visibility = Visibility.Collapsed;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
    }
}