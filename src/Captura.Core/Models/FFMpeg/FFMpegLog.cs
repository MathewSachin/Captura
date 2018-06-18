using System.Collections.ObjectModel;
using System.Threading;

namespace Captura.Models
{
    public class FFmpegLog : NotifyPropertyChanged
    {
        readonly SynchronizationContext _syncContext;

        FFmpegLog()
        {
            _syncContext = SynchronizationContext.Current;

            LogItems = new ReadOnlyObservableCollection<FFmpegLogItem>(_logItems);
        }

        readonly ObservableCollection<FFmpegLogItem> _logItems = new ObservableCollection<FFmpegLogItem>();

        public ReadOnlyObservableCollection<FFmpegLogItem> LogItems { get; }

        public FFmpegLogItem CreateNew(string Name)
        {
            var item = new FFmpegLogItem(Name);

            item.RemoveRequested += () => _logItems.Remove(item);

            _syncContext.Post(S => _logItems.Insert(0, item), null);

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

        public static FFmpegLog Instance { get; } = new FFmpegLog();
    }
}
