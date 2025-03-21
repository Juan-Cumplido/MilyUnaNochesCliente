﻿using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
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
                var providersList = _providerManager.GetProviders();
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
                MessageBoxResult result = MessageBox.Show(
                    $"Estás seguro de que deseas eliminar este proveedor: {selectedProvider.providerName}?", 
                    "Confirmar eliminación", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question 
                );

                if (result == MessageBoxResult.Yes) {
                    var deleteResult = _providerManager.DeleteProvider(selectedProvider.IdProvider);
                    if (deleteResult == Constants.SuccessOperation) {
                        MessageBox.Show("Proveedor eliminado con éxito.");
                        Providers.Remove(selectedProvider); 
                    } else {
                        MessageBox.Show("No se pudo eliminar el proveedor.");
                    }
                }
            } else {
                MessageBox.Show("Seleccione un proveedor primero");
            }
        }

        private void Archive_Click(object sender, RoutedEventArgs e) {
            if (ProviderDataGrid.SelectedItem is Provider selectedProvider) {
                    MessageBoxResult result = MessageBox.Show(
                    $"Estás seguro de que deseas archivar el proveedor: {selectedProvider.providerName}?", 
                    "Confirmar archivar", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question 
                );
                if (result == MessageBoxResult.Yes) {
                    var resultQuery = _providerManager.ArchiveProvider(selectedProvider.IdProvider);
                    if (resultQuery == Constants.SuccessOperation) {
                        MessageBox.Show("Proveedor archivado con éxito.");
                        Providers.Remove(selectedProvider);
                    } else {
                        MessageBox.Show("No se pudo archivar el proveedor.");
                    }
                }

            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e) {
            if (ProviderDataGrid.SelectedItem is Provider selectedProvider) {
                var editWindow = new EditProvider(selectedProvider);
                bool? result = editWindow.ShowDialog();

                if (result == true) {
                    MessageBox.Show("Proveedor actualizado.", "Editar Proveedor", MessageBoxButton.OK, MessageBoxImage.Information);
                    ProviderDataGrid.Items.Refresh();
                }
            } else {
                MessageBox.Show("Seleccione un proveedor primero", "Editar Proveedor", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShowArchived_Click(object sender, RoutedEventArgs e) {
            var mainWindow = Window.GetWindow(this);
            var archivedSuppliersWindow = new ArchivedSuppliersWindow(this);
            archivedSuppliersWindow.Owner = mainWindow;
            archivedSuppliersWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            archivedSuppliersWindow.ShowDialog();
        }
    }
 }
