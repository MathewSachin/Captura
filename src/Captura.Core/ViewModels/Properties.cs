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
            get { return _canChangeVideoSource; }
            set
            {
                if (_canChangeVideoSource == value)
                    return;

                _canChangeVideoSource = value;

                OnPropertyChanged();
            }
        }

        #region Time
        TimeSpan _ts = TimeSpan.Zero;
        readonly TimeSpan _addend = TimeSpan.FromSeconds(1);

        public TimeSpan TimeSpan
        {
            get { return _ts; }
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
            get { return _duration; }
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
            get { return _startDelay; }
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
            get { return _recorderState; }
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
            get { return _screenShotImageFormat; }
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
            var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = Settings.Instance.OutPath,
                Description = Resources.SelectOutFolder
            };

            if (dlg.ShowDialog() == DialogResult.OK)
                Settings.Instance.OutPath = dlg.SelectedPath;
        });

        public DelegateCommand SelectFFMpegFolderCommand { get; } = new DelegateCommand(() =>
        {
            var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = Settings.Instance.FFMpegFolder,
                Description = Resources.SelectFFMpegFolder
            };

            if (dlg.ShowDialog() == DialogResult.OK)
                Settings.Instance.FFMpegFolder = dlg.SelectedPath;
        });

        public DelegateCommand ResetFFMpegFolderCommand { get; } = new DelegateCommand(() => Settings.Instance.FFMpegFolder = "");
        #endregion

        #region Nested ViewModels
        public Settings Settings { get; } = Settings.Instance;

        public VideoViewModel VideoViewModel { get; } = new VideoViewModel();

        public AudioViewModel AudioViewModel { get; } = new AudioViewModel();

        public RecentViewModel RecentViewModel { get; } = new RecentViewModel();
        #endregion
    }
}
