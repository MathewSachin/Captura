using System.Collections.ObjectModel;

namespace Captura.Models
{
    public class FFMpegLog : NotifyPropertyChanged
    {
        FFMpegLog()
        {
            LogItems = new ReadOnlyObservableCollection<FFMpegLogItem>(_logItems);
        }

        readonly ObservableCollection<FFMpegLogItem> _logItems = new ObservableCollection<FFMpegLogItem>();

        public ReadOnlyObservableCollection<FFMpegLogItem> LogItems { get; }

        public FFMpegLogItem CreateNew(string Name)
        {
            var item = new FFMpegLogItem(Name);

            item.RemoveRequested += () => _logItems.Remove(item);

            _logItems.Insert(0, item);

            return item;
        }

        public static FFMpegLog Instance { get; } = new FFMpegLog();
    }
}
