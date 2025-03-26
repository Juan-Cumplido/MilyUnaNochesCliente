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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para FindProvider.xaml
    /// </summary>
    public partial class FindProvider : Page {
        public ObservableCollection<Provider> Providers { get; set; }
        IProviderManager _providerManager = new ProviderManagerClient();

        public FindProvider() {
            InitializeComponent();
            LoadProviders();
        }

        public async void LoadProviders() {
            try {
                var providersList = await _providerManager.GetProvidersAsync();
                Providers = new ObservableCollection<Provider>(providersList);
                ProviderDataGrid.ItemsSource = Providers;
            } catch (Exception ex) {
                MessageBox.Show($"Error al cargar proveedores: {ex.Message}");
            }
        }
        private void txtb_SearchBar_TextChanged(object sender, TextChangedEventArgs e) {
            txtb_PlaceholderText.Visibility = string.IsNullOrWhiteSpace(txtb_SearchBar.Text) ? Visibility.Visible : Visibility.Collapsed;

            var filteredProviders = Providers.Where(p =>
            p.providerName.ToLower().Contains(txtb_SearchBar.Text.ToLower()) ||
            p.providerContact.ToLower().Contains(txtb_SearchBar.Text.ToLower())).ToList();

            ProviderDataGrid.ItemsSource = filteredProviders;
        }

        private void Delete_Click(object sender, RoutedEventArgs e) {

            if (ProviderDataGrid.SelectedItem is Provider selectedProvider) {

                bool messageResult =
                DialogManager.ShowConfirmationMessageAlert($"Estás seguro de que deseas eliminar este proveedor: {selectedProvider.providerName}", 
                "Archivar proveedor");

                if (messageResult) {
                    var deleteResult = _providerManager.DeleteProvider(selectedProvider.IdProvider);
                    if (deleteResult == Constants.SuccessOperation) {
                        DialogManager.ShowConfirmationMessageAlert("Proveedor eliminado con exito", "Eliminacion exitosa");
                        LoadProviders();
                    } else {
                        DialogManager.ShowErrorMessageAlert("No se pudo eliminar el proveedor", "Eliminacion fallida");
                    }
                }
            } else {
                DialogManager.ShowWarningMessageAlert("Debe seleccionar un proveedor primero", "Seleccione un proveedor");
            }
        }

        private void Archive_Click(object sender, RoutedEventArgs e) {
            if (ProviderDataGrid.SelectedItem is Provider selectedProvider) {
                bool result = DialogManager.ShowConfirmationMessageAlert($"¿Desea archivar el proveedor: {selectedProvider.providerName}", "Archivar proveedor");
                if (result) {
                    var resultQuery = _providerManager.ArchiveProvider(selectedProvider.IdProvider);
                    if (resultQuery == Constants.SuccessOperation) {
                        DialogManager.ShowSuccessMessageAlert("Se ha archivado el proveedor", "Proveedor archivado");
                        LoadProviders();
                    } else {
                        DialogManager.ShowErrorMessageAlert("No se pudo archivar el proveedor", "Error al archivar el proveedor");
                    }
                }

            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e) {
            if (ProviderDataGrid.SelectedItem is Provider selectedProvider) {
                int idSupplier = selectedProvider.IdProvider;
                int idAddress = selectedProvider.idAddress;
                
                var mainWindow = Window.GetWindow(this);
                var editWindow = new EditSupplierWindow(idSupplier, idAddress, this);
                makeThisOwnerWindow(editWindow);

            }
        }

        private void ShowArchived_Click(object sender, RoutedEventArgs e) {
            var mainWindow = Window.GetWindow(this);
            var archivedSuppliersWindow = new ArchivedSuppliersWindow(this);
            makeThisOwnerWindow(archivedSuppliersWindow);
        }

        private void makeThisOwnerWindow(Window windowToOwn) {
            var mainWindow = Window.GetWindow(this);
            windowToOwn.Owner = mainWindow;
            windowToOwn.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            windowToOwn.ShowDialog();
        }
    }
 }
