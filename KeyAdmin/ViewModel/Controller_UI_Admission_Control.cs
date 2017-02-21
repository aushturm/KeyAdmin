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
using System.Windows.Input;
using System.Windows.Controls;
using KeyAdmin.View;

namespace KeyAdmin.ViewModel
{
    public class Controller_UI_Admission_Control : IUIPages
    {
        #region members
        private RelayCommand<object> _login;
        private RelayCommand _openPWDialog;
        private RelayCommand<object> _changePassword;
        private SecureString _entropy;
        private RelayCommand<RoutedEventArgs> _pageLoaded;
        #endregion

        #region events
        public event EventHandler<ViewStateChangedEventArgs> ViewStateChanged;
        #endregion

        #region properties
        public RelayCommand<object> Login
        {
            get { return _login; }
        }

        public RelayCommand<RoutedEventArgs> PageLoaded
        {
            get { return _pageLoaded; }
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
            _pageLoaded = new RelayCommand<RoutedEventArgs>(PageLoadedHandler);
        }
        #endregion

        #region event handlers
        private void PageLoadedHandler(RoutedEventArgs e)
        {
            var control = e.OriginalSource as UserControl;
            var win = Window.GetWindow(control);
            win.KeyDown += Win_KeyDown;
        }

        private void OpenPWDialogHandler()
        {
            View.ChangePwDialog changePwDialog = new View.ChangePwDialog();
            changePwDialog.Loaded += ChangePwDialog_Loaded;
            changePwDialog.ShowDialog();
        }

        private void ChangePwDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var win = e.OriginalSource as Window;
            win.KeyDown += Win_KeyDown;
        }

        private void Win_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var control = e.OriginalSource as Control;
                MainWindow parent = Window.GetWindow(control) as MainWindow;
                if (parent != null)
                {
                    Controller_MainWindow dc = parent.DataContext as Controller_MainWindow;
                    IHavePassword page = dc.Display_View as IHavePassword;
                    if (page != null)
                    {
                        LoginHandler(page);
                        return;
                    }
                }
                IHavePasswordsToChange win = Window.GetWindow(control) as IHavePasswordsToChange;
                if (win != null)
                    ChangePasswordHandler(win);
            }
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
                    return;
                }
                if (Settings.Default.Password.DecryptString(oldPassword) != null || 
                    Settings.Default.Password == "" && oldPassword.Length == 0)
                {
                    if (newPassword.SecureStringEqual(confirmPassword))
                    {
                        int errorCount = 0;
                        foreach(AccountItem item in Settings.Default.AccountItems)
                        {
                            var id = item.Identifier.DecryptString(oldPassword);
                            if (id != null)
                            {
                                item.Identifier = id.ToInsecureString();
                                item.Identifier = item.Identifier.ToSecureString().EncryptString(newPassword);
                            }
                            else
                                errorCount++;
                            foreach (AccountPropertiesItem properties in item.Properties)
                            {
                                var prpId = properties.Identifier.DecryptString(oldPassword);
                                if (prpId != null)
                                {
                                    properties.Identifier = prpId.ToInsecureString();
                                    properties.Identifier = properties.Identifier.ToSecureString().EncryptString(newPassword);
                                }
                                else
                                    errorCount++;

                                var valId = properties.Value.DecryptString(oldPassword);
                                if (valId != null)
                                {
                                    properties.Value = valId.ToInsecureString();
                                    properties.Value = properties.Value.ToSecureString().EncryptString(newPassword);
                                }
                                else
                                    errorCount++;
                            }
                        }
                        if (errorCount > 0)
                        {
                            GeneralExtensions.ShowErrorMessage(errorCount + "items failed to decrypt!");
                        }
                        Settings.Default.Password = newPassword.EncryptString(newPassword);
                        Settings.Default.Save();
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
