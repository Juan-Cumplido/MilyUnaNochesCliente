using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using System.Diagnostics;

namespace MilyUnaNochesWPFApp.Views
{
    public partial class ConsultProductsView : Page
    {
        private List<Logic.Product> _allProducts = new List<Logic.Product>();
        private List<Logic.Product> _filteredProducts = new List<Logic.Product>();

        public ConsultProductsView()
        {
            InitializeComponent();
            LoadProducts();
        }

        public List<Logic.Product> GetProductsFromServer()
        {
            IProductsManager proxy = new ProductsManagerClient();
            try
            {
                var productsDb = proxy.GetProducts();

                List<Logic.Product> products = productsDb.Select(p => new Logic.Product
                {
                    CodigoProducto = p.CodigoProducto,
                    NombreProducto = p.NombreProducto,
                    Descripcion = p.Descripcion,
                    Categoria = p.Categoria,
                    Cantidad = p.Cantidad,
                    PrecioVenta = p.PrecioVenta,
                    PrecioCompra = p.PrecioCompra,
                    Imagen = p.Imagen,
                }).ToList();

                return products;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ocurrió un error al obtener los productos: {ex.Message}");
                MessageBox.Show("Error al cargar los productos. Por favor, intente nuevamente.",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Logic.Product>();
            }
        }

        private void LoadProducts()
        {
            try
            {
                _allProducts = GetProductsFromServer();
                _filteredProducts = new List<Logic.Product>(_allProducts);
                ProductsDataGrid.ItemsSource = _filteredProducts;

                // Configurar el placeholder del buscador
                SearchTextBox.Text = "Buscar producto...";
                SearchTextBox.Foreground = Brushes.Gray;
                SearchTextBox.GotFocus += RemoveSearchPlaceholder;
                SearchTextBox.LostFocus += SetSearchPlaceholder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar productos: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text == "Buscar producto..." || string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                ProductsDataGrid.ItemsSource = _allProducts;
                return;
            }

            var searchText = SearchTextBox.Text.Trim().ToLower();

            _filteredProducts = _allProducts
                .Where(p => (p.NombreProducto != null && p.NombreProducto.ToLower().Contains(searchText)) ||
                            (p.Categoria != null && p.Categoria.ToLower().Contains(searchText)))
                .OrderByDescending(p => GetRelevanceScore(p, searchText))
                .ThenBy(p => p.NombreProducto)
                .ToList();

            ProductsDataGrid.ItemsSource = _filteredProducts;

            if (_filteredProducts.Any())
            {
                ProductsDataGrid.SelectedIndex = 0;
                ProductsDataGrid.ScrollIntoView(ProductsDataGrid.SelectedItem);
            }
        }

        private int GetRelevanceScore(Logic.Product product, string searchText)
        {
            if (product.NombreProducto != null && product.NombreProducto.ToLower().StartsWith(searchText))
                return 3;
            if (product.NombreProducto != null && product.NombreProducto.ToLower().Contains(searchText))
                return 2;
            if (product.Categoria != null && product.Categoria.ToLower().Contains(searchText))
                return 1;
            return 0;
        }

        private void RemoveSearchPlaceholder(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Buscar producto...")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = Brushes.Black;
            }
        }

        private void SetSearchPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Buscar producto...";
                SearchTextBox.Foreground = Brushes.Gray;
            }
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Mantener la selección única
            if (ProductsDataGrid.SelectedItem != null)
            {
                foreach (var item in ProductsDataGrid.Items)
                {
                    if (item != ProductsDataGrid.SelectedItem)
                    {
                        var row = ProductsDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                        if (row != null)
                        {
                            var checkBox = FindVisualChild<CheckBox>(row);
                            if (checkBox != null)
                            {
                                checkBox.IsChecked = false;
                            }
                        }
                    }
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }


        private void Register(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterProductView());
        }

        private void ConsultPage(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConsultProductsView());
        }

        private void Validate(object sender, RoutedEventArgs e)
        {

        }


        private void Consult(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsDataGrid.SelectedItem as Logic.Product;

            if (selectedProduct == null)
            {
                MessageBox.Show("Por favor, seleccione un producto antes de consultar.",
                              "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NavigationService?.Navigate(new ProductDetailView(selectedProduct));
        }

        private void Edit(object sender, RoutedEventArgs e)
        {

        }

        private void Delete(object sender, RoutedEventArgs e)
        {

        }
    }
}