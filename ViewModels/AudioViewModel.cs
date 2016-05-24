using System;
using System.Collections.ObjectModel;
using System.IO;
using Captura.Properties;
using NAudio.CoreAudioApi;
using Screna.Audio;
using Screna.Lame;
using Screna.NAudio;

namespace Captura
{
    public class AudioViewModel : ViewModelBase
    {
        public static bool IsLamePresent { get; private set; } = File.Exists
        (
            Path.Combine
            (
                Path.GetDirectoryName(typeof(AudioViewModel).Assembly.Location),
                $"lameenc{(Environment.Is64BitProcess ? "64" : "32")}.dll"
            )
        );

        public AudioViewModel()
        {
            CanEncode = IsLamePresent;

            if (!IsLamePresent)
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
            get { return _audioSource ?? "[No Sound]"; }
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
        
        public bool Encode
        {
            get { return Settings.Default.EncodeAudio; }
            set
            {
                if (Encode == value)
                    return;

                Settings.Default.EncodeAudio = value;

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
    
        public bool Stereo
        {
            get { return Settings.Default.UseStereo; }
            set
            {
                if (Stereo == value)
                    return;

                Settings.Default.UseStereo = value;

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

        public int BitRate => IsLamePresent ? Mp3EncoderLame.SupportedBitRates[Quality] : 0;

        public IAudioProvider GetAudioSource(int FrameRate, out WaveFormat Wf)
        {
            Wf = new WaveFormat(44100, 16, Stereo ? 2 : 1);
            
            if (SelectedAudioSource is WaveInDevice)
                return new WaveInProvider(SelectedAudioSource as WaveInDevice, FrameRate, Wf);

            if (SelectedAudioSource is MMDevice)
            {
                var audioSource = new LoopbackProvider(SelectedAudioSource as MMDevice);

                Wf = audioSource.WaveFormat;

                return audioSource;
            }

            return null;
        }

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf)
        {
            return Encode ? (IAudioFileWriter)new EncodedAudioFileWriter(FileName, new Mp3EncoderLame(Wf.Channels, Wf.SampleRate, BitRate))
                          : new WaveFileWriter(FileName, Wf);
        }
    }
}