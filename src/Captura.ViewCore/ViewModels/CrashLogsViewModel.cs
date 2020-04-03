using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CrashLogsViewModel : NotifyPropertyChanged
    {
        public ICommand CopyToClipboardCommand { get; }
        public ICommand RemoveCommand { get; }

        readonly ObservableCollection<FileContentItem> _crashLogs;

        public ReadOnlyObservableCollection<FileContentItem> CrashLogs { get; }

        public CrashLogsViewModel()
        {
            var folder = Path.Combine(ServiceProvider.SettingsDir, "Crashes");

            if (Directory.Exists(folder))
            {
                _crashLogs = new ObservableCollection<FileContentItem>(Directory
                    .EnumerateFiles(folder)
                    .Select(FileName => new FileContentItem(FileName))
                    .Reverse());

                CrashLogs = new ReadOnlyObservableCollection<FileContentItem>(_crashLogs);

                if (CrashLogs.Count > 0)
                {
                    SelectedCrashLog = CrashLogs[0];
                }
            }

            CopyToClipboardCommand = this
                .ObserveProperty(M => M.SelectedCrashLog)
                .Select(M => M != null)
                .ToReactiveCommand()
                .WithSubscribe(() => SelectedCrashLog.Content.WriteToClipboard());

            RemoveCommand = this
                .ObserveProperty(M => M.SelectedCrashLog)
                .Select(M => M != null)
                .ToReactiveCommand()
                .WithSubscribe(OnRemoveExecute);
        }

        void OnRemoveExecute()
        {
            if (SelectedCrashLog == null)
                return;

            if (File.Exists(SelectedCrashLog.FileName))
            {
                File.Delete(SelectedCrashLog.FileName);
            }

            _crashLogs.Remove(SelectedCrashLog);

            SelectedCrashLog = CrashLogs.Count > 0 ? CrashLogs[0] : null;
        }

        FileContentItem _selectedCrashLog;

        public FileContentItem SelectedCrashLog
        {
            get => _selectedCrashLog;
            set => Set(ref _selectedCrashLog, value);
        }
    }
}