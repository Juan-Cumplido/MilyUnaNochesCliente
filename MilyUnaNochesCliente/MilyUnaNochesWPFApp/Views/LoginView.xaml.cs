﻿using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MilyUnaNochesWPFApp.Views
{
    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            txtb_UserIdTextBox.BorderBrush = new SolidColorBrush(Colors.White);
            pwb_PasswordBox.BorderBrush = new SolidColorBrush(Colors.White);
            btnLogin.IsEnabled = false;
            imgLoading.Visibility = Visibility.Visible;
            try
            {
                Acceso userAccount = new Acceso
                {
                    usuario = txtb_UserIdTextBox.Text,
                    contraseña = pwb_PasswordBox.Password
                };

                if (verifyFields())
                {
                    int validateCredentials = await Task.Run(() => ValidateCredentials(userAccount));

                    if (validateCredentials == 1)
                    {
                        await ValidateExistingUserSession();
                    }
                    else if (validateCredentials == 0)
                    {
                        imgLoading.Visibility = Visibility.Collapsed;
                        DialogManager.ShowErrorMessageAlert("No se ha podido encontrar la cuenta. Por favor verifique que el nombre de usuario y contraseña sea correcto.");
                    }
                }
                else
                {
                    imgLoading.Visibility = Visibility.Collapsed;
                    DialogManager.ShowErrorMessageAlert("La información que ha ingresado es incorrecta. Intentelo de nuevo.");
                }

            }
            finally
            {
                imgLoading.Visibility = Visibility.Collapsed; 
                btnLogin.IsEnabled = true; 
            }
        }

        public bool verifyFields()
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(pwb_PasswordBox.Password))
            {
                if (pwb_PasswordBox.Parent is DockPanel dockPanel && dockPanel.Parent is Border border)
                {
                    border.BorderBrush = Brushes.Red;
                }
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(pwb_PasswordBox.Password))
            {
                txtb_UserIdTextBox.BorderBrush = Brushes.Red;
                isValid = false;
            }
            

            return isValid;
        }

        public async Task ValidateExistingUserSession()
        {
            bool existingSessionValidation = false;
            LoggerManager logger = new LoggerManager(this.GetType());
            MilyUnaNochesProxy.UserSessionManagerClient userSessionManagerClient = new UserSessionManagerClient();
            try
            {
                string userId = string.Empty;
                    txtb_UserIdTextBox.Dispatcher.Invoke(() =>
                {
                    userId = txtb_UserIdTextBox.Text;
                });

                UserSession session = new UserSession()
                {
                    usuario = userId,
                    idAcceso = UserProfileSingleton.idAcceso
                };
                existingSessionValidation = userSessionManagerClient.VerifyConnectivity(session);
                if (existingSessionValidation)
                {
                    DialogManager.ShowWarningMessageAlert("Ya existe una session abierta con ese usuario");
                }
                else
                {
                    userSessionManagerClient.Connect(session);
                    DisplayMainMenuView();
                }
            }
            catch (EndpointNotFoundException endPointException)
            {
                logger.LogFatal(endPointException);
                DialogManager.ShowErrorMessageAlert("No se pudo establecer conexión con el servidor. Por favor, verifique la configuración de red e intente nuevamente.");
            }
            catch (TimeoutException timeOutException)
            {
                logger.LogWarn(timeOutException);
                DialogManager.ShowErrorMessageAlert("Inténtalo de nuevo. El tiempo de espera ha expirado. Por favor, verifica tu conexión al servidor.");
            }
            catch (CommunicationException communicationException)
            {
                logger.LogFatal(communicationException);
                DialogManager.ShowErrorMessageAlert("Se ha producido un fallo para establecer la conexión al servidor. Chequee su conexión a internet e inténtelo de nuevo.");
            }
        }

        public int ValidateCredentials(Acceso access)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            int validationResult = -2;

            try
            {
                string hashedPassword = Hasher.hashToSHA2(access.usuario);
                string username = access.usuario;
                IUserManager userManager = new MilyUnaNochesProxy.UserManagerClient();
                validationResult = userManager.VerifyCredentials(username, hashedPassword);
            }
            catch (EndpointNotFoundException endPointException)
            {
                logger.LogFatal(endPointException);
                DialogManager.ShowErrorMessageAlert("No se pudo establecer conexión con el servidor. Por favor, verifique la configuración de red e intente nuevamente.");
            }
            catch (TimeoutException timeOutException)
            {
                logger.LogWarn(timeOutException);
                DialogManager.ShowErrorMessageAlert("Inténtalo de nuevo. El tiempo de espera ha expirado. Por favor, verifica tu conexión al servidor.");
            }
            catch (CommunicationException communicationException)
            {
                logger.LogFatal(communicationException);
                DialogManager.ShowErrorMessageAlert("Se ha producido un fallo para establecer la conexión al servidor. Chequee su conexión a internet e inténtelo de nuevo.");
            }

            switch (validationResult)
            {
                case -1:
                    DialogManager.ShowErrorMessageAlert("Ha ocurrido un error al intentar establecer conexión con la base de datos. ");
                    break;
                case 1:
                    obtainSingletonData(access);
                    break;
            }

            return validationResult;
        }

        public void obtainSingletonData(Acceso access)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                IUserManager userManager = new UserManagerClient();
                string hashedPassword = Hasher.hashToSHA2(access.contraseña);
                Empleado userAccount = userManager.GetUserProfile(access.usuario,hashedPassword );

                if (userAccount != null)
                {
                    UserProfileSingleton.Instance.CreateInstance(userAccount);
                }
            }
            catch (EndpointNotFoundException endPointException)
            {
                logger.LogFatal(endPointException);
                DialogManager.ShowErrorMessageAlert("No se pudo establecer conexión con el servidor. Por favor, verifique la configuración de red e intente nuevamente.");
            }
            catch (TimeoutException timeOutException)
            {
                logger.LogWarn(timeOutException);
                DialogManager.ShowErrorMessageAlert("Inténtalo de nuevo. El tiempo de espera ha expirado. Por favor, verifica tu conexión al servidor.");
            }
            catch (CommunicationException communicationException)
            {
                logger.LogFatal(communicationException);
                DialogManager.ShowErrorMessageAlert("Se ha producido un fallo para establecer la conexión al servidor. Chequee su conexión a internet e inténtelo de nuevo.");
            }
        }

        private void DisplayMainMenuView()
        {
            string tipoEmpleado = UserProfileSingleton.typeEmployee; 

            switch (tipoEmpleado)
            {
                case "Gerente":
                    ManagerMenu managerMenu = new ManagerMenu();
                    this.NavigationService.Navigate(managerMenu);
                    break;

                case "Cajero":
                    CashierMenu cashierMenu = new CashierMenu();
                    this.NavigationService.Navigate(cashierMenu);
                    break;

                case "Bodeguista":
                    RegisterProductView registerProductView = new RegisterProductView();
                    this.NavigationService.Navigate(registerProductView);
                    break;

                default:
                     break;
            }
        }

        private void UserIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtb_UserIdTextBox.Text == "Usuario")
            {
                txtb_UserIdTextBox.Text = "";
                txtb_UserIdTextBox.Foreground = Brushes.Black;
            }
        }

        private void UserIdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtb_UserIdTextBox.Text))
            {
                txtb_UserIdTextBox.Text = "Usuario";
                txtb_UserIdTextBox.Foreground = Brushes.Gray;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pwb_PasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(pwb_PasswordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
