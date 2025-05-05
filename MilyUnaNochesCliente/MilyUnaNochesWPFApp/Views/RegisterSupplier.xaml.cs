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
using static MilyUnaNochesWPFApp.Views.CustomDialog;

namespace MilyUnaNochesWPFApp.Views {

    public partial class RegisterProvider : Page {

        IAdressManager _adressManager = new AdressManagerClient();
        IProviderManager _providerManager = new ProviderManagerClient();
        public RegisterProvider() {
            InitializeComponent();
        }
        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            if (NavigationService != null && NavigationService.CanGoBack) {
                NavigationService.GoBack();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            if (IsProviderRegistered(txtProviderName.Text) == Constants.DataMatches)
            {
                ShowCustomMessage("Ya existe un proveedor registrado con esta información", DialogType.Warning);
                return;
            }
            else if (IsProviderRegistered(txtProviderName.Text) == Constants.ErrorOperation)
            {
                ShowCustomMessage("Ha ocurrido un error al intentar establecer conexión con la base de datos", DialogType.Error);
                return;
            }

            int addressId = CreateAddressFromForm();
            int providerResult = CreateProviderFromForm(addressId);
            if (providerResult == Constants.SuccessOperation)
            {
                ShowCustomMessage("Registro realizado con éxito", DialogType.Success);
                ClearFields();
            }
            else
            {
                ShowCustomMessage("Ha ocurrido un error al intentar establecer conexión con la base de datos.", DialogType.Error);
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

        private bool ValidateForm()
        {
            if (!VerifyNoEmptyFields())
            {
                ShowCustomMessage("Los campos no deben estar vacíos.", DialogType.Warning);
                return false;
            }

            if (!Validator.ValidateProviderName(txtProviderName.Text))
            {
                ShowCustomMessage("El nombre del proveedor no es válido.", DialogType.Warning);
                return false;
            }
            if (!Validator.ValidateContact(txtContact.Text))
            {
                ShowCustomMessage("El contacto no es válido.", DialogType.Warning);
                return false;
            }
            if (!Validator.ValidatePhone(txtPhone.Text))
            {
                ShowCustomMessage("El teléfono no es válido.", DialogType.Warning);
                return false;
            }
            if (!Validator.ValidateEmail(txtEmail.Text))
            {
                ShowCustomMessage("El correo electrónico no es válido.", DialogType.Warning);
                return false;
            }
            if (!Validator.ValidateStreet(txtStreet.Text))
            {
                ShowCustomMessage("La calle no es válida.", DialogType.Warning);
                return false;
            }
            if (!Validator.ValidateNumber(txtNumber.Text))
            {
                ShowCustomMessage("El número no es válido.", DialogType.Warning);
                return false;
            }
            if (!Validator.ValidatePostalCode(txtPostalCode.Text))
            {
                ShowCustomMessage("El código postal no es válido.", DialogType.Warning);
                return false;
            }
            if (!Validator.ValidateCity(txtCity.Text))
            {
                ShowCustomMessage("La ciudad no es válida.", DialogType.Warning);
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
