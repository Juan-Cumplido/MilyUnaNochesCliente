using System;
using System.Collections.Generic;
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
using MilyUnaNochesWPFApp.Logic;

namespace MilyUnaNochesWPFApp.Views
{
    public partial class ProductDetailView : Page
    {
        public ProductDetailView(Product producto)
        {
            InitializeComponent();

            // Cargar los detalles del producto en los controles
            txtNombre.Text = producto.NombreProducto;
            txtCodigo.Text = producto.CodigoProducto;
            txtDescripcion.Text = producto.Descripcion;
            txtCategoria.Text = producto.Categoria;
            txtCantidad.Text = producto.Cantidad.ToString();
            txtPrecioVenta.Text = producto.PrecioVenta.ToString("C"); // Formato de moneda
            txtPrecioCompra.Text = producto.PrecioCompra.ToString("C"); // Formato de moneda

            // Convertir la imagen de byte[] a BitmapImage
            if (producto.Imagen != null && producto.Imagen.Length > 0)
            {
                imgProducto.Source = ConvertImage(producto.Imagen);
            }
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

        private void Register(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterProductView());
        }

        private void Consult(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConsultProductsView());
        }

        private void Validate(object sender, RoutedEventArgs e)
        {

        }
    }
}
