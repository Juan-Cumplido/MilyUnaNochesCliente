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
        private string selectedUserId;
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
            if (!this.IsLoaded || _serviceClient == null) return; 

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
                    IdUsuario = c.idUsuario,
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
            if (grd_ProviderDataGrid.SelectedItem != null)
            {
                var selectedRow = grd_ProviderDataGrid.SelectedItem;

                var idUsuarioProperty = selectedRow.GetType().GetProperty("IdUsuario");
                if (idUsuarioProperty != null)
                {
                    selectedUserId = idUsuarioProperty.GetValue(selectedRow)?.ToString();
                }
                else
                {
                    selectedUserId = null;
                }
            }
        }


        private async void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedUserId))
            {
                DialogManager.ShowErrorMessageAlert("Por favor, seleccione un cliente antes de eliminar.");
                return;
            }

            MessageBoxResult result = MessageBox.Show("¿Está seguro de archivar este cliente?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return; 
            }

            try
            {
                int.TryParse(selectedUserId, out int userId);
                int response = await Task.Run(() => _serviceClient.ArchiveClient(userId));

                if (response == 1) 
                {
                    DialogManager.ShowSuccessMessageAlert("Cliente archivado exitosamente.");
                    SearchForClient_TextChanged(null, null);
                }
                else
                {
                    DialogManager.ShowErrorMessageAlert("No se pudo archivar el cliente. Verifique e intente nuevamente.");
                }
            }
            catch (EndpointNotFoundException endPointException)
            {
                _logger.LogFatal(endPointException);
                DialogManager.ShowErrorMessageAlert("No se pudo conectar con el servidor. Verifique su red.");
            }
            catch (TimeoutException timeOutException)
            {
                _logger.LogWarn(timeOutException);
                DialogManager.ShowErrorMessageAlert("Tiempo de espera agotado. Revise su conexión.");
            }
            catch (CommunicationException communicationException)
            {
                _logger.LogFatal(communicationException);
                DialogManager.ShowErrorMessageAlert("Error en la conexión con el servidor.");
            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex);
                DialogManager.ShowErrorMessageAlert("Ocurrió un error inesperado.");
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            // Implementación del botón Editar
        }
    }
}
