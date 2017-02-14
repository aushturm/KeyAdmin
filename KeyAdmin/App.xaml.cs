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
            foreach (AccountItem item in Settings.Default.AccountItems)
            {
                //AccountPropertiesItem pw = item.Properties.Find(x => x.Identifier.Trim().ToLower() == "password"
                //                          || x.Identifier.Trim().ToLower() == "pw"
                //                          || x.Identifier.Trim().ToLower() == "passwort");
                //if (pw != null)
                foreach(AccountPropertiesItem propertie in item.Properties)
                {
                    string value = propertie.Value.ToSecureString().EncryptString(null);
                    string identifier = propertie.Identifier.ToSecureString().EncryptString(null);
                    propertie.Value = (value != null) ? value : propertie.Value;
                    propertie.Identifier = (identifier != null) ? identifier : propertie.Identifier;
                }
            }
            Settings.Default.Save();
        }
    }
}
