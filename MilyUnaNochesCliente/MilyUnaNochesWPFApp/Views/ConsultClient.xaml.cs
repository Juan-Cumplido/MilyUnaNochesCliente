using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MilyUnaNochesWPFApp.Logic;
using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using static MilyUnaNochesWPFApp.Views.CustomDialog;

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

        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private async void SearchForClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded || _serviceClient == null) return; 

            string searchTerm = txtb_searchForClient?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm == "Nombre o teléfono (ej. Juan)")
            {
                grd_ClientDataGrid.ItemsSource = null;
                return;
            }

            try
            {
                var clients = await _serviceClient.GetUserProfileByNamePhoneAsync(searchTerm);

                if (clients == null || !clients.Any())
                {
                    grd_ClientDataGrid.ItemsSource = null;
                    return;
                }

                grd_ClientDataGrid.ItemsSource = clients.Select(c => new
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
                ShowCustomMessage("No se pudo establecer conexión con el servidor. Por favor, verifique la configuración de red e intente nuevamente.", DialogType.Error);

            }
            catch (TimeoutException timeOutException)
            {
                _logger.LogWarn(timeOutException);
                ShowCustomMessage("Inténtalo de nuevo. El tiempo de espera ha expirado. Por favor, verifica tu conexión al servidor.", DialogType.Error);
            }
            catch (CommunicationException communicationException)
            {
                _logger.LogFatal(communicationException);
                ShowCustomMessage("Se ha producido un fallo para establecer la conexión al servidor. Cheque su conexión a internet e inténtelo de nuevo.", DialogType.Error);

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
            if (grd_ClientDataGrid.SelectedItem != null)
            {
                var selectedRow = grd_ClientDataGrid.SelectedItem;

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
                ShowCustomMessage("Por favor, seleccione un cliente antes de eliminar.", DialogType.Warning);

               return;
            }

            var dialog = new CustomDialog("¿Está seguro de archivar este cliente?", CustomDialog.DialogType.Confirmation);
            dialog.ShowDialog();
            if (dialog.UserConfirmed == true)
            {
                return;
            }

            try
            {
                int.TryParse(selectedUserId, out int userId);
                int response = await Task.Run(() => _serviceClient.ArchiveClient(userId));

                if (response == 1) 
                {
                    ShowCustomMessage("Cliente archivado exitosamente.", DialogType.Success);
                    SearchForClient_TextChanged(null, null);
                }
                else
                {
                    ShowCustomMessage("No se pudo archivar el cliente. Verifique e intente nuevamente.", DialogType.Warning);


                }
            }
            catch (EndpointNotFoundException endPointException)
            {
                _logger.LogFatal(endPointException);
                ShowCustomMessage("No se pudo establecer conexión con el servidor. Por favor, verifique la configuración de red e intente nuevamente.", DialogType.Error);

            }
            catch (TimeoutException timeOutException)
            {
                _logger.LogWarn(timeOutException);
                ShowCustomMessage("Inténtalo de nuevo. El tiempo de espera ha expirado. Por favor, verifica tu conexión al servidor.", DialogType.Error);
            }
            catch (CommunicationException communicationException)
            {
                _logger.LogFatal(communicationException);
                ShowCustomMessage("Se ha producido un fallo para establecer la conexión al servidor. Cheque su conexión a internet e inténtelo de nuevo.", DialogType.Error);

            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (grd_ClientDataGrid.SelectedItem == null)
            {
                ShowCustomMessage("Por favor, seleccione un cliente antes de editar.", DialogType.Warning);

                return;
            }

            var selectedRow = grd_ClientDataGrid.SelectedItem;
            var idUsuarioProperty = selectedRow.GetType().GetProperty("IdUsuario");

            if (idUsuarioProperty != null)
            {
                int idUsuario = Convert.ToInt32(idUsuarioProperty.GetValue(selectedRow));
                var editWindow = new EditClient(idUsuario, this);
                makeThisOwnerWindow(editWindow);
            }
            else
            {
                ShowCustomMessage("No se pudo obtener el ID del usurio seleccionado.", DialogType.Warning);
            }
        }
        private void makeThisOwnerWindow(Window windowToOwn)
        {
            var mainWindow = Window.GetWindow(this);
            windowToOwn.Owner = mainWindow;
            windowToOwn.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            windowToOwn.ShowDialog();
        }
    }
}
