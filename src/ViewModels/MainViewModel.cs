using Captura.Properties;
using Screna;
using Screna.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using Timer = System.Timers.Timer;
using Window = Screna.Window;

namespace Captura
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        readonly Timer _timer;
        IRecorder _recorder;
        string _currentFileName;
        readonly MouseCursor _cursor;
        bool isVideo;
        #endregion

        public static MainViewModel Instance { get; } = new MainViewModel();

        MainViewModel()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;

            #region Commands
            ScreenShotCommand = new DelegateCommand(CaptureScreenShot);

            RecordCommand = new DelegateCommand(() =>
            {
                if (RecorderState == RecorderState.NotRecording)
                    StartRecording();
                else StopRecording();
            });

            RefreshCommand = new DelegateCommand(() =>
            {
                VideoViewModel.RefreshVideoSources();

                VideoViewModel.RefreshCodecs();

                AudioViewModel.RefreshAudioSources();

                Status = "Refreshed";
            });

            OpenOutputFolderCommand = new DelegateCommand(() => Process.Start("explorer.exe", OutPath));

            PauseCommand = new DelegateCommand(() =>
            {
                if (RecorderState == RecorderState.Paused)
                {
                    _recorder.Start();
                    _timer.Start();

                    RecorderState = RecorderState.Recording;
                    Status = "Recording...";
                }
                else
                {
                    _recorder.Stop();
                    _timer.Stop();

                    RecorderState = RecorderState.Paused;
                    Status = "Paused";

                    SystemTrayManager.ShowNotification("Recording Paused", " ", 500, null);
                }
            }, false);

            SelectOutputFolderCommand = new DelegateCommand(() =>
            {
                var dlg = new FolderBrowserDialog
                {
                    SelectedPath = OutPath,
                    Description = "Select Output Folder"
                };

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                OutPath = dlg.SelectedPath;
                Settings.Default.OutputPath = dlg.SelectedPath;
                Settings.Default.Save();
            });
            #endregion

            //Populate Available Codecs, Audio and Video Sources ComboBoxes
            RefreshCommand.Execute(null);

            AudioViewModel.PropertyChanged += (Sender, Args) =>
            {
                if (Args.PropertyName == nameof(AudioViewModel.SelectedRecordingSource)
                || Args.PropertyName == nameof(AudioViewModel.SelectedLoopbackSource))
                    CheckFunctionalityAvailability();
            };

            VideoViewModel.PropertyChanged += (Sender, Args) =>
            {
                if (Args.PropertyName == nameof(VideoViewModel.SelectedVideoSource))
                    CheckFunctionalityAvailability();
            };

            _cursor = new MouseCursor(OthersViewModel.Cursor);

            OthersViewModel.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(OthersViewModel.Cursor):
                        _cursor.Include = OthersViewModel.Cursor;
                        break;
                }
            };

            // If Output Dircetory is not set. Set it to Documents\Captura\
            if (string.IsNullOrWhiteSpace(OutPath))
                OutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura\\");
            
            // Create the Output Directory if it does not exist
            if (!Directory.Exists(OutPath))
                Directory.CreateDirectory(OutPath);

            Settings.Default.OutputPath = OutPath;

            HotKeyManager.RegisterAll();
            SystemTrayManager.Init();
        }

        // Call before Exit to free Resources
        public void Dispose()
        {
            HotKeyManager.Dispose();
            SystemTrayManager.Dispose();

            AudioViewModel.Dispose();
        }
        
        void TimerOnElapsed(object Sender, ElapsedEventArgs Args)
        {
            TimeSpan += _addend;
            
            if (OthersViewModel.Duration <= 0 || !(TimeSpan.TotalSeconds >= OthersViewModel.Duration))
                return;

            // If Capture Duration is set
            StopRecording();
        }
        
        void CheckFunctionalityAvailability()
        {
            var audioAvailable = AudioViewModel.SelectedRecordingSource != null || AudioViewModel.SelectedLoopbackSource != null;

            var videoAvailable = VideoViewModel.SelectedVideoSourceKind != VideoSourceKind.NoVideo;

            RecordCommand.RaiseCanExecuteChanged(audioAvailable || videoAvailable);

            ScreenShotCommand.RaiseCanExecuteChanged(videoAvailable);
        }

        #region Commands
        public DelegateCommand ScreenShotCommand { get; }

        public DelegateCommand RecordCommand { get; }

        public DelegateCommand RefreshCommand { get; }

        public DelegateCommand OpenOutputFolderCommand { get; }

        public DelegateCommand PauseCommand { get; }

        public DelegateCommand SelectOutputFolderCommand { get; }
        #endregion

        void CaptureScreenShot()
        {
            if (!Directory.Exists(OutPath))
                Directory.CreateDirectory(OutPath);

            string fileName = null;

            var imgFmt = ScreenShotViewModel.SelectedImageFormat;

            var extension = imgFmt.Equals(ImageFormat.Icon) ? "ico"
                : imgFmt.Equals(ImageFormat.Jpeg) ? "jpg"
                : imgFmt.ToString().ToLower();

            var saveToClipboard = ScreenShotViewModel.SelectedSaveTo == "Clipboard";

            if (!saveToClipboard)
                fileName = Path.Combine(OutPath,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + extension);

            Bitmap bmp = null;
            
            var selectedVideoSource = VideoViewModel.SelectedVideoSource;
            var includeCursor = OthersViewModel.Cursor;

            switch (VideoViewModel.SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var hWnd = (selectedVideoSource as WindowItem)?.Window ?? Window.DesktopWindow;

                    if (hWnd == Window.DesktopWindow)
                        bmp = ScreenShot.Capture(includeCursor);
                    else
                    {
                        bmp = ScreenShot.CaptureTransparent(hWnd, includeCursor,
                                 ScreenShotViewModel.DoResize, ScreenShotViewModel.ResizeWidth, ScreenShotViewModel.ResizeHeight);

                        // Capture without Transparency
                        if (bmp == null)
                            bmp = ScreenShot.Capture(hWnd, includeCursor);
                    }
                    break;

                case VideoSourceKind.Screen:
                    bmp = (selectedVideoSource as ScreenItem)?.Capture(includeCursor);
                    break;

                case VideoSourceKind.Region:
                    bmp = ScreenShot.Capture(RegionSelector.Instance.Rectangle, includeCursor);
                    break;
            }

            // Save to Disk or Clipboard
            if (bmp != null)
            {
                if (saveToClipboard)
                {
                    bmp.WriteToClipboard(imgFmt.Equals(ImageFormat.Png));
                    Status = "Image Saved to Clipboard";
                }
                else
                {
                    try
                    {
                        bmp.Save(fileName, imgFmt);
                        Status = "Image Saved to Disk";
                        RecentViewModel.Add(fileName, RecentItemType.Image);

                        SystemTrayManager.ShowNotification("ScreenShot Saved", Path.GetFileName(fileName), 3000, () => Process.Start(fileName));
                    }
                    catch (Exception E)
                    {
                        Status = "Not Saved. " + E.Message;
                    }
                }

                bmp.Dispose();
            }
            else Status = "Not Saved - Image taken was Empty";
        }

        void StartRecording()
        {
            if (OthersViewModel.MinimizeOnStart)
                WindowState = WindowState.Minimized;
            
            CanChangeVideoSource = VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.Window;

            var duration = OthersViewModel.Duration;
            var delay = OthersViewModel.StartDelay;

            if (delay < 0)
                delay = 0;

            if (duration != 0 && (delay * 1000 > duration))
            {
                Status = "Delay cannot be greater than Duration";
                SystemSounds.Asterisk.Play();
                return;
            }

            RecorderState = RecorderState.Recording;
            
            isVideo = VideoViewModel.SelectedVideoSourceKind != VideoSourceKind.NoVideo;
            
            var extension = isVideo
                ? VideoViewModel.SelectedVideoWriter.Extension
                : (AudioViewModel.Encode ? ".mp3" : ".wav");

            _currentFileName = Path.Combine(OutPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + extension);

            Status = delay > 0 ? $"Recording from t = {delay} ms..." : "Recording...";

            _timer.Stop();
            TimeSpan = TimeSpan.Zero;
            
            var audioSource = AudioViewModel.GetAudioSource();

            var imgProvider = GetImageProvider();
            
            var videoEncoder = GetVideoFileWriter(imgProvider, audioSource);
            
            if (_recorder == null)
            {
                if (isVideo)
                    _recorder = new Recorder(videoEncoder, imgProvider, VideoViewModel.FrameRate, audioSource);

                else _recorder = new Recorder(AudioViewModel.GetAudioFileWriter(_currentFileName, audioSource.WaveFormat), audioSource);
            }

            /*_recorder.RecordingStopped += (s, E) =>
            {
                OnStopped();

                if (E?.Error == null)
                    return;

                Status = "Error";
                MessageBox.Show(E.ToString());
            };*/
            
            if (delay > 0)
            {
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(delay);

                    _recorder.Start();
                });
            }
            else _recorder.Start();

            _timer.Start();
        }

        IVideoFileWriter GetVideoFileWriter(IImageProvider ImgProvider, IAudioProvider AudioProvider)
        {
            if (VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.NoVideo)
                return null;
            
            IVideoFileWriter videoEncoder = null;

            // VideoVideoModel.Quality not used for now

            var encoder = VideoViewModel.SelectedVideoWriter.GetVideoFileWriter(_currentFileName, VideoViewModel.FrameRate, ImgProvider, AudioProvider);

            switch (encoder)
            {
                case GifWriter gif:
                    if (GifViewModel.Unconstrained)
                        _recorder = new UnconstrainedFrameRateGifRecorder(gif, ImgProvider);
                    
                    else videoEncoder = gif;
                    break;

                default:
                    videoEncoder = encoder;
                    break;
            }

            return videoEncoder;
        }
        
        IImageProvider GetImageProvider()
        {
            Func<System.Drawing.Point> offset = () => System.Drawing.Point.Empty;

            var imageProvider = VideoViewModel.SelectedVideoSource?.GetImageProvider(out offset);

            if (imageProvider == null)
                return null;

            var overlays = new List<IOverlay> { _cursor };

            if (OthersViewModel.MouseKeyHookAvailable)
                overlays.Add(new MouseKeyHook(OthersViewModel.MouseClicks, OthersViewModel.KeyStrokes));

            return new OverlayedImageProvider(imageProvider, offset, overlays.ToArray());
        }
        
        async void StopRecording()
        {
            Status = "Stopped";

            var savingRecentItem = RecentViewModel.AddTemp(_currentFileName);
            
            RecorderState = RecorderState.NotRecording;

            CanChangeVideoSource = true;
            
            if (OthersViewModel.MinimizeOnStart)
                WindowState = WindowState.Normal;

            _timer.Stop();

            var rec = _recorder;
            _recorder = null;

            await Task.Run(() => rec.Dispose());

            // After Save
            RecentViewModel.Remove(savingRecentItem);
            RecentViewModel.Add(_currentFileName, isVideo ? RecentItemType.Video : RecentItemType.Audio);

            SystemTrayManager.ShowNotification($"{(isVideo ? "Video" : "Audio")} Saved", Path.GetFileName(_currentFileName), 3000, () => Process.Start(_currentFileName));
        }
        
        #region Properties
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

        public string OutPath
        {
            get { return Settings.Default.OutputPath; }
            set
            {
                if (OutPath == value)
                    return;

                Settings.Default.OutputPath = value;

                OnPropertyChanged();
            }
        }

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
        
        WindowState _windowState = WindowState.Normal;

        public WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                if (_windowState == value)
                    return;

                _windowState = value;

                OnPropertyChanged();

                if (WindowState == WindowState.Minimized && OthersViewModel.MinimizeToTray)
                    App.Current.MainWindow.Hide();
            }
        }

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
        #endregion

        #region Nested ViewModels
        public VideoViewModel VideoViewModel { get; } = new VideoViewModel();

        public AudioViewModel AudioViewModel { get; } = new AudioViewModel();
        
        public GifViewModel GifViewModel { get; } = new GifViewModel();

        public ScreenShotViewModel ScreenShotViewModel { get; } = new ScreenShotViewModel();

        public OthersViewModel OthersViewModel { get; } = new OthersViewModel();

        public RecentViewModel RecentViewModel { get; } = new RecentViewModel();
        #endregion
    }
}