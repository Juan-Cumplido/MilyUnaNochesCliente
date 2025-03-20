using Microsoft.Win32;
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
    /// <summary>
    /// Lógica de interacción para CashierMenu.xaml
    /// </summary>
    public partial class CashierMenu : Page
    {
        public CashierMenu()
        {
            InitializeComponent();
            MainFrame.Source = new Uri("ConsultClient.xaml", UriKind.Relative);
        }

        private void Lbl_RegisterClient_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Source = new System.Uri("RegisterClient.xaml", System.UriKind.Relative);
        }

        private void Lbl_ConsultClient_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Source = new System.Uri("ConsultClient.xaml", System.UriKind.Relative);
        }

        private void Lbl_Sale_Click(object sender, MouseButtonEventArgs e)
        {
            MenuSale menuSale = new MenuSale();
            this.NavigationService.Navigate(menuSale);
        }
        

    }
}
