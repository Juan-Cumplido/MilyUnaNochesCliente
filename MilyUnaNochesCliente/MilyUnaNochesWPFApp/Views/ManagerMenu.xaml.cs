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

namespace MilyUnaNochesWPFApp.Views {
    /// <summary>
    /// Lógica de interacción para ManagerMenu.xaml
    /// </summary>
    public partial class ManagerMenu : Page {
        public ManagerMenu() {
            InitializeComponent();
        }

        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }


        private void Sales_Click(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new MenuSale("ManagerMenu"));
        }
        private void Client_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CashierMenu("ManagerMenu"));
        }

        private void ImgLogo1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            fra_mainFrame.Source = new System.Uri("ManagerMenuContent.xaml", System.UriKind.Relative);
        }
        private void ImgLogo2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            fra_mainFrame.Source = new System.Uri("ManagerMenuContent.xaml", System.UriKind.Relative);
        }
        private void Employee_Click(object sender, RoutedEventArgs e) {
            
            MenuEmployees menuEmployees = new MenuEmployees();
            this.NavigationService.Navigate(menuEmployees);
        }

        private void Products_Click(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new ConsultProductsView("ManagerMenu"));
        }

        private void Suppliers_Click(object sender, RoutedEventArgs e) {
            fra_mainFrame.Source = new System.Uri("MenuSupplier.xaml", System.UriKind.Relative);
        }

        private void Reports_Click(object sender, RoutedEventArgs e) {
            GenerateReport generateReport = new GenerateReport();
            this.NavigationService.Navigate(generateReport);
        }

        private void Purchases_Click(object sender, RoutedEventArgs e) {
            fra_mainFrame.Source = new System.Uri("PurchasesMenu.xaml", System.UriKind.Relative);
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
