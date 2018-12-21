using System;
using System.IO;
using System.Linq;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainModel : NotifyPropertyChanged, IDisposable
    {
        readonly Settings _settings;
        bool _persist, _hotkeys, _remembered;

        readonly RememberByName _rememberByName;
        readonly IRecentList _recentList;

        readonly IWebCamProvider _webCamProvider;
        readonly VideoWritersViewModel _videoWritersViewModel;
        readonly RecordingModel _recordingModel;
        readonly AudioSource _audioSource;
        readonly HotKeyManager _hotKeyManager;

        public MainModel(Settings Settings,
            IWebCamProvider WebCamProvider,
            VideoWritersViewModel VideoWritersViewModel,
            AudioSource AudioSource,
            HotKeyManager HotKeyManager,
            RememberByName RememberByName,
            IRecentList RecentList,
            RecordingModel RecordingModel)
        {
            _settings = Settings;
            _webCamProvider = WebCamProvider;
            _videoWritersViewModel = VideoWritersViewModel;
            _audioSource = AudioSource;
            _hotKeyManager = HotKeyManager;
            _rememberByName = RememberByName;
            _recentList = RecentList;
            _recordingModel = RecordingModel;

            // If Output Dircetory is not set. Set it to Documents\Captura\
            if (string.IsNullOrWhiteSpace(Settings.OutPath))
                Settings.OutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura");

            // Create the Output Directory if it does not exist
            Settings.EnsureOutPath();
        }

        public void Refresh()
        {
            _videoWritersViewModel.RefreshCodecs();

            _audioSource.Refresh();

            #region Webcam
            var lastWebcamName = _webCamProvider.SelectedCam?.Name;

            _webCamProvider.Refresh();

            var matchingWebcam = _webCamProvider.AvailableCams.FirstOrDefault(M => M.Name == lastWebcamName);

            if (matchingWebcam != null)
            {
                _webCamProvider.SelectedCam = matchingWebcam;
            }
            #endregion
        }

        public void Init(bool Persist, bool Remembered, bool Hotkeys)
        {
            _persist = Persist;
            _hotkeys = Hotkeys;

            // Register Hotkeys if not console
            if (_hotkeys)
                _hotKeyManager.RegisterAll();

            if (Remembered)
            {
                _remembered = true;

                _rememberByName.RestoreRemembered();
            }
        }

        public void ViewLoaded()
        {
            if (_remembered)
            {
                // Restore Webcam
                if (!string.IsNullOrEmpty(_settings.Video.Webcam))
                {
                    var webcam = _webCamProvider.AvailableCams.FirstOrDefault(C => C.Name == _settings.Video.Webcam);

                    if (webcam != null)
                    {
                        _webCamProvider.SelectedCam = webcam;
                    }
                }
            }

            _hotKeyManager.ShowNotRegisteredOnStartup();
        }

        public void Dispose()
        {
            // Remember things if not console.
            if (_persist)
            {
                _rememberByName.Remember();

                _settings.Save();
            }
        }
    }
}