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
using static MilyUnaNochesWPFApp.Views.CustomDialog;

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
        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
        public async void LoadProviders() {
            try {
                var providersList = await _providerManager.GetProvidersAsync();
                Providers = new ObservableCollection<Provider>(providersList);
                ProviderDataGrid.ItemsSource = Providers;
            } catch (Exception ex) {
                ShowCustomMessage($"Error al cargar proveedores: {ex.Message}", DialogType.Warning);
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
                var dialog = new CustomDialog($"Estás seguro de que deseas eliminar este proveedor: {selectedProvider.providerName}", CustomDialog.DialogType.Confirmation);
                dialog.ShowDialog();
                if (dialog.UserConfirmed == true){
                    var deleteResult = _providerManager.DeleteProvider(selectedProvider.IdProvider);
                    if (deleteResult == Constants.SuccessOperation) {
                        ShowCustomMessage("Proveedor eliminado con exito", DialogType.Success);
                        LoadProviders();
                    } else {
                        ShowCustomMessage("No se pudo eliminar el proveedor", DialogType.Error);
                    }
                }
            } else {
                ShowCustomMessage("Debe seleccionar un proveedor primero", DialogType.Warning);
            }
        }

        private void Archive_Click(object sender, RoutedEventArgs e) {
            if (ProviderDataGrid.SelectedItem is Provider selectedProvider) {
                    var dialog = new CustomDialog($"¿Desea archivar el proveedor: {selectedProvider.providerName}", CustomDialog.DialogType.Confirmation);
                    dialog.ShowDialog();
                    if (dialog.UserConfirmed == true)
                    {
                        var resultQuery = _providerManager.ArchiveProvider(selectedProvider.IdProvider);
                    if (resultQuery == Constants.SuccessOperation) {
                        ShowCustomMessage("Se ha archivado el proveedor", DialogType.Success);
                        LoadProviders();
                    } else {
                        ShowCustomMessage("No se pudo archivar el proveedor", DialogType.Warning);
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
