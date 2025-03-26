using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MilyUnaNochesWPFApp.Logic
{
    public static class DialogManager
    {
        public static void ShowErrorMessageAlert(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowErrorMessageAlert(string errorMessage, string title) {
            MessageBox.Show(errorMessage, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowWarningMessageAlert(string warningMessage)
        {
            MessageBox.Show(warningMessage, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        public static void ShowWarningMessageAlert(string warningMessage, string title) {
            MessageBox.Show(warningMessage, title, MessageBoxButton.OK, MessageBoxImage.Warning); 
        }

        public static void ShowSuccessMessageAlert(string successMessage)
        {
            MessageBox.Show(successMessage, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowSuccessMessageAlert(string successMessage, string title) {
            MessageBox.Show(successMessage, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool ShowConfirmationMessageAlert(string confirmMessage)
        {
            MessageBoxResult result = MessageBox.Show(confirmMessage, "Confirmación", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return result == MessageBoxResult.OK;
        }

        public static bool ShowConfirmationMessageAlert(string confirmMessage, string title) {
            MessageBoxResult result = MessageBox.Show(confirmMessage, title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return result == MessageBoxResult.OK;
        }
    }
}

