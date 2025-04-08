using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using System;
using System.Collections.Generic;
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
using System.Globalization;
using Microsoft.Win32;
using System.IO;


namespace MilyUnaNochesWPFApp.Views
{
    /// <summary>
    /// Lógica de interacción para RegisterProductView.xaml
    /// </summary>
    public partial class RegisterProductView : Page
    {
        public RegisterProductView()
        {
            InitializeComponent();
        }

        private bool ValidarCampos()
        {
            TextBox[] fields = { txtb_NombreProducto, txtb_CodigoProducto, txtb_Descripcion, txtb_PrecioCompra, 
                txtb_PrecioVenta, txtb_Categoria, txtb_Cantidad };
            TextBox[] numericFields = { txtb_PrecioCompra, txtb_PrecioVenta, txtb_Cantidad };

            //Validar que no hayan campos vacíos
            foreach (TextBox field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.Text))
                {
                    MessageBox.Show($"Completar datos faltantes",
                                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            //Validar que los campos numericos y la foto se cumplan
            foreach (TextBox field in numericFields)
            {
                if (!EsNumero(field.Text) || img_Photo.Source == null)
                {
                    MessageBox.Show($"Existen datos invalidos",
                                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            //Validar que el nombre del producto no exista en un registro previo
            if (!ValidateProductName())
            {
                MessageBox.Show($"Existen datos invalidos",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            IProductsManager proxy = new ProductsManagerClient();

            try
            {
                bool exist = proxy.ValidateProductName(txtb_NombreProducto.Text);
                return exist;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
                MessageBox.Show($"Error en el registro",
                                "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private void SaveProduct(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                IProductsManager proxy = new ProductsManagerClient();
                
                try
                {
                    MilyUnaNochesService.Product producto = new MilyUnaNochesService.Product
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

                    if (proxy.SaveProduct(producto))
                    {
                        MessageBox.Show($"Registro realizado con éxito",
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService?.Navigate(new ConsultProductsView());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                    MessageBox.Show($"Error en el registro",
                                    "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Register(object sender, RoutedEventArgs e)
        {
        }


        private void Consult(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConsultProductsView());
        }

        private void Validate(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void UploadPhoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar una imagen",
                Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = false 
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string rutaImagen = openFileDialog.FileName;
                ShowImage(rutaImagen);
                btn_Photo.Visibility = Visibility.Collapsed;
                btn_PhotoBorder.Visibility = Visibility.Collapsed;  
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
                MessageBox.Show("Error al cargar la imagen: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
