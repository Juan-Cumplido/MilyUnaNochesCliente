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

namespace MilyUnaNochesWPFApp.Views
{

    public partial class RegisterEmployee : Page
    {
        public RegisterEmployee()
        {
            InitializeComponent();
        }

        private void Registrar_Click(object sender, RoutedEventArgs e)
        {
            RestartColorTxtBox();
            if (VerifyField())
            {
                Usuario newUser = new Usuario
            {
                nombre = txtb_Name.Text,
                primerApellido = txtb_firstSurname.Text,
                segundoApellido = txtb_secondSurname.Text,
                correo = txtb_email.Text,
                telefono = txtb_telephone.Text
            };

            UserDireccion newAddress = new UserDireccion
            {
                calle = txtb_street.Text,
                numero = txtb_number.Text,
                codigoPostal = txtb_postalCode.Text,
                ciudad = txtb_city.Text
            };

            Empleado newEmployee = new Empleado
            {
                tipoEmpleado = cmb_EmployeeType.Text
            };


            Acceso newAccess = new Acceso
            {
                usuario = txtb_User.Text,
                contraseña = txtb_password.Text,
            };


            
                int insertionResult = AddEmployee(newUser, newAddress, newEmployee, newAccess);
                if (insertionResult == 1)
                {
                    DialogManager.ShowSuccessMessageAlert("Cliente registrado exitosamente");
                    ClearFields();
                }
            }
            else
            {
                DialogManager.ShowErrorMessageAlert("La información que ha ingresado es incorrecta. Intentelo de nuevo.");
            }
        }

        public int AddEmployee(Usuario user, UserDireccion address, Empleado employee, Acceso access)
        {
            Logic.LoggerManager logger = new Logic.LoggerManager(this.GetType());
            int insertionResult = -1;

            try
            {
                MilyUnaNochesProxy.UserManagerClient userManagerClient = new MilyUnaNochesProxy.UserManagerClient();
                int validationExisted = userManagerClient.VerifyExistinClient(user.nombre, user.primerApellido, user.segundoApellido);
                int validationExistedEmail = userManagerClient.VerifyExistinEmployee(user.correo, user.telefono);

                if (validationExisted == 0)
                {
                    if (validationExistedEmail == 0)
                    {
                        insertionResult = userManagerClient.AddEmployee(user, address, employee, access);
                    }
                    else if (validationExisted >= 1)
                    {
                        DialogManager.ShowErrorMessageAlert("El correo o telefono ya esta registrado, para verificar puede buscarlo por su el telefono");
                    }
                    else if (validationExisted == -1)
                    {
                        DialogManager.ShowErrorMessageAlert("Ha ocurrido un error al intentar establecer conexión con la base de datos.");
                    }
                }
                else if (validationExisted >= 1)
                {
                    DialogManager.ShowErrorMessageAlert("Un empleado con ese nombre ya esta registrado, para verificar puede buscarlo por su nombre o correo");
                }
                else if (validationExisted == -1)
                {
                    DialogManager.ShowErrorMessageAlert("Ha ocurrido un error al intentar establecer conexión con la base de datos.");
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

            return insertionResult;
        }

        public void RestartColorTxtBox()
        {
            ResetBorder(txtb_Name);
            ResetBorder(txtb_firstSurname);
            ResetBorder(txtb_secondSurname);
            ResetBorder(txtb_email);
            ResetBorder(txtb_street);
            ResetBorder(txtb_city);
            ResetBorder(txtb_User);
            ResetBorder(txtb_password); 
            cmb_EmployeeType.BorderBrush = Brushes.White;
        }

        private void ResetBorder(TextBox textBox)
        {
            if (textBox.Parent is DockPanel dockPanel && dockPanel.Parent is Border border)
            {
                border.BorderBrush = Brushes.White;
            }
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
            txtb_street.Text = string.Empty;
            txtb_number.Text = string.Empty;
            txtb_postalCode.Text = string.Empty;
            txtb_city.Text = string.Empty;
            txtb_password.Text = string.Empty;
            txtb_User.Text = string.Empty;
            cmb_EmployeeType.Text = string.Empty;

        }

        public bool VerifyField()
        {
            bool username = Validator.ValidateName(txtb_Name.Text);
            bool first = Validator.ValidateName(txtb_firstSurname.Text);
            bool second = Validator.ValidateName(txtb_secondSurname.Text);
            bool email = Validator.ValidateEmail(txtb_email.Text);
            bool employeeTypeSelected = cmb_EmployeeType.SelectedItem != null; 

            if (!username) SetErrorBorder(txtb_Name);
            if (!first) SetErrorBorder(txtb_firstSurname);
            if (!second) SetErrorBorder(txtb_secondSurname);
            if (!email) SetErrorBorder(txtb_email);
            if (!employeeTypeSelected) cmb_EmployeeType.BorderBrush = Brushes.Red; 

            return username && first && second && email&& employeeTypeSelected;
        }

    }
}
