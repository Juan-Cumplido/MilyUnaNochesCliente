using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesService;
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

            int addressId = CreateAddressFromForm();
            int providerResult = CreateProviderFromForm(addressId);

            if (providerResult > 0) {
                MessageBox.Show("Proveedor registrado exitosamente.", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
            } else {
                MessageBox.Show("Error al registrar el proveedor.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // Verificar si algún campo está vacío
            if (string.IsNullOrWhiteSpace(txtProviderName.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtStreet.Text) ||
                string.IsNullOrWhiteSpace(txtNumber.Text) ||
                string.IsNullOrWhiteSpace(txtPostalCode.Text) ||
                string.IsNullOrWhiteSpace(txtCity.Text)) {
                MessageBox.Show("Los campos no deben estar vacíos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validaciones de formato usando Regex
            if (!Validator.ValidateProviderName(txtProviderName.Text)) {
                MessageBox.Show("El nombre del proveedor no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Validator.ValidateContact(txtContact.Text)) {
                MessageBox.Show("El contacto no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Validator.ValidatePhone(txtPhone.Text)) {
                MessageBox.Show("El teléfono no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Validator.ValidateEmail(txtEmail.Text)) {
                MessageBox.Show("El correo electrónico no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Validator.ValidateStreet(txtStreet.Text)) {
                MessageBox.Show("La calle no es válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Validator.ValidateNumber(txtNumber.Text)) {
                MessageBox.Show("El número no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Validator.ValidatePostalCode(txtPostalCode.Text)) {
                MessageBox.Show("El código postal no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Validator.ValidateCity(txtCity.Text)) {
                MessageBox.Show("La ciudad no es válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

    }
}
