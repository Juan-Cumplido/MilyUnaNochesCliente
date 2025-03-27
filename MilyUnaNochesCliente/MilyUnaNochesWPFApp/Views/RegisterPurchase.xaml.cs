using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
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

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para RegisterPurchase.xaml
    /// </summary>
    public partial class RegisterPurchase : Page {
        private readonly IProviderManager _providerClient;
        private readonly IAdressManager _addressClient;
        public RegisterPurchase() {
            InitializeComponent();
            _providerClient = new ProviderManagerClient();
            _addressClient = new AdressManagerClient();
            LoadProvidersAsync();
        }
        private async void LoadProvidersAsync() {
            try {
                Provider[] providers = await _providerClient.GetProvidersAsync();

                txtProviderName.ItemsSource = providers.ToList();
                txtProviderName.DisplayMemberPath = "providerName";
                txtProviderName.SelectedValuePath = "IdProvider";
            } catch (Exception ex) {
                DialogManager.ShowErrorMessageAlert("Ha ocurrido un error intentando cargar los proveedores");
            }
        }

        private Provider GetSelectedProvider() {
            if (txtProviderName.SelectedItem is Provider selectedProvider) {
                return selectedProvider;
            }
            return null;
        }

        private void Register_Click(object sender, RoutedEventArgs e) {
            
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {

        }

        private void txtProviderName_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selectedProvider = GetSelectedProvider();

            if (selectedProvider != null) {
                txtContact.Text = selectedProvider.providerContact;
                txtPhone.Text = selectedProvider.phoneNumber;
                txtEmail.Text = selectedProvider.email;

                try {
                    Address address = _addressClient.GetAddress(selectedProvider.idAddress);
                    if (address != null) {
                        txtStreet.Text = address.Calle;
                        txtNumber.Text = address.Numero;
                        txtPostalCode.Text = address.CodigoPostal;
                        txtCity.Text = address.Ciudad;
                    } else {
                        txtStreet.Text = "Indefinido";
                        txtNumber.Text = "Indefinido";
                        txtPostalCode.Text = "Indefinido";
                        txtCity.Text = "Indefinido";
                    }
                } catch (Exception ex) {
                    DialogManager.ShowErrorMessageAlert("Ha ocurrido un error al obtener la dirección del proveedor.");
                }
            } else {
                ClearFields();
            }
        }

        private async void LoadProviderDetails(Provider provider) {
            try {
                txtContact.Text = provider.providerContact;
                txtPhone.Text = provider.phoneNumber;
                txtEmail.Text = provider.email;

                var address = await _addressClient.GetAddressAsync(provider.idAddress);

                if (address != null) {
                    txtStreet.Text = address.Calle;
                    txtNumber.Text = address.Numero;
                    txtPostalCode.Text = address.CodigoPostal;
                    txtCity.Text = address.Ciudad;
                }
            } catch (Exception ex) {
                DialogManager.ShowErrorMessageAlert($"Error cargando detalles: {ex.Message}");
            }
        }
        private void ClearFields() {
            txtContact.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtStreet.Text = string.Empty;
            txtNumber.Text = string.Empty;
            txtPostalCode.Text = string.Empty;
            txtCity.Text = string.Empty;
        }

        private void OpenAddProductsView_Click(object sender, RoutedEventArgs e) {
            var PurchaseProductSelectionWindow = new PurchaseProductSelectionWindow(this);
            makeThisOwnerWindow(PurchaseProductSelectionWindow);
        }

        private void makeThisOwnerWindow(Window windowToOwn) {
            var mainWindow = Window.GetWindow(this);
            windowToOwn.Owner = mainWindow;
            windowToOwn.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            windowToOwn.ShowDialog();
        }
    }
}
