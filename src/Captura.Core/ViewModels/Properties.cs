using Captura.Models;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace Captura.ViewModels
{
    public partial class MainViewModel
    {
        bool MouseKeyHookAvailable { get; } = File.Exists("Gma.System.MouseKeyHook.dll");

        string _status = "Ready";

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status == value)
                    return;

                _status = value;

                OnPropertyChanged();
            }
        }

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

        public string[] ScreenShotSaveTo => new[] { "Disk", "Clipboard" };
        #endregion
        
        #region Commands
        public DelegateCommand ScreenShotCommand { get; }

        public DelegateCommand RecordCommand { get; }

        public DelegateCommand RefreshCommand { get; }

        public DelegateCommand OpenOutputFolderCommand { get; }

        public DelegateCommand PauseCommand { get; }

        public DelegateCommand SelectOutputFolderCommand { get; }
        #endregion

        #region Nested ViewModels
        public Settings Settings { get; } = Settings.Instance;

        public VideoViewModel VideoViewModel { get; } = new VideoViewModel();

        public AudioViewModel AudioViewModel { get; } = new AudioViewModel();

        public RecentViewModel RecentViewModel { get; } = new RecentViewModel();
        #endregion
    }
}
