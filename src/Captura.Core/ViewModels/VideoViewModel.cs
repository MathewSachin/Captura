using System;
using System.Collections.Generic;
using Captura.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VideoViewModel : ViewModelBase
    {
        readonly IRegionProvider _regionProvider;
        readonly IMainWindow _mainWindow;
        readonly FullScreenSourceProvider _fullScreenProvider;

        public NoVideoSourceProvider NoVideoSourceProvider { get; }

        public ObservableCollection<VideoSourceModel> VideoSources { get; } = new ObservableCollection<VideoSourceModel>();

        const string NoVideoDescription = @"No Video recorded.
Can be used for audio-only recording.";

        const string FullScreenDescription = "Record Fullscreen.";

        const string ScreenDescription = "Record a specific screen.";

        const string WindowDescription = @"Record a specific window.
The video is of the initial size of the window.";

        const string RegionDescription = "Record region selected using Region Selector.";

        const string DeskDuplDescription = @"Faster API for recording screen as well as fullscreen DirectX games.
Not all games are recordable.
Requires Windows 8 or above.
If it does not work, try running Captura on the Integrated Graphics card.";

        public ICommand SetWriterCommand { get; }

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
            DiscardWriterProvider DiscardWriterProvider,
            // ReSharper restore SuggestBaseTypeForParameter
            IMainWindow MainWindow) : base(Settings, LanguageManager)
        {
            this.NoVideoSourceProvider = NoVideoSourceProvider;
            _mainWindow = MainWindow;

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

            SetWriterCommand = new DelegateCommand(M =>
            {
                if (!(M is Type type))
                    return;

                if (type == typeof(FFmpegWriterProvider))
                {
                    SelectedVideoWriterKind = FFmpegWriterProvider;
                }
                else if (type == typeof(SharpAviWriterProvider))
                {
                    SelectedVideoWriterKind = SharpAviWriterProvider;
                }
                else if (type == typeof(GifWriterProvider))
                {
                    SelectedVideoWriterKind = GifWriterProvider;
                }
                else if (type == typeof(StreamingWriterProvider))
                {
                    SelectedVideoWriterKind = StreamingWriterProvider;
                }
                else if (type == typeof(DiscardWriterProvider))
                {
                    SelectedVideoWriterKind = DiscardWriterProvider;
                }
            });

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

        async Task SetWindowSource(WindowSourceProvider WindowSourceProvider, IMainWindow MainWindow)
        {
            MainWindow.IsVisible = false;

            // Wait for MainWindow to hide
            await Task.Delay(300);

            try
            {
                if (WindowSourceProvider.PickWindow(new[] {_regionProvider.Handle}))
                {
                    _videoSourceKind = WindowSourceProvider;
                }
            }
            finally
            {
                MainWindow.IsVisible = true;
            }
        }

        async Task SetDeskDuplSource(DeskDuplSourceProvider DeskDuplSourceProvider, IMainWindow MainWindow)
        {
            // Select first screen if there is only one
            if (ScreenItem.Count == 1 && DeskDuplSourceProvider.SelectFirst())
            {
                _videoSourceKind = DeskDuplSourceProvider;
            }
            else
            {
                MainWindow.IsVisible = false;

                // Wait for MainWindow to hide
                await Task.Delay(300);

                try
                {
                    if (DeskDuplSourceProvider.PickScreen())
                    {
                        _videoSourceKind = DeskDuplSourceProvider;
                    }
                }
                finally
                {
                    MainWindow.IsVisible = true;
                }
            }
        }

        async Task SetScreenSource(ScreenSourceProvider ScreenSourceProvider, IMainWindow MainWindow)
        {
            // Select first screen if there is only one
            if (ScreenItem.Count == 1)
            {
                ScreenSourceProvider.Set(0);
                _videoSourceKind = ScreenSourceProvider;
            }
            else
            {
                MainWindow.IsVisible = false;

                // Wait for MainWindow to hide
                await Task.Delay(300);

                try
                {
                    if (ScreenSourceProvider.PickScreen())
                    {
                        _videoSourceKind = ScreenSourceProvider;
                    }
                }
                finally
                {
                    MainWindow.IsVisible = true;
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

                _writerKind = value;

                OnPropertyChanged();

                RefreshCodecs();
            }
        }

        IVideoSourceProvider _videoSourceKind;

        async void SetSelectedVideoSourceKind(IVideoSourceProvider Value)
        {
            if (_videoSourceKind == Value)
                return;

            switch (Value)
            {
                case ScreenSourceProvider screenSourceProvider:
                    await SetScreenSource(screenSourceProvider, _mainWindow);
                    break;

                case DeskDuplSourceProvider deskDuplSourceProvider:
                    await SetDeskDuplSource(deskDuplSourceProvider, _mainWindow);
                    break;

                case WindowSourceProvider windowSourceProvider:
                    await SetWindowSource(windowSourceProvider, _mainWindow);
                    break;
                
                default:
                    _videoSourceKind = Value;
                    break;
            }

            RaisePropertyChanged(nameof(SelectedVideoSourceKind));

            RefreshVideoSources();
        }

        public IVideoSourceProvider SelectedVideoSourceKind
        {
            get => _videoSourceKind;
            set => SetSelectedVideoSourceKind(value);
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