using KeyAdmin.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyAdmin.ViewModel
{
    class Controller_AddAccountDialog : INotifyPropertyChanged
    {
        #region members
        private List<AccountItem> _accountData = new List<AccountItem>();
        #endregion

        #region properties
        public List<AccountItem> AccountData
        {
            get
            {
                return _accountData;
            }
            set
            {
                _accountData = value;
                OnPropertyChanged("AccountData");
            }
        }
        #endregion

        #region constructors
        public Controller_AddAccountDialog()
        {
            List<AccountPropertiesItem> propertiesList = new List<AccountPropertiesItem>();
            AccountPropertiesItem properiesss = new AccountPropertiesItem() { Identifier = "propertie identifier", Value = "propertie value" };
            propertiesList.Add(properiesss);
            AccountItem accountItem = new AccountItem() { Identifier = "account Name", Properties = propertiesList };
            AccountData.Add(accountItem);
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
    }
}
