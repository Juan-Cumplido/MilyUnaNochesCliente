using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using MilyUnaNochesWPFApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static MilyUnaNochesWPFApp.Views.CustomDialog;

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para EditSupplierWindow.xaml
    /// </summary>
    public partial class EditClient : Window {
        private int idUsuario;
        private ConsultClient parentPage;
        private Usuario originalUser;
        IUserManager _userManager = new UserManagerClient();
  

        public EditClient(int idUsuario, ConsultClient parentPage) {
            InitializeComponent();
            this.idUsuario = idUsuario;
            this.parentPage = parentPage;
            this.Loaded += EditClient_Loaded;

           
        }
        private void EditClient_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProviderInfo();
        }

        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

       

        private void LoadProviderInfo()
        {
            var userInfo = _userManager.GetClient(idUsuario);

            if (userInfo == null || !CheckObjectValidId(userInfo.idUsuario))
            {
                ShowCustomMessage("Ha ocurrido un error al recuperar la información del cliente.", DialogType.Error);
                this.Close();
                return;
            }

            SaveClientInfoCopy(userInfo);
            LoadTextFields(userInfo);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Logic.LoggerManager logger = new Logic.LoggerManager(this.GetType());
            RestartColorTxtBox();

            if (!ValidateFields())
            {
                ShowCustomMessage("La información que ha ingresado es incorrecta. Verifique los campos.", DialogType.Warning);
                return;
            }

            if (!HasSupplierDataChanged())
            {
                ShowCustomMessage("Los datos del cliente son iguales. No se realizaron cambios.", DialogType.Warning);
                return;
            }

            try
            {
                int nameExists = _userManager.VerifyExistinClient(txt_Name.Text, txtb_firstSurname.Text, txtb_secondSurname.Text);
                int emailOrPhoneExists = _userManager.VerifyExistinEmployee(txt_Email.Text, txt_Phone.Text);

               if ((nameExists >= 1 || emailOrPhoneExists >= 1) &&
                    (txt_Email.Text != originalUser.correo || txt_Phone.Text != originalUser.telefono))
                {
                    ShowCustomMessage("Ya existe un cliente con ese nombre o contacto. Verifique los datos.", DialogType.Warning);
                    return;
                }

                Usuario userInfo = GetUserInfo();

                if (_userManager.UpdateClient(userInfo) == Constants.SuccessOperation)
                {
                    ShowCustomMessage("El client ha sido editado satisfactoriamente", DialogType.Success);
                    this.Close();
                }
                else
                {
                    ShowCustomMessage("Ha ocurrido un error intentando editar el cliente", DialogType.Error);
                }
            }
            catch (EndpointNotFoundException endPointException)
            {
                logger.LogFatal(endPointException);
                ShowCustomMessage("No se pudo establecer conexión con el servidor. Por favor, verifique la configuración de red e intente nuevamente.", DialogType.Error);

            }
            catch (TimeoutException timeOutException)
            {
                logger.LogWarn(timeOutException);
                ShowCustomMessage("Inténtalo de nuevo. El tiempo de espera ha expirado. Por favor, verifica tu conexión al servidor.", DialogType.Error);
            }
            catch (CommunicationException communicationException)
            {
                logger.LogFatal(communicationException);
                ShowCustomMessage("Se ha producido un fallo para establecer la conexión al servidor. Cheque su conexión a internet e inténtelo de nuevo.", DialogType.Error);

            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private bool CheckObjectValidId(int id) {
            if (id == Constants.ErrorOperation || id == Constants.NoDataMatches) {
                return false;
            }
            return true;
        }

        private void LoadTextFields(Usuario user)
        {
            txt_Name.Text = user.nombre ?? string.Empty;
            txtb_firstSurname.Text = user.primerApellido ?? string.Empty;
            txtb_secondSurname.Text = user.segundoApellido ?? string.Empty;
            txt_Email.Text = user.correo ?? string.Empty;
            txt_Phone.Text = user.telefono ?? string.Empty;
        }


        private Usuario GetUserInfo() {
            Usuario userInfo = new Usuario() {
                idUsuario = idUsuario,
                nombre = txt_Name.Text,
                primerApellido = txtb_firstSurname.Text,
                segundoApellido = txtb_secondSurname.Text,
                correo = txt_Email.Text,
                telefono = txt_Phone.Text
            };
            return userInfo;
            
        }

        private void SaveClientInfoCopy(Usuario userInfo) {
            originalUser = new Usuario {
                idUsuario = userInfo.idUsuario,
                nombre = userInfo.nombre,
                primerApellido = userInfo.primerApellido,
                segundoApellido = userInfo.segundoApellido,
                correo = userInfo.correo,
                telefono = userInfo.telefono
            };
        }

        private bool HasSupplierDataChanged()
        {
            return !(
                originalUser.nombre == txt_Name.Text &&
                originalUser.primerApellido == txtb_firstSurname.Text &&
                originalUser.segundoApellido == txtb_secondSurname.Text &&
                originalUser.correo == txt_Email.Text &&
                originalUser.telefono == txt_Phone.Text 
                
                );
        }

        private void RestartColorTxtBox()
        {
            ResetBorder(txt_Name);
            ResetBorder(txtb_firstSurname);
            ResetBorder(txtb_secondSurname);
            ResetBorder(txt_Email);
            ResetBorder(txt_Phone);
        }

        private void Telephone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void Telephone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Space ||
                (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control) ||
                (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control) ||
                (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control))
            {
                e.Handled = true;
            }
        }

        private void ResetBorder(TextBox textBox)
        {
            if (textBox.Parent is DockPanel dockPanel && dockPanel.Parent is Border border)
            {
                border.BorderBrush = Brushes.White;
            }
        }

        private void SetErrorBorder(TextBox textBox)
        {
            if (textBox.Parent is DockPanel dockPanel && dockPanel.Parent is Border border)
            {
                border.BorderBrush = Brushes.Red;
            }
        }

        private bool ValidateFields()
        {
            bool nameValid = Validator.ValidateName(txt_Name.Text);
            bool firstSurnameValid = Validator.ValidateName(txtb_firstSurname.Text);
            bool secondSurnameValid = Validator.ValidateName(txtb_secondSurname.Text);
            bool emailValid = Validator.ValidateEmail(txt_Email.Text);

            if (!nameValid) SetErrorBorder(txt_Name);
            if (!firstSurnameValid) SetErrorBorder(txtb_firstSurname);
            if (!secondSurnameValid) SetErrorBorder(txtb_secondSurname);
            if (!emailValid) SetErrorBorder(txt_Email);

            return nameValid && firstSurnameValid && secondSurnameValid && emailValid;
        }


    }
}
