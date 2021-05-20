using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StartUpAppsViewer
{
    public class ViewModel
    {
        public Files Files { get; set; }
        public ICommand AddCommand { get; } 

        public ViewModel()
        {
            AddCommand = new RelayCommand(OpenDirectory);
            Files = new Files();
            Task.Run( Files.FilesFromRegistry);
        }

        public static void OpenDirectory(object obj)
        {
            var path = (string) obj;
            Process.Start("explorer.exe", Path.GetDirectoryName(path));
        }
    }
}
