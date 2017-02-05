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
using System.Collections.ObjectModel;
using System.Threading;

namespace KeyAdmin.ViewModel
{
    class Controller_UI_Main_Page : IUIPages, INotifyPropertyChanged
    {
        #region members
        private ObservableCollection<AccountItem> _accountItems = new ObservableCollection<AccountItem>();
        private RelayCommand _addAccountDetails;
        #endregion

        #region properties
        public ObservableCollection<AccountItem> AccountItems
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
            List<AccountPropertiesItem> propertiesList = new List<AccountPropertiesItem>();
            for (int cnt = 0; cnt < 20; cnt++)
            {
                AccountPropertiesItem properiesss = new AccountPropertiesItem() { Identifier = "propertie identifier", Value = "propertie value" };
                propertiesList.Add(properiesss);
            }
            AccountItem accountItem = new AccountItem() { Identifier = "accountItem identifier", Properties = propertiesList };
            AccountItems.Add(accountItem);
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
