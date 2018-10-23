using System.Collections.ObjectModel;
using System.Threading;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegLog : NotifyPropertyChanged
    {
        readonly SynchronizationContext _syncContext;

        readonly IClipboardService _clipboardService;

        public FFmpegLog(IClipboardService ClipboardService)
        {
            _clipboardService = ClipboardService;
            _syncContext = SynchronizationContext.Current;

            LogItems = new ReadOnlyObservableCollection<FFmpegLogItem>(_logItems);
        }

        readonly ObservableCollection<FFmpegLogItem> _logItems = new ObservableCollection<FFmpegLogItem>();

        public ReadOnlyObservableCollection<FFmpegLogItem> LogItems { get; }

        public FFmpegLogItem CreateNew(string Name)
        {
            var item = new FFmpegLogItem(Name, _clipboardService);

            item.RemoveRequested += () => _logItems.Remove(item);

            void Insert()
            {
                _logItems.Insert(0, item);
            }

            if (_syncContext != null)
            {
                _syncContext.Post(S => Insert(), null);
            }
            else Insert();

            SelectedLogItem = item;

            return item;
        }

        FFmpegLogItem _selectedLogItem;

        public FFmpegLogItem SelectedLogItem
        {
            get => _selectedLogItem;
            set
            {
                _selectedLogItem = value; 
                
                OnPropertyChanged();
            }
        }
    }
}
