using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using KeyAdmin.Interfaces;
using System.Runtime.InteropServices;
using System.Security;

namespace KeyAdmin.ViewModel
{
    public class Controller_UI_Admission_Control
    {
        private RelayCommand<IHavePassword> _login;

        public RelayCommand<IHavePassword> Login
        {
            get { return _login; }
        }

        public Controller_UI_Admission_Control()
        {
            _login = new RelayCommand<IHavePassword>(LoginHandler);
        }

        private void LoginHandler(IHavePassword passwordContainer)
        {
            if (passwordContainer != null)
            {
                var secureString = passwordContainer.Password;
                string PasswordInVM = Model.PasswordHandler.ToInsecureString(secureString);
            }
        }

        
    }
}
