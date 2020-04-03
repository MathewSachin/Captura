using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Captura.FFmpeg
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegLog : NotifyPropertyChanged, IFFmpegLogRepository
    {
        public FFmpegLog()
        {
            LogItems = new ReadOnlyObservableCollection<FFmpegLogItem>(_logItems);
        }

        readonly ObservableCollection<FFmpegLogItem> _logItems = new ObservableCollection<FFmpegLogItem>();

        public ReadOnlyObservableCollection<FFmpegLogItem> LogItems { get; }

        public FFmpegLogItem CreateNew(string Name, string Args)
        {
            var item = new FFmpegLogItem(Name, Args);

            _logItems.Insert(0, item);

            return item;
        }

        public void Remove(FFmpegLogItem Item)
        {
            _logItems.Remove(Item);
        }

        public IEnumerator<IFFmpegLogEntry> GetEnumerator() => _logItems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IFFmpegLogEntry IFFmpegLogRepository.CreateNew(string Name, string Args) => CreateNew(Name, Args);

        void IFFmpegLogRepository.Remove(IFFmpegLogEntry Entry)
        {
            if (Entry is FFmpegLogItem logItem)
            {
                Remove(logItem);
            }
        }
    }
}
