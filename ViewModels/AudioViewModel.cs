using System.Collections.ObjectModel;
using Screna.Lame;
using Screna.NAudio;

namespace Captura
{
    public class AudioViewModel : ViewModelBase
    {
        public AudioViewModel()
        {
            CanEncode = App.IsLamePresent;

            if (!App.IsLamePresent)
                Encode = false;
            else
            {
                MaxQuality = Mp3EncoderLame.SupportedBitRates.Length - 1;
                Quality = Mp3EncoderLame.SupportedBitRates.Length / 2;
            }
            
            RefreshAudioSources();
        }

        public ObservableCollection<object> AvailableAudioSources { get; } = new ObservableCollection<object>();

        object _audioSource = "[No Sound]";

        public object SelectedAudioSource
        {
            get { return _audioSource; }
            set
            {
                if (_audioSource == value)
                    return;

                _audioSource = value;
                
                OnPropertyChanged();
            }
        }

        int _maxQuality;

        public int MaxQuality
        {
            get { return _maxQuality; }
            set
            {
                if (_maxQuality == value)
                    return;

                _maxQuality = value;

                OnPropertyChanged();
            }
        }

        int _quality;

        public int Quality
        {
            get { return _quality; }
            set
            {
                if (_quality == value)
                    return;

                _quality = value;

                OnPropertyChanged();
            }
        }

        bool _encode;

        public bool Encode
        {
            get { return _encode; }
            set
            {
                if (_encode == value)
                    return;

                _encode = value;

                OnPropertyChanged();
            }
        }

        bool _canEncode;

        public bool CanEncode
        {
            get { return _canEncode; }
            set
            {
                if (_canEncode == value)
                    return;

                _canEncode = value;

                OnPropertyChanged();
            }
        }
    
        bool _stereo;

        public bool Stereo
        {
            get { return _stereo; }
            set
            {
                if (_stereo == value)
                    return;

                _stereo = value;

                OnPropertyChanged();
            }
        }

        public void RefreshAudioSources()
        {
            AvailableAudioSources.Clear();

            AvailableAudioSources.Add("[No Sound]");

            foreach (var dev in WaveInDevice.Enumerate())
                AvailableAudioSources.Add(dev);

            foreach (var dev in LoopbackProvider.EnumerateDevices())
                AvailableAudioSources.Add(dev);
        }
    }
}