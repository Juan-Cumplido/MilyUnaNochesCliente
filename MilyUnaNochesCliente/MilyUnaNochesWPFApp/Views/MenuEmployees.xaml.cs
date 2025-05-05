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

    public partial class MenuEmployees : Page
    {
        public MenuEmployees()
        {
            InitializeComponent();
            Frame.Source = new Uri("ConsultEmployee.xaml", UriKind.Relative);
        }

        private void Lbl_RegisterEmploye_Click(object sender, MouseButtonEventArgs e)
        {
            Frame.Source = new System.Uri("RegisterEmployee.xaml", System.UriKind.Relative);
        }

        private void Lbl_ConsultEmploye_Click(object sender, MouseButtonEventArgs e)
        {
            Frame.Source = new System.Uri("ConsultEmployee.xaml", System.UriKind.Relative);
        }

        private void Image_MouseDownGoBack(object sender, MouseButtonEventArgs e)
        {
            ManagerMenu managerMenu = new ManagerMenu();
            this.NavigationService.Navigate(managerMenu);
        }
    }
}
