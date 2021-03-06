﻿using System;
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
using System.Security;
using System.Windows.Input;
using System.Windows;
using KeyAdmin.Properties;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace KeyAdmin.ViewModel
{
    class Controller_UI_Main_Page : IUIPages, INotifyPropertyChanged
    {
        #region members
        private ObservableCollection<AccountItem> _accountItems = Properties.Settings.Default.AccountItems;
        private RelayCommand _addAccountDetails;
        private RelayCommand _export;
        private RelayCommand<ListViewItem> _deleteAccountDetails;
        private RelayCommand<ListViewItem> _editAccountDetails;
        private RelayCommand<RoutedEventArgs> _pageLoaded;
        private RelayCommand _import;
        private object[] _parameters;
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

        public object[] Parameters
        {
            set
            {
                _parameters = value;
            }
        }

        public RelayCommand<RoutedEventArgs> PageLoaded
        {
            get { return _pageLoaded; }
        }

        public RelayCommand AddAccountDetails
        {
            get { return _addAccountDetails; }
        }
        public RelayCommand Export
        {
            get { return _export; }
        }
        public RelayCommand Import
        {
            get { return _import; }
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
            _export = new RelayCommand(ExportHandler);
            _deleteAccountDetails = new RelayCommand<ListViewItem>(DeleteAccountDetailsHandler);
            _editAccountDetails = new RelayCommand<ListViewItem>(EditAccountDetailsHandler);
            _pageLoaded = new RelayCommand<RoutedEventArgs>(PageLoadedHandler);
            _import = new RelayCommand(ImportHandler);

            DecryptItems();
        }
        #endregion

        #region event handlers

        private void PageLoadedHandler(RoutedEventArgs e)
        {
            var control = e.OriginalSource as UserControl;
            var win = Window.GetWindow(control);
            win.Closed += Win_Closed;
            win.KeyDown += Win_KeyDown;
        }

        private void Win_Closed(object sender, System.EventArgs e)
        {
            System.Windows.Forms.DialogResult result = GeneralExtensions.ShowDecisionMessage("Do you want to save changes?");
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (AccountItem item in Settings.Default.AccountItems)
                {
                    string itemId = item.Identifier.ToSecureString().EncryptString(null);
                    item.Identifier = (itemId != null) ? itemId : item.Identifier;
                    foreach (AccountPropertiesItem propertie in item.Properties)
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

        private void Win_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AddAccountDetailsHandler();
        }

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

        private void ImportHandler()
        {
            string fileName;
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Title = "Select the file containing your xml to import.";
                openFileDialog.CheckPathExists = true;
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;
                fileName = openFileDialog.FileName;
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DeserializeItemHolder));
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                {
                    DeserializeItemHolder items = (DeserializeItemHolder)serializer.Deserialize(fileStream);
                    _accountItems.Clear();
                    foreach (AccountItem item in items.Data)
                    {
                        _accountItems.Add(item);
                    }
                }

                $"Successfully imported Accountinformation from: {fileName}".ShowInfoMessage();
            }
            catch (Exception ex)
            {
                $"Failed to import Accountinformation: {ex.Message}".ShowErrorMessage();
            }
        }

        private void ExportHandler()
        {
            string folderPath;
            using (System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveFileDialog.Title = "Choose a folder to export into";
                saveFileDialog.FileName = $"{DateTime.Now.ToString("yyyyMMdd")}_Exported_Passwords_KeyAdmin.xml";
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = ".xml";
                saveFileDialog.Filter = "*.xml|*.*";
                saveFileDialog.CheckPathExists = true;
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;
                folderPath = saveFileDialog.FileName;
            }

            DeserializeItemHolder container = new DeserializeItemHolder
            {
                Data = _accountItems.ToList()
            };
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DeserializeItemHolder));
                using (FileStream fileStream = new FileStream(folderPath, FileMode.CreateNew))
                {
                    serializer.Serialize(fileStream, container);
                }

                $"Successfully exported Accountinformation to: {folderPath}".ShowInfoMessage();
            }

            catch (Exception ex)
            {
                $"Failed to export Accountinformation: {ex.Message}".ShowErrorMessage();
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
            AccountItem item = obj.Content as AccountItem;
            if (item == null)
                return;
            dataContext.AccountData.Add((AccountItem)item.Clone());
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

        #region private functions
        private void DecryptItems()
        {
            foreach (AccountItem item in _accountItems)
            {
                var itemId = item.Identifier.DecryptString(null);
                item.Identifier = (itemId != null) ? itemId.ToInsecureString() : item.Identifier;
                foreach (AccountPropertiesItem propertie in item.Properties)
                {
                    var value = propertie.Value.DecryptString(null);
                    propertie.Value = (value != null) ? value.ToInsecureString() : propertie.Value;
                    var id = propertie.Identifier.DecryptString(null);
                    propertie.Identifier = (id != null) ? id.ToInsecureString() : propertie.Identifier;
                }
            }
        }
        #endregion
    }
}
