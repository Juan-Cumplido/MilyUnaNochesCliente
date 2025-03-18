using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;

namespace MilyUnaNochesWPFApp.Views
{
    public partial class ConsultClient : Page
    {
        private readonly UserManagerClient _serviceClient;
        private readonly LoggerManager _logger;

        public ConsultClient()
        {
            InitializeComponent();
            _serviceClient = new UserManagerClient();
            _logger = new LoggerManager(this.GetType());
        }

        private void SearchForClient_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtb_searchForClient.Text == "Nombre o teléfono (ej. Juan)")
            {
                txtb_searchForClient.Text = "";
                txtb_searchForClient.Foreground = Brushes.Black;
            }
        }

        private async void SearchForClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded || _serviceClient == null) return; // Evita llamadas si la UI no ha terminado de cargar

            string searchTerm = txtb_searchForClient?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm == "Nombre o teléfono (ej. Juan)")
            {
                grd_ProviderDataGrid.ItemsSource = null;
                return;
            }

            try
            {
                var clients = await _serviceClient.GetUserProfileByNamePhoneAsync(searchTerm);

                if (clients == null || !clients.Any())
                {
                    grd_ProviderDataGrid.ItemsSource = null;
                    return;
                }

                grd_ProviderDataGrid.ItemsSource = clients.Select(c => new
                {
                    Nombre = c.nombre ?? "N/A",
                    PrimerApellido = c.primerApellido ?? "N/A",
                    SegundoApellido = c.segundoApellido ?? "N/A",
                    Telefono = c.telefono ?? "N/A",
                    Correo = c.correo ?? "N/A"
                }).ToList();
            }
            catch (EndpointNotFoundException endPointException)
            {
                _logger.LogFatal(endPointException);
                DialogManager.ShowErrorMessageAlert("No se pudo establecer conexión con el servidor. Verifique su red.");
            }
            catch (TimeoutException timeOutException)
            {
                _logger.LogWarn(timeOutException);
                DialogManager.ShowErrorMessageAlert("Tiempo de espera agotado. Revise su conexión al servidor.");
            }
            catch (CommunicationException communicationException)
            {
                _logger.LogFatal(communicationException);
                DialogManager.ShowErrorMessageAlert("Error en la conexión con el servidor. Verifique su internet.");
            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex);
                DialogManager.ShowErrorMessageAlert("Ocurrió un error inesperado al buscar clientes.");
            }
        }

        private void SearchForClient_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtb_searchForClient.Text))
            {
                txtb_searchForClient.Text = "Nombre o teléfono (ej. Juan)";
                txtb_searchForClient.Foreground = Brushes.Gray;
            }
        }

        private void grd_ProviderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            // Implementación del botón Eliminar
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            // Implementación del botón Editar
        }
    }
}
