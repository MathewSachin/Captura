using System.ComponentModel;
using System.Windows.Controls;
using SharpAvi.Codecs;

namespace Captura
{
    public partial class AudioVideoSettings : UserControl, INotifyPropertyChanged
    {
        static readonly int MaxAudioQuality = Mp3AudioEncoderLame.SupportedBitRates.Length - 1;

        public AudioVideoSettings()
        {
            InitializeComponent();

            DataContext = this;

            AudioQualitySlider.Maximum = MaxAudioQuality;
        }

        public static int AudioQuality = Mp3AudioEncoderLame.SupportedBitRates.Length / 2;

        public double _AudioQuality
        {
            get { return AudioQuality; }
            set
            {
                if (AudioQuality != (int)value)
                {
                    AudioQuality = (int)value;
                    OnPropertyChanged("_AudioQuality");
                }
            }
        }

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
