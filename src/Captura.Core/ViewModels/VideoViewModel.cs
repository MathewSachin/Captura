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

        public ObservableCollection<IVideoSourceProvider> VideoSources { get; } = new ObservableCollection<IVideoSourceProvider>();

        public ObservableCollection<IVideoWriterProvider> VideoWriterProviders { get; } = new ObservableCollection<IVideoWriterProvider>();

        public VideoViewModel(IRegionProvider RegionProvider,
            IEnumerable<IImageWriterItem> ImageWriters,
            Settings Settings,
            LanguageManager LanguageManager,
            FullScreenSourceProvider FullScreenProvider,
            NoVideoSourceProvider NoVideoSourceProvider,
            IEnumerable<IVideoSourceProvider> SourceProviders,
            IEnumerable<IVideoWriterProvider> WriterProviders
            ) : base(Settings, LanguageManager)
        {
            this.NoVideoSourceProvider = NoVideoSourceProvider;

            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            AvailableImageWriters = new ReadOnlyObservableCollection<IImageWriterItem>(_imageWriters);

            _regionProvider = RegionProvider;
            _fullScreenProvider = FullScreenProvider;

            foreach (var sourceProvider in SourceProviders)
            {
                VideoSources.Add(sourceProvider);
            }

            foreach (var writerProvider in WriterProviders)
            {
                VideoWriterProviders.Add(writerProvider);
            }

            foreach (var imageWriter in ImageWriters)
            {
                _imageWriters.Add(imageWriter);
            }

            SetDefaultSource();

            if (!AvailableImageWriters.Any(M => M.Active))
                AvailableImageWriters[0].Active = true;

            if (VideoWriterProviders.Count > 0)
                SelectedVideoWriterKind = VideoWriterProviders[0];
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