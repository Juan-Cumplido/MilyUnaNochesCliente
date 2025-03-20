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

namespace MilyUnaNochesWPFApp.Views
{
    public partial class ConsultProductsView : Page
    {
        public ConsultProductsView()
        {
            InitializeComponent();
            LoadProducts();
        }
        public List<MilyUnaNochesWPFApp.Logic.Product> GetProductsFromServer()
        {
            IProductsManager proxy = new ProductsManagerClient();
            try
            {
                var productsDb = proxy.GetProducts();

                List<MilyUnaNochesWPFApp.Logic.Product> products = productsDb.Select(p => new MilyUnaNochesWPFApp.Logic.Product
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
                Console.WriteLine($"Ocurrió un error al obtener los productos: {ex.Message}");
                return new List<MilyUnaNochesWPFApp.Logic.Product>();
            }
        }

        private void LoadProducts()
        {
            List<MilyUnaNochesWPFApp.Logic.Product> products = GetProductsFromServer();

            ProductsDataGrid.ItemsSource = products;
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterProductView());
        }

        private void Consult(object sender, RoutedEventArgs e)
        {
            // Obtener el producto seleccionado
            var selectedProduct = ProductsDataGrid.SelectedItem as MilyUnaNochesWPFApp.Logic.Product;

            if (selectedProduct == null)
            {
                MessageBox.Show("Por favor, seleccione un producto antes de consultar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NavigationService?.Navigate(new ProductDetailView(selectedProduct));
        }

        private void Validate(object sender, RoutedEventArgs e)
        {

        }

        private void Delete(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Edit(object sender, RoutedEventArgs e)
        {

        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem != null)
            {
                // Desmarcar todos los CheckBox excepto el de la fila seleccionada
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
    }
}
