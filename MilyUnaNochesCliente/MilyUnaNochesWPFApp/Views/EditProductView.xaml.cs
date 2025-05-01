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

            // Cargar los detalles del producto en los controles
            txtb_NombreProducto.Text = producto.NombreProducto;
            txtb_CodigoProducto.Text = producto.CodigoProducto;
            txtb_Descripcion.Text = producto.Descripcion;
            txtb_Categoria.Text = producto.Categoria;
            txtb_Cantidad.Text = producto.Cantidad.ToString();
            txtb_PrecioVenta.Text = producto.PrecioVenta.ToString(); 
            txtb_PrecioCompra.Text = producto.PrecioCompra.ToString(); 

            // Convertir la imagen de byte[] a BitmapImage
            if (producto.Imagen != null && producto.Imagen.Length > 0)
            {
                img_Photo.Source = ConvertImage(producto.Imagen);
            }

            SetOldProductName(producto.NombreProducto);
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

            //Validar que no hayan campos vacíos
            foreach (System.Windows.Controls.TextBox field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.Text))
                {
                    System.Windows.MessageBox.Show($"Completar datos faltantes",
                                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            //Validar que los campos numericos y la foto se cumplan
            foreach (System.Windows.Controls.TextBox field in numericFields)
            {
                if (!EsNumero(field.Text) || img_Photo.Source == null)
                {
                    System.Windows.MessageBox.Show($"Existen datos invalidos",
                                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            //Validar que el nombre del producto no exista en un registro previo
            if (!ValidateProductName())
            {
                System.Windows.MessageBox.Show($"Existen datos invalidos",
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
                System.Windows.MessageBox.Show($"Error en el registro",
                                "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show(
                "¿Estás seguro que deseas cancelar la edición?",
                "Confirmar",
                 MessageBoxButtons.YesNo, // Botones Sí/No (o OK/Cancel)
                 MessageBoxIcon.Question // Icono de pregunta
            );

            if (result == DialogResult.Yes)
            {
                NavigationService?.Navigate(new RegisterProductView());
            }
        }

        private void Consult(object sender, RoutedEventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show(
                "¿Estás seguro que deseas cancelar la edición?",
                "Confirmar",
                 MessageBoxButtons.YesNo, // Botones Sí/No (o OK/Cancel)
                 MessageBoxIcon.Question // Icono de pregunta
            );

            if (result == DialogResult.Yes)
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
                System.Windows.MessageBox.Show("Error al cargar la imagen: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        System.Windows.MessageBox.Show($"Cambio realizado con éxito",
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService?.Navigate(new ConsultProductsView());
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Hubo un error al actualizar la información del producto",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                    System.Windows.MessageBox.Show($"Error en el registro",
                                    "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }
    }
}
