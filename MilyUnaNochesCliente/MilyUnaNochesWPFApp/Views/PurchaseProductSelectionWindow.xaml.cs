using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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

        }

        private void Accept_Click(object sender, RoutedEventArgs e) {
            if (parentPage != null) {
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
    }
}
