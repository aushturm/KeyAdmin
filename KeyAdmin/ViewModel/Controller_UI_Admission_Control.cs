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
using KeyAdmin.EventArgs;

namespace KeyAdmin.ViewModel
{
    public class Controller_UI_Admission_Control : IUIPages
    {
        #region members
        private RelayCommand<object> _login;
        private RelayCommand _openPWDialog;
        private RelayCommand<object> _changePassword;
        private SecureString _entropy;
        #endregion

        #region events
        public event EventHandler<ViewStateChangedEventArgs> ViewStateChanged;
        #endregion

        #region properties
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

        public object[] Parameters
        {
            set
            {
                _entropy = value[0] as SecureString;
            }
        }
        #endregion

        #region constructor
        public Controller_UI_Admission_Control()
        {
            _login = new RelayCommand<object>(LoginHandler);
            _openPWDialog = new RelayCommand(OpenPWDialogHandler);
            _changePassword = new RelayCommand<object>(ChangePasswordHandler);
        }
        #endregion

        #region event handlers
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
                if (newPassword.Length < 1)
                {
                    GeneralExtensions.ShowErrorMessage("Your new password must have a length of minimum 1 character.");
                }
                if (Settings.Default.Password.DecryptString(oldPassword) != null || Settings.Default.Password == "")
                {
                    if (newPassword.SecureStringEqual(confirmPassword))
                    {
                        Settings.Default.Password = newPassword.EncryptString(newPassword);
                        Window dialog = passwordContainer as Window;
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
                var password = Settings.Default.Password;
                if (password == "")
                {
                    GeneralExtensions.ShowErrorMessage("Set new password first");
                    return;
                }
                if (Settings.Default.Password.DecryptString(userPassword) != null)
                {
                    PasswordHandler.Entropy = userPassword;
                    OnViewStateChanged(new ViewStateChangedEventArgs()
                    { Message = "Access permitted", viewState = ViewState.Finished });
                }
                else
                {
                    GeneralExtensions.ShowErrorMessage("Wrong Password!");
                }
            }
        }
#endregion

        #region raise event handler
        protected void OnViewStateChanged(ViewStateChangedEventArgs e)
        {
            EventHandler<ViewStateChangedEventArgs> handler = ViewStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
