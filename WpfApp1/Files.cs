using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace WpfApp1
{
    public class Files : INotifyPropertyChanged
    {
        private List<StartFile> _list;

        public List<StartFile> List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged("List");
            }
        }

        public void FilesFromRegistry()
        {
            _list = new List<StartFile>();
            GetValuesFromRegistry("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            GetValuesFromRegistry("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
            GetValuesFromStartUpMenu();
            OnPropertyChanged("List");
        }

        private void GetValuesFromRegistry(string key)
        {
            using var startupKeyMachine = Registry.LocalMachine.OpenSubKey(key);
            List.AddRange(GetValues(startupKeyMachine, "Registry: machine"));
            using var startupKeyUser = Registry.CurrentUser.OpenSubKey(key);
            List.AddRange(GetValues(startupKeyUser, "Registry: user"));
        }

        private static IEnumerable<StartFile> GetValues(RegistryKey regKey, string type)
        {
            var valueNames = regKey.GetValueNames();
            return (from file in valueNames let path = regKey.GetValue(file).ToString() select new StartFile(file, path, type)).ToList();
        }

        private void  GetValuesFromStartUpMenu()
        {
            var userFilesPath = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            var machineFilesPath =
                Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            List.AddRange(StartUpFilesToList(userFilesPath, "StartUpMenu: user"));
            List.AddRange(StartUpFilesToList(machineFilesPath, "StartUpMenu: machine"));
        }

        private static IEnumerable<StartFile> StartUpFilesToList(IEnumerable<string> filesPath, string type)
        {
            return (from file in filesPath let info = new FileInfo(file) where info.Extension == ".lnk" select new StartFile(info.Name, file, type)).ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}