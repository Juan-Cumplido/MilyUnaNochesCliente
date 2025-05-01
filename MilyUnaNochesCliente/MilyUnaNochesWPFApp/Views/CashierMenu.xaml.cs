using Microsoft.Win32;
using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using MilyUnaNochesWPFApp.Utilities;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MilyUnaNochesWPFApp.Views.CustomDialog;

namespace MilyUnaNochesWPFApp.Views
{
    /// <summary>
    /// Lógica de interacción para CashierMenu.xaml
    /// </summary>
    public partial class CashierMenu : Page
    {

        public CashierMenu()
        {
            InitializeComponent();
            MainFrame.Source = new Uri("ConsultClient.xaml", UriKind.Relative);
        }

        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void Lbl_RegisterClient_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Source = new System.Uri("RegisterClient.xaml", System.UriKind.Relative);
        }

        private void Lbl_ConsultClient_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Source = new System.Uri("ConsultClient.xaml", System.UriKind.Relative);
        }

        private void Lbl_Sale_Click(object sender, MouseButtonEventArgs e)
        {
            MenuSale menuSale = new MenuSale();
            this.NavigationService.Navigate(menuSale);
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
                        usuario = UserProfileSingleton.email,
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
