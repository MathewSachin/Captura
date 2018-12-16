using Captura.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Captura.ViewModels
{
    public class MainModel : NotifyPropertyChanged, IDisposable
    {
        readonly Settings _settings;
        bool _persist, _hotkeys, _remembered;

        readonly RememberByName _rememberByName;
        readonly IRecentList _recentList;

        readonly IWebCamProvider _webCamProvider;
        readonly VideoWritersViewModel _videoWritersViewModel;
        readonly RecordingViewModel _recordingViewModel;
        readonly AudioSource _audioSource;
        readonly HotKeyManager _hotKeyManager;

        public MainModel(Settings Settings,
            HotkeyActionRegisterer HotkeyActionRegisterer,
            IWebCamProvider WebCamProvider,
            VideoWritersViewModel VideoWritersViewModel,
            AudioSource AudioSource,
            HotKeyManager HotKeyManager,
            RememberByName RememberByName,
            IRecentList RecentList,
            RecordingViewModel RecordingViewModel)
        {
            _settings = Settings;
            _webCamProvider = WebCamProvider;
            _videoWritersViewModel = VideoWritersViewModel;
            _audioSource = AudioSource;
            _hotKeyManager = HotKeyManager;
            _rememberByName = RememberByName;
            _recentList = RecentList;
            _recordingViewModel = RecordingViewModel;

            // If Output Dircetory is not set. Set it to Documents\Captura\
            if (string.IsNullOrWhiteSpace(Settings.OutPath))
                Settings.OutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura");

            // Create the Output Directory if it does not exist
            Settings.EnsureOutPath();

            // Handle Hoykeys
            HotkeyActionRegisterer.Register();
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
            _recordingViewModel.Dispose();

            if (_hotkeys)
                _hotKeyManager.Dispose();

            _audioSource.Dispose();

            _recentList.Dispose();

            // Remember things if not console.
            if (_persist)
            {
                _rememberByName.Remember();

                _settings.Save();
            }
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainViewModel : ViewModelBase, IDisposable
    {
        readonly MainModel _mainModel;

        #region Fields
        readonly IDialogService _dialogService;

        public ScreenShotViewModel ScreenShotViewModel { get; }
        public RecordingViewModel RecordingViewModel { get; }
        public VideoSourcesViewModel VideoSourcesViewModel { get; }
        #endregion

        #region Commands
        public ICommand ShowPreviewCommand { get; }

        public DelegateCommand RefreshCommand { get; }

        public DelegateCommand OpenOutputFolderCommand { get; }

        public DelegateCommand SelectOutputFolderCommand { get; }

        public DelegateCommand SelectFFmpegFolderCommand { get; } = new DelegateCommand(FFmpegService.SelectFFmpegFolder);

        public DelegateCommand ResetFFmpegFolderCommand { get; }

        public DelegateCommand TrayLeftClickCommand { get; }
        #endregion

        public MainViewModel(VideoSourcesViewModel VideoSourcesViewModel,
            Settings Settings,
            LanguageManager LanguageManager,
            HotKeyManager HotKeyManager,
            IPreviewWindow PreviewWindow,
            IDialogService DialogService,
            ScreenShotViewModel ScreenShotViewModel,
            RecordingViewModel RecordingViewModel,
            MainModel MainModel) : base(Settings, LanguageManager)
        {
            this.VideoSourcesViewModel = VideoSourcesViewModel;
            _dialogService = DialogService;
            this.ScreenShotViewModel = ScreenShotViewModel;
            this.RecordingViewModel = RecordingViewModel;
            _mainModel = MainModel;

            ShowPreviewCommand = new DelegateCommand(PreviewWindow.Show);

            #region Commands
            RefreshCommand = new DelegateCommand(() =>
            {
                MainModel.Refresh();

                Refreshed?.Invoke();
            });

            OpenOutputFolderCommand = new DelegateCommand(OpenOutputFolder);

            SelectOutputFolderCommand = new DelegateCommand(SelectOutputFolder);

            ResetFFmpegFolderCommand = new DelegateCommand(() => Settings.FFmpeg.FolderPath = "");

            TrayLeftClickCommand = new DelegateCommand(() => HotKeyManager.FakeHotkey(Settings.Tray.LeftClickAction));
            #endregion

            Settings.Audio.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(Settings.Audio.Enabled):
                    case null:
                    case "":
                        CheckFunctionalityAvailability();
                        break;
                }
            };

            VideoSourcesViewModel.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(VideoSourcesViewModel.SelectedVideoSourceKind):
                    case null:
                    case "":
                        CheckFunctionalityAvailability();
                        break;
                }
            };

            RecordingViewModel.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(RecordingViewModel.RecorderState):
                    case null:
                    case "":
                        RefreshCommand.RaiseCanExecuteChanged(RecordingViewModel.RecorderState == RecorderState.NotRecording);
                        break;
                }
            };
        }

        public void Init(bool Persist, bool Remembered, bool Hotkeys)
        {
            _mainModel.Init(Persist, Remembered, Hotkeys);
        }

        public void ViewLoaded()
        {
            _mainModel.ViewLoaded();
        }
        
        public void Dispose()
        {
            _mainModel.Dispose();
        }
        
        void CheckFunctionalityAvailability()
        {
            var audioAvailable = Settings.Audio.Enabled;

            var videoAvailable = !(VideoSourcesViewModel.SelectedVideoSourceKind is NoVideoSourceProvider);
            
            RecordingViewModel.RecordCommand.RaiseCanExecuteChanged(audioAvailable || videoAvailable);

            ScreenShotViewModel.ScreenShotCommand.RaiseCanExecuteChanged(videoAvailable);
        }

        public static IEnumerable<ObjectLocalizer<Alignment>> XAlignments { get; } = new[]
        {
            new ObjectLocalizer<Alignment>(Alignment.Start, nameof(LanguageManager.Left)),
            new ObjectLocalizer<Alignment>(Alignment.Center, nameof(LanguageManager.Center)),
            new ObjectLocalizer<Alignment>(Alignment.End, nameof(LanguageManager.Right))
        };

        public static IEnumerable<ObjectLocalizer<Alignment>> YAlignments { get; } = new[]
        {
            new ObjectLocalizer<Alignment>(Alignment.Start, nameof(LanguageManager.Top)),
            new ObjectLocalizer<Alignment>(Alignment.Center, nameof(LanguageManager.Center)),
            new ObjectLocalizer<Alignment>(Alignment.End, nameof(LanguageManager.Bottom))
        };

        void OpenOutputFolder()
        {
            Settings.EnsureOutPath();

            Process.Start(Settings.OutPath);
        }

        void SelectOutputFolder()
        {
            var folder = _dialogService.PickFolder(Settings.OutPath, Loc.SelectOutFolder);

            if (folder != null)
                Settings.OutPath = folder;
        }

        public event Action Refreshed;
    }
}