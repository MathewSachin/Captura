using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Captura
{
    public partial class OtherSettings : INotifyPropertyChanged
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

        public static int CaptureDuration;

        public int _CaptureDuration
        {
            get { return CaptureDuration; }
            set
            {
                if (CaptureDuration == value)
                    return;

                CaptureDuration = value;
                OnPropertyChanged();
            }
        }

        public static int StartDelay;

        public int _StartDelay
        {
            get { return StartDelay; }
            set
            {
                if (StartDelay == value)
                    return;

                StartDelay = value;
                OnPropertyChanged();
            }
        }

        #region RegionSelector
        public static readonly RegionSelector RegionSelector = RegionSelector.Instance;

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
        public static bool CaptureClicks;

        public bool _CaptureClicks
        {
            get { return CaptureClicks; }
            set
            {
                if (CaptureClicks == value)
                    return;

                CaptureClicks = value;
                OnPropertyChanged();
            }
        }

        public static bool CaptureKeystrokes;

        public bool _CaptureKeystrokes
        {
            get { return CaptureKeystrokes; }
            set
            {
                if (CaptureKeystrokes == value)
                    return;

                CaptureKeystrokes = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public static bool StaticRegionCapture;

        public bool _StaticRegionCapture
        {
            get { return StaticRegionCapture; }
            set
            {
                if (StaticRegionCapture == value)
                    return;

                StaticRegionCapture = value;
                OnPropertyChanged();
            }
        }

        public static bool IncludeCursor;

        public bool _IncludeCursor
        {
            get { return IncludeCursor; }
            set
            {
                if (IncludeCursor == value)
                    return;

                IncludeCursor = value;
                OnPropertyChanged();
            }
        }

        public static bool MinimizeToSysTray;

        public bool _MinToSysTray
        {
            get { return MinimizeToSysTray; }
            set
            {
                if (MinimizeToSysTray == value)
                    return;

                MinimizeToSysTray = value;
                OnPropertyChanged();
            }
        }

        public static bool MinimizeOnStart;

        public bool _MinOnStart
        {
            get { return MinimizeOnStart; }
            set
            {
                if (MinimizeOnStart == value)
                    return;

                MinimizeOnStart = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged
        void OnPropertyChanged([CallerMemberName] string e = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
