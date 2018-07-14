using Captura.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class MainViewModel
    {
        public bool MouseKeyHookAvailable { get; } = ServiceProvider.FileExists("Gma.System.MouseKeyHook.dll");

        public TextLocalizer Status { get; } = new TextLocalizer(nameof(LanguageManager.Ready));
        
        #region Time
        TimeSpan _ts;

        public TimeSpan TimeSpan
        {
            get => _ts;
            set
            {
                if (_ts == value)
                    return;

                _ts = value;

                OnPropertyChanged();
            }
        }
        #endregion

        public IEnumerable<KeyValuePair<RotateBy, string>> Rotations { get; } = new[]
        {
            new KeyValuePair<RotateBy, string>(RotateBy.RotateNone, "No Rotation"),
            new KeyValuePair<RotateBy, string>(RotateBy.Rotate90, "90° Clockwise"),
            new KeyValuePair<RotateBy, string>(RotateBy.Rotate180, "180° Clockwise"),
            new KeyValuePair<RotateBy, string>(RotateBy.Rotate270, "90° Anticlockwise")
        };
        
        RecorderState _recorderState = RecorderState.NotRecording;

        public RecorderState RecorderState
        {
            get => _recorderState;
            private set
            {
                if (_recorderState == value)
                    return;

                _recorderState = value;

                RefreshCommand.RaiseCanExecuteChanged(value == RecorderState.NotRecording);

                PauseCommand.RaiseCanExecuteChanged(value != RecorderState.NotRecording);

                OnPropertyChanged();
            }
        }

        public FFmpegLog FFmpegLog { get; }

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

        public HotKeyManager HotKeyManager { get; }

        public IWebCamProvider WebCamProvider { get; }

        public CustomOverlaysViewModel CustomOverlays { get; }

        public CustomImageOverlaysViewModel CustomImageOverlays { get; }

        public CensorOverlaysViewModel CensorOverlays { get; }
        
        #region Commands
        public DelegateCommand RecordCommand { get; }

        public DelegateCommand RefreshCommand { get; }

        public DelegateCommand OpenOutputFolderCommand { get; }

        void OpenOutputFolder()
        {
            Settings.EnsureOutPath();

            Process.Start(Settings.OutPath);
        }

        public DelegateCommand PauseCommand { get; }

        public DelegateCommand SelectOutputFolderCommand { get; }

        void SelectOutputFolder()
        {
            var folder = _dialogService.PickFolder(Settings.OutPath, Loc.SelectOutFolder);

            if (folder != null)
                Settings.OutPath = folder;
        }

        public DelegateCommand SelectFFmpegFolderCommand { get; } = new DelegateCommand(FFmpegService.SelectFFmpegFolder);

        public DelegateCommand ResetFFmpegFolderCommand { get; }
        #endregion

        #region Nested ViewModels
        public VideoViewModel VideoViewModel { get; }

        public AudioSource AudioSource { get; }

        public RecentViewModel RecentViewModel { get; }
        #endregion
    }
}
