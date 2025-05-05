using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using MilyUnaNochesWPFApp.Logic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using static MilyUnaNochesWPFApp.Views.CustomDialog;

namespace MilyUnaNochesWPFApp.Views
{
   
    public partial class ConsultEmployee : Page
    {
        private readonly UserManagerClient _serviceClient;
        private readonly LoggerManager _logger;
        private string selectedUserId;
        public ConsultEmployee()
        {
            InitializeComponent();
            _serviceClient = new UserManagerClient();
            _logger = new LoggerManager(this.GetType());
        }

        private void ShowCustomMessage(string message, DialogType type)
        {
            var dialog = new CustomDialog(message, type);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void SearchForClient_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtb_searchForEmployee.Text == "Nombre o teléfono (ej. Juan)")
            {
                txtb_searchForEmployee.Text = "";
                txtb_searchForEmployee.Foreground = Brushes.Black;
            }
        }

        private void SearchForClient_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtb_searchForEmployee.Text))
            {
                txtb_searchForEmployee.Text = "Nombre o teléfono (ej. Juan)";
                txtb_searchForEmployee.Foreground = Brushes.Gray;
            }
        }

        private async void SearchForClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded || _serviceClient == null) return;

            string searchTerm = txtb_searchForEmployee?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm == "Nombre o teléfono (ej. Juan)")
            {
                grd_EmployeeDataGrid.ItemsSource = null;
                return;
            }

            try
            {
                var employee = await _serviceClient.GetActiveEmployeesBySearchTermAsync(searchTerm);

                if (employee == null || !employee.Any())
                {
                    grd_EmployeeDataGrid.ItemsSource = null;
                    return;
                }

                grd_EmployeeDataGrid.ItemsSource = employee.Select(c => new
                {
                    IdUsuario = c.idUsuario,
                    Nombre = c.nombre ?? "N/A",
                    PrimerApellido = c.primerApellido ?? "N/A",
                    SegundoApellido = c.segundoApellido ?? "N/A",
                    Telefono = c.telefono ?? "N/A",
                    Correo = c.correo ?? "N/A",
                    TipoEmpleado = c.tipoEmpleado ?? "N/A",
                    Direccion = $"{c.calle ?? "N/A"}, {c.numero ?? "N/A"}, {c.codigoPostal ?? "N/A"}, {c.ciudad ?? "N/A"}"
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

        private void grd_ProviderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grd_EmployeeDataGrid.SelectedItem != null)
            {
                var selectedRow = grd_EmployeeDataGrid.SelectedItem;

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
                ShowCustomMessage("Por favor, seleccione un Empleado antes de eliminar.", DialogType.Warning);
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
                    ShowCustomMessage("Empleado archivado exitosamente.", DialogType.Success);
                    SearchForClient_TextChanged(null, null);
                }
                else
                {
                    ShowCustomMessage("No se pudo archivar el Empleado. Verifique e intente nuevamente.", DialogType.Warning);
                    
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
            if (grd_EmployeeDataGrid.SelectedItem == null)
            {
                ShowCustomMessage("Por favor, seleccione un empleado antes de editar.", DialogType.Warning);

                return;
            }

            var selectedRow = grd_EmployeeDataGrid.SelectedItem;
            var idUsuarioProperty = selectedRow.GetType().GetProperty("IdUsuario");

            if (idUsuarioProperty != null)
            {
                int idUsuario = Convert.ToInt32(idUsuarioProperty.GetValue(selectedRow));
                var editWindow = new EditEmployee(idUsuario, this);
                makeThisOwnerWindow(editWindow);
            }
            else
            {
                ShowCustomMessage("No se pudo obtener el ID del empleado seleccionado.", DialogType.Warning);
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
