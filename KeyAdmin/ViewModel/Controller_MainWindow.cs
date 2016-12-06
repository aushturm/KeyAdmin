using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KeyAdmin.ViewModel
{
    public class Controller_MainWindow
    {
        private UserControl _display_view;

        public UserControl Displayed_View
        {
            get
            {
                return _display_view;
            }
            set
            {
                _display_view = value;
            }
        }


    }
}
