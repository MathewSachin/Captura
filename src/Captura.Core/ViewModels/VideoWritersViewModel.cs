using System.Collections.Generic;
using Captura.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VideoWritersViewModel : NotifyPropertyChanged, IRefreshable
    {
        public IReadOnlyList<IVideoWriterProvider> VideoWriterProviders { get; }
        readonly ObservableCollection<IVideoWriterItem> _videoWriters = new ObservableCollection<IVideoWriterItem>();
        public ReadOnlyObservableCollection<IVideoWriterItem> AvailableVideoWriters { get; }

        public VideoWritersViewModel(IEnumerable<IVideoWriterProvider> WriterProviders)
        {
            VideoWriterProviders = WriterProviders.ToList();

            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            if (VideoWriterProviders.Count > 0)
                SelectedVideoWriterKind = VideoWriterProviders[0];
        }

        public void RefreshCodecs()
        {
            // Remember selected codec
            var lastVideoCodecName = SelectedVideoWriter?.ToString();

            _videoWriters.Clear();

            foreach (var writerItem in SelectedVideoWriterKind)
            {
                _videoWriters.Add(writerItem);
            }

            // Set First
            if (_videoWriters.Count > 0)
                SelectedVideoWriter = _videoWriters[0];

            var matchingVideoCodec = AvailableVideoWriters.FirstOrDefault(M => M.ToString() == lastVideoCodecName);

            if (matchingVideoCodec != null)
            {
                SelectedVideoWriter = matchingVideoCodec;
            }
        }

        void IRefreshable.Refresh() => RefreshCodecs();

        IVideoWriterProvider _writerKind;

        public IVideoWriterProvider SelectedVideoWriterKind
        {
            get => _writerKind;
            set
            {
                if (_writerKind == value)
                    return;

                if (value != null)
                    _writerKind = value;

                OnPropertyChanged();

                RefreshCodecs();
            }
        }

        IVideoWriterItem _writer;

        public IVideoWriterItem SelectedVideoWriter
        {
            get => _writer;
            set
            {
                _writer = value ?? (AvailableVideoWriters.Count == 0 ? null : AvailableVideoWriters[0]);

                OnPropertyChanged();
            }
        }
    }
}