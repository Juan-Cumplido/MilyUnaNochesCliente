using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using MilyUnaNochesWPFApp.Utilities;
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
using System.Windows.Shapes;

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para ArchivedSuppliersWindow.xaml
    /// </summary>
    public partial class ArchivedSuppliersWindow : Window {
        public ObservableCollection<Provider> Providers { get; set; }
        IProviderManager _providerManager = new ProviderManagerClient();
        private FindProvider _parentPage;
        public ArchivedSuppliersWindow(FindProvider parentPage) {
            InitializeComponent();
            _parentPage = parentPage;
            LoadArchivedProviders();
        }

        private async void LoadArchivedProviders() {
            try {
                var providersList = _providerManager.GetArchivedProviders();
                Providers = new ObservableCollection<Provider>(providersList);
                ProviderDataGrid.ItemsSource = Providers;
            } catch (System.Exception ex) {
                MessageBox.Show($"Error al cargar proveedores: {ex.Message}");
            }
        }

        private void txtb_SearchBar_TextChanged(object sender, TextChangedEventArgs e) {
            txtb_PlaceholderText.Visibility = string.IsNullOrWhiteSpace(txtb_SearchBar.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            var filteredProviders = Providers.Where(p =>
                p.providerName.ToLower().Contains(txtb_SearchBar.Text.ToLower()) ||
                p.providerContact.ToLower().Contains(txtb_SearchBar.Text.ToLower())).ToList();

            ProviderDataGrid.ItemsSource = filteredProviders;
        }

        private void Unarchive_Click(object sender, RoutedEventArgs e) {
            if (ProviderDataGrid.SelectedItem is Provider selectedProvider) {
                var result = _providerManager.UnArchiveProvider(selectedProvider.IdProvider);
                if (result == Constants.SuccessOperation) {
                    DialogManager.ShowSuccessMessageAlert("Proveedor desarchivado con éxito.");
                    Providers.Remove(selectedProvider);
                    _parentPage.LoadProviders();
                } else {
                    DialogManager.ShowErrorMessageAlert("No se pudo archivar el proveedor.");
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
