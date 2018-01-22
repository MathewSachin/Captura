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
            Settings Settings,
            LanguageManager LanguageManager) : base(Settings, LanguageManager)
        {
            AvailableVideoWriterKinds = new ReadOnlyObservableCollection<VideoWriterKind>(_videoWriterKinds);
            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            AvailableVideoSourceKinds = new ReadOnlyObservableCollection<ObjectLocalizer<VideoSourceKind>>(_videoSourceKinds);
            AvailableVideoSources = new ReadOnlyObservableCollection<IVideoItem>(_videoSources);

            AvailableImageWriters = new ReadOnlyObservableCollection<IImageWriterItem>(_imageWriters);

            _regionProvider = RegionProvider;

            foreach (var imageWriter in ImageWriters)
            {
                _imageWriters.Add(imageWriter);
            }
            
            SelectedImageWriter = AvailableImageWriters[0];
        }

        public void Init()
        {
            // Check if SharpAvi is available
            if (ServiceProvider.FileExists("SharpAvi.dll"))
            {
                _videoWriterKinds.Add(VideoWriterKind.SharpAvi);
            }
                                               
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
        
        void InitSharpAviCodecs()
        {
            foreach (var codec in AviWriter.EnumerateEncoders())
            {
                var item = new SharpAviItem(codec);

                _videoWriters.Add(item);

                // Set MotionJpeg as default
                if (codec == AviCodec.MotionJpeg)
                    SelectedVideoWriter = item;
            }
        }

        public void RefreshCodecs()
        {
            // Available Codecs
            _videoWriters.Clear();

            switch (SelectedVideoWriterKind)
            {
                case VideoWriterKind.SharpAvi:
                    InitSharpAviCodecs();
                    break;

                case VideoWriterKind.Gif:
                    _videoWriters.Add(GifItem.Instance);

                    SelectedVideoWriter = GifItem.Instance;
                    break;

                case VideoWriterKind.FFMpeg:
                    foreach (var item in FFMpegItem.Items)
                        _videoWriters.Add(item);

                    SelectedVideoWriter = AvailableVideoWriters[0];
                    break;

                case VideoWriterKind.Streaming_Alpha:
                    foreach (var item in StreamingItem.StreamingItems)
                        _videoWriters.Add(item);

                    SelectedVideoWriter = AvailableVideoWriters[0];
                    break;
            }
        }

        readonly ObservableCollection<VideoWriterKind> _videoWriterKinds = new ObservableCollection<VideoWriterKind>()
        {
            // Gif is always availble
            VideoWriterKind.Gif,

            VideoWriterKind.FFMpeg,

            VideoWriterKind.Streaming_Alpha
        };

        public ReadOnlyObservableCollection<VideoWriterKind> AvailableVideoWriterKinds { get; }

        readonly ObservableCollection<IVideoWriterItem> _videoWriters = new ObservableCollection<IVideoWriterItem>();

        public ReadOnlyObservableCollection<IVideoWriterItem> AvailableVideoWriters { get; }
        
        VideoWriterKind _writerKind = VideoWriterKind.FFMpeg;

        public VideoWriterKind SelectedVideoWriterKind
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