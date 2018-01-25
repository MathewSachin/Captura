using System.Collections.Generic;
using Captura.Models;
using System.Collections.ObjectModel;

namespace Captura.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        readonly IRegionProvider _regionProvider;

        public VideoViewModel(IRegionProvider RegionProvider,
            IEnumerable<IImageWriterItem> ImageWriters,
            IEnumerable<IVideoWriterProvider> VideoWriterProviders,
            IEnumerable<IVideoSourceProvider> VideoSourceProviders,
            Settings Settings,
            LanguageManager LanguageManager) : base(Settings, LanguageManager)
        {
            AvailableVideoWriterKinds = new ReadOnlyObservableCollection<IVideoWriterProvider>(_videoWriterKinds);
            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            AvailableVideoSourceKinds = new ReadOnlyObservableCollection<IVideoSourceProvider>(_videoSourceKinds);
            AvailableVideoSources = new ReadOnlyObservableCollection<IVideoItem>(_videoSources);

            AvailableImageWriters = new ReadOnlyObservableCollection<IImageWriterItem>(_imageWriters);

            _regionProvider = RegionProvider;

            foreach (var imageWriter in ImageWriters)
            {
                _imageWriters.Add(imageWriter);
            }

            foreach (var videoWriterProvider in VideoWriterProviders)
            {
                _videoWriterKinds.Add(videoWriterProvider);
            }

            foreach (var videoSourceProvider in VideoSourceProviders)
            {
                _videoSourceKinds.Add(videoSourceProvider);
            }
            
            if (AvailableImageWriters.Count > 0)
                SelectedImageWriter = AvailableImageWriters[0];

            if (AvailableVideoWriterKinds.Count > 0)
                SelectedVideoWriterKind = AvailableVideoWriterKinds[0];

            if (AvailableVideoSourceKinds.Count > 0)
                SelectedVideoSourceKind = AvailableVideoSourceKinds[0];
        }

        public void Init()
        {                                               
            RefreshCodecs();

            RefreshVideoSources();
            
            _regionProvider.SelectorHidden += () =>
            {
                if (SelectedVideoSourceKind is RegionSourceProvider)
                    SelectedVideoSourceKind = AvailableVideoSourceKinds[0];
            };
        }
        
        public void RefreshVideoSources()
        {
            _videoSources.Clear();

            // RegionSelector should only be shown on Region Capture.
            _regionProvider.SelectorVisible = SelectedVideoSourceKind is RegionSourceProvider;

            foreach (var source in SelectedVideoSourceKind)
            {
                _videoSources.Add(source);
            }

            // Set first source as default
            if (AvailableVideoSources.Count > 0)
                SelectedVideoSource = AvailableVideoSources[0];
        }

        public void RefreshCodecs()
        {
            // Available Codecs
            _videoWriters.Clear();

            foreach (var writerItem in SelectedVideoWriterKind)
            {
                _videoWriters.Add(writerItem);
            }

            if (_videoWriters.Count > 0)
                SelectedVideoWriter = _videoWriters[0];
        }

        readonly ObservableCollection<IVideoWriterProvider> _videoWriterKinds = new ObservableCollection<IVideoWriterProvider>();

        public ReadOnlyObservableCollection<IVideoWriterProvider> AvailableVideoWriterKinds { get; }

        readonly ObservableCollection<IVideoWriterItem> _videoWriters = new ObservableCollection<IVideoWriterItem>();

        public ReadOnlyObservableCollection<IVideoWriterItem> AvailableVideoWriters { get; }
        
        IVideoWriterProvider _writerKind;

        public IVideoWriterProvider SelectedVideoWriterKind
        {
            get => _writerKind;
            set
            {
                if (_writerKind == value)
                    return;

                _writerKind = value;

                OnPropertyChanged();

                RefreshCodecs();
            }
        }

        readonly ObservableCollection<IVideoSourceProvider> _videoSourceKinds = new ObservableCollection<IVideoSourceProvider>();

        public ReadOnlyObservableCollection<IVideoSourceProvider> AvailableVideoSourceKinds { get; }

        readonly ObservableCollection<IVideoItem> _videoSources = new ObservableCollection<IVideoItem>();

        public ReadOnlyObservableCollection<IVideoItem> AvailableVideoSources { get; }

        IVideoSourceProvider _videoSourceKind;

        public IVideoSourceProvider SelectedVideoSourceKind
        {
            get => _videoSourceKind;
            set
            {
                if (_videoSourceKind == value)
                    return;

                _videoSourceKind = value;
                
                OnPropertyChanged();

                RefreshVideoSources();
            }
        }

        IVideoItem _videoSource = FullScreenItem.Instance;

        public IVideoItem SelectedVideoSource
        {
            get => _videoSource;
            set
            {
                if (value == null && AvailableVideoSources.Count > 0)
                    value = AvailableVideoSources[0];

                _videoSource = value;

                OnPropertyChanged();
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

        readonly ObservableCollection<IImageWriterItem> _imageWriters = new ObservableCollection<IImageWriterItem>();

        public ReadOnlyObservableCollection<IImageWriterItem> AvailableImageWriters { get; }

        IImageWriterItem _imgWriter;

        public IImageWriterItem SelectedImageWriter
        {
            get => _imgWriter;
            set
            {
                _imgWriter = value;

                OnPropertyChanged();
            }
        }
    }
}