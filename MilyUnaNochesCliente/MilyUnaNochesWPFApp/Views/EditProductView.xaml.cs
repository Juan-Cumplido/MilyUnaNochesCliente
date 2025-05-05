using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using MilyUnaNochesWPFApp.Logic;
using Product = MilyUnaNochesWPFApp.Logic.Product;
using MilyUnaNochesWPFApp.Utilities;
using static MilyUnaNochesWPFApp.Views.CustomDialog;
using System.ServiceModel;

namespace MilyUnaNochesWPFApp.Views
{
    /// <summary>
    /// Lógica de interacción para EditProductView.xaml
    /// </summary>
    public partial class EditProductView : Page
    {
        private string oldProductName;
        public EditProductView(Product producto)
        {
            InitializeComponent();

            txtb_NombreProducto.Text = producto.NombreProducto;
            txtb_CodigoProducto.Text = producto.CodigoProducto;
            txtb_Descripcion.Text = producto.Descripcion;
            txtb_Categoria.Text = producto.Categoria;
            txtb_Cantidad.Text = producto.Cantidad.ToString();
            txtb_PrecioVenta.Text = producto.PrecioVenta.ToString(); 
            txtb_PrecioCompra.Text = producto.PrecioCompra.ToString(); 

            if (producto.Imagen != null && producto.Imagen.Length > 0)
            {
                img_Photo.Source = ConvertImage(producto.Imagen);
            }

            SetOldProductName(producto.NombreProducto);
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
        private void SetOldProductName(string productName)
        {
            this.oldProductName = productName;
        }
        private string GetOldProductName()
        {
            return oldProductName;
        }

        private BitmapImage ConvertImage(byte[] imageData)
        {
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private bool ValidarCampos()
        {
            System.Windows.Controls.TextBox[] fields = { txtb_NombreProducto, txtb_CodigoProducto, txtb_Descripcion, txtb_PrecioCompra,
                txtb_PrecioVenta, txtb_Categoria, txtb_Cantidad };
            System.Windows.Controls.TextBox[] numericFields = { txtb_PrecioCompra, txtb_PrecioVenta, txtb_Cantidad };

           
            foreach (System.Windows.Controls.TextBox field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.Text))
                {
                  
                    ShowCustomMessage("Completar datos faltantes", DialogType.Warning);
                    return false;
                }
            }
            foreach (System.Windows.Controls.TextBox field in numericFields)
            {
                if (!EsNumero(field.Text) || img_Photo.Source == null)
                {
                    
                    ShowCustomMessage("Existen datos invalidos", DialogType.Warning);
                    return false;
                }
            }
            if (!ValidateProductName())
            {
                ShowCustomMessage("Existen datos invalidos", DialogType.Warning);

                return false;
            }
            return true;
        }
        private bool EsNumero(string texto)
        {
            return double.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }
        public byte[] ConvertImage(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            return null;

        }
        private bool ValidateProductName()
        {
           
            if (txtb_NombreProducto.Text == GetOldProductName())
                return true;

            IProductsManager proxy = new ProductsManagerClient();

            try
            {
                bool exist = proxy.ValidateProductName(txtb_NombreProducto.Text);
                return !exist; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
                ShowCustomMessage("Error en el registro", DialogType.Error);
                return false;
            }
        }


        private void Register(object sender, RoutedEventArgs e)
        {

            var dialog = new CustomDialog("¿Estás seguro que deseas cancelar la edición?", CustomDialog.DialogType.Confirmation);
            dialog.ShowDialog();
            if (dialog.UserConfirmed == true)
            {
                NavigationService?.Navigate(new RegisterProductView());
            }
        }

        private void Consult(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomDialog("¿Estás seguro que deseas cancelar la edición?", CustomDialog.DialogType.Confirmation);
            dialog.ShowDialog();
            if (dialog.UserConfirmed == true)
            {
                NavigationService?.Navigate(new ConsultProductsView());
            }
           
            
        }

        private void Validate(object sender, RoutedEventArgs e)
        {

        }

        private void UploadPhoto(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Seleccionar una imagen",
                Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string rutaImagen = openFileDialog.FileName;
                ShowImage(rutaImagen);
            }
        }
        private void ShowImage(string ruta)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(ruta);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                img_Photo.Source = bitmap;
            }
            catch (Exception ex)
            {
                ShowCustomMessage("Error al cargar la imagen:", DialogType.Error);
            }
        }

        private void UploadProduct(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                IProductsManager proxy = new ProductsManagerClient();

                try
                {
                    MilyUnaNochesProxy.Product producto = new MilyUnaNochesProxy.Product
                    {
                        NombreProducto = txtb_NombreProducto.Text,
                        CodigoProducto = txtb_CodigoProducto.Text,
                        Descripcion = txtb_Descripcion.Text,
                        PrecioCompra = decimal.Parse(txtb_PrecioCompra.Text),
                        PrecioVenta = decimal.Parse(txtb_PrecioVenta.Text),
                        Categoria = txtb_Categoria.Text,
                        Cantidad = int.Parse(txtb_Cantidad.Text),
                        Imagen = ConvertImage(img_Photo.Source)
                    };

                    if (proxy.UpdateProduct(producto, GetOldProductName()))
                    {
                        
                        ShowCustomMessage("Cambio realizado con éxito", DialogType.Success);
                        NavigationService?.Navigate(new ConsultProductsView());
                    }
                    else
                    {
                        
                        ShowCustomMessage("Hubo un error al actualizar la información del producto", DialogType.Error);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                    ShowCustomMessage("Error en el registro", DialogType.Error);
                }
            }

        }
    }
}
