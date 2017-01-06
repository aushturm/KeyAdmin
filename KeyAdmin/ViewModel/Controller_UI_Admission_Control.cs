using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using KeyAdmin.Interfaces;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using KeyAdmin.Properties;
using KeyAdmin.Model;

namespace KeyAdmin.ViewModel
{
    public class Controller_UI_Admission_Control
    {
        private RelayCommand<object> _login;
        private RelayCommand _openPWDialog;
        private RelayCommand<object> _changePassword;

        public RelayCommand<object> Login
        {
            get { return _login; }
        }

        public RelayCommand OpenPWDialog
        {
            get { return _openPWDialog; }
        }

        public RelayCommand<object> ChangePassword
        {
            get { return _changePassword; }
        }

        public Controller_UI_Admission_Control()
        {
            _login = new RelayCommand<object>(LoginHandler);
            _openPWDialog = new RelayCommand(OpenPWDialogHandler);
            _changePassword = new RelayCommand<object>(ChangePasswordHandler);
        }

        private void OpenPWDialogHandler()
        {
            View.ChangePwDialog changePwDialog = new View.ChangePwDialog();
            changePwDialog.ShowDialog();
        }
        public void ChangePasswordHandler(object parameter)
        {
            var passwordContainer = parameter as IHavePasswordsToChange;
            if (passwordContainer != null)
            {
                var oldPassword = passwordContainer.OldPassword;
                var newPassword = passwordContainer.NewPassword;
                var confirmPassword = passwordContainer.ConfirmPassword;
                var password = Settings.Default.Password.DecryptString();
                if (oldPassword.SecureStringEqual(password))
                {
                    if (newPassword.SecureStringEqual(confirmPassword))
                    {
                        Settings.Default.Password = newPassword.EncryptString();
                        View.ChangePwDialog dialog = passwordContainer as View.ChangePwDialog;
                        dialog.Close();
                        GeneralExtensions.ShowInfoMessage("Password successfully changed!");
                    }
                    else
                    {
                        GeneralExtensions.ShowErrorMessage("New passwords are not equal!");
                    }
                }
                else
                {
                    GeneralExtensions.ShowErrorMessage("wrong password!");
                }
            }
        }


        private void LoginHandler(object parameter)
        {
            var passwordContainer = parameter as IHavePassword;
            if (passwordContainer != null)
            {
                var userPassword = passwordContainer.Password;
                var password = Settings.Default.Password.DecryptString();
                if (userPassword.SecureStringEqual(password))
                {
                    MessageBox.Show("right pw");
                }
                else
                {
                    GeneralExtensions.ShowErrorMessage("Wrong Password!");
                }
            }
        }

        
    }
}
