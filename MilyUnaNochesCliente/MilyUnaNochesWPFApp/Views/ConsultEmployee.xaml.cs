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

namespace MilyUnaNochesWPFApp.Views
{
    public partial class ConsultEmployee : Page
    {
        public ConsultEmployee()
        {
            InitializeComponent();
        }

        private void SearchForClient_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtb_searchForClient.Text == "Nombre o contacto (ej. Juan)")
            {
                txtb_searchForClient.Text = "";
                txtb_searchForClient.Foreground = Brushes.Black;
            }
        }

        private void SearchForClient_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtb_searchForClient.Text))
            {
                txtb_searchForClient.Text = "Nombre o contacto (ej. Juan)";
                txtb_searchForClient.Foreground = Brushes.Gray;
            }
        }


        private void ProviderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    


   
}
