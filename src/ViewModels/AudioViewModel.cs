using Captura.Properties;
using Screna.Audio;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Captura
{
    public class AudioViewModel : ViewModelBase
    {
        // Separate method required for BASS to be optional.
        static void InitBass() => MixedAudioProvider.Init();

        static AudioViewModel()
        {
            if (BassExists())
                InitBass();
        }

        public AudioViewModel()
        {
            CanEncode = File.Exists("ffmpeg.exe");

            if (!CanEncode)
                Encode = false;

            RefreshAudioSources();
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
        
        static bool BassExists()
        {
            return AllExist("Screna.Bass.dll", "ManagedBass.dll", "ManagedBass.Mix.dll", "bass.dll", "bassmix.dll");
        }

        public void RefreshAudioSources()
        {
            AvailableRecordingSources.Clear();
            AvailableLoopbackSources.Clear();

            AvailableRecordingSources.Add(new KeyValuePair<int?, string>(null, "[No Sound]"));
            AvailableLoopbackSources.Add(new KeyValuePair<int?, string>(null, "[No Sound]"));

            if (BassExists())
                LoadBassDevices();

            SelectedRecordingSource = SelectedLoopbackSource = null;
        }

        // Separate method required for BASS to be optional.
        void LoadBassDevices()
        {
            MixedAudioProvider.GetDevices(out var recs, out var loops);

            foreach (var rec in recs)
                AvailableRecordingSources.Add(rec);

            foreach (var loop in loops)
                AvailableLoopbackSources.Add(loop);
        }

        // Separate method required for BASS to be optional.
        IAudioProvider GetMixedAudioProvider()
        {
            return new MixedAudioProvider(SelectedRecordingSource, SelectedLoopbackSource);
        }
        
        public IAudioProvider GetAudioSource()
        {
            if (SelectedRecordingSource == null && SelectedLoopbackSource == null)
                return null;

            return GetMixedAudioProvider();
        }

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf)
        {
            return Encode ? (IAudioFileWriter)new FFMpegAudioWriter(FileName)
                          : new AudioFileWriter(FileName, Wf);
        }
    }
}