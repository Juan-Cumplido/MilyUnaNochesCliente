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
        private RegisterPurchase_sv _currentPurchase;
        public string SelectedPayMethod { get; set; }


        public RegisterPurchase() {
            InitializeComponent();
            _providerClient = new ProviderManagerClient();
            _addressClient = new AdressManagerClient();
            InitializeRegisterPurchase();
            LoadProvidersAsync();
        }

        public void InitializeRegisterPurchase() {
            _currentPurchase = new RegisterPurchase_sv() {
                Fecha = DateTime.Now.Date,
                Hora = DateTime.Now.TimeOfDay,
                Products = new ProductPurchase[0]
            };
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

        private async void Register_Click(object sender, RoutedEventArgs e) {
            try {

                if (!validateFields()) {
                    return;
                }

                _currentPurchase.IdProveedor = (int)txtProviderName.SelectedValue;

                _currentPurchase.MontoTotal = _currentPurchase.Products.Sum(p => p.MontoProducto);

                _currentPurchase.Fecha = DateTime.Now.Date;
                _currentPurchase.Hora = DateTime.Now.TimeOfDay;
                _currentPurchase.ContactoProveedor = txtContact.Text;
                

                _currentPurchase.PayMethod = SelectedPayMethod;
                var purchaseClient = new PurchaseManagerClient();
                int result = await purchaseClient.SavePurchaseAsync(_currentPurchase);

                if (result > 0) {
                    DialogManager.ShowSuccessMessageAlert("Compra registrada exitosamente", "Compra registrada");
                    ClearFields();
                    InitializeRegisterPurchase();
                    SelectedPayMethod = "";
                } else {
                    DialogManager.ShowErrorMessageAlert("No se pudo registrar la compra");
                }
            } catch (Exception ex) {
                DialogManager.ShowErrorMessageAlert("No se pudo registrar la compra. Intente mas tarde");
            }
        }

        private bool validateFields() {
            if (txtProviderName.SelectedValue == null) {
                DialogManager.ShowWarningMessageAlert("Seleccione un proveedor válido.");
                return false;
            }

            // Validar que se hayan agregado productos
            if (_currentPurchase.Products == null || _currentPurchase.Products.Length == 0) {
                DialogManager.ShowWarningMessageAlert("Debe agregar al menos un producto");
                return false;
            }
            return true;
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

        public void AddPurchasedProducts(List<ProductPurchase> products) {
            _currentPurchase.Products = products.ToArray();

            _currentPurchase.MontoTotal = products.Sum(p => p.MontoProducto);
        }
        private void ClearFields() {
            txtProviderName.SelectedIndex = -1;
            SelectedPayMethod = string.Empty;
            txtContact.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtStreet.Text = string.Empty;
            txtNumber.Text = string.Empty;
            txtPostalCode.Text = string.Empty;
            txtCity.Text = string.Empty;
        }

        private void OpenAddProductsView_Click(object sender, RoutedEventArgs e) {
            var selectionWindow = new PurchaseProductSelectionWindow(this);

            selectionWindow.LoadExistingProducts(_currentPurchase.Products?.ToList() ?? new List<ProductPurchase>());

            makeThisOwnerWindow(selectionWindow);
        }

        private void makeThisOwnerWindow(Window windowToOwn) {
            var mainWindow = Window.GetWindow(this);
            windowToOwn.Owner = mainWindow;
            windowToOwn.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            windowToOwn.ShowDialog();
        }
    }
}
