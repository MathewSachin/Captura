using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Captura.Properties;
using NAudio.CoreAudioApi;
using Screna;
using Screna.Audio;
using Screna.Avi;
using Screna.Lame;
using Screna.NAudio;

namespace Captura
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        readonly Timer _timer;
        IRecorder _recorder;
        string _lastFileName;
        readonly MouseCursor _cursor;
        #endregion

        public MainViewModel()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;

            //Populate Available Codecs, Audio and Video Sources ComboBoxes
            RefreshCommand.Execute(null);

            ScreenShotCommand = new DelegateCommand(CaptureScreenShot, () => _canScreenShot);

            RecordCommand = new DelegateCommand(() =>
            {
                if (ReadyToRecord)
                    StartRecording();
                else StopRecording();
            }, () => _canRecord);

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
            bool AudioAvailable = AudioViewModel.SelectedAudioSource.ToString() != "[No Sound]",
                VideoAvailable = VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.Window
                    || VideoViewModel.SelectedVideoSourceKind == VideoSourceKind.Screen;

            _canRecord = AudioAvailable || VideoAvailable;

            _canScreenShot = VideoAvailable;
        }

        #region Commands
        bool _canScreenShot = true, _canRecord = true;

        public ICommand ScreenShotCommand { get; }

        public ICommand RecordCommand { get; }

        public ICommand RefreshCommand => new DelegateCommand(() =>
        {
            VideoViewModel.RefreshVideoSources();

            VideoViewModel.RefreshCodecs();

            AudioViewModel.RefreshAudioSources();
            
            Status = $"{VideoViewModel.AvailableCodecs.Count} Encoder(s) and {AudioViewModel.AvailableAudioSources.Count - 1} AudioDevice(s) found";
        });

        public ICommand OpenOutputFolderCommand => new DelegateCommand(() => Process.Start("explorer.exe", OutPath));

        public ICommand PauseCommand => new DelegateCommand(() =>
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

        public ICommand SelectOutputFolderCommand => new DelegateCommand(() =>
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = OutPath,
                Description = "Select Output Folder"
            };

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            OutPath = dlg.SelectedPath;
            Settings.Default.OutputPath = dlg.SelectedPath;
            Settings.Default.Save();
        });
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

            var selectedVideoSourceKind = VideoViewModel.SelectedVideoSourceKind;
            var selectedVideoSource = VideoViewModel.SelectedVideoSource;
            var includeCursor = Settings.Default.IncludeCursor;

            switch (selectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var hWnd = (selectedVideoSource as WindowVSLI).Handle;

                    if (hWnd == WindowProvider.DesktopHandle)
                        bmp = ScreenShot.Capture(includeCursor);
                    else if (hWnd == RegionSelector.Instance.Handle)
                        bmp = ScreenShot.Capture(RegionSelector.Instance.Rectangle, includeCursor);
                    else bmp = ScreenShot.CaptureTransparent(hWnd, includeCursor,
                        ScreenShotViewModel.DoResize, ScreenShotViewModel.ResizeWidth, ScreenShotViewModel.ResizeHeight);
                    break;

                case VideoSourceKind.Screen:
                    bmp = (selectedVideoSource as ScreenVSLI).Capture(includeCursor);
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
            var selectedAudioSource = AudioViewModel.SelectedAudioSource;
            var selectedVideoSourceKind = VideoViewModel.SelectedVideoSourceKind;
            var selectedVideoSource = VideoViewModel.SelectedVideoSource;
            var encoder = VideoViewModel.SelectedCodec;

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

            var extension = selectedVideoSourceKind == VideoSourceKind.NoVideo
                ? (AudioViewModel.Encode && selectedAudioSource is WaveInDevice ? ".mp3" : ".wav")
                : (encoder.Name == "Gif" ? ".gif" : ".avi");

            _lastFileName = Path.Combine(OutPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + extension);

            Status = delay > 0 ? $"Recording from t={delay}ms..." : "Recording...";

            _timer.Stop();
            TimeSpan = TimeSpan.Zero;

            _timer.Start();

            var audioBitRate = App.IsLamePresent ? Mp3EncoderLame.SupportedBitRates[AudioViewModel.Quality] : 0;

            IAudioProvider audioSource = null;
            var wf = new WaveFormat(44100, 16, AudioViewModel.Stereo ? 2 : 1);

            if (selectedAudioSource.ToString() != "[No Sound]")
            {
                if (selectedAudioSource is WaveInDevice)
                    audioSource = new WaveInProvider(selectedAudioSource as WaveInDevice, VideoViewModel.FrameRate, wf);
                else if (selectedAudioSource is MMDevice)
                {
                    audioSource = new LoopbackProvider(selectedAudioSource as MMDevice);
                    wf = audioSource.WaveFormat;
                }
            }

            #region ImageProvider
            IImageProvider imgProvider = null;

            Func<System.Windows.Media.Color, System.Drawing.Color> convertColor = C => System.Drawing.Color.FromArgb(C.A, C.R, C.G, C.B);

            var mouseKeyHook = new MouseKeyHook(OthersViewModel.MouseClicks,
                                                OthersViewModel.KeyStrokes);

            switch (selectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var src = selectedVideoSource as WindowVSLI;

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
                    imgProvider = new ScreenProvider((selectedVideoSource as ScreenVSLI).Screen,
                        _cursor,
                        mouseKeyHook);
                    break;
            }
            #endregion

            #region VideoEncoder
            IVideoFileWriter videoEncoder = null;
            encoder.Quality = VideoViewModel.Quality;

            if (encoder.Name == "Gif")
            {
                if (GifViewModel.Unconstrained)
                    _recorder = new UnconstrainedFrameRateGifRecorder(
                               new GifWriter(_lastFileName,
                                             Repeat: GifViewModel.Repeat ? GifViewModel.RepeatCount : -1),
                                             imgProvider);

                else videoEncoder = new GifWriter(_lastFileName, 1000 / VideoViewModel.FrameRate,
                                                   GifViewModel.Repeat ? GifViewModel.RepeatCount : -1);
            }

            else if (selectedVideoSourceKind != VideoSourceKind.NoVideo)
                videoEncoder = new AviWriter(_lastFileName,
                                             encoder,
                                             audioBitRate == 0 ? null
                                                               : new Mp3EncoderLame(wf.Channels, wf.SampleRate, audioBitRate));
            #endregion

            if (_recorder == null)
            {
                if (selectedVideoSourceKind == VideoSourceKind.NoVideo)
                    _recorder = AudioViewModel.Encode ? new AudioRecorder(audioSource, new EncodedAudioFileWriter(_lastFileName, new Mp3EncoderLame(wf.Channels, wf.SampleRate, audioBitRate)))
                                                      : new AudioRecorder(audioSource, new WaveFileWriter(_lastFileName, wf));

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

            _recorder.Start(delay);

            RecentViewModel.Add(_lastFileName, videoEncoder == null ? RecentItemType.Audio : RecentItemType.Video);
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

        string _outPath;

        public string OutPath
        {
            get { return _outPath; }
            set
            {
                if (_outPath == value)
                    return;

                _outPath = value;

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