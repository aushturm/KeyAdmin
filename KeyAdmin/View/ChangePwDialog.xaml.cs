using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KeyAdmin.Interfaces;

namespace KeyAdmin.View
{
    /// <summary>
    /// Interaction logic for ChangePwDialog.xaml
    /// </summary>
    public partial class ChangePwDialog : Window, IHavePasswordsToChange
    {
        public ChangePwDialog()
        {
            InitializeComponent();
        }

        public System.Security.SecureString OldPassword
        {
            get
            {
                return PWBox_old.SecurePassword;
            }
        }

        public System.Security.SecureString NewPassword
        {
            get
            {
                return PWBox_new.SecurePassword;
            }
        }

        public System.Security.SecureString ConfirmPassword
        {
            get
            {
                return PWBox_confirm.SecurePassword;
            }
        }
    }
}
