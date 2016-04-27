using Screna.NAudio;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Screna.Lame;

namespace Captura
{
    public partial class AudioSettings : INotifyPropertyChanged
    {
        static AudioSettings Instance;

        #region Init
        static AudioSettings()
        {
            if (!App.IsLamePresent)
                EncodeAudio = false;
            else
            {
                MaxAudioQuality = Mp3EncoderLame.SupportedBitRates.Length - 1;
                AudioQuality = Mp3EncoderLame.SupportedBitRates.Length / 2;
            }

            AvailableAudioSources = new ObservableCollection<object>();
        }

        public AudioSettings()
        {
            InitializeComponent();

            DataContext = this;

            Instance = this;

            if (App.IsLamePresent)
                AudioQualitySlider.Maximum = MaxAudioQuality;
            else
            {
                AudioQualitySlider.IsEnabled = false;
                EncodeMp3Box.IsEnabled = false;
            }

            RefreshAudioSources();

            AudioSourcesBox.SelectedIndex = 0;
        }
        #endregion

        void AudioVideoSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VideoSettings.CheckFunctionalityAvailability();
        }

        public static ObservableCollection<object> AvailableAudioSources { get; }

        public ObservableCollection<object> _AvailableAudioSources { get; } = AvailableAudioSources;

        public static object SelectedAudioSource = "[No Sound]";

        public static void RefreshAudioSources()
        {
            AvailableAudioSources.Clear();

            AvailableAudioSources.Add("[No Sound]");

            foreach (var dev in WaveInDevice.Enumerate())
                AvailableAudioSources.Add(dev);

            foreach (var dev in LoopbackProvider.EnumerateDevices())
                AvailableAudioSources.Add(dev);

            if (Instance != null)
                Instance.AudioSourcesBox.SelectedIndex = 0;
        }

        public object _SelectedAudioSource
        {
            get { return SelectedAudioSource; }
            set
            {
                if (SelectedAudioSource == value)
                    return;

                SelectedAudioSource = value;
                OnPropertyChanged();
            }
        }

        #region Audio
        static readonly int MaxAudioQuality;

        public static int AudioQuality;

        public double _AudioQuality
        {
            get { return AudioQuality; }
            set
            {
                if (AudioQuality == (int) value)
                    return;

                AudioQuality = (int)value;
                OnPropertyChanged();
            }
        }

        public static bool EncodeAudio = true;

        public bool _EncodeAudio
        {
            get { return EncodeAudio; }
            set
            {
                if (EncodeAudio == value)
                    return;

                EncodeAudio = value;
                OnPropertyChanged();
            }
        }

        public static bool Stereo;

        public bool _Stereo
        {
            get { return Stereo; }
            set
            {
                if (Stereo == value)
                    return;

                Stereo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region INotifyPropertyChanged
        void OnPropertyChanged([CallerMemberName] string e = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
