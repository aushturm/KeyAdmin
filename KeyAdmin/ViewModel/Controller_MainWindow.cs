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
using KeyAdmin.Interfaces;

namespace KeyAdmin.ViewModel
{
    public class Controller_MainWindow : INotifyPropertyChanged
    {
        #region private members
        private UserControl _display_view;
        #endregion

        #region properties
        public UserControl Display_View
        {
            get
            {
                return _display_view;
            }
            set
            {
                _display_view = value;
                OnPropertyChanged("Display_View");
            }
        }
        #endregion

        #region events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region raise event handlers
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        # region constuctors
        public Controller_MainWindow()
        {
            UI_Admission_Control ui_adms_ctrl = new UI_Admission_Control();
            IUIPages pageControl = ui_adms_ctrl.DataContext as IUIPages;
            pageControl.ViewStateChanged += Display_View_ViewStateChanged;
            Display_View = ui_adms_ctrl;
        }
        #endregion

        #region event handler
        private void Display_View_ViewStateChanged(object sender, EventArgs.ViewStateChangedEventArgs e)
        {
            UI_Main_Page mainPage = new UI_Main_Page();
            IUIPages pageControl = mainPage.DataContext as IUIPages;
            pageControl.ViewStateChanged += Display_View_ViewStateChanged;
            pageControl.Parameters = e.Parameters;
            Display_View = mainPage;
        }
        #endregion
    }
}
