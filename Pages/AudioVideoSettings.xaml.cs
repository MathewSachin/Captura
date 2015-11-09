using System.ComponentModel;
using System.Windows.Controls;
using SharpAvi.Codecs;

namespace Captura
{
    public partial class AudioVideoSettings : UserControl, INotifyPropertyChanged
    {
        static readonly int MaxAudioQuality = 0;

        public static int AudioQuality = 0;

        static AudioVideoSettings()
        {
            if (!App.IsLamePresent) EncodeAudio = false;
            else
            {
                MaxAudioQuality = Mp3AudioEncoderLame.SupportedBitRates.Length - 1;
                AudioQuality = Mp3AudioEncoderLame.SupportedBitRates.Length / 2;
            }
        }

        public AudioVideoSettings()
        {
            InitializeComponent();

            DataContext = this;

            if (App.IsLamePresent) AudioQualitySlider.Maximum = MaxAudioQuality;
            else
            {
                AudioQualitySlider.IsEnabled = false;
                EncodeMp3Box.IsEnabled = false;
            }
        }        

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

        public static bool Stereo = false;

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

        void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
