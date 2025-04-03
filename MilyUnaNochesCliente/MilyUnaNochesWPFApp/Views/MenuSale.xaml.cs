using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MilyUnaNochesWPFApp.Views;

namespace MilyUnaNochesWPFApp.Views {
    public partial class MenuSale : Page {
        public MenuSale() {
            InitializeComponent();
            // Cargar ConsultSale por defecto
            Frame.Navigate(new ConsultSale());
        }

        private void lbl_RegisterSale_Click(object sender, MouseButtonEventArgs e) {
            // Navegar a Sale con ID por defecto (1)
            Frame.Navigate(new Sale(1)); // Usando constructor con parámetro
        }

        private void lbl_ConsultSale_Click(object sender, MouseButtonEventArgs e) {
            Frame.Navigate(new ConsultSale());
        }

        private void Lbl_Client_Click(object sender, MouseButtonEventArgs e) {
            CashierMenu cashierMenu = new CashierMenu();
            this.NavigationService.Navigate(cashierMenu);
        }
    }
}