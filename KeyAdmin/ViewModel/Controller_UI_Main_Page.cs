using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyAdmin.Interfaces;
using KeyAdmin.EventArgs;
using KeyAdmin.Model;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using KeyAdmin.View;

namespace KeyAdmin.ViewModel
{
    class Controller_UI_Main_Page : IUIPages, INotifyPropertyChanged
    {
        #region members
        private List<AccountItem> _accountItems = new List<AccountItem>();
        private RelayCommand _addAccountDetails;
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

        public RelayCommand AddAccountDetails
        {
            get { return _addAccountDetails; }
        }
        #endregion

        #region constructors
        public Controller_UI_Main_Page()
        {
            _addAccountDetails = new RelayCommand(AddAccountDetailsHandler);
            for (int cnt = 0; cnt < 20; cnt++)
            {
                List<AccountPropertiesItem> propertiesList = new List<AccountPropertiesItem>();
                for (int cnt2 = 0; cnt2 < 20; cnt2++)
                {
                    AccountPropertiesItem properiesss = new AccountPropertiesItem() { Identifier = "propertie identifier/ " + cnt + "." + cnt2, Value = "propertie value" };
                    propertiesList.Add(properiesss);
                }
                AccountItem accountItem = new AccountItem() { Identifier = "accountItem identifier/ " + cnt, Properties = propertiesList };
                AccountItems.Add(accountItem);
            }
        }
        #endregion

        #region event handlers
        private void AddAccountDetailsHandler()
        {
            AddAccountDialog addDialog = new AddAccountDialog();
            addDialog.ShowDialog();
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
