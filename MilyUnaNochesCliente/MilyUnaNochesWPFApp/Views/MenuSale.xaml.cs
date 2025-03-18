using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
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

namespace MilyUnaNochesWPFApp.Views {
    public partial class MenuSale : Page {
        public MenuSale() {
            InitializeComponent();
            Frame.Source = new Uri("ConsultSale.xaml", UriKind.Relative);
        }

        private void lbl_RegisterSale_Click(object sender, MouseButtonEventArgs e)
        {
            Frame.Source = new System.Uri("Sale.xaml", System.UriKind.Relative);
        }

        private void lbl_ConsultSale_Click(object sender, MouseButtonEventArgs e)
        {
            Frame.Source = new System.Uri("ConsultSale.xaml", System.UriKind.Relative);
        }

        private void Lbl_Client_Click(object sender, MouseButtonEventArgs e)
        {
            CashierMenu cashierMenu = new CashierMenu();
            this.NavigationService.Navigate(cashierMenu);
        }

    }
}

