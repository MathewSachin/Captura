using System.Collections.Generic;
using Captura.Models;
using Screna;
using System.Collections.ObjectModel;

namespace Captura.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        readonly IRegionProvider _regionProvider;

        public VideoViewModel(IRegionProvider RegionProvider,
            IEnumerable<IImageWriterItem> ImageWriters,
            IEnumerable<IVideoWriterProvider> VideoWriterProviders,
            Settings Settings,
            LanguageManager LanguageManager) : base(Settings, LanguageManager)
        {
            AvailableVideoWriterKinds = new ReadOnlyObservableCollection<IVideoWriterProvider>(_videoWriterKinds);
            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            AvailableVideoSourceKinds = new ReadOnlyObservableCollection<ObjectLocalizer<VideoSourceKind>>(_videoSourceKinds);
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
            
            if (AvailableImageWriters.Count > 0)
                SelectedImageWriter = AvailableImageWriters[0];

            if (AvailableVideoWriterKinds.Count > 0)
                SelectedVideoWriterKind = AvailableVideoWriterKinds[0];
        }

        public void Init()
        {                                               
            RefreshCodecs();

            RefreshVideoSources();
            
            _regionProvider.SelectorHidden += () =>
            {
                if (SelectedVideoSourceKind == VideoSourceKind.Region)
                    SelectedVideoSourceKind = VideoSourceKind.Screen;
            };
        }
        
        public void RefreshVideoSources()
        {
            _videoSources.Clear();

            // RegionSelector should only be shown on Region Capture.
            _regionProvider.SelectorVisible = SelectedVideoSourceKind == VideoSourceKind.Region;

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    _videoSources.Add(WindowItem.TaskBar);
                    
                    foreach (var win in Window.EnumerateVisible())
                        _videoSources.Add(new WindowItem(win));
                    break;

                case VideoSourceKind.DesktopDuplication:
                    foreach (var screen in ScreenItem.Enumerate(true))
                        _videoSources.Add(screen);
                    break;

                case VideoSourceKind.Screen:
                    _videoSources.Add(FullScreenItem.Instance);

                    foreach (var screen in ScreenItem.Enumerate(false))
                        _videoSources.Add(screen);
                    break;

                case VideoSourceKind.Region:
                    _videoSources.Add(_regionProvider.VideoSource);
                    break;

                case VideoSourceKind.NoVideo:
                    _videoSources.Add(WaveItem.Instance);

                    foreach (var item in FFMpegAudioItem.Items)
                        _videoSources.Add(item);

                    break;
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
        
        readonly ObservableCollection<ObjectLocalizer<VideoSourceKind>> _videoSourceKinds = new ObservableCollection<ObjectLocalizer<VideoSourceKind>>
        {
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.NoVideo, nameof(LanguageManager.OnlyAudio)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Screen, nameof(LanguageManager.Screen)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.DesktopDuplication, nameof(LanguageManager.DesktopDuplication)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Window, nameof(LanguageManager.Window)),
            new ObjectLocalizer<VideoSourceKind>(VideoSourceKind.Region, nameof(LanguageManager.Region))
        };

        public ReadOnlyObservableCollection<ObjectLocalizer<VideoSourceKind>> AvailableVideoSourceKinds { get; }

        readonly ObservableCollection<IVideoItem> _videoSources = new ObservableCollection<IVideoItem>();

        public ReadOnlyObservableCollection<IVideoItem> AvailableVideoSources { get; }

        VideoSourceKind _videoSourceKind = VideoSourceKind.Screen;

        public VideoSourceKind SelectedVideoSourceKind
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