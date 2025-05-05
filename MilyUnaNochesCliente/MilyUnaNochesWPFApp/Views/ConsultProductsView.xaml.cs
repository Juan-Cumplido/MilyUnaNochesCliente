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
using System.Windows.Forms;
using static MilyUnaNochesWPFApp.Views.CustomDialog;
using MilyUnaNochesWPFApp.Utilities;
using System.ServiceModel;
using System.Windows.Input;

namespace MilyUnaNochesWPFApp.Views
{
    public partial class ConsultProductsView : Page
    {
        private List<Logic.Product> _allProducts = new List<Logic.Product>();
        private List<Logic.Product> _filteredProducts = new List<Logic.Product>();
        private string origen;
    
        public ConsultProductsView(string origen = "")
        {
            InitializeComponent();
            this.origen = origen;

            if (origen == "ManagerMenu")
            {
                img_GoOut.Visibility = Visibility.Collapsed;
                img_goBack.Visibility = Visibility.Visible;
                btn_Registrar.Visibility = Visibility.Collapsed;
                btn_Registrar.Visibility = Visibility.Collapsed;
                btn_Validar.Visibility = Visibility.Collapsed;
                btn_Editar.Visibility = Visibility.Collapsed;
                btn_Eliminar.Visibility = Visibility.Collapsed;
                brd_editar.Visibility = Visibility.Collapsed;
                brd_eliminar.Visibility = Visibility.Collapsed;
                btn_consultar.Visibility = Visibility.Collapsed;
                brd_consultar.Visibility = Visibility.Collapsed;
            }

        LoadProducts();
        }

        private void Image_MouseDownGoBack(object sender, MouseButtonEventArgs e)
        {
            ManagerMenu managerMenu = new ManagerMenu();
            this.NavigationService.Navigate(managerMenu);
        }

        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = new CustomDialog("Regresará a la ventana de inicio de sesión. ¿Está seguro de salir?", CustomDialog.DialogType.Confirmation);
            dialog.ShowDialog();
            LoggerManager logger = new LoggerManager(this.GetType());
            if (dialog.UserConfirmed == true)
            {
                try
                {
                    MilyUnaNochesProxy.UserSessionManagerClient userSessionManagerClient = new MilyUnaNochesProxy.UserSessionManagerClient();
                    UserSession userSession = new UserSession()
                    {

                        idAcceso = UserProfileSingleton.idAcceso
                    };


                    int disconnectionResult = userSessionManagerClient.Disconnect(userSession, false);
                    if (disconnectionResult == Constants.SuccessOperation)
                    {
                        UserProfileSingleton.Instance.ResetSingleton();
                        LoginView login = new LoginView();
                        this.NavigationService.Navigate(login);
                    }
                    else if (disconnectionResult == Constants.NoDataMatches)
                    {
                        ShowCustomMessage("No se ha podido encontrar la sesión de usuario", DialogType.Warning);
                    }
                    else
                    {
                        ShowCustomMessage("Ocurrio un error al cerrar la sesión, intentelo de nuevo", DialogType.Warning);
                    }
                }
                catch (EndpointNotFoundException endPointException)
                {
                    logger.LogFatal(endPointException);
                    ShowCustomMessage("No se pudo establecer conexión con el servidor. Por favor, verifique la configuración de red e intente nuevamente.\r\n", DialogType.Error);

                }
                catch (TimeoutException timeOutException)
                {
                    logger.LogWarn(timeOutException);
                    ShowCustomMessage("Inténtalo de nuevo. El tiempo de espera ha expirado. Por favor, verifica tu conexión al servidor.", DialogType.Error);
                }
                catch (CommunicationException communicationException)
                {
                    logger.LogFatal(communicationException);
                    ShowCustomMessage("Se ha producido un fallo para establecer la conexión al servidor. Cheque su conexión a internet e inténtelo de nuevo", DialogType.Error);

                }
            }
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
                
                ShowCustomMessage("Error al cargar los productos. Por favor, intente nuevamente.", DialogType.Error);
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
       
            if (ProductsDataGrid.SelectedItem != null)
            {
                foreach (var item in ProductsDataGrid.Items)
                {
                    if (item != ProductsDataGrid.SelectedItem)
                    {
                        var row = ProductsDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                        if (row != null)
                        {
                            var checkBox = FindVisualChild<System.Windows.Controls.CheckBox>(row);
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
            ValidateProductsView validateProducts = new ValidateProductsView();
            Window mainWindow = Window.GetWindow(this);
            validateProducts.Owner = mainWindow;
            validateProducts.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            validateProducts.ShowDialog();

        }


        private void Consult(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsDataGrid.SelectedItem as Logic.Product;

            if (selectedProduct == null)
            {
           
                ShowCustomMessage("Por favor, seleccione un producto antes de consultar.", DialogType.Warning);
                return;
            }

            NavigationService?.Navigate(new ProductDetailView(selectedProduct));
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsDataGrid.SelectedItem as Logic.Product;

            if (selectedProduct == null)
            {
                
                ShowCustomMessage("Por favor, seleccione un producto antes de editarlo.", DialogType.Warning);
                return;
            }

            NavigationService?.Navigate(new EditProductView(selectedProduct));

        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsDataGrid.SelectedItem as Logic.Product;

            if (selectedProduct == null)
            {
              
                ShowCustomMessage("Por favor, seleccione un producto antes de eliminarlo.", DialogType.Warning);
                return;
            }        

            var dialog = new CustomDialog($"¿Estás seguro que deseas borrar el producto {selectedProduct.NombreProducto} ?", CustomDialog.DialogType.Confirmation);
            dialog.ShowDialog();
            if (dialog.UserConfirmed == true)
            {
                IProductsManager proxy = new ProductsManagerClient();
                if (proxy.DeleteProduct(selectedProduct.NombreProducto))
                {
                    
                    ShowCustomMessage("Eliminación realizada con éxito", DialogType.Success);
                    NavigationService?.Navigate(new ConsultProductsView());
                }

            }

        }
    }
}