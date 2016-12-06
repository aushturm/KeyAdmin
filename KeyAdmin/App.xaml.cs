﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using KeyAdmin.Properties;

namespace KeyAdmin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            View.MainWindow mainView = new View.MainWindow();
            mainView.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
