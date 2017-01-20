using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyAdmin.Interfaces;
using KeyAdmin.EventArgs;
using KeyAdmin.Model;
using System.ComponentModel;

namespace KeyAdmin.ViewModel
{
    class Controller_UI_Main_Page : IUIPages, INotifyPropertyChanged
    {
        #region members
        private List<AccountItem> _accountItems = new List<AccountItem>();
        #endregion

        #region properties
        public List<AccountItem> AccountItems
        {
            get
            {
                return _accountItems;
            }
            set
            {
                _accountItems = value;
                OnPropertyChanged("AccountItems");
            }
        }
        #endregion

        #region constructors
        public Controller_UI_Main_Page()
        {
            AccountPropertiesItem properiesss = new AccountPropertiesItem() { identifier = "propertie identifier", value = "propertie value" };
            AccountItem accountItem = new AccountItem() { identifier = "accountItem identifier", properties = properiesss };
            AccountItems.Add(accountItem);
        }
        #endregion

        #region events
        public event EventHandler<ViewStateChangedEventArgs> ViewStateChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region raise event handlers
        protected void OnViewStateChanged(ViewStateChangedEventArgs e)
        {
            EventHandler<ViewStateChangedEventArgs> handler = ViewStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

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
