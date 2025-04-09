    using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    namespace MilyUnaNochesWPFApp.Views {
        /// <summary>
        /// Lógica de interacción para PurchaseProductSelectionWindow.xaml
        /// </summary>
        public partial class PurchaseProductSelectionWindow : Window {
            private RegisterPurchase parentPage;
            private IProductsManager _productsManager;
            private ObservableCollection<Product> _products;
            private ObservableCollection<ProductPurchase> _selectedProducts = new ObservableCollection<ProductPurchase>();

            public PurchaseProductSelectionWindow(RegisterPurchase parentPage) {
                InitializeComponent();
                this.parentPage = parentPage;
                _productsManager = new ProductsManagerClient();
                LoadProductsAsync();
                lstSelectedProducts.ItemsSource = _selectedProducts;

                if (!string.IsNullOrEmpty(parentPage.SelectedPayMethod)) {
                    foreach (ComboBoxItem item in cb_SelectedPayMethod.Items) {
                        if (item.Content.ToString() == parentPage.SelectedPayMethod) {
                            cb_SelectedPayMethod.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            private async void LoadProductsAsync() {
                try {
                    var products = await Task.Run(() => _productsManager.GetProducts());
                    _products = new ObservableCollection<Product>(products);
                    cmbProducts.ItemsSource = _products;
                    cmbProducts.DisplayMemberPath = "NombreProducto";
                    cmbProducts.SelectedValuePath = "IdProducto";
                } catch (Exception ex) {
                    MessageBox.Show("Error cargando productos: " + ex.Message);
                }
            }

            private void Cancel_Click(object sender, RoutedEventArgs e) {
                this.Close();
            }

            private void Accept_Click(object sender, RoutedEventArgs e) {
                if (parentPage != null) {
                    if (cb_SelectedPayMethod.SelectedItem == null) {
                        MessageBox.Show("Selecciona un método de pago válido.");
                        return; 
                    }
                    var selectedItem = cb_SelectedPayMethod.SelectedItem as ComboBoxItem;
                    parentPage.SelectedPayMethod = selectedItem?.Content.ToString();
                    parentPage.AddPurchasedProducts(_selectedProducts.ToList());
                }
                this.Close();
            }

            private void AddProduct_Click(object sender, RoutedEventArgs e) {
                if (cmbProducts.SelectedItem is Product selectedProduct &&
                    int.TryParse(txtQuantity.Text, out int quantity) && quantity > 0) {

                    ProductPurchase existing = _selectedProducts.FirstOrDefault(p => p.IdProducto == selectedProduct.IdProducto);
              
                    if (existing != null) {
                        existing.Cantidad += quantity;
                        existing.MontoProducto = selectedProduct.PrecioCompra * existing.Cantidad;
                    } else {
                        _selectedProducts.Add(new ProductPurchase {
                            IdProducto = selectedProduct.IdProducto,
                            CodigoProducto = selectedProduct.CodigoProducto,
                            Cantidad = quantity,
                            NombreProducto = selectedProduct.NombreProducto,
                            MontoProducto = selectedProduct.PrecioCompra * quantity
                        });
                    }
                    UpdateTotal();
                } else {
                    MessageBox.Show("Seleccione un producto e ingrese cantidad válida");
                }
            }
            private void UpdateTotal() {
                decimal total = _selectedProducts.Sum(p => p.MontoProducto);
                lbTotalAmount.Content = total.ToString();
            }

            public void LoadExistingProducts(List<ProductPurchase> existingProducts) {
                _selectedProducts.Clear();
                foreach (var product in existingProducts) {
                    _selectedProducts.Add(product);
                }
                UpdateTotal();
            }

            // Método para aumentar la cantidad en 1
            private void Increase_Click(object sender, RoutedEventArgs e) {
                if (sender is Button btn && btn.Tag is ProductPurchase product) {
                    product.Cantidad += 1;
                    // Actualiza el monto según el precio de compra. 
                    // Se asume que puedes obtener el precio base (PrecioCompra) del producto. 
                    // Si no lo tienes en la clase ProductPurchase, podrías necesitar consultarlo.
                    // Aquí se asume que MontoProducto se actualiza de la siguiente manera:
                    var prodBase = _products.FirstOrDefault(p => p.IdProducto == product.IdProducto);
                    if (prodBase != null) {
                        product.MontoProducto = prodBase.PrecioCompra * product.Cantidad;
                    }
                    UpdateTotal();
                    // Actualizamos la vista
                    lstSelectedProducts.Items.Refresh();
                }
            }

            private void Decrease_Click(object sender, RoutedEventArgs e) {
                if (sender is Button btn && btn.Tag is ProductPurchase product) {
                    if (product.Cantidad > 1) {
                        product.Cantidad -= 1;
                        var prodBase = _products.FirstOrDefault(p => p.IdProducto == product.IdProducto);
                        if (prodBase != null) {
                            product.MontoProducto = prodBase.PrecioCompra * product.Cantidad;
                        }
                        UpdateTotal();
                        lstSelectedProducts.Items.Refresh();
                    } else {
                        RemoveProduct_Click(sender, e);
                    }
                }
            }

            private void RemoveProduct_Click(object sender, RoutedEventArgs e) {
                if (sender is Button btn && btn.Tag is ProductPurchase product) {
                    _selectedProducts.Remove(product);
                    UpdateTotal();
                }
            }
        }
    }
