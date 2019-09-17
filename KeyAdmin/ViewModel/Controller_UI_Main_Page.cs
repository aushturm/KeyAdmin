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
using System.Security;
using System.Windows.Input;
using System.Windows;
using KeyAdmin.Properties;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Timers;

namespace KeyAdmin.ViewModel
{
    class Controller_UI_Main_Page : IUIPages, INotifyPropertyChanged
    {
        #region members
        private ObservableCollection<AccountItem> _accountItems = new ObservableCollection<AccountItem>();
        private List<AccountItem> _accountItemsOriginal = new List<AccountItem>();
        private RelayCommand _addAccountDetails;
        private RelayCommand _plainExport;
        private RelayCommand<ListViewItem> _deleteAccountDetails;
        private RelayCommand<ListViewItem> _copyPwToClipboard;
        private RelayCommand<ListViewItem> _editAccountDetails;
        private RelayCommand<RoutedEventArgs> _pageLoaded;
        private RelayCommand _plainImport;
        private RelayCommand<bool> _cryptoImport;
        private RelayCommand<bool> _cryptoExport;
        private RelayCommand _SearchQueryChanged;
        private string _searchQuery;
        private object[] _parameters;
        private Dictionary<string, string> _passwordStore = new Dictionary<string, string>();
        private List<string> _pwAlias = new List<string> { "password", "passwort", "pw" };
        private const string _pwPlaceholder = "***;)***";
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

        public string SearchQuery
        {
            get
            {
                return _searchQuery;
            }
            set
            {
                _searchQuery = value;
                OnPropertyChanged("SearchQuery");
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
        public RelayCommand PlainExport
        {
            get { return _plainExport; }
        }
        public RelayCommand SearchQueryChanged
        {
            get { return _SearchQueryChanged; }
        }
        public RelayCommand PlainImport
        {
            get { return _plainImport; }
        }
        public RelayCommand<bool> CryptoImport
        {
            get { return _cryptoImport; }
        }
        public RelayCommand<bool> CryptoExport
        {
            get { return _cryptoExport; }
        }
        public RelayCommand<ListViewItem> DeleteAccountDetails
        {
            get { return _deleteAccountDetails; }
        }
        public RelayCommand<ListViewItem> CopyPwToClipboard
        {
            get { return _copyPwToClipboard; }
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
            _plainExport = new RelayCommand(ExportHandler);
            _deleteAccountDetails = new RelayCommand<ListViewItem>(DeleteAccountDetailsHandler);
            _copyPwToClipboard = new RelayCommand<ListViewItem>(CopyPwToClipboardHandler);
            _editAccountDetails = new RelayCommand<ListViewItem>(EditAccountDetailsHandler);
            _pageLoaded = new RelayCommand<RoutedEventArgs>(PageLoadedHandler);
            _plainImport = new RelayCommand(ImportHandler);
            _SearchQueryChanged = new RelayCommand(SearchQueryChangedHandler);
            _cryptoExport = new RelayCommand<bool>(CryptoExportHandler);
            _cryptoImport = new RelayCommand<bool>(CryptoImportHandler);

            if (!string.IsNullOrWhiteSpace(Settings.Default.DefaultFilePath) && File.Exists(Settings.Default.DefaultFilePath))
                CryptoImportHandler();
            else
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Settings.Default.DefaultFilePath = path + $"\\{DateTime.Now.ToString("yyyyMMdd")}_Exported_Passwords_KeyAdmin.xml";
                File.Create(Settings.Default.DefaultFilePath);
                Settings.Default.Save();
            }
        }
        #endregion

        #region event handlers

        private void CopyPwToClipboardHandler(ListViewItem obj)
        {
            if (obj.Content is AccountItem item)
            {
                if (_passwordStore.TryGetValue(item.Guid, out string pw))
                {
                    Clipboard.SetText(pw.Decrypt(null));
                }
            }
        }

        private void CryptoImportHandler(bool isSystem = true)
        {
            if (!isSystem)
            {
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Title = "Select the file containing your xml to import.";
                    openFileDialog.CheckPathExists = true;
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                        return;
                    Settings.Default.DefaultFilePath = openFileDialog.FileName;
                    Settings.Default.Save();
                }
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DeserializeItemHolder));
                using (FileStream fileStream = new FileStream(Settings.Default.DefaultFilePath, FileMode.Open))
                {
                    DeserializeItemHolder items;
                    try
                    {
                        items = (DeserializeItemHolder)serializer.Deserialize(fileStream);
                    }
                    catch (InvalidOperationException) {return; }//document is empty

                    _accountItems.Clear();
                    _accountItemsOriginal.Clear();
                    foreach (AccountItem item in items.Data)
                    {
                        _accountItems.Add(item);
                    }
                    DecryptItems();
                    _accountItemsOriginal = new List<AccountItem>(_accountItems.ToList());
                }

                $"Successfully imported Accountinformation from: {Settings.Default.DefaultFilePath}".ShowInfoMessage();
            }
            catch (Exception ex)
            {
                $"Failed to import Accountinformation: {ex.Message}".ShowErrorMessage();
            }
        }

        private void CryptoExportHandler(bool isSystem = true)
        {
            if (!isSystem)
            {
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
                    Settings.Default.DefaultFilePath = saveFileDialog.FileName;
                    Settings.Default.Save();
                }
            }

            foreach (AccountItem item in _accountItemsOriginal)
            {
                string itemId = item.Identifier.Encrypt(null);
                item.Identifier = itemId ?? item.Identifier;
                foreach (AccountPropertiesItem propertie in item.Properties)
                {
                    string value = propertie.Value.Encrypt(null);
                    string identifier = propertie.Identifier.Encrypt(null);
                    propertie.Value = value ?? propertie.Value;
                    propertie.Identifier = identifier ?? propertie.Identifier;
                }
            }
            DeserializeItemHolder container = new DeserializeItemHolder
            {
                Data = _accountItemsOriginal
            };
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DeserializeItemHolder));
                using (FileStream fileStream = new FileStream(Settings.Default.DefaultFilePath, FileMode.Create))
                {
                    serializer.Serialize(fileStream, container);
                }

                $"Successfully exported Accountinformation to: {Settings.Default.DefaultFilePath}".ShowInfoMessage();
            }

            catch (Exception ex)
            {
                $"Failed to export Accountinformation: {ex.Message}".ShowErrorMessage();
            }
        }

        private void SearchQueryChangedHandler()
        {
            if (SearchQuery == string.Empty)
            {
                InsertIntoAccountItems(_accountItemsOriginal);
                return;
            }

            List<AccountItem> filteredItems = new List<AccountItem>(_accountItemsOriginal.Where
                                                                    (x => x.Identifier.ToLower().Trim().Contains(
                                                                        SearchQuery.ToLower().Trim())));
            InsertIntoAccountItems(filteredItems);
        }

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
                CryptoExportHandler();
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
                AccountItem item = dataContext.AccountData[0];
                AccountPropertiesItem pwProperty = item.Properties.FirstOrDefault(x => _pwAlias.Contains(x.Identifier));
                _passwordStore.Add(item.Guid, pwProperty.Value.Encrypt(null));
                pwProperty.Value = _pwPlaceholder;
                AccountItems.Add(dataContext.AccountData[0]);
                _accountItemsOriginal.Add(dataContext.AccountData[0]);
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
                    _accountItemsOriginal.Clear();
                    foreach (AccountItem item in items.Data)
                    {
                        _accountItemsOriginal.Add((AccountItem)item.Clone());
                        _accountItems.Add(item);
                    }
                }

                Settings.Default.DefaultFilePath = Path.GetDirectoryName(fileName) + $@"\{DateTime.Now.ToString("yyyyMMdd")}_Passwords_KeyAdmin.xml";
                Settings.Default.Save();

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
                Data = _accountItemsOriginal
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
            if (obj.Content is AccountItem item)
            {
                _accountItemsOriginal.Remove(_accountItemsOriginal.First(x => x.Guid == item.Guid));
                _accountItems.Remove(item);
                OnPropertyChanged("AccountItems");
            }
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
                AccountItem accItem = dataContext.AccountData[0];
                AccountPropertiesItem pwProperty = accItem.Properties.FirstOrDefault(x => _pwAlias.Contains(x.Identifier));
                _passwordStore.Add(accItem.Guid, pwProperty.Value.Encrypt(null));
                pwProperty.Value = _pwPlaceholder;
                AccountItems.Add(dataContext.AccountData[0]);
                _accountItemsOriginal.Remove(_accountItemsOriginal.First(x => x.Guid == item.Guid));
                _accountItemsOriginal.Add(dataContext.AccountData[0]);
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
        private void InsertIntoAccountItems(List<AccountItem> values)
        {
            _accountItems.Clear();
            values.ForEach(x => _accountItems.Add(x));
        }

        private void DecryptItems()
        {
            _passwordStore.Clear();
            foreach (AccountItem item in _accountItems)
            {
                var itemId = item.Identifier.Decrypt(null);
                item.Identifier = itemId ?? item.Identifier;
                foreach (AccountPropertiesItem propertie in item.Properties)
                {
                    var id = propertie.Identifier.Decrypt(null);
                    propertie.Identifier = id ?? propertie.Identifier;
                    string cleanedId = propertie.Identifier.Trim().ToLower();
                    if (_pwAlias.Contains(cleanedId))
                    {
                        propertie.Value = _pwPlaceholder;
                        _passwordStore.Add(item.Guid, propertie.Value);
                    }
                    else
                    {
                        var value = propertie.Value.Decrypt(null);
                        propertie.Value = value ?? propertie.Value;
                    }
                }
            }
        }
        #endregion
    }
}
