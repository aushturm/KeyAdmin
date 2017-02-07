using GalaSoft.MvvmLight.Command;
using KeyAdmin.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KeyAdmin.ViewModel
{
    public class Controller_EditAccountDialog : INotifyPropertyChanged
    {
        #region members
        private ObservableCollection<AccountItem> _accountData = new ObservableCollection<AccountItem>();
        private int _selectedPropertie = 0;
        private RelayCommand _addPropertie;
        private RelayCommand _deletePropertie;
        private RelayCommand<Window> _closeDialogOk;
        private string _windowTitle;
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

        public string WindowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; }
        }

        public RelayCommand AddPropertie
        {
            get {return _addPropertie;}
        }

        public RelayCommand<Window> CloseDialogOk
        {
            get { return _closeDialogOk; }
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
            get {return _deletePropertie;}
        }
        #endregion

        #region constructors
        public Controller_EditAccountDialog()
        {
            _addPropertie = new RelayCommand(AddPropertieHandler);
            _deletePropertie = new RelayCommand(DeletePropertieHandler);
            _closeDialogOk = new RelayCommand<Window>(CloseDialogOkHandler);

            List<AccountPropertiesItem> propertiesList = new List<AccountPropertiesItem>();
            AccountPropertiesItem properiesss = new AccountPropertiesItem() { Identifier = "name", Value = "value" };
            propertiesList.Add(properiesss);
            AccountItem accountItem = new AccountItem() { Identifier = "account name", Properties = propertiesList };
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

        #region Event Handlers
        private void AddPropertieHandler()
        {
            AccountItem account = AccountData[0];
            account.Properties.Add(new AccountPropertiesItem() { Identifier = "name", Value="value"});
            AccountData.Clear();
            AccountData.Add(account);
            OnPropertyChanged("AccountData");
        }

        private void DeletePropertieHandler()
        {
            List<AccountPropertiesItem> propertiesList = AccountData[0].Properties;
            if (_selectedPropertie < 0)
                return;
            propertiesList.RemoveAt(_selectedPropertie);
            AccountItem account = AccountData[0];
            account.Properties = propertiesList;
            AccountData.Clear();
            AccountData.Add(account);
            OnPropertyChanged("AccountData");
            _selectedPropertie = -1;
        }

        private void CloseDialogOkHandler(Window dialog)
        {
            dialog.DialogResult = true;
            dialog.Close();
        }
        #endregion

        #region private functions
        #endregion
    }
}
