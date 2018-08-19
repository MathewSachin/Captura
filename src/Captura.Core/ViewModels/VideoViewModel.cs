using System;
using System.Collections.Generic;
using Captura.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VideoViewModel : ViewModelBase
    {
        readonly IRegionProvider _regionProvider;
        readonly FullScreenSourceProvider _fullScreenProvider;

        public ICommand SetFullScreenSourceCommand { get; }
        public ICommand SetScreenSourceCommand { get; }
        public ICommand SetWindowSourceCommand { get; }
        public ICommand SetRegionSourceCommand { get; }
        public ICommand SetNoVideoSourceCommand { get; }

        public VideoViewModel(IRegionProvider RegionProvider,
            IEnumerable<IImageWriterItem> ImageWriters,
            IEnumerable<IVideoWriterProvider> VideoWriterProviders,
            Settings Settings,
            LanguageManager LanguageManager,
            FullScreenSourceProvider FullScreenProvider,
            // ReSharper disable SuggestBaseTypeForParameter
            ScreenSourceProvider ScreenSourceProvider,
            WindowSourceProvider WindowSourceProvider,
            RegionSourceProvider RegionSourceProvider,
            NoVideoSourceProvider NoVideoSourceProvider) : base(Settings, LanguageManager)
            // ReSharper restore SuggestBaseTypeForParameter
        {
            AvailableVideoWriterKinds = new ReadOnlyObservableCollection<IVideoWriterProvider>(_videoWriterKinds);
            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            AvailableImageWriters = new ReadOnlyObservableCollection<IImageWriterItem>(_imageWriters);

            _regionProvider = RegionProvider;
            _fullScreenProvider = FullScreenProvider;

            SetFullScreenSourceCommand = new DelegateCommand(() => SelectedVideoSourceKind = FullScreenProvider);
            SetRegionSourceCommand = new DelegateCommand(() => SelectedVideoSourceKind = RegionSourceProvider);
            SetNoVideoSourceCommand = new DelegateCommand(() => SelectedVideoSourceKind = NoVideoSourceProvider);

            SetScreenSourceCommand = new DelegateCommand(() =>
            {
                // Select first screen if there is only one
                if (ScreenItem.Count == 1)
                {
                    ScreenSourceProvider.Set(0);
                    SelectedVideoSourceKind = ScreenSourceProvider;
                }
                else if (ScreenSourceProvider.PickScreen())
                {
                    SelectedVideoSourceKind = ScreenSourceProvider;
                }
            });

            SetWindowSourceCommand = new DelegateCommand(() =>
            {
                if (WindowSourceProvider.PickWindow(new [] { RegionProvider.Handle }))
                {
                    SelectedVideoSourceKind = WindowSourceProvider;
                }
            });

            foreach (var imageWriter in ImageWriters)
            {
                _imageWriters.Add(imageWriter);
            }

            foreach (var videoWriterProvider in VideoWriterProviders)
            {
                _videoWriterKinds.Add(videoWriterProvider);
            }

            SetDefaultSource();

            if (!AvailableImageWriters.Any(M => M.Active))
                AvailableImageWriters[0].Active = true;

            if (AvailableVideoWriterKinds.Count > 0)
                SelectedVideoWriterKind = AvailableVideoWriterKinds[0];
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
        
        public void RefreshVideoSources()
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