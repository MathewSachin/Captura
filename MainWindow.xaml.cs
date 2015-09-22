using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
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
            DependencyProperty.Register("SelectedAudioSourceIndex", typeof(int), typeof(MainWindow));

        public int SelectedAudioSourceId
        {
            get { return (int)GetValue(SelectedAudioSourceIdProperty); }
            set { SetValue(SelectedAudioSourceIdProperty, value); }
        }

        //public static readonly DependencyProperty SelectedWindowProperty =
        //    DependencyProperty.Register("SelectedWindow", typeof(WindowHandler), typeof(MainWindow), new UIPropertyMetadata(WindowHandler.DesktopWindow));

        //public WindowHandler SelectedWindow
        //{
        //    get { return (WindowHandler)GetValue(SelectedWindowProperty); }
        //    set { SetValue(SelectedWindowProperty, value); }
        //}

        //public IEnumerable<WindowHandler> AvailableWindows { get; private set; }

        public IEnumerable<CodecInfo> AvailableCodecs { get; private set; }

        public IEnumerable<KeyValuePair<int, string>> AvailableAudioSources { get; private set; }
        #endregion

        public static readonly RoutedUICommand PauseCommand = new RoutedUICommand("Pause", "Pause", typeof(MainWindow)),
            ResumeCommand = new RoutedUICommand("Pause", "Pause", typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

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

            OutPath.Text = Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().Location).LocalPath);
                        
            AudioQuality.Maximum = Mp3AudioEncoderLame.SupportedBitRates.Length - 1;
            AudioQuality.Value = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;
            AudioQuality.Value = (AudioQuality.Maximum + 1) / 2;
        }

        void Refresh()
        {
            Status.Content = string.Format("{0} Encoder(s) and {1} AudioDevice(s) found", InitAvailableCodecs(), InitAvailableAudioSources());

            //var list = new List<WindowHandler>();
            //list.Add(WindowHandler.DesktopWindow);

            //foreach (var win in WindowHandler.Enumerate())
            //{
            //    var hWnd = win.Handle;
            //    if (!win.IsVisible) continue;
            //    if (!(User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_EXSTYLE).HasFlag(WindowStyles.WS_EX_APPWINDOW)))
            //    {
            //        if (User32.GetWindow(hWnd, GetWindowEnum.Owner) != IntPtr.Zero)
            //            continue;
            //        if (User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_EXSTYLE).HasFlag(WindowStyles.WS_EX_TOOLWINDOW))
            //            continue;
            //        if (User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_STYLE).HasFlag(WindowStyles.WS_CHILD))
            //            continue;
            //    }

            //    list.Add(win);
            //}

            //AvailableWindows = list;

            //SelectedWindow = WindowHandler.DesktopWindow;
        }

        void ToggleRecorderState<T>(object sender = null, T e = default(T))
        {
            if (ReadyToRecord) StartRecording();
            else StopRecording();
        }

        void StartRecording()
        {
            if (MinOnStart.IsChecked.Value) WindowState = WindowState.Minimized;

            IsCollapsed = true;
                        
            // UI Buttons
            RecordThumb.Description = "Stop";
            RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Stop.png"));
            RecordButton.ToolTip = "Stop";
            RecordButton.Content = "pack://application:,,,/Captura;Component/Images/Stop.png";

            ReadyToRecord = false;

            lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".avi");

            Status.Content = "Recording...";

            TimeManager.Reset();
            TimeManager.Start();

            Recorder = new Recorder(new RecorderParams(lastFileName, FrameRate, Encoder, Quality, SelectedAudioSourceId, UseStereo.IsChecked.Value, EncodeAudio, AudioQuality, IncludeCursor));
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
            var dlg = new System.Windows.Forms.FolderBrowserDialog()
            {
                SelectedPath = OutPath.Text,
                ShowNewFolderButton = true,
                Description = "Select Output Folder"
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) OutPath.Text = dlg.SelectedPath;
        }

        int InitAvailableCodecs()
        {
            var codecs = new List<CodecInfo>();
            codecs.Add(new CodecInfo(KnownFourCCs.Codecs.Uncompressed, "(none)"));
            codecs.Add(new CodecInfo(KnownFourCCs.Codecs.MotionJpeg, "Motion JPEG"));
            codecs.AddRange(Mpeg4VideoEncoderVcm.GetAvailableCodecs());
            AvailableCodecs = codecs;
            return codecs.Count - 1;
        }

        int InitAvailableAudioSources()
        {
            var deviceList = new Dictionary<int, string>();
            deviceList.Add(-1, "(No Sound)");

            for (var i = 0; i < WaveInEvent.DeviceCount; i++)
                deviceList.Add(i, WaveInEvent.GetCapabilities(i).ProductName);

            //foreach (var device in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            //    deviceList.Add(device.ID, device.FriendlyName + " (Loopback)");

            AvailableAudioSources = deviceList;
            SelectedAudioSourceId = -1;

            return deviceList.Count - 1;
        }
    }
}