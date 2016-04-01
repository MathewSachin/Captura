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
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Captura
{
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }

        #region Fields
        DispatcherTimer DTimer;
        int _seconds, _minutes, _delay, _duration;

        KeyboardHookList KeyHook;

        IRecorder Recorder;
        string lastFileName;
        readonly NotifyIcon SystemTray;

        readonly MouseCursor cursor;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            #region Init Timer
            DTimer = new DispatcherTimer(TimeSpan.FromSeconds(1),
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
                        if (SystemTray.Visible) SystemTray.ShowBalloonTip(3000, "Capture Completed",
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
                var dlg = new FolderBrowserDialog
                {
                    SelectedPath = OutPath.Text,
                    Title = "Select Output Folder"
                };

                if (!dlg.ShowDialog().Value)
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
                Recorder.Pause();
                DTimer.Stop();

                PauseButton.Command = ResumeCommand;
                RotationEffect.Angle = 90;
                Status.Content = "Paused";
                PauseButton.ToolTip = "Pause";
            }, (s, e) => e.CanExecute = !ReadyToRecord && Recorder != null));

            CommandBindings.Add(new CommandBinding(ResumeCommand, (s, e) =>
            {
                Recorder.Start();
                DTimer.Start();

                PauseButton.Command = PauseCommand;
                RotationEffect.Angle = 0;
                Status.Content = "Recording...";
                PauseButton.ToolTip = "Resume";
            }, (s, e) => e.CanExecute = !ReadyToRecord && Recorder != null));
            #endregion

            #region SystemTray
            SystemTray = new NotifyIcon
            {
                Visible = false,
                Text = "Captura",
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location)
            };

            SystemTray.Click += (s, e) =>
            {
                SystemTray.Visible = false;
                Show();
                WindowState = WindowState.Normal;
            };

            StateChanged += (s, e) =>
            {
                if (WindowState == WindowState.Minimized && OtherSettings.MinimizeToSysTray)
                {
                    Hide();
                    SystemTray.Visible = true;
                }
            };
            #endregion

            #region KeyHook
            KeyHook = new KeyboardHookList(this);

            KeyHook.Register(KeyCode.R, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
                () => Dispatcher.Invoke(() => ToggleRecorderState()));

            KeyHook.Register(KeyCode.S, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
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

            cursor = new MouseCursor(OtherSettings.IncludeCursor);
            OtherSettings.Instance.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "_IncludeCursor")
                        cursor.Include = OtherSettings.IncludeCursor;
                };

        }

        void OpenOutputFolder(object sender, MouseButtonEventArgs MouseButtonEventArgs) { Process.Start("explorer.exe", OutPath.Text); }

        void CaptureScreenShot(object sender = null, RoutedEventArgs e = null)
        {
            if (!Directory.Exists(OutPath.Text)) Directory.CreateDirectory(OutPath.Text);

            string FileName = null;
            var ImgFmt = ScreenShotSettings.SelectedImageFormat;
            var Extension = ImgFmt == ImageFormat.Icon ? "ico"
                : ImgFmt == ImageFormat.Jpeg ? "jpg"
                : ImgFmt.ToString().ToLower();
            var SaveToClipboard = ScreenShotSettings.SaveToClipboard;

            if (!SaveToClipboard)
                FileName = Path.Combine(OutPath.Text,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + Extension);

            Bitmap BMP = null;

            var SelectedVideoSourceKind = VideoSettings.SelectedVideoSourceKind;
            var SelectedVideoSource = VideoSettings.SelectedVideoSource;
            var IncludeCursor = Settings.Default.IncludeCursor;

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var hWnd = (SelectedVideoSource as WindowVSLI).Handle;

                    if (hWnd == WindowProvider.DesktopHandle)
                        BMP = ScreenShot.Capture(IncludeCursor);
                    else if (hWnd == RegionSelector.Instance.Handle)
                        BMP = ScreenShot.Capture(RegionSelector.Instance.Rectangle, IncludeCursor);
                    else BMP = ScreenShot.CaptureTransparent(hWnd, IncludeCursor,
                        ScreenShotSettings.DoResize, ScreenShotSettings.ResizeWidth, ScreenShotSettings.ResizeHeight);
                    break;

                case VideoSourceKind.Screen:
                    BMP = (SelectedVideoSource as ScreenVSLI).Capture(IncludeCursor);
                    break;
            }

            // Save to Disk or Clipboard
            if (BMP != null)
            {
                if (SaveToClipboard)
                {
                    BMP.WriteToClipboard(ImgFmt == ImageFormat.Png);
                    Status.Content = "Image Saved to Clipboard";
                }
                else
                {
                    try
                    {
                        BMP.Save(FileName, ImgFmt);
                        Status.Content = "Image Saved to Disk";
                    }
                    catch (Exception E)
                    {
                        Status.Content = "Not Saved. " + E.Message;
                        return;
                    }
                }

                if (FileName != null && !SaveToClipboard) Recent.Add(FileName, RecentItemType.Image);
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
            var SelectedAudioSourceId = AudioSettings.SelectedAudioSourceId;
            var SelectedVideoSourceKind = VideoSettings.SelectedVideoSourceKind;
            var SelectedVideoSource = VideoSettings.SelectedVideoSource;
            var Encoder = VideoSettings.Encoder;

            _duration = OtherSettings.CaptureDuration;
            _delay = OtherSettings.StartDelay;

            if (_duration != 0 && (_delay * 1000 > _duration))
            {
                Status.Content = "Delay cannot be greater than Duration";
                SystemSounds.Asterisk.Play();
                return;
            }

            if (OtherSettings.MinimizeOnStart) WindowState = WindowState.Minimized;

            VideoSettings.Instance.VideoSourceKindBox.IsEnabled = false;
            VideoSettings.Instance.VideoSourceBox.IsEnabled = SelectedVideoSourceKind == VideoSourceKind.Window;

            // UI Buttons
            RecordButton.ToolTip = "Stop";
            RecordButton.IconData = (RectangleGeometry)FindResource("StopIcon");

            ReadyToRecord = false;

            int temp;

            var Extension = SelectedVideoSourceKind == VideoSourceKind.NoVideo
                ? (AudioSettings.EncodeAudio && int.TryParse(SelectedAudioSourceId, out temp) ? ".mp3" : ".wav")
                : (Encoder.Name == "Gif" ? ".gif" : ".avi");

            lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + Extension);

            Status.Content = _delay > 0 ? $"Recording from t={_delay}ms..." : "Recording...";

            DTimer.Stop();
            _seconds = _minutes = 0;
            TimeManager.Content = "00:00";

            DTimer.Start();

            var AudioBitRate = App.IsLamePresent ? Mp3EncoderLame.SupportedBitRates[AudioSettings.AudioQuality] : 0;

            IAudioProvider AudioSource = null;
            var wf = new WaveFormat(44100, 16, AudioSettings.Stereo ? 2 : 1);

            if (SelectedAudioSourceId != "-1")
            {
                int i;
                if (int.TryParse(SelectedAudioSourceId, out i))
                    AudioSource = new WaveIn(i, VideoSettings.FrameRate, wf);
                else
                {
                    AudioSource = new WasapiLoopbackCapture(WasapiAudioDevice.Get(SelectedAudioSourceId));
                    wf = AudioSource.WaveFormat;
                }
            }

            #region ImageProvider
            IImageProvider ImgProvider = null;

            Func<System.Windows.Media.Color, System.Drawing.Color> ConvertColor = C => System.Drawing.Color.FromArgb(C.A, C.R, C.G, C.B);

            var mouseKeyHook = new MouseKeyHook(OtherSettings.CaptureClicks,
                                                OtherSettings.CaptureKeystrokes);

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    var Src = SelectedVideoSource as WindowVSLI;

                    if (Src.Handle == RegionSelector.Instance.Handle
                        && OtherSettings.StaticRegionCapture)
                    {
                        ImgProvider = new StaticRegionProvider(RegionSelector.Instance,
                            cursor,
                            mouseKeyHook);
                        VideoSettings.Instance.VideoSourceBox.IsEnabled = false;
                    }
                    else ImgProvider = new WindowProvider(() => (VideoSettings.SelectedVideoSource as WindowVSLI).Handle,
                        ConvertColor(VideoSettings.BackgroundColor),
                        cursor,
                        mouseKeyHook);
                    break;

                case VideoSourceKind.Screen:
                    ImgProvider = new ScreenProvider((SelectedVideoSource as ScreenVSLI).Screen,
                        cursor,
                        mouseKeyHook);
                    break;
            }
            #endregion

            #region VideoEncoder
            IVideoFileWriter VideoEncoder = null;

            if (Encoder.Name == "Gif")
            {
                if (GifSettings.UnconstrainedGif)
                    Recorder = new UnconstrainedFrameRateGifRecorder(
                               new GifWriter(lastFileName,
                                              Repeat: GifSettings.GifRepeat ? GifSettings.GifRepeatCount : -1),
                                              ImgProvider);

                else VideoEncoder = new GifWriter(lastFileName, 1000 / VideoSettings.FrameRate,
                                                   GifSettings.GifRepeat ? GifSettings.GifRepeatCount : -1);
            }

            else if (SelectedVideoSourceKind != VideoSourceKind.NoVideo)
                VideoEncoder = new AviWriter(lastFileName,
                                              ImgProvider,
                                              Encoder,
                                              VideoSettings.VideoQuality,
                                              VideoSettings.FrameRate,
                                              AudioSource,
                                              AudioBitRate == 0 ? null
                                                                : new Mp3EncoderLame(wf.Channels, wf.SampleRate, AudioBitRate));
            #endregion

            if (Recorder == null)
            {
                if (SelectedVideoSourceKind == VideoSourceKind.NoVideo)
                    Recorder = AudioSettings.EncodeAudio ? new AudioRecorder(AudioSource, new EncodedAudioFileWriter(lastFileName, new Mp3EncoderLame(wf.Channels, wf.SampleRate, AudioBitRate))) 
                                                         : new AudioRecorder(AudioSource, new WaveFileWriter(lastFileName, wf));
                
                else Recorder = new Recorder(VideoEncoder, ImgProvider, AudioSource);
            }

            Recorder.RecordingStopped += E => Dispatcher.Invoke(() =>
            {
                OnStopped();

                if (E != null)
                {
                    Status.Content = "Error";
                    MessageBox.Show(E.ToString());
                }
            });

            Recorder.Start(_delay);

            Recent.Add(lastFileName,
                       VideoEncoder == null ? RecentItemType.Audio : RecentItemType.Video);
        }

        void OnStopped()
        {
            Recorder = null;

            VideoSettings.Instance.VideoSourceKindBox.IsEnabled = VideoSettings.Instance.VideoSourceBox.IsEnabled = true;

            ReadyToRecord = true;

            WindowState = WindowState.Normal;

            Status.Content = "Saved to Disk";

            DTimer.Stop();

            // UI Buttons
            PauseButton.Command = PauseCommand;
            RotationEffect.Angle = 0;
            RecordButton.ToolTip = "Record";
            RecordButton.IconData = (Geometry)FindResource("RecordIcon");
            PauseButton.ToolTip = "Pause";
        }

        void StopRecording()
        {
            Recorder.Stop();
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
            Status.Content =
                $"{VideoSettings.AvailableCodecs.Count} Encoder(s) and {AudioSettings.AvailableAudioSources.Count - 1} AudioDevice(s) found";
        }

        void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
