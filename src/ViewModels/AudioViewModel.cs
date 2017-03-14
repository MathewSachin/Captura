using Captura.Properties;
using Screna.Audio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Captura
{
    public class AudioViewModel : ViewModelBase, IDisposable
    {
        static AudioViewModel()
        {
            if (BassExists)
                MixedAudioProvider.Init();
        }

        public AudioViewModel()
        {
            CanEncode = BassExists && File.Exists("ffmpeg.exe");
            
            RefreshAudioSources();
        }

        public void Dispose()
        {
            if (BassExists)
                MixedAudioProvider.Free();
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
        
        public bool CanEncode { get; }
        
        // Check if all BASS dependencies are present
        static bool BassExists { get; } = AllExist("ManagedBass.dll", "ManagedBass.Mix.dll", "bass.dll", "bassmix.dll");

        public void RefreshAudioSources()
        {
            AvailableRecordingSources.Clear();
            AvailableLoopbackSources.Clear();

            AvailableRecordingSources.Add(new KeyValuePair<int?, string>(null, "[No Sound]"));
            AvailableLoopbackSources.Add(new KeyValuePair<int?, string>(null, "[No Sound]"));

            if (BassExists)
                MixedAudioProvider.GetDevices(AvailableRecordingSources, AvailableLoopbackSources);

            SelectedRecordingSource = SelectedLoopbackSource = null;
        }
                
        public IAudioProvider GetAudioSource()
        {
            if (SelectedRecordingSource == null && SelectedLoopbackSource == null)
                return null;

            return new MixedAudioProvider(SelectedRecordingSource, SelectedLoopbackSource);
        }

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf)
        {
            if (CanEncode && Encode)
                return new FFMpegAudioWriter(FileName);

            return new AudioFileWriter(FileName, Wf);
        }
    }
}