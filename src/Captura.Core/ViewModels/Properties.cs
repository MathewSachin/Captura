using Captura.Models;
using Ookii.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Captura.ViewModels
{
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

        int _duration;

        public int Duration
        {
            get => _duration;
            set
            {
                if (_duration == value)
                    return;

                _duration = Math.Max(0, value);

                OnPropertyChanged();
            }
        }

        int _startDelay;

        public int StartDelay
        {
            get => _startDelay;
            set
            {
                if (_startDelay == value)
                    return;

                _startDelay = Math.Max(0, value);

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

        public FFMpegLog FFMpegLog { get; } = FFMpegLog.Instance;

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

        #region ScreenShot
        public IEnumerable<ImageFormat> ScreenShotImageFormats { get; } = new[]
        {
            ImageFormat.Png,
            ImageFormat.Jpeg,
            ImageFormat.Bmp,
            ImageFormat.Tiff,
            ImageFormat.Wmf,
            ImageFormat.Exif,
            ImageFormat.Gif,
            ImageFormat.Icon,
            ImageFormat.Emf
        };

        ImageFormat _screenShotImageFormat = ImageFormat.Png;

        public ImageFormat SelectedScreenShotImageFormat
        {
            get => _screenShotImageFormat;
            set
            {
                if (_screenShotImageFormat == value)
                    return;

                _screenShotImageFormat = value;

                OnPropertyChanged();
            }
        }
        #endregion
        
        #region Commands
        public DelegateCommand ScreenShotCommand { get; }

        public DelegateCommand ScreenShotActiveCommand { get; }

        public DelegateCommand ScreenShotDesktopCommand { get; }

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
            using (var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = Settings.OutPath,
                UseDescriptionForTitle = true,
                Description = LanguageManager.Instance.SelectOutFolder
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    Settings.OutPath = dlg.SelectedPath;
            }
        }

        public DelegateCommand SelectFFMpegFolderCommand { get; } = new DelegateCommand(FFMpegService.SelectFFMpegFolder);

        public DelegateCommand ResetFFMpegFolderCommand { get; }
        #endregion

        #region Nested ViewModels
        public VideoViewModel VideoViewModel { get; }

        public AudioSource AudioSource { get; }

        public RecentViewModel RecentViewModel { get; }
        #endregion
    }
}
