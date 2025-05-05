﻿using log4net.Core;
using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MilyUnaNochesWPFApp.Views.CustomDialog;

namespace MilyUnaNochesWPFApp.Views
{
    public partial class RegisterClient : Page
    {
        public RegisterClient()
        {
            InitializeComponent();
        }
        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void Registrar_Click(object sender, RoutedEventArgs e)
        {
            RestartColorTxtBox();

            Usuario newUser = new Usuario
            {
                nombre = txtb_Name.Text,
                primerApellido = txtb_firstSurname.Text,
                segundoApellido = txtb_secondSurname.Text,
                correo = txtb_email.Text,
                telefono = txtb_telephone.Text
            };

            if (VerifyField())
            {
                int insertionResult = AddUser(newUser);
                if (insertionResult == 1)
                {
                    ShowCustomMessage("Cliente registrado exitosamente.", DialogType.Success);
                    ClearFields();
                }
            }
            else
            {
                ShowCustomMessage("La información que ha ingresado es incorrecta. Intentelo de nuevo.", DialogType.Warning);
            }
        }

        public int AddUser(Usuario user)
        {
            Logic.LoggerManager logger = new Logic.LoggerManager(this.GetType());
            int insertionResult = -1;

            try
            {
                MilyUnaNochesProxy.UserManagerClient userManagerClient = new MilyUnaNochesProxy.UserManagerClient();
                int validationExisted = userManagerClient.VerifyExistinClient(user.nombre, user.primerApellido, user.segundoApellido);
                int emailOrPhoneExists = userManagerClient.VerifyExistinEmployee(user.correo, user.telefono);

                if (validationExisted == 0 && emailOrPhoneExists == 0)
                {
                    insertionResult = userManagerClient.AddClient(user);
                }
                else if (validationExisted >= 1)
                {
                    ShowCustomMessage("El cliente ya esta registrado, para verificar puede buscarlo por su nombre o correo", DialogType.Warning);
                }
                else if (validationExisted == -1)
                {
                    ShowCustomMessage("Ha ocurrido un error al intentar establecer conexión con la base de datos.", DialogType.Error);
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

            return insertionResult;
        }

        public void RestartColorTxtBox()
        {
            ResetBorder(txtb_Name);
            ResetBorder(txtb_firstSurname);
            ResetBorder(txtb_secondSurname);
            ResetBorder(txtb_email);
            ResetBorder(txtb_telephone);
        }

        private void ResetBorder(TextBox textBox)
        {
            if (textBox.Parent is DockPanel dockPanel && dockPanel.Parent is Border border)
            {
                border.BorderBrush = Brushes.White;
            }
        }

        public bool VerifyField()
        {
            bool username = Validator.ValidateName(txtb_Name.Text);
            bool first = Validator.ValidateName(txtb_firstSurname.Text);
            bool second = Validator.ValidateName(txtb_secondSurname.Text);
            bool email = Validator.ValidateEmail(txtb_email.Text);

            if (!username) SetErrorBorder(txtb_Name);
            if (!first) SetErrorBorder(txtb_firstSurname);
            if (!second) SetErrorBorder(txtb_secondSurname);
            if (!email) SetErrorBorder(txtb_email);

            return username && first && second && email;
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

        private void SetErrorBorder(TextBox textBox)
        {
            if (textBox.Parent is DockPanel dockPanel && dockPanel.Parent is Border border)
            {
                border.BorderBrush = Brushes.Red;
            }
        }

        private void ClearFields()
        {
            txtb_Name.Text = string.Empty;
            txtb_firstSurname.Text = string.Empty;
            txtb_secondSurname.Text = string.Empty;
            txtb_email.Text = string.Empty;
            txtb_telephone.Text = string.Empty;
        }
    }
}
