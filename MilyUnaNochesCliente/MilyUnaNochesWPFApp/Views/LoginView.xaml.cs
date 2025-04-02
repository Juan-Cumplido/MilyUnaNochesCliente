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

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            txtb_UserIdTextBox.BorderBrush = new SolidColorBrush(Colors.White);
            pwb_PasswordBox.BorderBrush = new SolidColorBrush(Colors.White);

            Profile userAccount = new Profile
            {
                username = txtb_Username.Text,
                password = pwb_Password.Password
            };

            if (verifyFields())
            {
                int validateCredentials = ValidateCredentials(userAccount);

                if (validateCredentials == 1)
                {
                    ValidateExistingUserSession();
                }
                else if (validateCredentials == 0)
                {
                    DialogManager.ShowErrorMessageAlert(Properties.Resources.dialogMissmatchesCredentials);
                }
            }
            else
            {
                DialogManager.ShowErrorMessageAlert(Properties.Resources.dialogWrongData);
            }
        }

        private void DisplayMainMenuView()
        {
            MenuEmployees menuEmployees = new MenuEmployees();
            this.NavigationService.Navigate(menuEmployees);
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
