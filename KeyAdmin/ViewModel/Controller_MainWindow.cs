using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using KeyAdmin.UI_Pages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.ComponentModel;

namespace KeyAdmin.ViewModel
{
    public class Controller_MainWindow
    {
        //private members
        private UserControl _display_view;

        //properties
        public UserControl Displayed_View
        {
            get
            {
                return _display_view;
            }
            set
            {
                _display_view = value;
                OnPropertyChanged("Displayed_View");
            }
        }

        //events
        public event PropertyChangedEventHandler PropertyChanged;

        //event handlers
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //constuctors
        public Controller_MainWindow()
        {
            UI_Admission_Control ui_adms_ctrl = new UI_Admission_Control();
            Displayed_View = ui_adms_ctrl;
        }


    }
}
