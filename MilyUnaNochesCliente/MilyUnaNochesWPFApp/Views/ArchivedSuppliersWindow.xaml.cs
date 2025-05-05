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
using static MilyUnaNochesWPFApp.Views.CustomDialog;

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para ArchivedSuppliersWindow.xaml
    /// </summary>
    public partial class ArchivedSuppliersWindow : Window {
        public ObservableCollection<Provider> Providers { get; set; }
        IProviderManager _providerManager = new ProviderManagerClient();
        private FindProvider parentPage;
        public ArchivedSuppliersWindow(FindProvider parentPage) {
            InitializeComponent();
            this.parentPage = parentPage;
            LoadArchivedProviders();
        }
        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private async void LoadArchivedProviders() {
            try {
                var providersList = await _providerManager.GetArchivedProvidersAsync();
                Providers = new ObservableCollection<Provider>(providersList);
                ProviderDataGrid.ItemsSource = Providers;
            } catch (System.Exception ex) {
                ShowCustomMessage($"Error al cargar proveedores: { ex.Message}", DialogType.Error);
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
                    ShowCustomMessage("Proveedor desarchivado con éxito.", DialogType.Success);

                    Providers.Remove(selectedProvider);
                    parentPage.LoadProviders();
                } else {
                    ShowCustomMessage("No se pudo archivar el proveedor.", DialogType.Error);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
