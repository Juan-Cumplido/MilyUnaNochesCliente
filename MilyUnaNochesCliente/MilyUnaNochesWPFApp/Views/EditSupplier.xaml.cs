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
using System.Windows.Shapes;

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para EditProvider.xaml
    /// </summary>
    public partial class EditProvider : Window {
        public Provider ProviderToEdit { get; private set; }
        IProviderManager _providerManager = new ProviderManagerClient();
        public EditProvider(Provider provider) {
            InitializeComponent();
            ProviderToEdit = provider;
            LoadProviderData();
        }

        private void LoadProviderData() {
            if (ProviderToEdit != null) {
                txtProviderName.Text = ProviderToEdit.providerName;
                txtContact.Text = ProviderToEdit.providerContact;
                txtPhone.Text = ProviderToEdit.phoneNumber;
                txtEmail.Text = ProviderToEdit.email;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            //Validacion de campos

            // Actualizar la información del proveedor
            ProviderToEdit.providerName = txtProviderName.Text;
            ProviderToEdit.providerContact = txtContact.Text;
            ProviderToEdit.phoneNumber = txtPhone.Text;
            ProviderToEdit.email = txtEmail.Text;

            //_providerManager.EditProvider(ProviderToEdit);

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
