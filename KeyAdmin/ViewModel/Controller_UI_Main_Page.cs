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
using System.Windows.Controls;

namespace KeyAdmin.ViewModel
{
    class Controller_UI_Main_Page : IUIPages, INotifyPropertyChanged
    {
        #region members
        private ObservableCollection<AccountItem> _accountItems = new ObservableCollection<AccountItem>();
        private RelayCommand _addAccountDetails;
        private RelayCommand<ListViewItem> _deleteAccountDetails;
        private RelayCommand<ListViewItem> _editAccountDetails;
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
        public RelayCommand<ListViewItem> DeleteAccountDetails
        {
            get { return _deleteAccountDetails; }
        }
        public RelayCommand<ListViewItem> EditAccountDetails
        {
            get { return _editAccountDetails; }
        }
        #endregion

        #region constructors
        public Controller_UI_Main_Page()
        {
            _addAccountDetails = new RelayCommand(AddAccountDetailsHandler);
            _deleteAccountDetails = new RelayCommand<ListViewItem>(DeleteAccountDetailsHandler);
            _editAccountDetails = new RelayCommand<ListViewItem>(EditAccountDetailsHandler);
        }
        #endregion

        #region event handlers
        private void AddAccountDetailsHandler()
        {
            EditAccountDialog addDialog = new EditAccountDialog();
            Controller_EditAccountDialog dataContext = addDialog.DataContext as Controller_EditAccountDialog;
            dataContext.WindowTitle = "add account";
            addDialog.ShowDialog();
            if (addDialog.DialogResult == true)
            {
                AccountItems.Add(dataContext.AccountData[0]);
                OnPropertyChanged("AccountItems");
            }
        }

        private void DeleteAccountDetailsHandler(ListViewItem obj)
        {
            if (GeneralExtensions.ShowDecisionMessage(
                "Do you really want to delete this account-info?") == System.Windows.Forms.DialogResult.No) return;
            AccountItems.Remove(obj.Content as AccountItem);
            OnPropertyChanged("AccountItems");
        }

        private void EditAccountDetailsHandler(ListViewItem obj)
        {
            EditAccountDialog addDialog = new EditAccountDialog();
            Controller_EditAccountDialog dataContext = addDialog.DataContext as Controller_EditAccountDialog;
            dataContext.WindowTitle = "edit account";
            dataContext.AccountData.Clear();
            addDialog.ShowDialog();
            if (addDialog.DialogResult == true)
            {
                AccountItems.Remove(obj.Content as AccountItem);
                AccountItems.Add(dataContext.AccountData[0]);
                OnPropertyChanged("AccountItems");
            }
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
