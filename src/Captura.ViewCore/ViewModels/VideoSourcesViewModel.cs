using System.Collections.Generic;
using System.Threading;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VideoSourcesViewModel : NotifyPropertyChanged
    {
        readonly FullScreenSourceProvider _fullScreenProvider;
        public NoVideoSourceProvider NoVideoSourceProvider { get; }

        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public IEnumerable<IVideoSourceProvider> VideoSources { get; }

        public VideoSourcesViewModel(FullScreenSourceProvider FullScreenProvider,
            NoVideoSourceProvider NoVideoSourceProvider,
            IEnumerable<IVideoSourceProvider> SourceProviders)
        {
            this.NoVideoSourceProvider = NoVideoSourceProvider;
            _fullScreenProvider = FullScreenProvider;
            VideoSources = SourceProviders;

            SetDefaultSource();
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

                _videoSourceKind?.OnUnselect();

                _videoSourceKind = NewSourceProvider;
            }
            finally
            {
                // Delay parameter needs to be used with Binding for handling cancellation
                RaisePropertyChanged(nameof(SelectedVideoSourceKind));
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
    }
}