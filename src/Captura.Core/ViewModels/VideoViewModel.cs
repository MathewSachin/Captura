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
        readonly FullScreenSourceProvider _fullScreenProvider;

        public NoVideoSourceProvider NoVideoSourceProvider { get; }

        public ICommand SetSourceCommand { get; }
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

            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            AvailableImageWriters = new ReadOnlyObservableCollection<IImageWriterItem>(_imageWriters);

            _regionProvider = RegionProvider;
            _fullScreenProvider = FullScreenProvider;

            SetSourceCommand = new DelegateCommand(async M =>
            {
                if (!(M is Type type))
                    return;

                if (type == typeof(FullScreenSourceProvider))
                {
                    SelectedVideoSourceKind = FullScreenProvider;
                }
                else if (type == typeof(RegionSourceProvider))
                {
                    SelectedVideoSourceKind = RegionSourceProvider;
                }
                else if (type == typeof(NoVideoSourceProvider))
                {
                    SelectedVideoSourceKind = NoVideoSourceProvider;
                }
                else if (type == typeof(ScreenSourceProvider))
                {
                    await SetScreenSource(ScreenSourceProvider, MainWindow);
                }
                else if (type == typeof(DeskDuplSourceProvider))
                {
                    await SetDeskDuplSource(DeskDuplSourceProvider, MainWindow);
                }
                else if (type == typeof(WindowSourceProvider))
                {
                    await SetWindowSource(RegionProvider, WindowSourceProvider, MainWindow);
                }
            });

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

        async Task SetWindowSource(IRegionProvider RegionProvider, WindowSourceProvider WindowSourceProvider, IMainWindow MainWindow)
        {
            MainWindow.IsVisible = false;

            // Wait for MainWindow to hide
            await Task.Delay(300);

            try
            {
                if (WindowSourceProvider.PickWindow(new[] {RegionProvider.Handle}))
                {
                    SelectedVideoSourceKind = WindowSourceProvider;
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
                SelectedVideoSourceKind = DeskDuplSourceProvider;
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
                        SelectedVideoSourceKind = DeskDuplSourceProvider;
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
                SelectedVideoSourceKind = ScreenSourceProvider;
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
                        SelectedVideoSourceKind = ScreenSourceProvider;
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