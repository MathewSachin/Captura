using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Captura
{
    public partial class OtherSettings : UserControl, INotifyPropertyChanged
    {
        public static OtherSettings Instance { get; private set; }

        public OtherSettings()
        {
            InitializeComponent();

            DataContext = Instance = this;

            RegionSelector.Closing += (s, e) =>
            {
                Instance.RegSelBox.IsChecked = false;
                e.Cancel = true;
            };
        }

        public static int CaptureDuration = 0;

        public int _CaptureDuration
        {
            get { return CaptureDuration; }
            set
            {
                if (CaptureDuration != value)
                {
                    CaptureDuration = value;
                    OnPropertyChanged();
                }
            }
        }

        public static int StartDelay = 0;

        public int _StartDelay
        {
            get { return StartDelay; }
            set
            {
                if (StartDelay != value)
                {
                    StartDelay = value;
                    OnPropertyChanged();
                }
            }
        }

        #region RegionSelector
        public static RegionSelector RegionSelector = RegionSelector.Instance;

        void ShowRegionSelector(object sender, RoutedEventArgs e)
        {
            RegionSelector.Show();
            VideoSettings.RefreshVideoSources();
        }

        void HideRegionSelector(object sender, RoutedEventArgs e)
        {
            RegionSelector.Hide();
            VideoSettings.RefreshVideoSources();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public static bool IncludeCursor = false;

        public bool _IncludeCursor
        {
            get { return IncludeCursor; }
            set
            {
                if (IncludeCursor != value)
                {
                    IncludeCursor = value;
                    OnPropertyChanged();
                }
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
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
