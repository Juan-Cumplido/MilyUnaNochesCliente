using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using MilyUnaNochesWPFApp.Utilities;
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
using System.Windows.Shapes;

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para EditSupplierWindow.xaml
    /// </summary>
    public partial class EditSupplierWindow : Window {
        private int idSupplier;
        private int idAddress;
        private Provider originalProvider;
        private Address originalAddress;
        private FindProvider parentPage;
        IProviderManager _providerManager = new ProviderManagerClient();
        IAdressManager _addresManager = new AdressManagerClient();


        public EditSupplierWindow(int idSupplier, int idAddress, FindProvider parentPage) {
            InitializeComponent();
            this.idSupplier = idSupplier;
            this.idAddress = idAddress;
            this.parentPage = parentPage;
            loadProviderInfo();
        }

        private void loadProviderInfo() {
            var supplierInfo = _providerManager.GetSupplier(idSupplier);
            var supplierAddres = _addresManager.GetAddress(idAddress);
            if (!(checkObjectValidId(idAddress) || checkObjectValidId(idAddress))){
                DialogManager.ShowErrorMessageAlert("Ha ocurrido un error al recuperar la informacion", "Error");
                this.Close();
            }

            SaveSupplierInfoCopy(supplierInfo);
            SaveAddressInfoCopy(supplierAddres);

            loadTextFields(supplierInfo, supplierAddres);
        }
        private void Save_Click(object sender, RoutedEventArgs e) {

            if (!HasSupplierDataChanged()) {
                DialogManager.ShowWarningMessageAlert("Los datos del proveedor son iguales. No se realizaron cambios.");
                return;
            }

            Provider providerInfo = GetSupplierInfo();
            Address addressInfo = GetAddressInfo();

            if ((_providerManager.EditSupplier(providerInfo, addressInfo) == Constants.SuccessOperation)){
                DialogManager.ShowSuccessMessageAlert("El proveedor ha sido editado satisfactoriamente", "Proveedor editado con exito");
                parentPage.LoadProviders();
                this.Close();
            } else {
                DialogManager.ShowErrorMessageAlert("Ha ocurrido un error intentando editar el proveedor");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private bool checkObjectValidId(int id) {
            if (id == Constants.ErrorOperation || id == Constants.NoDataMatches) {
                return false;
            }
            return true;
        }

        private void loadTextFields(Provider provider, Address address) {
            txt_Contact.Text = provider.providerContact.ToString();
            txt_ProviderName.Text = provider.providerName.ToString();    
            txt_Email.Text = provider.email.ToString();
            txt_Phone.Text = provider.phoneNumber.ToString();
            txt_City.Text = address.Ciudad.ToString();
            txt_Street.Text = address.Calle.ToString();
            txt_PostalCode.Text = address.CodigoPostal.ToString();
            txt_Number.Text = address.Numero.ToString();
        }

        private Provider GetSupplierInfo() {
            Provider providerInfo = new Provider() {
                IdProvider = idSupplier,
                providerName = txt_ProviderName.Text,
                providerContact = txt_Contact.Text,
                phoneNumber = txt_Phone.Text,
                idAddress = this.idAddress,
                email = txt_Email.Text,
            };
            return providerInfo;
            
        }

        private Address GetAddressInfo() {
            Address addressInfo = new Address() {
                Calle = txt_Street.Text,
                Numero = txt_Number.Text,
                CodigoPostal = txt_PostalCode.Text,
                Ciudad = txt_City.Text
            };
            return addressInfo;
        }

        private void SaveSupplierInfoCopy(Provider supplierInfo) {
            originalProvider = new Provider {
                IdProvider = supplierInfo.IdProvider,
                providerName = supplierInfo.providerName,
                providerContact = supplierInfo.providerContact,
                phoneNumber = supplierInfo.phoneNumber,
                idAddress = supplierInfo.idAddress,
                email = supplierInfo.email
            };
        }

        private void SaveAddressInfoCopy(Address addresInfo) {
            originalAddress = new Address {
                Calle = addresInfo.Calle,
                Numero = addresInfo.Numero,
                CodigoPostal = addresInfo.CodigoPostal,
                Ciudad = addresInfo.Ciudad
            };
        }

        private bool HasSupplierDataChanged() {
            return !(originalProvider.providerName == txt_ProviderName.Text &&
                     originalProvider.providerContact == txt_Contact.Text &&
                     originalProvider.phoneNumber == txt_Phone.Text &&
                     originalProvider.email == txt_Email.Text &&
                     originalAddress.Calle == txt_Street.Text &&
                     originalAddress.Numero == txt_Number.Text &&
                     originalAddress.CodigoPostal == txt_PostalCode.Text &&
                     originalAddress.Ciudad == txt_City.Text);
        }

    }
}
