using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para ConsultPurchase.xaml
    /// </summary>
    public partial class ConsultPurchase : Page {
        public ObservableCollection<ConsultPurchase_SV> Purchases { get; set; }
        IPurchaseManager _purchaseManager = new PurchaseManagerClient();
        public ConsultPurchase() {
            InitializeComponent();
            LoadPurchases();
            this.DataContext = this;
        }

        public async void LoadPurchases() {
            try {
                var purchasesList = await _purchaseManager.GetPurchasesAsync();
                Purchases = new ObservableCollection<ConsultPurchase_SV>(purchasesList);
                PurchasesDataGrid.ItemsSource = Purchases;
            } catch (Exception ex) {
                DialogManager.ShowErrorMessageAlert("Error al cargar proveedores");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) {
            if (NavigationService != null && NavigationService.CanGoBack) {
                NavigationService.GoBack();
            }
        }

        private void txtb_SearchBar_TextChanged(object sender, TextChangedEventArgs e) {
            txtb_PlaceholderText.Visibility = string.IsNullOrWhiteSpace(txtb_SearchBar.Text)
                                            ? Visibility.Visible
                                            : Visibility.Collapsed;

            var filteredPurchases = Purchases.Where(p => p.providerName.ToLower().Contains(txtb_SearchBar.Text.ToLower()) ||
                p.providerContact.ToLower().Contains(txtb_SearchBar.Text.ToLower())).ToList();
            PurchasesDataGrid.ItemsSource = filteredPurchases;
        }
    }
}
