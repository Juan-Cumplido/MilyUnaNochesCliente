using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MilyUnaNochesWPFApp.Views;
using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using MilyUnaNochesWPFApp.Utilities;
using static MilyUnaNochesWPFApp.Views.CustomDialog;
using System.ServiceModel;
using System;

namespace MilyUnaNochesWPFApp.Views {
    public partial class MenuSale : Page {
        private string origen;
        public MenuSale(string origen = "")
        {
            InitializeComponent();
            this.origen = origen;

            if (origen == "ManagerMenu")
            {
                img_GoOut.Visibility = Visibility.Collapsed;
                img_goBack.Visibility = Visibility.Visible;
                lbl_Client.Visibility = Visibility.Collapsed;
            }

            Frame.Navigate(new ConsultSale());
        }

        private void Image_MouseDownGoBack(object sender, MouseButtonEventArgs e)
        {
            ManagerMenu managerMenu = new ManagerMenu();
            this.NavigationService.Navigate(managerMenu);
        }

        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
        private void lbl_RegisterSale_Click(object sender, MouseButtonEventArgs e) {
            int employeeId = UserProfileSingleton.idEmployee;
            Frame.Navigate(new Sale(employeeId)); 
        }

        private void lbl_ConsultSale_Click(object sender, MouseButtonEventArgs e) {
            Frame.Navigate(new ConsultSale());
        }

        private void Lbl_Client_Click(object sender, MouseButtonEventArgs e) {
            CashierMenu cashierMenu = new CashierMenu();
            this.NavigationService.Navigate(cashierMenu);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = new CustomDialog("Regresará a la ventana de inicio de sesión. ¿Está seguro de salir?", CustomDialog.DialogType.Confirmation);
            dialog.ShowDialog();
            LoggerManager logger = new LoggerManager(this.GetType());
            if (dialog.UserConfirmed == true)
            {
                try
                {
                    MilyUnaNochesProxy.UserSessionManagerClient userSessionManagerClient = new MilyUnaNochesProxy.UserSessionManagerClient();
                    UserSession userSession = new UserSession()
                    {

                        idAcceso = UserProfileSingleton.idAcceso
                    };


                    int disconnectionResult = userSessionManagerClient.Disconnect(userSession, false);
                    if (disconnectionResult == Constants.SuccessOperation)
                    {
                        UserProfileSingleton.Instance.ResetSingleton();
                        LoginView login = new LoginView();
                        this.NavigationService.Navigate(login);
                    }
                    else if (disconnectionResult == Constants.NoDataMatches)
                    {
                        ShowCustomMessage("No se ha podido encontrar la sesión de usuario", DialogType.Warning);
                    }
                    else
                    {
                        ShowCustomMessage("Ocurrio un error al cerrar la sesión, intentelo de nuevo", DialogType.Warning);
                    }
                }
                catch (EndpointNotFoundException endPointException)
                {
                    logger.LogFatal(endPointException);
                    ShowCustomMessage("No se pudo establecer conexión con el servidor. Por favor, verifique la configuración de red e intente nuevamente.\r\n", DialogType.Error);

                }
                catch (TimeoutException timeOutException)
                {
                    logger.LogWarn(timeOutException);
                    ShowCustomMessage("Inténtalo de nuevo. El tiempo de espera ha expirado. Por favor, verifica tu conexión al servidor.", DialogType.Error);
                }
                catch (CommunicationException communicationException)
                {
                    logger.LogFatal(communicationException);
                    ShowCustomMessage("Se ha producido un fallo para establecer la conexión al servidor. Cheque su conexión a internet e inténtelo de nuevo", DialogType.Error);

                }
            }
        }
    }
}