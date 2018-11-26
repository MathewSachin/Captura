using System.Collections.Generic;
using Captura.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VideoViewModel : ViewModelBase
    {
        readonly FullScreenSourceProvider _fullScreenProvider;
        public NoVideoSourceProvider NoVideoSourceProvider { get; }

        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public IEnumerable<IVideoSourceProvider> VideoSources { get; }
        public IReadOnlyList<IImageWriterItem> AvailableImageWriters { get; }

        public IReadOnlyList<IVideoWriterProvider> VideoWriterProviders { get; }
        readonly ObservableCollection<IVideoWriterItem> _videoWriters = new ObservableCollection<IVideoWriterItem>();
        public ReadOnlyObservableCollection<IVideoWriterItem> AvailableVideoWriters { get; }

        public VideoViewModel(IEnumerable<IImageWriterItem> ImageWriters,
            Settings Settings,
            LanguageManager LanguageManager,
            FullScreenSourceProvider FullScreenProvider,
            NoVideoSourceProvider NoVideoSourceProvider,
            IEnumerable<IVideoSourceProvider> SourceProviders,
            IEnumerable<IVideoWriterProvider> WriterProviders)
            : base(Settings, LanguageManager)
        {
            this.NoVideoSourceProvider = NoVideoSourceProvider;
            _fullScreenProvider = FullScreenProvider;

            AvailableImageWriters = ImageWriters.ToList();
            VideoWriterProviders = WriterProviders.ToList();
            VideoSources = SourceProviders;

            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            SetDefaultSource();

            if (!AvailableImageWriters.Any(M => M.Active))
                AvailableImageWriters[0].Active = true;

            if (VideoWriterProviders.Count > 0)
                SelectedVideoWriterKind = VideoWriterProviders[0];
        }

        public void SetDefaultSource()
        {
            SelectedVideoSourceKind = _fullScreenProvider;
        }

        void ChangeSource(IVideoSourceProvider NewSourceProvider, bool CallOnSelect)
        {
            try
            {
                if (NewSourceProvider == null || _videoSourceKind == NewSourceProvider)
                    return;

                if (CallOnSelect && !NewSourceProvider.OnSelect())
                {
                    return;
                }

                if (_videoSourceKind != null)
                {
                    _videoSourceKind.OnUnselect();

                    _videoSourceKind.UnselectRequested -= SetDefaultSource;
                }

                _videoSourceKind = NewSourceProvider;

                _videoSourceKind.UnselectRequested += SetDefaultSource;
            }
            finally
            {
                // Important to send PropertyChanged event over SynchronizationContext for consistency in UI

                void PropChange()
                {
                    RaisePropertyChanged(nameof(SelectedVideoSourceKind));
                }

                if (_syncContext != null)
                {
                    _syncContext.Post(S => PropChange(), null);
                }
                else PropChange();
            }
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

                if (_syncContext != null)
                    _syncContext.Post(S => RaisePropertyChanged(nameof(SelectedVideoWriterKind)), null);
                else OnPropertyChanged();

                RefreshCodecs();
            }
        }

        IVideoSourceProvider _videoSourceKind;

        public IVideoSourceProvider SelectedVideoSourceKind
        {
            get => _videoSourceKind;
            set => ChangeSource(value, true);
        }

        public void RestoreSourceKind(IVideoSourceProvider SourceProvider)
        {
            ChangeSource(SourceProvider, false);
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