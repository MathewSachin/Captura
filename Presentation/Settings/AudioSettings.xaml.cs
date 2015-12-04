using System.ComponentModel;
using System.Windows.Controls;
using ScreenWorks;

namespace Captura
{
    public partial class AudioSettings : UserControl, INotifyPropertyChanged
    {
        static AudioSettings Instance;

        #region Init
        static AudioSettings()
        {
            if (!App.IsLamePresent) EncodeAudio = false;
            else
            {
                MaxAudioQuality = SharpAviEncoder.LameSupportedBitRatesCount - 1;
                AudioQuality = SharpAviEncoder.LameSupportedBitRatesCount / 2;
            }
        }

        public AudioSettings()
        {
            InitializeComponent();

            DataContext = this;

            Instance = this;

            if (App.IsLamePresent) AudioQualitySlider.Maximum = MaxAudioQuality;
            else
            {
                AudioQualitySlider.IsEnabled = false;
                EncodeMp3Box.IsEnabled = false;
            }
        }
        #endregion
                
        #region Audio
        static readonly int MaxAudioQuality = 0;

        public static int AudioQuality = 0;

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
        #endregion
        
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
