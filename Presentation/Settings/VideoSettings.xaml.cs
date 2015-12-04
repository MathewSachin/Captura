using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public partial class VideoSettings : UserControl, INotifyPropertyChanged
    {
        static VideoSettings Instance;

        public VideoSettings()
        {
            InitializeComponent();

            DataContext = this;

            Instance = this;
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
                    OnPropertyChanged("_VideoQuality");
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
                    OnPropertyChanged("_FrameRate");
                }
            }
        }
        #endregion
        
        #region MouseKeyHooks
        public static bool CaptureClicks = false;

        public bool _CaptureClicks
        {
            get { return CaptureClicks; }
            set
            {
                if (CaptureClicks != value)
                {
                    CaptureClicks = value;
                    OnPropertyChanged("_CaptureClicks");
                }
            }
        }

        public static bool CaptureKeystrokes = false;

        public bool _CaptureKeystrokes
        {
            get { return CaptureKeystrokes; }
            set
            {
                if (CaptureKeystrokes != value)
                {
                    CaptureKeystrokes = value;
                    OnPropertyChanged("_CaptureKeystrokes");
                }
            }
        }
        #endregion

        public static bool StaticRegionCapture = false;

        public bool _StaticRegionCapture
        {
            get { return StaticRegionCapture; }
            set
            {
                if (StaticRegionCapture != value)
                {
                    StaticRegionCapture = value;
                    OnPropertyChanged("_StaticRegionCapture");
                }
            }
        }

        public static Color BackgroundColor
        {
            get
            {
                return Instance == null 
                    || Instance.BgPicker.SelectedColor == null
                    ? Colors.Transparent
                    : (Color)Instance.BgPicker.Dispatcher.Invoke(
                    new Func<Color>(() => (Instance.BgPicker.SelectedColor as SolidColorBrush).Color));
            }
        }

        public static bool MinimizeToSysTray = false;

        public bool _MinToSysTray
        {
            get { return MinimizeToSysTray; }
            set
            {
                if (MinimizeToSysTray != value)
                {
                    MinimizeToSysTray = value;
                    OnPropertyChanged("_MinToSysTray");
                }
            }
        }

        public static bool MinimizeOnStart = false;

        public bool _MinOnStart
        {
            get { return MinimizeOnStart; }
            set
            {
                if (MinimizeOnStart != value)
                {
                    MinimizeOnStart = value;
                    OnPropertyChanged("_MinOnStart");
                }
            }
        }

        #region INotifyPropertyChanged
        void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
