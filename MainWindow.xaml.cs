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
using ManagedWin32;
using ManagedWin32.Api;
using Microsoft.Win32;
using NWaveIn;
using SharpAvi;
using SharpAvi.Codecs;

namespace Captura
{
    public partial class MainWindow : Fluent.RibbonWindow
    {
        #region Fields
        SaveFileDialog SFD;
        KeyboardHook KeyHook;

        Recorder recorder;
        string lastFileName;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            
            #region Command Bindings
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => Close()));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, (s, e) => StartRecording(),
                (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, (s, e) => StopRecording(),
                (s, e) => e.CanExecute = !ReadyToRecord));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (s, e) => OutputFolderBrowse(),
                (s, e) => e.CanExecute = ReadyToRecord));

            CommandBindings.Add(new CommandBinding(NavigationCommands.PreviousPage,
                (s, e) => Process.Start("explorer.exe", string.Format("/select, \"{0}\"", lastFileName)),
                (s, e) => e.CanExecute = !string.IsNullOrWhiteSpace(lastFileName) && File.Exists(lastFileName)));

            CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh, (s, e) =>
                    Status.Content = string.Format("{0} Encoder(s) and {1} AudioDevice(s) found", InitAvailableCodecs(), InitAvailableAudioSources())
                    , (s, e) => e.CanExecute = ReadyToRecord));
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
            KeyHook.Triggered += () => Dispatcher.Invoke(new Action(() => RecordControl_Click<int>()));

            OutPath.Text = Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().Location).LocalPath);
            
            AudioWaveFormat = SupportedWaveFormat.WAVE_FORMAT_44M16;

            AudioQuality.Maximum = Mp3AudioEncoderLame.SupportedBitRates.Length - 1;
            AudioQuality.Value = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;
            AudioQuality.Value = (AudioQuality.Maximum + 1) / 2;
        }

        #region Control
        void RecordControl_Click<T>(object sender = null, T e = default(T))
        {
            if (ReadyToRecord) StartRecording();
            else StopRecording();
        }

        void StartRecording()
        {
            if (MinOnStart.IsChecked.Value) WindowState = WindowState.Minimized;

            IsCollapsed = true;

            NewButton.Opacity = 0.1;
            StopButton.Opacity = 0.7;
            
            RecordThumb.Description = "Stop";
            RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Stop.png"));

            ReadyToRecord = false;

            lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".avi");

            recorder = new Recorder(new RecorderParams(lastFileName, FrameRate, Encoder, Quality, SelectedAudioSourceId, AudioWaveFormat, EncodeAudio, AudioQuality, IncludeCursor));

            Status.Content = "Recording...";

            TimeManager.Reset();
            TimeManager.Start();
        }

        void StopRecording()
        {
            if (ReadyToRecord) throw new InvalidOperationException("Not recording.");

            StopButton.Opacity = 0.1;
            NewButton.Opacity = 0.7;

            RecordThumb.Description = "Record";
            RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Record.png"));

            recorder.Dispose();
            recorder = null;

            ReadyToRecord = true;

            WindowState = WindowState.Normal;

            Status.Content = "Ready";
            
            TimeManager.Stop();
        }
        #endregion

        void Window_Closing(object sender, EventArgs e)
        {
            KeyHook.Dispose();

            if (!ReadyToRecord) StopRecording();
        }

        #region Settings
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
            {
                var caps = WaveInEvent.GetCapabilities(i);
                if (audioFormats.All(caps.SupportsWaveFormat))
                    deviceList.Add(i, caps.ProductName);
            }

            //foreach (var device in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            //    deviceList.Add(device.ID, device.FriendlyName + " (Loopback)");

            AvailableAudioSources = deviceList;
            SelectedAudioSourceId = -1;

            return deviceList.Count - 1;
        }

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

        public SupportedWaveFormat AudioWaveFormat
        {
            // TODO: Make wave format more adjustable
            get { return UseStereo.IsChecked.Value ? audioFormats[1] : audioFormats[0]; }
            set { UseStereo.IsChecked = (value == audioFormats[1]); }
        }

        public IEnumerable<CodecInfo> AvailableCodecs { get; private set; }

        public IEnumerable<KeyValuePair<int, string>> AvailableAudioSources { get; private set; }

        SupportedWaveFormat[] audioFormats = new[] { SupportedWaveFormat.WAVE_FORMAT_44M16, SupportedWaveFormat.WAVE_FORMAT_44S16 };
        #endregion
    }
}