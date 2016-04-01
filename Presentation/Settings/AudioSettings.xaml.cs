using Screna.Audio;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Captura
{
    public partial class AudioSettings : INotifyPropertyChanged
    {
        public static AudioSettings Instance;

        #region Init
        static AudioSettings()
        {
            if (!App.IsLamePresent) EncodeAudio = false;
            else
            {
                MaxAudioQuality = Mp3EncoderLame.SupportedBitRates.Length - 1;
                AudioQuality = Mp3EncoderLame.SupportedBitRates.Length / 2;
            }

            AvailableAudioSources = new ObservableCollection<KeyValuePair<string, string>>();
        }

        public AudioSettings()
        {
            _AvailableAudioSources = AvailableAudioSources;

            InitializeComponent();

            DataContext = this;

            Instance = this;

            if (App.IsLamePresent) AudioQualitySlider.Maximum = MaxAudioQuality;
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

        public static ObservableCollection<KeyValuePair<string, string>> AvailableAudioSources { get; }

        public ObservableCollection<KeyValuePair<string, string>> _AvailableAudioSources { get; private set; }

        public static string SelectedAudioSourceId = "-1";

        public static void RefreshAudioSources()
        {
            AvailableAudioSources.Clear();

            AvailableAudioSources.Add(new KeyValuePair<string, string>("-1", "[No Sound]"));

            foreach (var Dev in WaveInDevice.Enumerate())
                AvailableAudioSources.Add(new KeyValuePair<string, string>(Dev.DeviceNumber.ToString(), Dev.Name));

            foreach (var Dev in WasapiLoopbackCapture.EnumerateDevices())
                AvailableAudioSources.Add(new KeyValuePair<string, string>(Dev.ID, Dev.Name + " (Loopback)"));

            if (Instance != null) Instance.AudioSourcesBox.SelectedIndex = 0;
        }

        public string _SelectedAudioSourceId
        {
            get { return SelectedAudioSourceId; }
            set
            {
                if (SelectedAudioSourceId == value)
                    return;

                SelectedAudioSourceId = value;
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
                if (AudioQuality != (int)value)
                {
                    AudioQuality = (int)value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public static bool Stereo;

        public bool _Stereo
        {
            get { return Stereo; }
            set
            {
                if (Stereo != value)
                {
                    Stereo = value;
                    OnPropertyChanged();
                }
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
