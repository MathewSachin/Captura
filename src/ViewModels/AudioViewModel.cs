using System.Collections.Generic;
using System.Collections.ObjectModel;
using Screna.Audio;
using ManagedBass;
using System.IO;
using System;
using Screna.Lame;
using Captura.Properties;

namespace Captura
{
    public class AudioViewModel : ViewModelBase
    {
        static bool IsLamePresent { get; } = File.Exists("Screna.SharpAvi.dll") && File.Exists("SharpAvi.dll")
            && File.Exists($"lameenc{(Environment.Is64BitProcess ? "64" : "32")}.dll");

        public AudioViewModel()
        {
            CanEncode = IsLamePresent;

            if (IsLamePresent)
                InitLame();
            else Encode = false;

            RefreshAudioSources();
        }

        // Separate method required for SharpAvi to be optional.
        void InitLame()
        {
            SupportedBitRates = Mp3EncoderLame.SupportedBitRates;
            _bitrate = Mp3EncoderLame.SupportedBitRates[1];
        }

        public ObservableCollection<KeyValuePair<int?, string>> AvailableRecordingSources { get; } = new ObservableCollection<KeyValuePair<int?, string>>();
        public ObservableCollection<KeyValuePair<int?, string>> AvailableLoopbackSources { get; } = new ObservableCollection<KeyValuePair<int?, string>>();

        int? _recordingSource, _loopbackSource;

        public int? SelectedRecordingSource
        {
            get { return _recordingSource; }
            set
            {
                _recordingSource = value;
                
                OnPropertyChanged();
            }
        }

        public int? SelectedLoopbackSource
        {
            get { return _loopbackSource; }
            set
            {
                _loopbackSource = value;

                OnPropertyChanged();
            }
        }

        public IEnumerable<int> SupportedBitRates { get; private set; }
 
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
               
        public void RefreshAudioSources()
        {
            AvailableRecordingSources.Clear();
            AvailableLoopbackSources.Clear();

            AvailableRecordingSources.Add(new KeyValuePair<int?, string>(null, "[No Sound]"));
            AvailableLoopbackSources.Add(new KeyValuePair<int?, string>(null, "[No Sound]"));

            for (int i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            {
                if (info.IsLoopback)
                    AvailableLoopbackSources.Add(new KeyValuePair<int?, string>(i, info.Name));
                else AvailableRecordingSources.Add(new KeyValuePair<int?, string>(i, info.Name));
            }

            SelectedRecordingSource = SelectedLoopbackSource = null;
        }

        // Separate method required for SharpAvi to be optional.
        IAudioEncoder GetLameEncoder(Screna.Audio.WaveFormat Wf)
        {
            return new Mp3EncoderLame(Wf.Channels, Wf.SampleRate, SelectedBitRate);
        }

        public IAudioProvider GetAudioSource()
        {
            if (SelectedRecordingSource == null && SelectedLoopbackSource == null)
                return null;

            var audioProvider = new MixedAudioProvider(SelectedRecordingSource, SelectedLoopbackSource);

            return Encode ? (IAudioProvider)new EncodedAudioProvider(audioProvider, GetLameEncoder(audioProvider.WaveFormat))
                : audioProvider;
        }

        public IAudioFileWriter GetAudioFileWriter(string FileName, Screna.Audio.WaveFormat Wf)
        {
            return Encode ? new AudioFileWriter(FileName, GetLameEncoder(Wf))
                          : new AudioFileWriter(FileName, Wf);
        }
    }
}