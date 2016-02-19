using Screna;
using Screna.Avi;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public enum VideoSourceKind { NoVideo, Window, Screen }

    public partial class VideoSettings : UserControl, INotifyPropertyChanged
    {
        public static VideoSettings Instance { get; private set; }

        static VideoSettings()
        {
            AvailableVideoSourceKinds = new ObservableCollection<KeyValuePair<VideoSourceKind, string>>();

            AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.NoVideo, "No Video"));
            AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Window, "Window"));

            if (ScreenVSLI.Count > 1)
                AvailableVideoSourceKinds.Add(new KeyValuePair<VideoSourceKind, string>(VideoSourceKind.Screen, "Screen"));

            AvailableCodecs = new ObservableCollection<AviCodec>();
            AvailableVideoSources = new ObservableCollection<IVideoSourceListItem>();
        }

        public VideoSettings()
        {
            InitializeComponent();

            DataContext = this;

            Instance = this;

            _AvailableCodecs = AvailableCodecs;
            _AvailableVideoSourceKinds = AvailableVideoSourceKinds;
            _AvailableVideoSources = AvailableVideoSources;

            RefreshCodecs();

            RefreshVideoSources();
        }

        public static void RefreshVideoSources()
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
            }

            if (Instance != null && SelectedVideoSourceKind != VideoSourceKind.NoVideo)
                Instance.VideoSourceBox.SelectedIndex = 0;
        }

        public static void RefreshCodecs()
        {
            // Available Codecs
            AvailableCodecs.Clear();
            AvailableCodecs.Add(new AviCodec("Gif"));

            foreach (var Codec in AviWriter.EnumerateEncoders())
                AvailableCodecs.Add(Codec);

            if (Instance != null)
                Instance.EncodersBox.SelectedIndex = 2;
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

        public void CheckFunctionalityAvailability()
        {
            bool AudioAvailable = AudioSettings.SelectedAudioSourceId != "-1",
                VideoAvailable = SelectedVideoSourceKind == VideoSourceKind.Window
                    || SelectedVideoSourceKind == VideoSourceKind.Screen;

            MainWindow.Instance.RecordButton.IsEnabled = AudioAvailable || VideoAvailable;

            MainWindow.Instance.ScreenShotButton.IsEnabled = VideoAvailable;
        }

        public static ObservableCollection<AviCodec> AvailableCodecs { get; private set; }

        public ObservableCollection<AviCodec> _AvailableCodecs { get; private set; }

        public static ObservableCollection<KeyValuePair<VideoSourceKind, string>> AvailableVideoSourceKinds { get; private set; }

        public ObservableCollection<KeyValuePair<VideoSourceKind, string>> _AvailableVideoSourceKinds { get; private set; }

        public static ObservableCollection<IVideoSourceListItem> AvailableVideoSources { get; private set; }

        public ObservableCollection<IVideoSourceListItem> _AvailableVideoSources { get; private set; }

        public static VideoSourceKind SelectedVideoSourceKind = VideoSourceKind.Window;

        public VideoSourceKind _SelectedVideoSourceKind
        {
            get { return SelectedVideoSourceKind; }
            set
            {
                if (SelectedVideoSourceKind != value)
                {
                    SelectedVideoSourceKind = value;
                    OnPropertyChanged();
                }
            }
        }

        public static IVideoSourceListItem SelectedVideoSource = WindowVSLI.Desktop;

        public IVideoSourceListItem _SelectedVideoSource
        {
            get { return SelectedVideoSource; }
            set
            {
                if (SelectedVideoSource != value)
                {
                    SelectedVideoSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public static AviCodec Encoder = AviCodec.MotionJpeg;

        public AviCodec _Encoder
        {
            get { return Encoder; }
            set
            {
                if (Encoder != value)
                {
                    Encoder = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Video
        public static int VideoQuality = 70;

        public double _VideoQuality
        {
            get { return VideoQuality; }
            set
            {
                if (VideoQuality != (int)value)
                {
                    VideoQuality = (int)value;
                    OnPropertyChanged();
                }
            }
        }

        public static int FrameRate = 10;

        public int _FrameRate
        {
            get { return FrameRate; }
            set
            {
                if (FrameRate != value)
                {
                    FrameRate = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        // TODO: Try binding here
        public static Color BackgroundColor
        {
            get
            {
                return Instance == null
                    || Instance.BgPicker.SelectedColor == null
                    ? Colors.Transparent
                    : Instance.BgPicker.Dispatcher.Invoke<Color>(() => (Instance.BgPicker.SelectedColor as SolidColorBrush).Color);
            }
        }

        #region INotifyPropertyChanged
        void OnPropertyChanged([CallerMemberName] string e = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
