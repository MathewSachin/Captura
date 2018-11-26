using Captura.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        bool _persist, _hotkeys, _remembered;

        readonly IDialogService _dialogService;
        readonly RememberByName _rememberByName;
        readonly IRecentList _recentList;

        public ScreenShotViewModel ScreenShotViewModel { get; }
        public RecordingViewModel RecordingViewModel { get; }
        public VideoViewModel VideoViewModel { get; }
        public AudioSource AudioSource { get; }
        public HotKeyManager HotKeyManager { get; }

        public IWebCamProvider WebCamProvider { get; }
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

        public MainViewModel(AudioSource AudioSource,
            VideoViewModel VideoViewModel,
            IWebCamProvider WebCamProvider,
            Settings Settings,
            LanguageManager LanguageManager,
            HotKeyManager HotKeyManager,
            IPreviewWindow PreviewWindow,
            IDialogService DialogService,
            RememberByName RememberByName,
            ScreenShotViewModel ScreenShotViewModel,
            RecordingViewModel RecordingViewModel,
            HotkeyActionRegisterer HotkeyActionRegisterer,
            IRecentList RecentList) : base(Settings, LanguageManager)
        {
            this.AudioSource = AudioSource;
            this.VideoViewModel = VideoViewModel;
            this.WebCamProvider = WebCamProvider;
            this.HotKeyManager = HotKeyManager;
            _dialogService = DialogService;
            _rememberByName = RememberByName;
            this.ScreenShotViewModel = ScreenShotViewModel;
            this.RecordingViewModel = RecordingViewModel;
            _recentList = RecentList;

            ShowPreviewCommand = new DelegateCommand(PreviewWindow.Show);

            #region Commands
            RefreshCommand = new DelegateCommand(OnRefresh);

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

            VideoViewModel.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(VideoViewModel.SelectedVideoSourceKind):
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

            // If Output Dircetory is not set. Set it to Documents\Captura\
            if (string.IsNullOrWhiteSpace(Settings.OutPath))
                Settings.OutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura");

            // Create the Output Directory if it does not exist
            Settings.EnsureOutPath();

            // Handle Hoykeys
            HotkeyActionRegisterer.Register();
        }

        void OnRefresh()
        {
            #region Video Codec
            var lastVideoCodecName = VideoViewModel.SelectedVideoWriter?.ToString();

            VideoViewModel.RefreshCodecs();

            var matchingVideoCodec = VideoViewModel.AvailableVideoWriters.FirstOrDefault(M => M.ToString() == lastVideoCodecName);

            if (matchingVideoCodec != null)
            {
                VideoViewModel.SelectedVideoWriter = matchingVideoCodec;
            }
            #endregion

            #region Audio
            var lastMicNames = AudioSource.AvailableRecordingSources
                .Where(M => M.Active)
                .Select(M => M.Name)
                .ToArray();

            var lastSpeakerNames = AudioSource.AvailableLoopbackSources
                .Where(M => M.Active)
                .Select(M => M.Name)
                .ToArray();

            AudioSource.Refresh();

            foreach (var source in AudioSource.AvailableRecordingSources)
            {
                source.Active = lastMicNames.Contains(source.Name);
            }

            foreach (var source in AudioSource.AvailableLoopbackSources)
            {
                source.Active = lastSpeakerNames.Contains(source.Name);
            }
            #endregion

            #region Webcam
            var lastWebcamName = WebCamProvider.SelectedCam?.Name;

            WebCamProvider.Refresh();

            var matchingWebcam = WebCamProvider.AvailableCams.FirstOrDefault(M => M.Name == lastWebcamName);

            if (matchingWebcam != null)
            {
                WebCamProvider.SelectedCam = matchingWebcam;
            }
            #endregion

            Refreshed?.Invoke();
        }

        public void Init(bool Persist, bool Timer, bool Remembered, bool Hotkeys)
        {
            _persist = Persist;
            _hotkeys = Hotkeys;

            if (Timer)
            {
                RecordingViewModel.InitTimer();
            }

            // Register Hotkeys if not console
            if (_hotkeys)
                HotKeyManager.RegisterAll();

            VideoViewModel.Init();

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
                if (!string.IsNullOrEmpty(Settings.Video.Webcam))
                {
                    var webcam = WebCamProvider.AvailableCams.FirstOrDefault(C => C.Name == Settings.Video.Webcam);

                    if (webcam != null)
                    {
                        WebCamProvider.SelectedCam = webcam;
                    }
                }
            }

            HotKeyManager.ShowNotRegisteredOnStartup();
        }
        
        public void Dispose()
        {
            RecordingViewModel.Dispose();

            if (_hotkeys)
                HotKeyManager.Dispose();

            AudioSource.Dispose();

            _recentList.Dispose();
            
            // Remember things if not console.
            if (_persist)
            {
                _rememberByName.Remember();

                Settings.Save();
            }
        }
        
        void CheckFunctionalityAvailability()
        {
            var audioAvailable = Settings.Audio.Enabled;

            var videoAvailable = !(VideoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider);
            
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