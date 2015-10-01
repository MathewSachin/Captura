using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ManagedWin32;
using ManagedWin32.Api;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SharpAvi;
using SharpAvi.Codecs;

namespace Captura
{
    public partial class MainWindow : Fluent.RibbonWindow
    {
        #region Fields
        SaveFileDialog SFD;
        KeyboardHook KeyHook;

        Recorder Recorder;
        string lastFileName;
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty ReadyToRecordProperty =
            DependencyProperty.Register("ReadyToRecord", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(true));

        public bool ReadyToRecord
        {
            get { return (bool)GetValue(ReadyToRecordProperty); }
            set { SetValue(ReadyToRecordProperty, value); }
        }

        public static readonly DependencyProperty EncoderProperty =
            DependencyProperty.Register("Encoder", typeof(FourCC), typeof(MainWindow), new UIPropertyMetadata(KnownFourCCs.Codecs.MotionJpeg));

        public FourCC Encoder
        {
            get { return (FourCC)GetValue(EncoderProperty); }
            set { SetValue(EncoderProperty, value); }
        }

        public static readonly DependencyProperty SelectedAudioSourceIdProperty =
            DependencyProperty.Register("SelectedAudioSourceIndex", typeof(string), typeof(MainWindow), new UIPropertyMetadata("-1"));

        public string SelectedAudioSourceId
        {
            get { return (string)GetValue(SelectedAudioSourceIdProperty); }
            set { SetValue(SelectedAudioSourceIdProperty, value); }
        }

        public static readonly DependencyProperty SelectedWindowProperty =
            DependencyProperty.Register("SelectedWindow", typeof(IntPtr), typeof(MainWindow), new UIPropertyMetadata(RecorderParams.Desktop));

        public IntPtr SelectedWindow
        {
            get { return (IntPtr)GetValue(SelectedWindowProperty); }
            set { SetValue(SelectedWindowProperty, value); }
        }
        #endregion

        #region Observable Collections
        public ObservableCollection<KeyValuePair<IntPtr, string>> AvailableWindows { get; private set; }

        public ObservableCollection<CodecInfo> AvailableCodecs { get; private set; }

        public ObservableCollection<KeyValuePair<string, string>> AvailableAudioSources { get; private set; }
        #endregion

        #region RoutedUICommands
        public static readonly RoutedUICommand PauseCommand = new RoutedUICommand("Pause", "Pause", typeof(MainWindow)),
            ResumeCommand = new RoutedUICommand("Pause", "Pause", typeof(MainWindow));
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            AvailableCodecs = new ObservableCollection<CodecInfo>();
            AvailableAudioSources = new ObservableCollection<KeyValuePair<string, string>>();
            AvailableWindows = new ObservableCollection<KeyValuePair<IntPtr, string>>();

            #region Command Bindings
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => Close(), (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, (s, e) => StartRecording(),
                (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, (s, e) => StopRecording(),
                (s, e) => e.CanExecute = !ReadyToRecord));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (s, e) => OutputFolderBrowse(),
                (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(NavigationCommands.PreviousPage,
                (s, e) => Process.Start("explorer.exe", string.Format("/select, \"{0}\"", lastFileName)),
                (s, e) => e.CanExecute = !string.IsNullOrWhiteSpace(lastFileName) && File.Exists(lastFileName)));

            CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh, (s, e) => Refresh(),
                (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(PauseCommand, (s, e) =>
                {
                    Recorder.Pause();
                    TimeManager.Stop();

                    PauseButton.Command = ResumeCommand;
                    RotationEffect.Angle = 90;
                    Status.Content = "Paused";
                    PauseButton.ToolTip = "Pause";
                }, (s, e) => e.CanExecute = !ReadyToRecord && !Recorder.IsPaused));

            CommandBindings.Add(new CommandBinding(ResumeCommand, (s, e) =>
                {
                    Recorder.Resume();
                    TimeManager.Start();

                    PauseButton.Command = PauseCommand;
                    RotationEffect.Angle = 0;
                    Status.Content = "Recording...";
                    PauseButton.ToolTip = "Resume";
                }, (s, e) => e.CanExecute = !ReadyToRecord && Recorder.IsPaused));
            #endregion

            SFD = new SaveFileDialog()
            {
                AddExtension = true,
                Title = "Output",
                ValidateNames = true,
                DefaultExt = ".avi",
                Filter = "Avi Video|*.avi"
            };

            NavigationCommands.Refresh.Execute(this, this);

            KeyHook = new KeyboardHook(this, VirtualKeyCodes.R, ModifierKeyCodes.Control | ModifierKeyCodes.Shift);
            KeyHook.Triggered += () => Dispatcher.Invoke(new Action(() => ToggleRecorderState<int>()));

            if (string.IsNullOrWhiteSpace(OutPath.Text)) OutPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura\\");
            if (!Directory.Exists(OutPath.Text)) Directory.CreateDirectory(OutPath.Text);

            AudioQuality.Maximum = Mp3AudioEncoderLame.SupportedBitRates.Length - 1;
            AudioQuality.Value = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;
            AudioQuality.Value = (AudioQuality.Maximum + 1) / 2;
        }

        void Refresh()
        {
            // Available Codecs
            AvailableCodecs.Clear();
            AvailableCodecs.Add(new CodecInfo(KnownFourCCs.Codecs.Uncompressed, "(None)"));
            AvailableCodecs.Add(new CodecInfo(KnownFourCCs.Codecs.MotionJpeg, "Motion JPEG"));
            foreach (var Codec in Mpeg4VideoEncoderVcm.GetAvailableCodecs()) AvailableCodecs.Add(Codec);

            // Available Audio Sources
            AvailableAudioSources.Clear();

            AvailableAudioSources.Add(new KeyValuePair<string, string>("-1", "(No Sound)"));

            for (var i = 0; i < WaveInEvent.DeviceCount; i++)
                AvailableAudioSources.Add(new KeyValuePair<string, string>(i.ToString(), WaveInEvent.GetCapabilities(i).ProductName));

            foreach (var device in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                AvailableAudioSources.Add(new KeyValuePair<string, string>(device.ID, device.FriendlyName + " (Loopback)"));

            // Status
            Status.Content = string.Format("{0} Encoder(s) and {1} AudioDevice(s) found", AvailableCodecs.Count - 1, AvailableAudioSources.Count - 1);

            // Available Windows
            AvailableWindows.Clear();
            AvailableWindows.Add(new KeyValuePair<IntPtr, string>((IntPtr)(-1), "None"));
            AvailableWindows.Add(new KeyValuePair<IntPtr, string>(RecorderParams.Desktop, "Desktop"));

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
        }

        void ToggleRecorderState<T>(object sender = null, T e = default(T))
        {
            if (ReadyToRecord) StartRecording();
            else StopRecording();
        }

        void StartRecording()
        {
            if (WindowsGallery.SelectedIndex == 0 && DevicesGallery.SelectedIndex == 0)
            {
                Status.Content = "Nothing to Record! Selected a Window(probably Desktop), Audio Device or both";
                return;
            }

            if (MinOnStart.IsChecked.Value) WindowState = WindowState.Minimized;

            IsCollapsed = true;

            // UI Buttons
            RecordThumb.Description = "Stop";
            RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Stop.png"));
            RecordButton.ToolTip = "Stop";
            RecordButton.Content = "pack://application:,,,/Captura;Component/Images/Stop.png";

            ReadyToRecord = false;

            lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ((WindowsGallery.SelectedIndex == 0) ? ".wav" : ".avi"));

            Status.Content = "Recording...";

            TimeManager.Reset();
            TimeManager.Start();
            
            Recorder = new Recorder(new RecorderParams(lastFileName, (int)FrameRate.Value, Encoder,
                (int)Quality.Value, SelectedAudioSourceId, UseStereo.IsChecked.Value, EncodeAudio.IsChecked.Value,
                (int)AudioQuality.Value, IncludeCursor.IsChecked.Value, SelectedWindow));
        }

        void StopRecording()
        {
            if (ReadyToRecord) throw new InvalidOperationException("Not recording.");

            Recorder.Dispose();
            Recorder = null;

            ReadyToRecord = true;

            WindowState = WindowState.Normal;

            Status.Content = "Saved to " + lastFileName;

            TimeManager.Stop();

            // UI Buttons
            RecordThumb.Description = "Record";
            RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Record.png"));
            PauseButton.Command = PauseCommand;
            RecordButton.Content = "pack://application:,,,/Captura;Component/Images/Record.png";
            RotationEffect.Angle = 0;
            RecordButton.ToolTip = "Record";
            PauseButton.ToolTip = "Pause";
        }

        void Window_Closing(object sender, EventArgs e)
        {
            KeyHook.Dispose();

            if (!ReadyToRecord) StopRecording();
        }

        void OutputFolderBrowse()
        {
            var dlg = new FolderBrowserDialog()
            {
                SelectedPath = OutPath.Text,
                Title = "Select Output Folder"
                //ShowNewFolderButton = true,
                //Description = "Select Output Folder"
            };

            if (dlg.ShowDialog().Value) OutPath.Text = dlg.SelectedPath;
        }

        void ScreenShot(object sender, RoutedEventArgs e)
        {
            Bitmap bmp;

            if (SelectedWindow == IntPtr.Zero)
            {
                bmp = IncludeCursor.IsChecked.Value ? ScreenCapture.CaptureDesktopWithCursor() :
                   ScreenCapture.CaptureDesktop();
            }
            else bmp = ScreenCapture.Capture(SelectedWindow);

            lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png");

            Status.Content = "Saved to " + lastFileName;

            bmp.Save(lastFileName, ImageFormat.Png);
        }

        #region Gallery Selection Changed Handlers
        void EncodersGallery_SelectionChanged(object sender, SelectionChangedEventArgs e) { if (EncodersGallery.SelectedIndex == -1) EncodersGallery.SelectedIndex = 1; }

        void WindowsGallery_SelectionChanged(object sender, SelectionChangedEventArgs e) { if (WindowsGallery.SelectedIndex == -1) WindowsGallery.SelectedIndex = 1; }

        void DevicesGallery_SelectionChanged(object sender, SelectionChangedEventArgs e) { if (DevicesGallery.SelectedIndex == -1) DevicesGallery.SelectedIndex = 0; }
        #endregion
    }
}