using System.ComponentModel;
using System.Windows.Controls;

namespace Captura
{
    public partial class AudioVideoSettings : UserControl, INotifyPropertyChanged
    {
        public AudioVideoSettings()
        {
            InitializeComponent();

            DataContext = this;

            AudioQuality.Maximum = Mp3AudioEncoderLame.SupportedBitRates.Length - 1;
            AudioQuality.Value = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;
            AudioQuality.Value = (AudioQuality.Maximum + 1) / 2;
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

        public static bool EncodeAudio = true;
                
        public bool _EncodeAudio
        {
            get { return EncodeAudio; }
            set
            {
                if (EncodeAudio != value)
                {
                    EncodeAudio = value;
                    OnPropertyChanged("_EncodeAudio");
                }
            }
        }

        public static bool Stereo = true;

        public bool _Stereo
        {
            get { return Stereo; }
            set
            {
                if (Stereo != value)
                {
                    Stereo = value;
                    OnPropertyChanged("_Stereo");
                }
            }
        }

        public static bool CaptureClicks = true;

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

        public static bool CaptureKeystrokes = true;

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

        void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
