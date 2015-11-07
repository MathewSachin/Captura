//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Threading;
//using ManagedWin32;
//using ManagedWin32.Api;
//using NAudio.CoreAudioApi;
//using NAudio.Wave;
//using SharpAvi;
//using SharpAvi.Codecs;
//using Color = System.Windows.Media.Color;

namespace Captura
{
    public partial class MainWindow : FirstFloor.ModernUI.Windows.Controls.ModernWindow//, INotifyPropertyChanged
    {
        // //        Color themeColor;       
        // public Color ThemeColor
        //        {
        //            get { return this.themeColor; }
        //            set
        //            {
        //                this.themeColor = value;
        //                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ThemeColor"));
        //            }
        //        }

        //        #region RoutedUICommands
        //        public static readonly RoutedUICommand PauseCommand = new RoutedUICommand("Pause", "Pause", typeof(MainWindow)),
        //            ResumeCommand = new RoutedUICommand("Pause", "Pause", typeof(MainWindow));
        //        #endregion

        //        #region RegionSelector
        //        RegionSelector RegionSelector = new RegionSelector();
        //        bool WindowClosing = false;

        //        void ShowRegionSelector(object sender, RoutedEventArgs e)
        //        {
        //            RegionSelector.Show();
        //            Refresh();
        //        }

        //        void HideRegionSelector(object sender, RoutedEventArgs e)
        //        {
        //            RegionSelector.Hide();
        //            Refresh();
        //        }
        //        #endregion

        //        public MainWindow()
        //        {
        //            InitializeComponent();

        //            DataContext = this;

        //            ThemeColor = Colors.Transparent;

        //            #region Init Timer
        //            DTimer = new DispatcherTimer();
        //            DTimer.Interval = TimeSpan.FromSeconds(1);
        //            DTimer.Tick += (s, e) =>
        //            {
        //                Seconds++;

        //                if (Seconds == 60)
        //                {
        //                    Seconds = 0;
        //                    Minutes++;
        //                }

        //                if (Duration > 0 && (Minutes * 60 + Seconds >= Duration)) StopRecording();

        //                TimeManager.Content = string.Format("{0:D2}:{1:D2}", Minutes, Seconds);
        //            };
        //            #endregion
                
        //            #region Command Bindings
        //            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => Close(), (s, e) => e.CanExecute = ReadyToRecord));

        //            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, (s, e) => StartRecording(),
        //                (s, e) => e.CanExecute = ReadyToRecord));

        //            CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, (s, e) => StopRecording(),
        //                (s, e) => e.CanExecute = !ReadyToRecord));
        
        //            CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh, (s, e) => Refresh()));

        //            CommandBindings.Add(new CommandBinding(PauseCommand, (s, e) =>
        //                {
        //                    Recorder.Pause();
        //                    DTimer.Stop();

        //                    PauseButton.Command = ResumeCommand;
        //                    RotationEffect.Angle = 90;
        //                    Status.Content = "Paused";
        //                    PauseButton.ToolTip = "Pause";
        //                }, (s, e) => e.CanExecute = !ReadyToRecord && Recorder != null));

        //            CommandBindings.Add(new CommandBinding(ResumeCommand, (s, e) =>
        //                {
        //                    Recorder.Start();
        //                    DTimer.Start();

        //                    PauseButton.Command = PauseCommand;
        //                    RotationEffect.Angle = 0;
        //                    Status.Content = "Recording...";
        //                    PauseButton.ToolTip = "Resume";
        //                }, (s, e) => e.CanExecute = !ReadyToRecord && Recorder != null));
        //            #endregion

        //            NavigationCommands.Refresh.Execute(this, this);

        //            RegionSelector.Closing += (s, e) =>
        //                {
        //                    if (!WindowClosing)
        //                    {
        //                        RegSelBox.IsChecked = false;
        //                        e.Cancel = true;
        //                    }
        //                };

        //            #region SystemTray
        //            SystemTray = new NotifyIcon()
        //            {
        //                Visibility = Visibility.Collapsed,
        //                IconSource = Icon
        //            };

        //            SystemTray.TrayLeftMouseUp += (s, e) =>
        //                {
        //                    SystemTray.Visibility = Visibility.Collapsed;
        //                    Show();
        //                    WindowState = WindowState.Normal;
        //                };

        //            StateChanged += (s, e) =>
        //                {
        //                    if (WindowState == WindowState.Minimized && Min2SysTray.IsChecked.Value)
        //                    {
        //                        Hide();
        //                        SystemTray.Visibility = Visibility.Visible;
        //                    }
        //                };
        //            #endregion

        //            #region KeyHook
        //            KeyHook = new KeyboardHookList(this);

        //            KeyHook.Register(KeyCode.VK_R, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
        //                () => Dispatcher.Invoke(new Action(() => ToggleRecorderState<int>())));

        //            KeyHook.Register(KeyCode.VK_S, ModifierKeyCodes.Control | ModifierKeyCodes.Shift | ModifierKeyCodes.Alt,
        //                () => Dispatcher.Invoke(new Action(() => ScreenShot<int>())));
        //            #endregion

        //            if (string.IsNullOrWhiteSpace(OutPath.Text)) OutPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura\\");
        //            if (!Directory.Exists(OutPath.Text)) Directory.CreateDirectory(OutPath.Text);

        //            AudioQuality.Maximum = Mp3AudioEncoderLame.SupportedBitRates.Length - 1;
        //            AudioQuality.Value = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;
        //            AudioQuality.Value = (AudioQuality.Maximum + 1) / 2;
        //        }

        //        void Refresh(object sender = null, RoutedEventArgs e = null)
        //        {
        //                        
        //        }

        //        void ToggleRecorderState<T>(object sender = null, T e = default(T))
        //        {
        //            if (ReadyToRecord) StartRecording();
        //            else StopRecording();
        //        }

        //        void StartRecording()
        //        {
        //            if (WindowsGallery.SelectedIndex == 0 && DevicesGallery.SelectedIndex == 0)
        //            {
        //                Status.Content = "Nothing to Record! Selected a Window(probably Desktop), Audio Device or both";
        //                return;
        //            }

        //            if (MinOnStart.IsChecked.Value) WindowState = WindowState.Minimized;

        //            WindowsGallery.IsEnabled = WindowsGallery.SelectedIndex != 0;

        //            IsCollapsed = true;

        //            // UI Buttons
        //            RecordThumb.Description = "Stop";
        //            RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Stop.png"));
        //            RecordButton.ToolTip = "Stop";
        //            RecordButton.Content = "pack://application:,,,/Captura;Component/Images/Stop.png";

        //            ReadyToRecord = false;

        //            string Extension = (WindowsGallery.SelectedIndex == 0) ? ".wav"
        //                : (Encoder == Recorder.GifFourCC ? ".gif" : ".avi");

        //            lastFileName = Path.Combine(OutPath.Text, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + Extension);

        //            Status.Content = "Recording...";

        //            DTimer.Stop();
        //            Seconds = Minutes = 0;
        //            TimeManager.Content = "00:00";
        //            DTimer.Start();

        //            Duration = (int)CaptureDuration.Value;

        //            Recorder = new Recorder(lastFileName, (int)FrameRate.Value, Encoder, (int)Quality.Value,
        //                        SelectedAudioSourceId, UseStereo.IsChecked.Value, EncodeAudio.IsChecked.Value,
        //                        Mp3AudioEncoderLame.SupportedBitRates.OrderBy(br => br).ElementAt((int)AudioQuality.Value),
        //                        CaptureMouseClicks.IsChecked.Value, CaptureKeyStrokes.IsChecked.Value, Commons.ConvertColor(themeColor),
        //                        () => (bool)Dispatcher.Invoke(new Func<bool>(() => IncludeCursor.IsChecked.Value)),
        //                        () => (IntPtr)Dispatcher.Invoke(new Func<IntPtr>(() => SelectedWindow)));

        //            Recorder.Error += (E) => Dispatcher.Invoke(new Action(() =>
        //                {
        //                    Status.Content = "Error - " + E.Message;
        //                    OnStopped();
        //                }));

        //            Recorder.Start((int)StartDelay.Value);

        //            RecentPanel.Children.Add(new RecentButton(lastFileName));
        //        }

        //        void OnStopped()
        //        {
        //            Recorder = null;

        //            WindowsGallery.IsEnabled = true;

        //            ReadyToRecord = true;

        //            WindowState = WindowState.Normal;

        //            Status.Content = "Saved to " + lastFileName;

        //            DTimer.Stop();

        //            // UI Buttons
        //            RecordThumb.Description = "Record";
        //            RecordThumb.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Captura;Component/Images/Record.png"));
        //            PauseButton.Command = PauseCommand;
        //            RecordButton.Content = "pack://application:,,,/Captura;Component/Images/Record.png";
        //            RotationEffect.Angle = 0;
        //            RecordButton.ToolTip = "Record";
        //            PauseButton.ToolTip = "Pause";
        //        }

        //        void StopRecording()
        //        {
        //            Recorder.Stop();
        //            OnStopped();
        //        }

        //        #region Gallery Selection Changed Handlers
        //        void EncodersGallery_SelectionChanged(object sender, SelectionChangedEventArgs e) { if (EncodersGallery.SelectedIndex == -1) EncodersGallery.SelectedIndex = 1; }

        //        void WindowsGallery_SelectionChanged(object sender, SelectionChangedEventArgs e) { if (WindowsGallery.SelectedIndex == -1) WindowsGallery.SelectedIndex = 1; }

        //        void DevicesGallery_SelectionChanged(object sender, SelectionChangedEventArgs e) { if (DevicesGallery.SelectedIndex == -1) DevicesGallery.SelectedIndex = 0; }
        //        #endregion
       
        //        public event PropertyChangedEventHandler PropertyChanged;
    }
}