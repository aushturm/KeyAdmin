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
using System.Xml.Serialization;
using System.IO;

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
                if (Window.GetWindow(control) is IHavePasswordsToChange win)
                    ChangePasswordHandler(win);
            }
        }

        public void ChangePasswordHandler(object parameter)
        {
            if(string.IsNullOrWhiteSpace(Settings.Default.DefaultFilePath))
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Settings.Default.DefaultFilePath = path + $"\\{DateTime.Now.ToString("yyyyMMdd")}_Exported_Passwords_KeyAdmin.xml";
                using (File.Create(Settings.Default.DefaultFilePath)) ;
                Settings.Default.Save();
            }
            if (parameter is IHavePasswordsToChange passwordContainer)
            {
                var oldPassword = passwordContainer.OldPassword;
                var newPassword = passwordContainer.NewPassword;
                var confirmPassword = passwordContainer.ConfirmPassword;
                if (newPassword.Length < 8)
                {
                    GeneralExtensions.ShowErrorMessage("Your new password must have a length of minimum 8 character.");
                    return;
                }
                if (Settings.Default.Password == "" && oldPassword.Length == 0 || 
                    Settings.Default.Password.Decrypt(oldPassword.ToInsecureString()) != null)
                {
                    if (newPassword.SecureStringEqual(confirmPassword))
                    {
                        int errorCount = 0;
                        foreach (AccountItem item in LoadAccountItems())
                        {
                            var id = item.Identifier.Decrypt(oldPassword.ToInsecureString());
                            if (id != null)
                            {
                                item.Identifier = id;
                                item.Identifier = item.Identifier.Encrypt(newPassword.ToInsecureString());
                            }
                            else
                                errorCount++;
                            foreach (AccountPropertiesItem properties in item.Properties)
                            {
                                var prpId = properties.Identifier.Decrypt(oldPassword.ToInsecureString());
                                if (prpId != null)
                                {
                                    properties.Identifier = prpId;
                                    properties.Identifier = properties.Identifier.Encrypt(newPassword.ToInsecureString());
                                }
                                else
                                    errorCount++;

                                var valId = properties.Value.Decrypt(oldPassword.ToInsecureString());
                                if (valId != null)
                                {
                                    properties.Value = valId;
                                    properties.Value = properties.Value.Encrypt(newPassword.ToInsecureString());
                                }
                                else
                                    errorCount++;
                            }
                        }
                        if (errorCount > 0)
                        {
                            GeneralExtensions.ShowErrorMessage(errorCount + "items failed to decrypt!");
                        }
                        Settings.Default.Password = newPassword.ToInsecureString().Encrypt(newPassword.ToInsecureString());
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

        private List<AccountItem> LoadAccountItems()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DeserializeItemHolder));
            using (FileStream fileStream = new FileStream(Settings.Default.DefaultFilePath, FileMode.Open))
            {
                DeserializeItemHolder items;
                try
                {
                    items = (DeserializeItemHolder)serializer.Deserialize(fileStream);
                }
                catch (InvalidOperationException) { return new List<AccountItem>(); }//document is empty

                List<AccountItem> accountItems = new List<AccountItem>();
                    foreach (AccountItem item in items.Data)
                    {
                        accountItems.Add(item);
                    }
                    return accountItems;
            }
        }

        private void LoginHandler(object parameter)
        {
            if (parameter is IHavePassword passwordContainer)
            {
                var userPassword = passwordContainer.Password;
                var password = Settings.Default.Password;

                if(!string.IsNullOrEmpty(Settings.Default.DefaultFilePath) && !File.Exists(Settings.Default.DefaultFilePath))
                {
                    Settings.Default.DefaultFilePath = "";
                    Settings.Default.Password = "";
                    Settings.Default.Save();
                    GeneralExtensions.ShowErrorMessage($"File '{Settings.Default.DefaultFilePath}' does not exist. Please set a new password.");
                    return;
                }
                if (password == string.Empty)
                {
                    GeneralExtensions.ShowErrorMessage("Set new password first");
                    return;
                }
                if (Settings.Default.Password.Decrypt(userPassword.ToInsecureString()) != null)
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
