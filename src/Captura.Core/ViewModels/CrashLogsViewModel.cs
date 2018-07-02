using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Captura.Models;
using Screna;

namespace Captura.ViewModels
{
    public class CrashLogsViewModel : NotifyPropertyChanged
    {
        public CrashLogsViewModel()
        {
            var folder = Path.Combine(ServiceProvider.SettingsDir, "Crashes");

            if (Directory.Exists(folder))
            {
                CrashLogs = new ObservableCollection<FileContentItem>(Directory
                    .EnumerateFiles(folder)
                    .Select(FileName => new FileContentItem(FileName))
                    .Reverse());

                if (CrashLogs.Count > 0)
                {
                    SelectedCrashLog = CrashLogs[0];
                }
            }

            CopyToClipboardCommand = new DelegateCommand(() => SelectedCrashLog?.Content.WriteToClipboard());

            RemoveCommand = new DelegateCommand(OnRemoveExecute);
        }

        void OnRemoveExecute()
        {
            if (SelectedCrashLog != null)
            {
                if (File.Exists(SelectedCrashLog.FileName))
                {
                    File.Delete(SelectedCrashLog.FileName);
                }

                CrashLogs.Remove(SelectedCrashLog);

                SelectedCrashLog = CrashLogs.Count > 0 ? CrashLogs[0] : null;
            }
        }

        public ObservableCollection<FileContentItem> CrashLogs { get; }

        FileContentItem _selectedCrashLog;

        public FileContentItem SelectedCrashLog
        {
            get => _selectedCrashLog;
            set
            {
                _selectedCrashLog = value;
                
                OnPropertyChanged();
            }
        }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand RemoveCommand { get; }
    }
}