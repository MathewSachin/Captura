using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Captura.Properties;
using Screna;
using Screna.Audio;
using Screna.Avi;
using Screna.Lame;
using Screna.NAudio;
using WColor = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Timers.Timer;

namespace Captura
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        readonly Timer _timer;
        IRecorder _recorder;
        string _currentFileName;
        readonly MouseCursor _cursor;
        #endregion

        public MainViewModel()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;

            #region Commands
            ScreenShotCommand = new DelegateCommand(CaptureScreenShot, () => _canScreenShot);

            RecordCommand = new DelegateCommand(() =>
            {
                if (ReadyToRecord)
                    StartRecording();
                else StopRecording();
            }, () => _canRecord);

            RefreshCommand = new DelegateCommand(() =>
            {
                VideoViewModel.RefreshVideoSources();

                VideoViewModel.RefreshCodecs();

                AudioViewModel.RefreshAudioSources();

                Status = $"{VideoViewModel.AvailableCodecs.Count} Encoder(s) and {AudioViewModel.AvailableAudioSources.Count - 1} AudioDevice(s) found";
            });

            OpenOutputFolderCommand = new DelegateCommand(() => Process.Start("explorer.exe", OutPath));

            PauseCommand = new DelegateCommand(() =>
            {
                if (IsPaused)
                {
                    _recorder.Start();
                    _timer.Start();

                    IsPaused = false;
                    Status = "Recording...";
                }
                else
                {
                    _recorder.Pause();
                    _timer.Stop();

                    IsPaused = true;
                    Status = "Paused";
                }
            }, () => !ReadyToRecord && _recorder != null);

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
                if (Args.PropertyName == nameof(AudioViewModel.SelectedAudioSource))
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
                    case nameof(OthersViewModel.RegionSelectorVisible):
                        VideoViewModel.RefreshVideoSources();
                        break;

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
            Settings.Default.Save();
        }

        void TimerOnElapsed(object Sender, ElapsedEventArgs Args)
        {
            TimeSpan += _addend;
            
            if (OthersViewModel.Duration <= 0 || !(TimeSpan.TotalSeconds >= OthersViewModel.Duration))
                return;

            // If Capture Duration is set
            StopRecording();
            SystemSounds.Exclamation.Play();
        }
        
        void CheckFunctionalityAvailability()
        {
            var audioAvailable = AudioViewModel.SelectedAudioSource != null
                                 && AudioViewModel.SelectedAudioSource.ToString() != "[No Sound]";

            var videoAvailable = VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.Window
                                 || VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.Screen;

            _canRecord = audioAvailable || videoAvailable;

            _canScreenShot = videoAvailable;
        }

        #region Commands
        bool _canScreenShot = true, _canRecord = true;

        public ICommand ScreenShotCommand { get; }

        public ICommand RecordCommand { get; }

        public ICommand RefreshCommand { get; }

        public ICommand OpenOutputFolderCommand { get; }

        public ICommand PauseCommand { get; }

        public ICommand SelectOutputFolderCommand { get; }
        #endregion

        void CaptureScreenShot()
        {
            if (!Directory.Exists(OutPath))
                Directory.CreateDirectory(OutPath);

            string fileName = null;

            var imgFmt = ScreenShotViewModel.SelectedImageFormat;

            var extension = imgFmt == ImageFormat.Icon ? "ico"
                : imgFmt == ImageFormat.Jpeg ? "jpg"
                : imgFmt.ToString().ToLower();

            var saveToClipboard = ScreenShotViewModel.SelectedSaveTo == "Clipboard";

            if (!saveToClipboard)
                fileName = Path.Combine(OutPath,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + extension);

            Bitmap bmp = null;
            
            var selectedVideoSource = VideoViewModel.SelectedVideoSource;
            var includeCursor = Settings.Default.IncludeCursor;

            switch (VideoViewModel.SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var hWnd = (selectedVideoSource as WindowVSLI)?.Handle ?? WindowProvider.DesktopHandle;

                    if (hWnd == WindowProvider.DesktopHandle)
                        bmp = ScreenShot.Capture(includeCursor);
                    else if (hWnd == RegionSelector.Instance.Handle)
                        bmp = ScreenShot.Capture(RegionSelector.Instance.Rectangle, includeCursor);
                    else bmp = ScreenShot.CaptureTransparent(hWnd, includeCursor,
                        ScreenShotViewModel.DoResize, ScreenShotViewModel.ResizeWidth, ScreenShotViewModel.ResizeHeight);
                    break;

                case VideoSourceKind.Screen:
                    bmp = (selectedVideoSource as ScreenVSLI)?.Capture(includeCursor);
                    break;
            }

            // Save to Disk or Clipboard
            if (bmp != null)
            {
                if (saveToClipboard)
                {
                    bmp.WriteToClipboard(imgFmt == ImageFormat.Png);
                    Status = "Image Saved to Clipboard";
                }
                else
                {
                    try
                    {
                        bmp.Save(fileName, imgFmt);
                        Status = "Image Saved to Disk";
                        RecentViewModel.Add(fileName, RecentItemType.Image);
                    }
                    catch (Exception E)
                    {
                        Status = "Not Saved. " + E.Message;
                    }
                }
            }
            else Status = "Not Saved - Image taken was Empty";
        }

        void StartRecording()
        {
            var duration = OthersViewModel.Duration;
            var delay = OthersViewModel.StartDelay;

            if (duration != 0 && (delay * 1000 > duration))
            {
                Status = "Delay cannot be greater than Duration";
                SystemSounds.Asterisk.Play();
                return;
            }

            if (OthersViewModel.MinimizeOnStart)
                WindowState = WindowState.Minimized;
            
            ReadyToRecord = false;
            
            var noVideo = VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.NoVideo;
            
            var extension = noVideo
                ? (AudioViewModel.Encode && AudioViewModel.SelectedAudioSource is WaveInDevice ? ".mp3" : ".wav")
                : (VideoViewModel.SelectedCodec.Name == "Gif" ? ".gif" : ".avi");

            _currentFileName = Path.Combine(OutPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + extension);

            Status = delay > 0 ? $"Recording from t = {delay} ms..." : "Recording...";

            _timer.Stop();
            TimeSpan = TimeSpan.Zero;
            
            WaveFormat wf;
            var audioSource = AudioViewModel.GetAudioSource(VideoViewModel.FrameRate, out wf);

            var imgProvider = GetImageProvider();
            
            var videoEncoder = GetVideoFileWriter(imgProvider, AudioViewModel.BitRate, wf);
            
            if (_recorder == null)
            {
                if (noVideo)
                    _recorder = new AudioRecorder(audioSource, AudioViewModel.GetAudioFileWriter(_currentFileName, wf));

                else _recorder = new Recorder(videoEncoder, imgProvider, VideoViewModel.FrameRate, audioSource);
            }

            _recorder.RecordingStopped += (s, E) =>
            {
                OnStopped();

                if (E?.Error == null)
                    return;

                Status = "Error";
                MessageBox.Show(E.ToString());
            };

            RecentViewModel.Add(_currentFileName, videoEncoder == null ? RecentItemType.Audio : RecentItemType.Video);

            _recorder.Start(delay);

            _timer.Start();
        }

        IVideoFileWriter GetVideoFileWriter(IImageProvider ImgProvider, int AudioBitRate, WaveFormat Wf)
        {
            var selectedVideoSourceKind = VideoViewModel.SelectedVideoSourceKind;
            var encoder = VideoViewModel.SelectedCodec;

            IVideoFileWriter videoEncoder = null;
            encoder.Quality = VideoViewModel.Quality;

            if (encoder.Name == "Gif")
            {
                if (GifViewModel.Unconstrained)
                    _recorder = new UnconstrainedFrameRateGifRecorder(
                        new GifWriter(_currentFileName,
                            Repeat: GifViewModel.Repeat ? GifViewModel.RepeatCount : -1),
                        ImgProvider);

                else
                    videoEncoder = new GifWriter(_currentFileName, 1000/VideoViewModel.FrameRate,
                        GifViewModel.Repeat ? GifViewModel.RepeatCount : -1);
            }

            else if (selectedVideoSourceKind != VideoSourceKind.NoVideo)
                videoEncoder = new AviWriter(_currentFileName,
                    encoder,
                    AudioBitRate == 0
                        ? null
                        : new Mp3EncoderLame(Wf.Channels, Wf.SampleRate, AudioBitRate));
            return videoEncoder;
        }
        
        IImageProvider GetImageProvider()
        {
            IImageProvider imgProvider = null;

            Func<WColor, Color> convertColor =
                C => Color.FromArgb(C.A, C.R, C.G, C.B);

            var mouseKeyHook = new MouseKeyHook(OthersViewModel.MouseClicks,
                OthersViewModel.KeyStrokes);

            switch (VideoViewModel.SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var src = VideoViewModel.SelectedVideoSource as WindowVSLI;

                    if (src.Handle == RegionSelector.Instance.Handle
                        && OthersViewModel.StaticRegion)
                    {
                        imgProvider = new StaticRegionProvider(RegionSelector.Instance,
                            _cursor,
                            mouseKeyHook);
                    }
                    else imgProvider = new WindowProvider(() => (VideoViewModel.SelectedVideoSource as WindowVSLI).Handle,
                            convertColor(VideoViewModel.BackgroundColor),
                            _cursor,
                            mouseKeyHook);
                    break;

                case VideoSourceKind.Screen:
                    imgProvider = new ScreenProvider((VideoViewModel.SelectedVideoSource as ScreenVSLI).Screen,
                        _cursor,
                        mouseKeyHook);
                    break;
            }

            return imgProvider;
        }

        void OnStopped()
        {
            _recorder = null;
            
            ReadyToRecord = true;

            if (OthersViewModel.MinimizeOnStart)
                WindowState = WindowState.Normal;

            Status = "Saved to Disk";
            
            _timer.Stop();
            
            IsPaused = false;
        }

        void StopRecording()
        {
            _recorder.Stop();
            OnStopped();
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
            }
        }

        bool _isPaused;

        public bool IsPaused
        {
            get { return _isPaused; }
            private set
            {
                if (_isPaused == value)
                    return;

                _isPaused = value;

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