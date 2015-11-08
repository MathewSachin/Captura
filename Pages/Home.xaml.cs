using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ManagedWin32;
using ManagedWin32.Api;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SharpAvi;
using SharpAvi.Codecs;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace Captura
{
    partial class Home : UserControl
    {
        #region Fields
        DispatcherTimer DTimer;
        int Seconds = 0, Minutes = 0, Duration = 0;

        KeyboardHookList KeyHook;

        Recorder Recorder;
        string lastFileName;
        NotifyIcon SystemTray;
        #endregion

        public Home()
        {
            InitializeComponent();

            MainWindow.Instance.RecordThumb.Click += ToggleRecorderState;
            MainWindow.Instance.ScreenShotThumb.Click += ScreenShot;

            DataContext = this;

            AvailableCodecs = new ObservableCollection<CodecInfo>();
            AvailableAudioSources = new ObservableCollection<KeyValuePair<string, string>>();
            AvailableWindows = new ObservableCollection<KeyValuePair<IntPtr, string>>();

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

                if (Duration > 0 && (Minutes * 60 + Seconds >= Duration)) StopRecording();

                TimeManager.Content = string.Format("{0:D2}:{1:D2}", Minutes, Seconds);
            };
            #endregion

            #region Command Bindings
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (s, e) => OutputFolderBrowse()));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => MainWindow.Instance.Close(), (s, e) => e.CanExecute = ReadyToRecord));

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

            NavigationCommands.Refresh.Execute(this, this);

            RegionSelector.Closing += (s, e) =>
                {
                    if (!WindowClosing)
                    {
                        RegSelBox.IsChecked = false;
                        e.Cancel = true;
                    }
                };

            #region SystemTray
            SystemTray = new NotifyIcon()
            {
                Visibility = Visibility.Collapsed,
                IconSource = MainWindow.Instance.Icon
            };

            SystemTray.TrayLeftMouseUp += (s, e) =>
                {
                    SystemTray.Visibility = Visibility.Collapsed;
                    MainWindow.Instance.Show();
                    MainWindow.Instance.WindowState = WindowState.Normal;
                };

            MainWindow.Instance.StateChanged += (s, e) =>
                {
                    if (MainWindow.Instance.WindowState == WindowState.Minimized && Min2SysTray.IsChecked.Value)
                    {
                        MainWindow.Instance.Hide();
                        SystemTray.Visibility = Visibility.Visible;
                    }
                };
            #endregion

            #region KeyHook
            KeyHook = new KeyboardHookList(MainWindow.Instance);

            KeyHook.Register(KeyCode.VK_R, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
                () => Dispatcher.Invoke(new Action(() => ToggleRecorderState<int>())));

            KeyHook.Register(KeyCode.VK_S, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
                () => Dispatcher.Invoke(new Action(() => ScreenShot<int>())));
            #endregion

            if (string.IsNullOrWhiteSpace(OutPath.Text)) OutPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura\\");
            if (!Directory.Exists(OutPath.Text)) Directory.CreateDirectory(OutPath.Text);

            AudioQuality.Maximum = Mp3AudioEncoderLame.SupportedBitRates.Length - 1;
            AudioQuality.Value = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;
            AudioQuality.Value = (AudioQuality.Maximum + 1) / 2;
        }

        //~Home()
        //{   
        //    if (KeyHook != null) KeyHook.Dispose();

        //    WindowClosing = true;
        //    RegionSelector.Close();

        //    if (!ReadyToRecord) StopRecording();
        //}

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(Home), new UIPropertyMetadata(Colors.Transparent));

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        #region RoutedUICommands
        public static readonly RoutedUICommand PauseCommand = new RoutedUICommand("Pause", "Pause", typeof(Home)),
            ResumeCommand = new RoutedUICommand("Pause", "Pause", typeof(Home));
        #endregion

        #region RegionSelector
        RegionSelector RegionSelector = RegionSelector.Instance;
        bool WindowClosing = false;

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

        public static readonly DependencyProperty ReadyToRecordProperty =
                    DependencyProperty.Register("ReadyToRecord", typeof(bool), typeof(Home), new UIPropertyMetadata(true));

        public bool ReadyToRecord
        {
            get { return (bool)GetValue(ReadyToRecordProperty); }
            set { SetValue(ReadyToRecordProperty, value); }
        }

        public ObservableCollection<CodecInfo> AvailableCodecs { get; private set; }

        public ObservableCollection<KeyValuePair<string, string>> AvailableAudioSources { get; private set; }

        public static readonly DependencyProperty EncoderProperty =
                    DependencyProperty.Register("Encoder", typeof(FourCC), typeof(Home), new UIPropertyMetadata(KnownFourCCs.Codecs.MotionJpeg));

        public FourCC Encoder
        {
            get { return (FourCC)GetValue(EncoderProperty); }
            set { SetValue(EncoderProperty, value); }
        }

        public static readonly DependencyProperty SelectedAudioSourceIdProperty =
            DependencyProperty.Register("SelectedAudioSourceIndex", typeof(string), typeof(Home), new UIPropertyMetadata("-1"));

        public string SelectedAudioSourceId
        {
            get { return (string)GetValue(SelectedAudioSourceIdProperty); }
            set { SetValue(SelectedAudioSourceIdProperty, value); }
        }

        void OpenOutputFolder<T>(object sender, T e) { Process.Start("explorer.exe", OutPath.Text); }

        void OutputFolderBrowse()
        {
            var dlg = new FolderBrowserDialog()
            {
                SelectedPath = OutPath.Text,
                Title = "Select Output Folder"
            };

            if (dlg.ShowDialog().Value) OutPath.Text = dlg.SelectedPath;
        }

        void ScreenShot<T>(object sender = null, T e = default(T))
        {
            string FileName = null;
            ImageFormat ImgFmt = ScreenShotSettings.SelectedImageFormat;
            string Extension = ImgFmt == ImageFormat.Icon ? "ico"
                : ImgFmt == ImageFormat.Jpeg ? "jpg"
                : ImgFmt.ToString();
            bool SaveToClipboard = ScreenShotSettings.SaveToClipboard;

            if (!SaveToClipboard)
                FileName = Path.Combine(Properties.Settings.Default.OutputPath,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + Extension);

            if (SelectedWindow == Recorder.DesktopHandle
                || SelectedWindow == RegionSelector.Instance.Handle
                || !ScreenShotSettings.UseDWM)
            {
                RECT Rect = Recorder.DesktopRectangle;

                if (SelectedWindow != Recorder.DesktopHandle)
                    User32.GetWindowRect(SelectedWindow, ref Rect);

                var BMP = new Bitmap(Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);

                using (var g = Graphics.FromImage(BMP))
                {
                    g.CopyFromScreen(Rect.Left, Rect.Top, 0, 0,
                        new System.Drawing.Size(Rect.Right - Rect.Left, Rect.Bottom - Rect.Top),
                        CopyPixelOperation.SourceCopy);

                    g.Flush();
                }

                if (SaveToClipboard)
                {
                    BMP.WriteToClipboard(ImgFmt == ImageFormat.Png);
                    Status.Content = "Saved to Clipboard";
                }
                else
                {
                    try { BMP.Save(FileName, ImgFmt); }
                    catch (Exception E)
                    {
                        Status.Content = "Not Saved. " + E.Message;
                        return;
                    }

                    Status.Content = "Saved to " + FileName;
                }
            }
            else new Screenshot().CaptureWindow(SelectedWindow, SaveToClipboard, IncludeCursor, FileName, ImgFmt);

            if (FileName != null && !SaveToClipboard) Recent.Add(FileName);
        }

        public static bool IncludeCursor { get { return Properties.Settings.Default.IncludeCursor; } }

        public ObservableCollection<KeyValuePair<IntPtr, string>> AvailableWindows { get; private set; }

        public static readonly DependencyProperty SelectedWindowProperty =
            DependencyProperty.Register("SelectedWindow", typeof(IntPtr), typeof(Home), new UIPropertyMetadata(Recorder.DesktopHandle));

        public IntPtr SelectedWindow
        {
            get { return (IntPtr)GetValue(SelectedWindowProperty); }
            set { SetValue(SelectedWindowProperty, value); }
        }

        public void Refresh(object sender = null, EventArgs e = null)
        {
            if (ReadyToRecord)
            {
                // Available Codecs
                AvailableCodecs.Clear();
                AvailableCodecs.Add(new CodecInfo(KnownFourCCs.Codecs.Uncompressed, "[Uncompressed]"));
                AvailableCodecs.Add(new CodecInfo(Recorder.GifFourCC, "[Gif]"));
                AvailableCodecs.Add(new CodecInfo(KnownFourCCs.Codecs.MotionJpeg, "Motion JPEG"));
                foreach (var Codec in Mpeg4VideoEncoderVcm.GetAvailableCodecs()) AvailableCodecs.Add(Codec);

                Encoder = KnownFourCCs.Codecs.MotionJpeg;
                EncodersBox.SelectedIndex = 2;

                // Available Audio Sources
                AvailableAudioSources.Clear();

                AvailableAudioSources.Add(new KeyValuePair<string, string>("-1", "[No Sound]"));

                for (var i = 0; i < WaveInEvent.DeviceCount; i++)
                    AvailableAudioSources.Add(new KeyValuePair<string, string>(i.ToString(), WaveInEvent.GetCapabilities(i).ProductName));

                foreach (var device in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                    AvailableAudioSources.Add(new KeyValuePair<string, string>(device.ID, device.FriendlyName + " (Loopback)"));

                SelectedAudioSourceId = "-1";
                AudioSourcesBox.SelectedIndex = 0;

                // Status
                Status.Content = string.Format("{0} Encoder(s) and {1} AudioDevice(s) found", AvailableCodecs.Count - 1, AvailableAudioSources.Count - 1);
            }

            AvailableWindows.Clear();
            AvailableWindows.Add(new KeyValuePair<IntPtr, string>((IntPtr)(-1), "[No Video]"));
            AvailableWindows.Add(new KeyValuePair<IntPtr, string>(Recorder.DesktopHandle, "[Desktop]"));
            AvailableWindows.Add(new KeyValuePair<IntPtr, string>(User32.FindWindow("Shell_TrayWnd", null), "[Taskbar]"));

            foreach (var win in WindowHandler.Enumerate())
            {
                var hWnd = win.Handle;
                if (!win.IsVisible) continue;
                if (!(User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_EXSTYLE).HasFlag(WindowStyles.WS_EX_APPWINDOW)))
                {
                    if (User32.GetWindow(hWnd, GetWindowEnum.Owner) != IntPtr.Zero)
                        continue;
                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_EXSTYLE).HasFlag(WindowStyles.WS_EX_TOOLWINDOW))
                        continue;
                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_STYLE).HasFlag(WindowStyles.WS_CHILD))
                        continue;
                }

                AvailableWindows.Add(new KeyValuePair<IntPtr, string>(hWnd, win.Title));
            }

            SelectedWindow = Recorder.DesktopHandle;

            WindowBox.SelectedIndex = 1;
        }

        void ToggleRecorderState<T>(object sender = null, T e = default(T))
        {
            if (ReadyToRecord) StartRecording();
            else StopRecording();
        }

        void StartRecording()
        {
            if (WindowBox.SelectedIndex == 0 && AudioSourcesBox.SelectedIndex == 0)
            {
                Status.Content = "Nothing to Record! Selected a Window(probably Desktop), Audio Device or both";
                return;
            }

            if (MinOnStart.IsChecked.Value) MainWindow.Instance.WindowState = WindowState.Minimized;

            WindowBox.IsEnabled = WindowBox.SelectedIndex != 0;
            
            // UI Buttons
            MainWindow.Instance.RecordThumb.Description = "Stop";
            MainWindow.Instance.RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Stop.png"));
            RecordButton.ToolTip = "Stop";
            RecordButton.Content = "pack://application:,,,/Captura;Component/Images/Stop.png";

            ReadyToRecord = false;

            string Extension = (WindowBox.SelectedIndex == 0) ? ".wav"
                : (Encoder == Recorder.GifFourCC ? ".gif" : ".avi");

            lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + Extension);

            Status.Content = "Recording...";

            DTimer.Stop();
            Seconds = Minutes = 0;
            TimeManager.Content = "00:00";
            DTimer.Start();

            Duration = (int)CaptureDuration.Value;

            Recorder = new Recorder(lastFileName, (int)FrameRate.Value, Encoder, (int)Quality.Value,
                        SelectedAudioSourceId, AudioVideoSettings.Stereo, AudioVideoSettings.EncodeAudio,
                        Mp3AudioEncoderLame.SupportedBitRates.OrderBy(br => br).ElementAt((int)AudioQuality.Value),
                        AudioVideoSettings.CaptureClicks, AudioVideoSettings.CaptureKeystrokes, Commons.ConvertColor(BackgroundColor),
                        () => (bool)Dispatcher.Invoke(new Func<bool>(() => IncludeCursor)),
                        () => (IntPtr)Dispatcher.Invoke(new Func<IntPtr>(() => SelectedWindow)));

            Recorder.Error += (E) => Dispatcher.Invoke(new Action(() =>
                {
                    Status.Content = "Error - " + E.Message;
                    OnStopped();
                }));

            Recorder.Start((int)StartDelay.Value);

            Recent.Add(lastFileName);
        }

        void OnStopped()
        {
            Recorder = null;

            WindowBox.IsEnabled = true;

            ReadyToRecord = true;

            MainWindow.Instance.WindowState = WindowState.Normal;

            Status.Content = "Saved to " + lastFileName;

            DTimer.Stop();

            // UI Buttons
            MainWindow.Instance.RecordThumb.Description = "Record";
            MainWindow.Instance.RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Record.png"));
            PauseButton.Command = PauseCommand;
            RecordButton.Content = "pack://application:,,,/Captura;Component/Images/Record.png";
            RotationEffect.Angle = 0;
            RecordButton.ToolTip = "Record";
            PauseButton.ToolTip = "Pause";
        }

        void StopRecording()
        {
            Recorder.Stop();
            OnStopped();
        }
    }
}
