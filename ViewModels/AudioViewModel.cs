using System;
using System.Collections.Generic;
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
        public const string NoSoundSource = "[No Sound]";

        static bool IsLamePresent { get; } = File.Exists
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

            if (IsLamePresent)
            {
                SupportedBitRates = Mp3EncoderLame.SupportedBitRates;
                _bitrate = Mp3EncoderLame.SupportedBitRates[1];
            }
            else Encode = false;

            RefreshAudioSources();
        }

        public ObservableCollection<object> AvailableAudioSources { get; } = new ObservableCollection<object>();

        object _audioSource = NoSoundSource;

        public object SelectedAudioSource
        {
            get { return _audioSource; }
            set
            {
                _audioSource = value ?? NoSoundSource;
                
                OnPropertyChanged();
            }
        }

        public IEnumerable<int> SupportedBitRates { get; }

        int _bitrate;

        public int SelectedBitRate
        {
            get { return _bitrate; }
            set
            {
                if (_bitrate == value)
                    return;

                _bitrate = value;

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

            AvailableAudioSources.Add(NoSoundSource);

            foreach (var dev in WaveInDevice.Enumerate())
                AvailableAudioSources.Add(dev);

            foreach (var dev in LoopbackProvider.EnumerateDevices())
                AvailableAudioSources.Add(dev);

            SelectedAudioSource = NoSoundSource;
        }
        
        public IAudioProvider GetAudioSource(int FrameRate, out WaveFormat Wf)
        {
            Wf = new WaveFormat(44100, 16, Stereo ? 2 : 1);

            IAudioEncoder audioEncoder = SelectedBitRate != 0 && IsLamePresent && Encode ? new Mp3EncoderLame(Wf.Channels, Wf.SampleRate, SelectedBitRate) : null;

            if (SelectedAudioSource is WaveInDevice)
                return new WaveInProvider(SelectedAudioSource as WaveInDevice, Wf, FrameRate);

            if (!(SelectedAudioSource is MMDevice))
                return null;

            IAudioProvider audioSource = new LoopbackProvider((MMDevice)SelectedAudioSource);

            Wf = audioSource.WaveFormat;

            return audioEncoder == null ? audioSource : new EncodedAudioProvider(audioSource, audioEncoder);
        }

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf)
        {
            return Encode ? new AudioFileWriter(FileName, new Mp3EncoderLame(Wf.Channels, Wf.SampleRate, SelectedBitRate))
                          : new AudioFileWriter(FileName, Wf);
        }
    }
}