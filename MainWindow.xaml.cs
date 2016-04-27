using Captura.Properties;
using Screna;
using Screna.Audio;
using Screna.Avi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using Screna.NAudio;
using Screna.Lame;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Captura
{
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }

        #region Fields
        readonly DispatcherTimer _dTimer;
        int _seconds, _minutes, _delay, _duration;

        readonly KeyboardHookList _keyHook;

        IRecorder _recorder;
        string _lastFileName;
        readonly NotifyIcon _systemTray;

        readonly MouseCursor _cursor;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            #region Init Timer
            _dTimer = new DispatcherTimer(TimeSpan.FromSeconds(1),
                DispatcherPriority.Normal,
                (s, e) =>
                {
                    _seconds++;

                    if (_seconds == 60)
                    {
                        _seconds = 0;
                        _minutes++;
                    }

                    // If Capture Duration is set
                    if (_duration > 0 && (_minutes * 60 + _seconds >= _duration))
                    {
                        StopRecording();
                        SystemSounds.Exclamation.Play();

                        // SystemTray Notification
                        if (_systemTray.Visible) _systemTray.ShowBalloonTip(3000, "Capture Completed",
                            $"Capture Completed in {OtherSettings.CaptureDuration} seconds",
                            System.Windows.Forms.ToolTipIcon.None);
                    }

                    TimeManager.Content = $"{_minutes:D2}:{_seconds:D2}";
                },
                TimeManager.Dispatcher) { IsEnabled = false };
            #endregion

            //Populate Available Codecs, Audio and Video Sources ComboBoxes
            Refresh();

            #region Command Bindings
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (s, e) =>
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog
                {
                    SelectedPath = OutPath.Text,
                    Description = "Select Output Folder"
                };

                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                OutPath.Text = dlg.SelectedPath;
                Settings.Default.OutputPath = dlg.SelectedPath;
                Settings.Default.Save();
            }));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, (s, e) => StartRecording(),
                (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, (s, e) => StopRecording(),
                (s, e) => e.CanExecute = !ReadyToRecord));

            CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh, (s, e) => Refresh()));

            CommandBindings.Add(new CommandBinding(PauseCommand, (s, e) =>
            {
                _recorder.Pause();
                _dTimer.Stop();

                PauseButton.Command = ResumeCommand;
                RotationEffect.Angle = 90;
                Status.Content = "Paused";
                PauseButton.ToolTip = "Pause";
            }, (s, e) => e.CanExecute = !ReadyToRecord && _recorder != null));

            CommandBindings.Add(new CommandBinding(ResumeCommand, (s, e) =>
            {
                _recorder.Start();
                _dTimer.Start();

                PauseButton.Command = PauseCommand;
                RotationEffect.Angle = 0;
                Status.Content = "Recording...";
                PauseButton.ToolTip = "Resume";
            }, (s, e) => e.CanExecute = !ReadyToRecord && _recorder != null));
            #endregion

            #region SystemTray
            _systemTray = new NotifyIcon
            {
                Visible = false,
                Text = "Captura",
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location)
            };

            _systemTray.Click += (s, e) =>
            {
                _systemTray.Visible = false;
                Show();
                WindowState = WindowState.Normal;
            };

            StateChanged += (s, e) =>
            {
                if (WindowState != WindowState.Minimized || !OtherSettings.MinimizeToSysTray)
                    return;

                Hide();
                _systemTray.Visible = true;
            };
            #endregion

            #region KeyHook
            _keyHook = new KeyboardHookList(this);

            _keyHook.Register(KeyCode.R, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
                () => Dispatcher.Invoke(() => ToggleRecorderState()));

            _keyHook.Register(KeyCode.S, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
                () => Dispatcher.Invoke(() => CaptureScreenShot()));
            #endregion

            // If Output Dircetory is not set. Set it to Documents\Captura\
            if (string.IsNullOrWhiteSpace(OutPath.Text))
                OutPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura\\");
            // Create the Output Directory if it does not exist
            if (!Directory.Exists(OutPath.Text))
                Directory.CreateDirectory(OutPath.Text);
            Settings.Default.OutputPath = OutPath.Text;
            Settings.Default.Save();

            Closed += (s, e) => Application.Current.Shutdown();

            _cursor = new MouseCursor(OtherSettings.IncludeCursor);
            OtherSettings.Instance.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "_IncludeCursor")
                        _cursor.Include = OtherSettings.IncludeCursor;
                };

        }

        void OpenOutputFolder(object sender, MouseButtonEventArgs MouseButtonEventArgs) => Process.Start("explorer.exe", OutPath.Text);

        void CaptureScreenShot(object sender = null, RoutedEventArgs e = null)
        {
            if (!Directory.Exists(OutPath.Text))
                Directory.CreateDirectory(OutPath.Text);

            string fileName = null;

            var imgFmt = ScreenShotSettings.SelectedImageFormat;

            var extension = imgFmt == ImageFormat.Icon ? "ico"
                : imgFmt == ImageFormat.Jpeg ? "jpg"
                : imgFmt.ToString().ToLower();

            var saveToClipboard = ScreenShotSettings.SaveToClipboard;

            if (!saveToClipboard)
                fileName = Path.Combine(OutPath.Text,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + extension);

            Bitmap bmp = null;

            var selectedVideoSourceKind = VideoSettings.SelectedVideoSourceKind;
            var selectedVideoSource = VideoSettings.SelectedVideoSource;
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
                        ScreenShotSettings.DoResize, ScreenShotSettings.ResizeWidth, ScreenShotSettings.ResizeHeight);
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
                    Status.Content = "Image Saved to Clipboard";
                }
                else
                {
                    try
                    {
                        bmp.Save(fileName, imgFmt);
                        Status.Content = "Image Saved to Disk";
                        Recent.Add(fileName, RecentItemType.Image);
                    }
                    catch (Exception E)
                    {
                        Status.Content = "Not Saved. " + E.Message;
                    }
                }
            }
            else Status.Content = "Not Saved - Image taken was Empty";
        }

        #region Recorder Control
        void ToggleRecorderState(object sender = null, RoutedEventArgs e = null)
        {
            if (ReadyToRecord) StartRecording();
            else StopRecording();
        }

        void StartRecording()
        {
            var selectedAudioSource = AudioSettings.SelectedAudioSource;
            var selectedVideoSourceKind = VideoSettings.SelectedVideoSourceKind;
            var selectedVideoSource = VideoSettings.SelectedVideoSource;
            var encoder = VideoSettings.Encoder;

            _duration = OtherSettings.CaptureDuration;
            _delay = OtherSettings.StartDelay;

            if (_duration != 0 && (_delay * 1000 > _duration))
            {
                Status.Content = "Delay cannot be greater than Duration";
                SystemSounds.Asterisk.Play();
                return;
            }

            if (OtherSettings.MinimizeOnStart)
                WindowState = WindowState.Minimized;

            VideoSettings.Instance.VideoSourceKindBox.IsEnabled = false;
            VideoSettings.Instance.VideoSourceBox.IsEnabled = selectedVideoSourceKind == VideoSourceKind.Window;

            // UI Buttons
            RecordButton.ToolTip = "Stop";
            RecordButton.IconData = (RectangleGeometry)FindResource("StopIcon");

            ReadyToRecord = false;
            
            var extension = selectedVideoSourceKind == VideoSourceKind.NoVideo
                ? (AudioSettings.EncodeAudio && selectedAudioSource is WaveInDevice ? ".mp3" : ".wav")
                : (encoder.Name == "Gif" ? ".gif" : ".avi");

            _lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + extension);

            Status.Content = _delay > 0 ? $"Recording from t={_delay}ms..." : "Recording...";

            _dTimer.Stop();
            _seconds = _minutes = 0;
            TimeManager.Content = "00:00";

            _dTimer.Start();

            var audioBitRate = App.IsLamePresent ? Mp3EncoderLame.SupportedBitRates[AudioSettings.AudioQuality] : 0;

            IAudioProvider audioSource = null;
            var wf = new WaveFormat(44100, 16, AudioSettings.Stereo ? 2 : 1);

            if (selectedAudioSource.ToString() != "[No Sound]")
            {
                if (selectedAudioSource is WaveInDevice)
                    audioSource = new WaveInProvider(selectedAudioSource as WaveInDevice, VideoSettings.FrameRate, wf);
                else if (selectedAudioSource is MMDevice)
                {
                    audioSource = new LoopbackProvider(selectedAudioSource as MMDevice);
                    wf = audioSource.WaveFormat;
                }
            }

            #region ImageProvider
            IImageProvider imgProvider = null;

            Func<System.Windows.Media.Color, System.Drawing.Color> convertColor = C => System.Drawing.Color.FromArgb(C.A, C.R, C.G, C.B);

            var mouseKeyHook = new MouseKeyHook(OtherSettings.CaptureClicks,
                                                OtherSettings.CaptureKeystrokes);

            switch (selectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var src = selectedVideoSource as WindowVSLI;

                    if (src.Handle == RegionSelector.Instance.Handle
                        && OtherSettings.StaticRegionCapture)
                    {
                        imgProvider = new StaticRegionProvider(RegionSelector.Instance,
                            _cursor,
                            mouseKeyHook);
                        VideoSettings.Instance.VideoSourceBox.IsEnabled = false;
                    }
                    else imgProvider = new WindowProvider(() => (VideoSettings.SelectedVideoSource as WindowVSLI).Handle,
                        convertColor(VideoSettings.BackgroundColor),
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
            encoder.Quality = VideoSettings.VideoQuality;

            if (encoder.Name == "Gif")
            {
                if (GifSettings.UnconstrainedGif)
                    _recorder = new UnconstrainedFrameRateGifRecorder(
                               new GifWriter(_lastFileName,
                                             Repeat: GifSettings.GifRepeat ? GifSettings.GifRepeatCount : -1),
                                             imgProvider);

                else videoEncoder = new GifWriter(_lastFileName, 1000 / VideoSettings.FrameRate,
                                                   GifSettings.GifRepeat ? GifSettings.GifRepeatCount : -1);
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
                    _recorder = AudioSettings.EncodeAudio ? new AudioRecorder(audioSource, new EncodedAudioFileWriter(_lastFileName, new Mp3EncoderLame(wf.Channels, wf.SampleRate, audioBitRate))) 
                                                         : new AudioRecorder(audioSource, new WaveFileWriter(_lastFileName, wf));
                
                else _recorder = new Recorder(videoEncoder, imgProvider, VideoSettings.FrameRate, audioSource);
            }

            _recorder.RecordingStopped += (s, E) => Dispatcher.Invoke(() =>
            {
                OnStopped();

                if (E?.Error == null)
                    return;

                Status.Content = "Error";
                MessageBox.Show(E.ToString());
            });

            _recorder.Start(_delay);

            Recent.Add(_lastFileName,
                       videoEncoder == null ? RecentItemType.Audio : RecentItemType.Video);
        }

        void OnStopped()
        {
            _recorder = null;

            VideoSettings.Instance.VideoSourceKindBox.IsEnabled = VideoSettings.Instance.VideoSourceBox.IsEnabled = true;

            ReadyToRecord = true;

            WindowState = WindowState.Normal;

            Status.Content = "Saved to Disk";

            _dTimer.Stop();

            // UI Buttons
            PauseButton.Command = PauseCommand;
            RotationEffect.Angle = 0;
            RecordButton.ToolTip = "Record";
            RecordButton.IconData = (Geometry)FindResource("RecordIcon");
            PauseButton.ToolTip = "Pause";
        }

        void StopRecording()
        {
            _recorder.Stop();
            OnStopped();
        }
        #endregion

        public static readonly RoutedUICommand PauseCommand = new RoutedUICommand("Pause", "Pause", typeof(MainWindow)),
            ResumeCommand = new RoutedUICommand("Pause", "Pause", typeof(MainWindow));

        public static readonly DependencyProperty ReadyToRecordProperty =
            DependencyProperty.Register("ReadyToRecord", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(true));

        public bool ReadyToRecord
        {
            get { return (bool)GetValue(ReadyToRecordProperty); }
            set { SetValue(ReadyToRecordProperty, value); }
        }

        public void Refresh(object sender = null, EventArgs e = null)
        {
            VideoSettings.RefreshVideoSources();

            VideoSettings.RefreshCodecs();

            AudioSettings.RefreshAudioSources();

            // Status
            Status.Content = $"{VideoSettings.AvailableCodecs.Count} Encoder(s) and {AudioSettings.AvailableAudioSources.Count - 1} AudioDevice(s) found";
        }

        void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) => Process.Start(e.Uri.AbsoluteUri);
    }
}
