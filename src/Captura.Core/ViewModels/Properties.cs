using Captura.Models;
using Captura.Properties;
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

        public TextLocalizer Status { get; } = new TextLocalizer(Resources.Ready);

        bool _canChangeVideoSource = true;

        public bool CanChangeVideoSource
        {
            get => _canChangeVideoSource;
            set
            {
                if (_canChangeVideoSource == value)
                    return;

                _canChangeVideoSource = value;

                OnPropertyChanged();
            }
        }

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

                _duration = value;

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

                _startDelay = value;

                OnPropertyChanged();
            }
        }
        #endregion

        public KeyValuePair<RotateBy, string>[] Rotations { get; } = new[]
        {
            new KeyValuePair<RotateBy, string>(RotateBy.RotateNone, "No Rotation"),
            new KeyValuePair<RotateBy, string>(RotateBy.Rotate90, "90° Clockwise"),
            new KeyValuePair<RotateBy, string>(RotateBy.Rotate180, "180° Clockwise"),
            new KeyValuePair<RotateBy, string>(RotateBy.Rotate270, "90° Anticlockwise"),
        };
        
        RecorderState _recorderState = RecorderState.NotRecording;

        public RecorderState RecorderState
        {
            get => _recorderState;
            set
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

        // Not a Setting
        public KeyValuePair<Alignment, string>[] XAlignments { get; } = new[]
        {
            new KeyValuePair<Alignment, string>(Alignment.Start, "Left"),
            new KeyValuePair<Alignment, string>(Alignment.Center, "Center"),
            new KeyValuePair<Alignment, string>(Alignment.End, "Right")
        };

        // Not a Setting
        public KeyValuePair<Alignment, string>[] YAlignments { get; } = new[]
        {
            new KeyValuePair<Alignment, string>(Alignment.Start, "Top"),
            new KeyValuePair<Alignment, string>(Alignment.Center, "Center"),
            new KeyValuePair<Alignment, string>(Alignment.End, "Bottom")
        };

        #region ScreenShot
        public ImageFormat[] ScreenShotImageFormats => new[]
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

        public DelegateCommand OpenOutputFolderCommand { get; } = new DelegateCommand(() =>
        {
            Settings.Instance.EnsureOutPath();

            Process.Start(Settings.Instance.OutPath);
        });

        public DelegateCommand PauseCommand { get; }

        public DelegateCommand SelectOutputFolderCommand { get; } = new DelegateCommand(() =>
        {
            using (var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = Settings.Instance.OutPath,
                UseDescriptionForTitle = true,
                Description = Resources.SelectOutFolder
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    Settings.Instance.OutPath = dlg.SelectedPath;
            }
        });

        public DelegateCommand SelectFFMpegFolderCommand { get; } = new DelegateCommand(() =>
        {
            using (var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = Settings.Instance.FFMpegFolder,
                UseDescriptionForTitle = true,
                Description = Resources.SelectFFMpegFolder
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    Settings.Instance.FFMpegFolder = dlg.SelectedPath;
            }
        });

        public DelegateCommand ResetFFMpegFolderCommand { get; } = new DelegateCommand(() => Settings.Instance.FFMpegFolder = "");
        #endregion

        #region Nested ViewModels
        public VideoViewModel VideoViewModel { get; } = new VideoViewModel();

        public AudioViewModel AudioViewModel { get; } = new AudioViewModel();

        public RecentViewModel RecentViewModel { get; } = new RecentViewModel();
        #endregion
    }
}
