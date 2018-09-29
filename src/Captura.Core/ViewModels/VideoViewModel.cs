using System;
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
        readonly IRegionProvider _regionProvider;
        readonly FullScreenSourceProvider _fullScreenProvider;

        // To prevent deselection or cancelling selection
        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public NoVideoSourceProvider NoVideoSourceProvider { get; }

        public ObservableCollection<VideoSourceModel> VideoSources { get; } = new ObservableCollection<VideoSourceModel>();

        public ObservableCollection<IVideoWriterProvider> VideoWriterProviders { get; } = new ObservableCollection<IVideoWriterProvider>();

        const string NoVideoDescription = @"No Video recorded.
Can be used for audio-only recording.
Make sure Audio sources are enabled.";

        const string FullScreenDescription = "Record Fullscreen.";

        const string ScreenDescription = "Record a specific screen.";

        const string WindowDescription = @"Record a specific window.
The video is of the initial size of the window.";

        const string RegionDescription = "Record region selected using Region Selector.";

        const string DeskDuplDescription = @"Faster API for recording screen as well as fullscreen DirectX games.
Not all games are recordable.
Requires Windows 8 or above.
If it does not work, try running Captura on the Integrated Graphics card.";

        public VideoViewModel(IRegionProvider RegionProvider,
            IEnumerable<IImageWriterItem> ImageWriters,
            Settings Settings,
            LanguageManager LanguageManager,
            FullScreenSourceProvider FullScreenProvider,
            // ReSharper disable SuggestBaseTypeForParameter
            ScreenSourceProvider ScreenSourceProvider,
            WindowSourceProvider WindowSourceProvider,
            RegionSourceProvider RegionSourceProvider,
            NoVideoSourceProvider NoVideoSourceProvider,
            DeskDuplSourceProvider DeskDuplSourceProvider,
            FFmpegWriterProvider FFmpegWriterProvider,
            SharpAviWriterProvider SharpAviWriterProvider,
            GifWriterProvider GifWriterProvider,
            StreamingWriterProvider StreamingWriterProvider,
            DiscardWriterProvider DiscardWriterProvider
            // ReSharper restore SuggestBaseTypeForParameter
            ) : base(Settings, LanguageManager)
        {
            this.NoVideoSourceProvider = NoVideoSourceProvider;

            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            AvailableImageWriters = new ReadOnlyObservableCollection<IImageWriterItem>(_imageWriters);

            _regionProvider = RegionProvider;
            _fullScreenProvider = FullScreenProvider;

            VideoSources.Add(new VideoSourceModel(NoVideoSourceProvider, nameof(Loc.OnlyAudio), NoVideoDescription, "IconNoVideo"));
            VideoSources.Add(new VideoSourceModel(FullScreenProvider, nameof(Loc.FullScreen), FullScreenDescription, "IconMultipleMonitor"));
            VideoSources.Add(new VideoSourceModel(ScreenSourceProvider, nameof(Loc.Screen), ScreenDescription, "IconScreen"));
            VideoSources.Add(new VideoSourceModel(WindowSourceProvider, nameof(Loc.Window), WindowDescription, "IconWindow"));
            VideoSources.Add(new VideoSourceModel(RegionSourceProvider, nameof(Loc.Region), RegionDescription, "IconRegion"));

            if (Windows8OrAbove)
            {
                VideoSources.Add(new VideoSourceModel(DeskDuplSourceProvider, nameof(Loc.DesktopDuplication), DeskDuplDescription, "IconGame"));
            }

            VideoWriterProviders.Add(FFmpegWriterProvider);
            VideoWriterProviders.Add(GifWriterProvider);
            VideoWriterProviders.Add(SharpAviWriterProvider);
            VideoWriterProviders.Add(StreamingWriterProvider);
            VideoWriterProviders.Add(DiscardWriterProvider);

            foreach (var imageWriter in ImageWriters)
            {
                _imageWriters.Add(imageWriter);
            }

            SetDefaultSource();

            if (!AvailableImageWriters.Any(M => M.Active))
                AvailableImageWriters[0].Active = true;

            SelectedVideoWriterKind = FFmpegWriterProvider;
        }

        public bool Windows8OrAbove
        {
            get
            {
                // All versions above Windows 8 give the same version number
                var version = new Version(6, 2, 9200, 0);

                return Environment.OSVersion.Platform == PlatformID.Win32NT &&
                       Environment.OSVersion.Version >= version;
            }
        }

        void SetDeskDuplSource(DeskDuplSourceProvider DeskDuplSourceProvider)
        {
            // Select first screen if there is only one
            if (ScreenItem.Count == 1 && DeskDuplSourceProvider.SelectFirst())
            {
                _videoSourceKind = DeskDuplSourceProvider;
            }
            else
            {
                if (DeskDuplSourceProvider.PickScreen())
                {
                    _videoSourceKind = DeskDuplSourceProvider;
                }
            }
        }

        void SetScreenSource(ScreenSourceProvider ScreenSourceProvider)
        {
            // Select first screen if there is only one
            if (ScreenItem.Count == 1)
            {
                ScreenSourceProvider.Set(0);
                _videoSourceKind = ScreenSourceProvider;
            }
            else
            {
                if (ScreenSourceProvider.PickScreen())
                {
                    _videoSourceKind = ScreenSourceProvider;
                }
            }
        }

        public void SetDefaultSource()
        {
            SelectedVideoSourceKind = _fullScreenProvider;
        }

        public void Init()
        {                                               
            RefreshCodecs();

            RefreshVideoSources();
            
            _regionProvider.SelectorHidden += () =>
            {
                if (SelectedVideoSourceKind is RegionSourceProvider)
                    SetDefaultSource();
            };
        }

        void RefreshVideoSources()
        {
            // RegionSelector should only be shown on Region Capture.
            _regionProvider.SelectorVisible = SelectedVideoSourceKind is RegionSourceProvider;
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
            set
            {
                if (_videoSourceKind == value)
                    return;

                switch (value)
                {
                    case ScreenSourceProvider screenSourceProvider:
                        SetScreenSource(screenSourceProvider);
                        break;

                    case DeskDuplSourceProvider deskDuplSourceProvider:
                        SetDeskDuplSource(deskDuplSourceProvider);
                        break;

                    case WindowSourceProvider windowSourceProvider:
                        if (windowSourceProvider.PickWindow())
                        {
                            _videoSourceKind = windowSourceProvider;
                        }
                        break;

                    default:
                        if (value != null)
                            _videoSourceKind = value;
                        break;
                }

                RefreshVideoSources();

                if (_syncContext != null)
                {
                    _syncContext.Post(S => RaisePropertyChanged(nameof(SelectedVideoSourceKind)), null);
                }
                else OnPropertyChanged();
            }
        }

        public void RestoreSourceKind(IVideoSourceProvider SourceProvider)
        {
            _videoSourceKind = SourceProvider;
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
    }
}