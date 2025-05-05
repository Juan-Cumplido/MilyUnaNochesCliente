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
    public partial class EditEmployee : Window {
        private int idUsuario;
        private ConsultEmployee parentPage;
        private Empleado originalEmployee;
        IUserManager _userManager = new UserManagerClient();
  

        public EditEmployee(int idUsuario, ConsultEmployee parentPage) {
            InitializeComponent();
            this.idUsuario = idUsuario;
            this.parentPage = parentPage;
            this.Loaded += EditEmployee_Loaded;

           
        }
        private void EditEmployee_Loaded(object sender, RoutedEventArgs e)
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
            var employeeInfo = _userManager.GetEmployee(idUsuario);

            if (employeeInfo == null || !CheckObjectValidId(employeeInfo.idUsuario))
            {
                ShowCustomMessage("Ha ocurrido un error al recuperar la información del empleado.", DialogType.Error);
                this.Close();
                return;
            }

            SaveemployeeInfoCopy(employeeInfo);
            LoadTextFields(employeeInfo);
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
                ShowCustomMessage("Los datos del empleado son iguales. No se realizaron cambios.", DialogType.Warning);
                return;
            }

            try
            {
                int nameExists = _userManager.VerifyExistinClient(txt_Name.Text, txtb_firstSurname.Text, txtb_secondSurname.Text);
                int emailOrPhoneExists = _userManager.VerifyExistinEmployee(txt_Email.Text, txt_Phone.Text);

               if ((nameExists >= 1 || emailOrPhoneExists >= 1) &&
                    (txt_Email.Text != originalEmployee.correo || txt_Phone.Text != originalEmployee.telefono))
                {
                    ShowCustomMessage("Ya existe un empleado con ese nombre o contacto. Verifique los datos.", DialogType.Warning);
                    return;
                }

                Empleado employeeInfo = GetEmployeeInfo();

                if (_userManager.UpdateEmployee(employeeInfo) == Constants.SuccessOperation)
                {
                    ShowCustomMessage("El empleado ha sido editado satisfactoriamente", DialogType.Success);
                    this.Close();
                }
                else
                {
                    ShowCustomMessage("Ha ocurrido un error intentando editar el empleado", DialogType.Error);
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

        private void LoadTextFields(Empleado employee)
        {
            txt_Name.Text = employee.nombre ?? string.Empty;
            txtb_firstSurname.Text = employee.primerApellido ?? string.Empty;
            txtb_secondSurname.Text = employee.segundoApellido ?? string.Empty;
            txt_Email.Text = employee.correo ?? string.Empty;
            txt_Phone.Text = employee.telefono ?? string.Empty;
            txt_Street.Text = employee.calle ?? string.Empty;
            txt_Number.Text = employee.numero ?? string.Empty;
            txt_PostalCode.Text = employee.codigoPostal ?? string.Empty;
            txt_City.Text = employee.ciudad ?? string.Empty;

            foreach (ComboBoxItem item in cmb_EmployeeType.Items)
            {
                if ((item.Content as string) == employee.tipoEmpleado)
                {
                    cmb_EmployeeType.SelectedItem = item;
                    break;
                }
            }
        }


        private Empleado GetEmployeeInfo() {
            Empleado employeeInfo = new Empleado() {
                idUsuario = idUsuario,
                nombre = txt_Name.Text,
                primerApellido = txtb_firstSurname.Text,
                segundoApellido = txtb_secondSurname.Text,
                correo = txt_Email.Text,
                telefono = txt_Phone.Text,
                calle = txt_Street.Text,
                numero = txt_Number.Text,
                codigoPostal = txt_PostalCode.Text,
                ciudad = txt_City.Text,
                tipoEmpleado = (cmb_EmployeeType.SelectedItem as ComboBoxItem)?.Content.ToString()
            };
            return employeeInfo;
            
        }

        private void SaveemployeeInfoCopy(Empleado employeeInfo) {
            originalEmployee = new Empleado {
                idUsuario = employeeInfo.idUsuario,
                nombre = employeeInfo.nombre,
                primerApellido = employeeInfo.primerApellido,
                segundoApellido = employeeInfo.segundoApellido,
                correo = employeeInfo.correo,
                telefono = employeeInfo.telefono,
                calle = employeeInfo.calle,
                numero = employeeInfo.numero,
                codigoPostal = employeeInfo.codigoPostal,
                ciudad = employeeInfo.ciudad,
                tipoEmpleado = employeeInfo.tipoEmpleado
            };
        }

        private bool HasSupplierDataChanged()
        {
            return !(
                originalEmployee.nombre == txt_Name.Text &&
                originalEmployee.primerApellido == txtb_firstSurname.Text &&
                originalEmployee.segundoApellido == txtb_secondSurname.Text &&
                originalEmployee.correo == txt_Email.Text &&
                originalEmployee.telefono == txt_Phone.Text &&
                originalEmployee.calle == txt_Street.Text &&
                originalEmployee.numero == txt_Number.Text &&
                originalEmployee.codigoPostal == txt_PostalCode.Text &&
                originalEmployee.ciudad == txt_City.Text &&
                originalEmployee.tipoEmpleado == (cmb_EmployeeType.SelectedItem as ComboBoxItem)?.Content.ToString()
            );
        }

        private void RestartColorTxtBox()
        {
            ResetBorder(txt_Name);
            ResetBorder(txtb_firstSurname);
            ResetBorder(txtb_secondSurname);
            ResetBorder(txt_Email);
            ResetBorder(txt_Phone);
            ResetBorder(txt_Street);
            ResetBorder(txt_Number);
            ResetBorder(txt_PostalCode);
            ResetBorder(txt_City);
            cmb_EmployeeType.BorderBrush = Brushes.White;
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
            bool employeeTypeSelected = cmb_EmployeeType.SelectedItem != null;

            if (!nameValid) SetErrorBorder(txt_Name);
            if (!firstSurnameValid) SetErrorBorder(txtb_firstSurname);
            if (!secondSurnameValid) SetErrorBorder(txtb_secondSurname);
            if (!emailValid) SetErrorBorder(txt_Email);
            if (!employeeTypeSelected) cmb_EmployeeType.BorderBrush = Brushes.Red;

            return nameValid && firstSurnameValid && secondSurnameValid && emailValid && employeeTypeSelected;
        }


    }
}
