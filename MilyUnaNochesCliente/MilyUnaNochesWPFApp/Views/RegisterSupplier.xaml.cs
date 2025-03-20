using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using MilyUnaNochesWPFApp.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

    public partial class RegisterProvider : Page {

        IAdressManager _adressManager = new AdressManagerClient();
        IProviderManager _providerManager = new ProviderManagerClient();
        public RegisterProvider() {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            if (NavigationService != null && NavigationService.CanGoBack) {
                NavigationService.GoBack();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e) {
            if (!ValidateForm()) {
                return;
            }

            if (IsProviderRegistered(txtProviderName.Text) == Constants.DataMatches) {
                DialogManager.ShowWarningMessageAlert("Ya existe un proveedor registrado con esta informacion", "Proveedor ya registrado");
                return;
            } else if (IsProviderRegistered(txtProviderName.Text) == Constants.ErrorOperation) {
                DialogManager.ShowWarningMessageAlert("Ha ocurrido un error al intentar establecer conexión con la base de datos", "Error de conexion");
                return;
            }

            int addressId = CreateAddressFromForm();
            int providerResult = CreateProviderFromForm(addressId);
            if (providerResult == Constants.SuccessOperation) {
                DialogManager.ShowSuccessMessageAlert("Registro realizado con exito");
                ClearFields();
            } else {
                DialogManager.ShowErrorMessageAlert("Ha ocurrido un error al intentar establecer conexión con la base de datos.");
            }

        }

        private int CreateAddressFromForm() {
            var address = new Address {
                Calle = txtStreet.Text,
                Numero = txtNumber.Text,
                CodigoPostal = txtPostalCode.Text,
                Ciudad = txtCity.Text
            };
            return _adressManager.createAddress(address);
        }

        private int CreateProviderFromForm(int addressId) {
            var provider = new Provider {
                providerName = txtProviderName.Text,
                providerContact = txtContact.Text,
                phoneNumber = txtPhone.Text,
                email = txtEmail.Text,
                idAddress = addressId,
            };
            return _providerManager.CreateProvider(provider);
        }
        private void ClearFields() {
            txtProviderName.Text = string.Empty;
            txtContact.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtStreet.Text = string.Empty;
            txtNumber.Text = string.Empty;
            txtPostalCode.Text = string.Empty;
            txtCity.Text = string.Empty;
        }

        private bool ValidateForm() {
            if (!VerifyNoEmptyFields()) {
                DialogManager.ShowWarningMessageAlert("Los campos no deben estar vacíos.", "Validación");
                return false;
            }

            if (!Validator.ValidateProviderName(txtProviderName.Text)) {
                DialogManager.ShowWarningMessageAlert("El nombre del proveedor no es válido.", "Validación");
                return false;
            }
            if (!Validator.ValidateContact(txtContact.Text)) {
                DialogManager.ShowWarningMessageAlert("El contacto no es válido.", "Validación");
                return false;
            }
            if (!Validator.ValidatePhone(txtPhone.Text)) {
                DialogManager.ShowWarningMessageAlert("El teléfono no es válido.", "Validación");
                return false;
            }
            if (!Validator.ValidateEmail(txtEmail.Text)) {
                DialogManager.ShowWarningMessageAlert("El correo electrónico no es válido.", "Validación");
                return false;
            }
            if (!Validator.ValidateStreet(txtStreet.Text)) {
                DialogManager.ShowWarningMessageAlert("La calle no es válida.", "Validación");
                return false;
            }
            if (!Validator.ValidateNumber(txtNumber.Text)) {
                DialogManager.ShowWarningMessageAlert("El número no es válido.", "Validación");
                return false;
            }
            if (!Validator.ValidatePostalCode(txtPostalCode.Text)) {
                MessageBox.Show("El código postal no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Validator.ValidateCity(txtCity.Text)) {
                DialogManager.ShowWarningMessageAlert("La ciudad no es válida.", "Validación");
                return false;
            }
            return true;
        }

        private int IsProviderRegistered(string providerName) {
            int verificationResult = _providerManager.VerifyProviderExistance(providerName);
            return verificationResult;
        }

        private bool VerifyNoEmptyFields() {
            bool result = true;
            if (string.IsNullOrWhiteSpace(txtProviderName.Text) ||
                  string.IsNullOrWhiteSpace(txtContact.Text) ||
                  string.IsNullOrWhiteSpace(txtPhone.Text) ||
                  string.IsNullOrWhiteSpace(txtEmail.Text) ||
                  string.IsNullOrWhiteSpace(txtStreet.Text) ||
                  string.IsNullOrWhiteSpace(txtNumber.Text) ||
                  string.IsNullOrWhiteSpace(txtPostalCode.Text) ||
                  string.IsNullOrWhiteSpace(txtCity.Text)) {
                result = false;
            }
            return result;
        }

    }
}
