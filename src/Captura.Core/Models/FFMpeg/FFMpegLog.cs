using System.Collections.ObjectModel;
using System.Threading;

namespace Captura.Models
{
    public class FFMpegLog : NotifyPropertyChanged
    {
        readonly SynchronizationContext _syncContext;

        FFMpegLog()
        {
            _syncContext = SynchronizationContext.Current;

            LogItems = new ReadOnlyObservableCollection<FFMpegLogItem>(_logItems);
        }

        readonly ObservableCollection<FFMpegLogItem> _logItems = new ObservableCollection<FFMpegLogItem>();

        public ReadOnlyObservableCollection<FFMpegLogItem> LogItems { get; }

        public FFMpegLogItem CreateNew(string Name)
        {
            var item = new FFMpegLogItem(Name);

            item.RemoveRequested += () => _logItems.Remove(item);

            _syncContext.Post(S => _logItems.Insert(0, item), null);

            SelectedLogItem = item;

            return item;
        }

        FFMpegLogItem _selectedLogItem;

        public FFMpegLogItem SelectedLogItem
        {
            get => _selectedLogItem;
            set
            {
                _selectedLogItem = value; 
                
                OnPropertyChanged();
            }
        }

        public static FFMpegLog Instance { get; } = new FFMpegLog();
    }
}
