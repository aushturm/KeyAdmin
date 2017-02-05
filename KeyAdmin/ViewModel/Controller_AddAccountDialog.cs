using GalaSoft.MvvmLight.Command;
using KeyAdmin.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KeyAdmin.ViewModel
{
    class Controller_AddAccountDialog : INotifyPropertyChanged
    {
        #region members
        private ObservableCollection<AccountItem> _accountData = new ObservableCollection<AccountItem>();
        private int _selectedPropertie = 0;
        private RelayCommand _addPropertie;
        private RelayCommand _deletePropertie;
        #endregion

        #region properties
        public ObservableCollection<AccountItem> AccountData
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

        public RelayCommand AddPropertie
        {
            get
            {
                return _addPropertie;
            }
            set
            {
                _addPropertie = value;
                OnPropertyChanged("AddPropertie");
            }
        }

        public int SelectedPropertie
        {
            get
            {
                return _selectedPropertie;
            }
            set
            {
                if (value > -1)
                {
                    _selectedPropertie = value;
                    OnPropertyChanged("SelectedPropertie");
                }
            }
        }

        public RelayCommand DeletePropertie
        {
            get
            {
                return _deletePropertie;
            }
            set
            {
                _deletePropertie = value;
                OnPropertyChanged("DeletePropertie");
            }
        }
        #endregion

        #region constructors
        public Controller_AddAccountDialog()
        {
            _addPropertie = new RelayCommand(AddPropertieHandler);
            _deletePropertie = new RelayCommand(DeletePropertieHandler);

            List<AccountPropertiesItem> propertiesList = new List<AccountPropertiesItem>();
            for (int cnt = 0; cnt < 5; cnt++)
            {
                AccountPropertiesItem properiesss = new AccountPropertiesItem() { Identifier = "propertie identifier", Value = "propertie value" };
                propertiesList.Add(properiesss);
            }
            AccountItem accountItem = new AccountItem() { Identifier = "accountItem identifier", Properties = propertiesList };
            AccountData.Add(accountItem);
            OnPropertyChanged("AccountData");

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

        #region Event Handlers
        private void AddPropertieHandler()
        {
            throw new NotImplementedException();
        }

        private void DeletePropertieHandler()
        {
            List<AccountPropertiesItem> propertiesList = AccountData[0].Properties;
            propertiesList.RemoveAt(_selectedPropertie);
            AccountItem account = AccountData[0];
            account.Properties = propertiesList;
            AccountData.Clear();
            AccountData.Add(account);
            OnPropertyChanged("AccountData");
        }
        #endregion
    }
}
