using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using KeyAdmin.Properties;
using KeyAdmin.Model;

namespace KeyAdmin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                View.MainWindow mainView = new View.MainWindow();
                mainView.Show();
            }
            catch (Exception ex)
            {
                GeneralExtensions.ShowErrorMessage(ex.Message);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            GeneralExtensions.ShowErrorMessage(e.Exception.Message);
        }
    }
}
