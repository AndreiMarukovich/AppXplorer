using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using AppXplorer.Model;

namespace AppXplorer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        readonly Dictionary<string, string> _apps = new Dictionary<string, string>();

        public ObservableCollection<string> AppsList { get; set; }

        private string _selectedItem;
        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                ShowDetails(_selectedItem);
                OnPropertyChanged("SelectedItem");
            }
        }

        private string _appDetails;
        public string AppDetails
        {
            get { return _appDetails; }
            set
            {
               _appDetails = value;
               OnPropertyChanged("AppDetails");
            }
        }

        public MainViewModel()
        {
            try
            {
                PopulateAppList();
            }
            catch(Exception e)
            {
                MessageBox.Show("Can't collect app information. Please run application as Administrator.");
                MessageBox.Show(e.Message);
            }
        }

        private void ShowDetails(string selectedItem)
        {
            if (String.IsNullOrWhiteSpace(selectedItem))
                return;

            string appInfo;
            if (_apps.TryGetValue(selectedItem, out appInfo))
            {
                AppDetails = appInfo;
            }
        }

        private void PopulateAppList()
        {
            AppsList = new ObservableCollection<string>();

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WindowsApps");
            var folders = Directory.GetDirectories(path);

            foreach (var folder in folders)
            {
                var manifestPath = Path.Combine(folder, "AppxManifest.xml");
                if (!File.Exists(manifestPath))
                    continue;

                var appInfo = AppInfo.ParseManifest(manifestPath);
                foreach (var info in appInfo)
                {
                    AppsList.Add(info.Name);
                    _apps[info.Name] = info.ToString();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}