using ScreenWorks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Captura
{
    public enum VideoSourceKind
    {
        NoVideo,
        Window,
        Screen,
        WebCam
    }

    /// <summary>
    /// Main Interface of Captura
    /// </summary>
    partial class Home : UserControl
    {
        /// <summary>
        /// Used to Identify Gif VideoEncoder Option
        /// </summary>
        static readonly string GifEncoderKey = "_gif";

        #region Fields
        DispatcherTimer DTimer;
        int Seconds = 0, Minutes = 0, Delay = 0, Duration = 0;

        KeyboardHookList KeyHook;

        IRecorder Recorder;
        string lastFileName;
        NotifyIcon SystemTray;
        #endregion

        public Home()
        {
            InitializeComponent();

            DataContext = this;

            // Init Observable Collections
            AvailableCodecs = new ObservableCollection<KeyValuePair<string, string>>();
            AvailableAudioSources = new ObservableCollection<KeyValuePair<string, string>>();
            AvailableVideoSources = new ObservableCollection<IVideoSourceListItem>();
            AvailableVideoSourceKinds = new ObservableCollection<KeyValuePair<VideoSourceKind, string>>();

            AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.NoVideo, "No Video"));
            AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Window, "Window"));

            if (App.IsDirectShowPresent)
                AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.WebCam, "WebCam"));
            if (ScreenVSLI.Count > 1)
                AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Screen, "Screen"));

            //Populate Available Codecs, Audio and Video Sources ComboBoxes
            Refresh();

            #region Init Timer
            DTimer = new DispatcherTimer();
            DTimer.Interval = TimeSpan.FromSeconds(1);
            DTimer.Tick += (s, e) =>
            {
                Seconds++;

                if (Seconds == 60)
                {
                    Seconds = 0;
                    Minutes++;
                }

                // If Capture Duration is set
                if (Duration > 0 && (Minutes * 60 + Seconds >= Duration))
                {
                    StopRecording();
                    SystemSounds.Exclamation.Play();

                    // SystemTray Notification
                    if (SystemTray.Visible) SystemTray.ShowBalloonTip(3000, "Capture Completed",
                        string.Format("Capture Completed in {0} seconds", CaptureDuration),
                        System.Windows.Forms.ToolTipIcon.None);
                }
                if (Delay > 0 && ((Minutes * 60 + Seconds) * 1000) >= Delay)
                {
                    Status.Content = "Recording...";
                    Delay = 0;
                }

                TimeManager.Content = string.Format("{0:D2}:{1:D2}", Minutes, Seconds);
            };
            #endregion

            #region Command Bindings
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (s, e) =>
                {
                    var dlg = new FolderBrowserDialog()
                    {
                        SelectedPath = OutPath.Text,
                        Title = "Select Output Folder"
                    };

                    if (dlg.ShowDialog().Value) OutPath.Text = dlg.SelectedPath;
                }));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => MainWindow.Instance.Close(), (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, (s, e) => StartRecording(),
                (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, (s, e) => StopRecording(),
                (s, e) => e.CanExecute = !ReadyToRecord));

            CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh));

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

            RegionSelector.Closing += (s, e) =>
                {
                    RegSelBox.IsChecked = false;
                    e.Cancel = true;
                };

            #region SystemTray
            SystemTray = new NotifyIcon()
            {
                Visible = false,
                Text = "Captura",
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location)
            };

            SystemTray.Click += (s, e) =>
                {
                    SystemTray.Visible = false;
                    MainWindow.Instance.Show();
                    MainWindow.Instance.WindowState = WindowState.Normal;
                };

            MainWindow.Instance.StateChanged += (s, e) =>
                {
                    if (MainWindow.Instance.WindowState == WindowState.Minimized && VideoSettings.MinimizeToSysTray)
                    {
                        MainWindow.Instance.Hide();
                        SystemTray.Visible = true;
                    }
                };
            #endregion

            #region KeyHook
            KeyHook = new KeyboardHookList(MainWindow.Instance);

            KeyHook.Register(KeyCode.R, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
                () => Dispatcher.Invoke(new Action(() => ToggleRecorderState<int>())));

            KeyHook.Register(KeyCode.S, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
                () => Dispatcher.Invoke(new Action(() => ScreenShot<int>())));
            #endregion

            // If Output Dircetory is not set. Set it to Documents\Captura\
            if (string.IsNullOrWhiteSpace(OutPath.Text))
                OutPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura\\");
            // Create the Output Directory if it does not exist
            if (!Directory.Exists(OutPath.Text))
                Directory.CreateDirectory(OutPath.Text);
        }

        #region RegionSelector
        RegionSelector RegionSelector = RegionSelector.Instance;

        void ShowRegionSelector(object sender, RoutedEventArgs e)
        {
            RegionSelector.Show();
            Refresh();
        }

        void HideRegionSelector(object sender, RoutedEventArgs e)
        {
            RegionSelector.Hide();
            Refresh();
        }
        #endregion

        #region Dependency Properties and RoutedUICommands
        public static readonly RoutedUICommand PauseCommand = new RoutedUICommand("Pause", "Pause", typeof(Home)),
            ResumeCommand = new RoutedUICommand("Pause", "Pause", typeof(Home));

        public static readonly DependencyProperty ReadyToRecordProperty =
                    DependencyProperty.Register("ReadyToRecord", typeof(bool), typeof(Home), new UIPropertyMetadata(true));

        public bool ReadyToRecord
        {
            get { return (bool)GetValue(ReadyToRecordProperty); }
            set { SetValue(ReadyToRecordProperty, value); }
        }

        public static bool IncludeCursor { get { return Properties.Settings.Default.IncludeCursor; } }
        #endregion

        void OpenOutputFolder<T>(object sender, T e) { Process.Start("explorer.exe", OutPath.Text); }

        void ScreenShot<T>(object sender = null, T e = default(T))
        {
            string FileName = null;
            ImageFormat ImgFmt = ScreenShotSettings.SelectedImageFormat;
            string Extension = ImgFmt == ImageFormat.Icon ? "ico"
                : ImgFmt == ImageFormat.Jpeg ? "jpg"
                : ImgFmt.ToString();
            bool SaveToClipboard = ScreenShotSettings.SaveToClipboard;

            if (!SaveToClipboard)
                FileName = Path.Combine(OutPath.Text,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + Extension);

            Bitmap BMP = null;

            if (SelectedVideoSourceKind == VideoSourceKind.Window)
            {
                IntPtr hWnd = (SelectedVideoSource as WindowVSLI).Handle;

                if (hWnd == WindowProvider.DesktopHandle || hWnd == RegionSelector.Handle)
                {
                    Rectangle Rect = WindowProvider.DesktopRectangle;

                    if (hWnd != WindowProvider.DesktopHandle)
                        Rect = new Rectangle((int)RegionSelector.Left, (int)RegionSelector.Top,
                            (int)RegionSelector.Width, (int)RegionSelector.Height);

                    BMP = new Bitmap(Rect.Width, Rect.Height);

                    using (var g = Graphics.FromImage(BMP))
                    {
                        g.CopyFromScreen(Rect.Location, System.Drawing.Point.Empty, Rect.Size, CopyPixelOperation.SourceCopy);

                        if (IncludeCursor) g.DrawCursor(Rect.Location);

                        g.Flush();
                    }
                }
                else BMP = new TransparentScreenshot().CaptureWindow(hWnd, IncludeCursor,
                    ScreenShotSettings.DoResize, ScreenShotSettings.ResizeWidth, ScreenShotSettings.ResizeHeight);
            }
            else if (SelectedVideoSourceKind == VideoSourceKind.WebCam)
                BMP = (SelectedVideoSource as WebCamVSLI).Capture();

            else if (SelectedVideoSourceKind == VideoSourceKind.Screen)
                BMP = (SelectedVideoSource as ScreenVSLI).Capture(IncludeCursor);

            // Save to Disk or Clipboard
            if (BMP != null)
            {
                if (SaveToClipboard)
                {
                    BMP.WriteToClipboard(ImgFmt == ImageFormat.Png);
                    Status.Content = "Saved to Clipboard";
                }
                else
                {
                    try
                    {
                        BMP.Save(FileName, ImgFmt);
                        Status.Content = "Saved to " + FileName;
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

        #region AudioSources, VideoSources and Encoders
        public ObservableCollection<KeyValuePair<string, string>> AvailableCodecs { get; private set; }

        public static readonly DependencyProperty EncoderProperty =
                    DependencyProperty.Register("Encoder", typeof(string), typeof(Home), new UIPropertyMetadata(GifEncoderKey));

        public string Encoder
        {
            get { return (string)GetValue(EncoderProperty); }
            set { SetValue(EncoderProperty, value); }
        }

        public ObservableCollection<KeyValuePair<string, string>> AvailableAudioSources { get; private set; }

        public static readonly DependencyProperty SelectedAudioSourceIdProperty =
            DependencyProperty.Register("SelectedAudioSourceIndex", typeof(string), typeof(Home), new UIPropertyMetadata("-1"));

        public string SelectedAudioSourceId
        {
            get { return (string)GetValue(SelectedAudioSourceIdProperty); }
            set { SetValue(SelectedAudioSourceIdProperty, value); }
        }

        public ObservableCollection<KeyValuePair<VideoSourceKind, string>> AvailableVideoSourceKinds { get; private set; }

        public static readonly DependencyProperty SelectedVideoSourceKindProperty =
            DependencyProperty.Register("SelectedVideoSourceKind", typeof(VideoSourceKind), typeof(Home),
            new UIPropertyMetadata(VideoSourceKind.Window));

        public VideoSourceKind SelectedVideoSourceKind
        {
            get { return (VideoSourceKind)GetValue(SelectedVideoSourceKindProperty); }
            set { SetValue(SelectedVideoSourceKindProperty, value); }
        }

        public ObservableCollection<IVideoSourceListItem> AvailableVideoSources { get; private set; }

        public static readonly DependencyProperty SelectedVideoSourceProperty =
            DependencyProperty.Register("SelectedVideoSource", typeof(IVideoSourceListItem), typeof(Home),
            new UIPropertyMetadata(WindowVSLI.Desktop));

        public IVideoSourceListItem SelectedVideoSource
        {
            get { return (IVideoSourceListItem)GetValue(SelectedVideoSourceProperty); }
            set { SetValue(SelectedVideoSourceProperty, value); }
        }

        void VideoSourceKindBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckFunctionalityAvailability();

            RefreshVideoSources();
        }

        void AudioVideoSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckFunctionalityAvailability();
        }

        void CheckFunctionalityAvailability()
        {
            bool AudioAvailable = SelectedAudioSourceId != "-1",
                VideoAvailable = SelectedVideoSourceKind == VideoSourceKind.Window
                    || SelectedVideoSourceKind == VideoSourceKind.Screen;

            RecordButton.IsEnabled = AudioAvailable || VideoAvailable;

            ScreenShotButton.IsEnabled = VideoAvailable
                || SelectedVideoSourceKind == VideoSourceKind.WebCam;
        }

        void RefreshVideoSources()
        {
            AvailableVideoSources.Clear();

            switch (SelectedVideoSourceKind)
            {
                case VideoSourceKind.Window:
                    AvailableVideoSources.Add(WindowVSLI.Desktop);
                    AvailableVideoSources.Add(WindowVSLI.TaskBar);

                    foreach (var win in WindowHandler.EnumerateVisible())
                        AvailableVideoSources.Add(new WindowVSLI(win.Handle));
                    break;

                case VideoSourceKind.Screen:
                    foreach (var Screen in ScreenVSLI.Enumerate())
                        AvailableVideoSources.Add(Screen);
                    break;

                case VideoSourceKind.WebCam:
                    foreach (var Cam in WebCamVSLI.Enumerate())
                        AvailableVideoSources.Add(Cam);
                    break;
            }

            if (SelectedVideoSourceKind != VideoSourceKind.NoVideo)
                VideoSourceBox.SelectedIndex = 0;
        }

        public void Refresh(object sender = null, EventArgs e = null)
        {
            AvailableCodecs.Clear();

            RefreshVideoSources();

            VideoSourceKindBox.SelectedIndex = 1;
            VideoSourceBox.SelectedIndex = 0;

            if (ReadyToRecord)
            {
                // Available Codecs
                AvailableCodecs.Clear();
                AvailableCodecs.Add(new KeyValuePair<string, string>(GifEncoderKey, "[Gif]"));

                foreach (var Codec in SharpAviEncoder.EnumerateEncoders())
                    AvailableCodecs.Add(Codec);

                Encoder = SharpAviEncoder.MotionJpegKey;
                EncodersBox.SelectedIndex = 2;

                // Available Audio Sources
                AvailableAudioSources.Clear();

                AvailableAudioSources.Add(new KeyValuePair<string, string>("-1", "[No Sound]"));

                foreach (var Dev in NAudioFacade.EnumerateAudioDevices())
                    AvailableAudioSources.Add(Dev);

                SelectedAudioSourceId = "-1";
                AudioSourcesBox.SelectedIndex = 0;

                // Status
                Status.Content = string.Format("{0} Encoder(s) and {1} AudioDevice(s) found", AvailableCodecs.Count, AvailableAudioSources.Count - 1);
            }
        }
        #endregion

        #region Recorder Control
        void ToggleRecorderState<T>(object sender = null, T e = default(T))
        {
            if (ReadyToRecord) StartRecording();
            else StopRecording();
        }

        void StartRecording()
        {
            if (CaptureDuration.Value != 0 && (StartDelay.Value * 1000 > CaptureDuration.Value))
            {
                Status.Content = "Delay cannot be greater than Duration";
                SystemSounds.Asterisk.Play();
                return;
            }

            if (VideoSettings.MinimizeOnStart) MainWindow.Instance.WindowState = WindowState.Minimized;

            VideoSourceKindBox.IsEnabled = false;
            VideoSourceBox.IsEnabled = SelectedVideoSourceKind == VideoSourceKind.Window;

            // UI Buttons
            RecordButton.ToolTip = "Stop";
            RecordButton.IconData = (RectangleGeometry)FindResource("StopIcon");

            ReadyToRecord = false;

            int temp;

            string Extension = SelectedVideoSourceKind == VideoSourceKind.NoVideo
                ? (AudioSettings.EncodeAudio && int.TryParse(SelectedAudioSourceId, out temp) ? ".mp3" : ".wav")
                : (Encoder == GifEncoderKey ? ".gif" : ".avi");

            lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + Extension);

            Delay = StartDelay.Value;

            Status.Content = Delay > 0 ? "Waiting..." : "Recording...";

            DTimer.Stop();
            Seconds = Minutes = 0;
            TimeManager.Content = "00:00";

            Duration = CaptureDuration.Value;

            DTimer.Start();

            int AudioBitRate = App.IsLamePresent ? SharpAviEncoder.AudioQualityToBitRate(AudioSettings.AudioQuality) : 0;

            NAudioFacade AudioSource = null;

            if (SelectedAudioSourceId != "-1")
                AudioSource = new NAudioFacade(SelectedAudioSourceId,
                    AudioSettings.Stereo,
                    AudioSettings.EncodeAudio,
                    AudioBitRate,
                    VideoSettings.FrameRate);

            #region ImageProvider
            IImageProvider ImgProvider = null;

            Func<bool> IncludeCursorLambda = () => (bool)Dispatcher.Invoke(new Func<bool>(() => IncludeCursor));

            Func<System.Windows.Media.Color, System.Drawing.Color> ConvertColor = (C) => System.Drawing.Color.FromArgb(C.A, C.R, C.G, C.B);

            if (SelectedVideoSourceKind == VideoSourceKind.Window)
            {
                var Src = SelectedVideoSource as WindowVSLI;

                if (Src.Handle == RegionSelector.Handle)
                {
                    ImgProvider = new StaticRegionProvider(RegionSelector,
                        IncludeCursorLambda,
                        VideoSettings.CaptureClicks,
                        VideoSettings.CaptureKeystrokes);
                    VideoSourceBox.IsEnabled = false;
                }
                else ImgProvider = new WindowProvider(() => (IntPtr)Dispatcher.Invoke(new Func<IntPtr>(() => (SelectedVideoSource as WindowVSLI).Handle)),
                    IncludeCursorLambda,
                    VideoSettings.CaptureClicks,
                    VideoSettings.CaptureKeystrokes,
                    ConvertColor(VideoSettings.BackgroundColor));
            }
            else if (SelectedVideoSourceKind == VideoSourceKind.Screen)
                ImgProvider = new ScreenProvider((SelectedVideoSource as ScreenVSLI).Screen,
                                                    IncludeCursorLambda,
                                                    VideoSettings.CaptureClicks,
                                                    VideoSettings.CaptureKeystrokes,
                                                    ConvertColor(VideoSettings.BackgroundColor));

            else if (SelectedVideoSourceKind == VideoSourceKind.WebCam)
                ImgProvider = (SelectedVideoSource as WebCamVSLI).ToWebCamProvider();
            #endregion

            #region VideoEncoder
            IEncoder VideoEncoder = null;

            if (Encoder == GifEncoderKey)
            {
                if (GifSettings.UnconstrainedGif)
                    Recorder = new UnconstrainedFrameRateGifRecorder(
                        new GifWriter(lastFileName,
                        Repeat: GifSettings.GifRepeat ? GifSettings.GifRepeatCount : -1),
                        ImgProvider);

                else VideoEncoder = new GifWriter(lastFileName, 1000 / VideoSettings.FrameRate,
                    GifSettings.GifRepeat ? GifSettings.GifRepeatCount : -1);

                if (AudioSource != null) AudioSource.CreateWaveWriter(
                    Path.ChangeExtension(lastFileName,
                    AudioSource.EncodeMp3 && !AudioSource.IsLoopback ? ".mp3" : ".wav"));
            }

            else if (SelectedVideoSourceKind != VideoSourceKind.NoVideo)
                VideoEncoder = new SharpAviEncoder(lastFileName, Encoder,
                    VideoSettings.VideoQuality, VideoSettings.FrameRate,
                    AudioSource, ImgProvider);

            else AudioSource.CreateWaveWriter(lastFileName);
            #endregion

            if (Recorder == null) Recorder = new Recorder(VideoEncoder, ImgProvider, AudioSource);

            Recorder.Error += (E) => Dispatcher.Invoke(new Action(() =>
                {
                    OnStopped();
                    Status.Content = "Error - " + E.Message;
                }));

            Recorder.Start(Delay);

            Recent.Add(lastFileName, VideoEncoder == null ? RecentItemType.Image : RecentItemType.Video);
        }

        void OnStopped()
        {
            Recorder = null;

            VideoSourceKindBox.IsEnabled = VideoSourceBox.IsEnabled = true;

            ReadyToRecord = true;

            MainWindow.Instance.WindowState = WindowState.Normal;

            Status.Content = "Saved to " + lastFileName;

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
    }
}
