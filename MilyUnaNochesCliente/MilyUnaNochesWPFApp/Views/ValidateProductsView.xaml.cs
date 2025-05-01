using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

namespace MilyUnaNochesWPFApp.Views
{
    /// <summary>
    /// Lógica de interacción para ValidateProductsView.xaml
    /// </summary>
    public partial class ValidateProductsView : Window
    {
        public ValidateProductsView()
        {
            InitializeComponent();
        }

        private bool ValidarCampos()
        {
            //Validar que no hayan campos vacíos
            if (string.IsNullOrWhiteSpace(txtb_ProductNum.Text))
            {
                System.Windows.MessageBox.Show($"Completar datos faltantes",
                                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            //Validar que el campo numericos se cumpla
            if (!EsNumero(txtb_ProductNum.Text))
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

        private void Validate(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                IProductsManager proxy = new ProductsManagerClient();

                try
                {
                    int valor;
                    int.TryParse(txtb_ProductNum.Text, out valor);
                    int numProducts = proxy.NumProducts();

                    if (numProducts == valor)
                    {
                        Text.VerticalAlignment = VerticalAlignment.Center;
                        Text.HorizontalAlignment = HorizontalAlignment.Center;
                        Text.TextAlignment = TextAlignment.Center;

                        Text.Text = "El número de productos coincide";
                        productNumBorder.Visibility = Visibility.Collapsed;
                        btn_Validate.Visibility = Visibility.Collapsed;
                        btn_Ok.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Text.Text = "El número de productos no coincide";
                        productNumBorder.Visibility = Visibility.Collapsed;
                        btn_Validate.Visibility = Visibility.Collapsed;
                        btn_Ok.Visibility = Visibility.Visible;

                        InformativeText.Visibility = Visibility.Visible;
                        InformativeText2.Visibility = Visibility.Visible;

                        InformativeTextNum.Visibility = Visibility.Visible;
                        InformativeTextNum.Text += valor;
                        InformativeText2Num.Visibility = Visibility.Visible;
                        InformativeText2Num.Text += numProducts;

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

        private void Ok(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
