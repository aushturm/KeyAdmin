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

        public RelayCommand<object> Login
        {
            get { return _login; }
        }

        public Controller_UI_Admission_Control()
        {
            _login = new RelayCommand<object>(LoginHandler);
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

                }
                else
                {
                    GeneralExtensions.ShowErrorMessage("Wrong Password!");
                }
            }
        }

        
    }
}
