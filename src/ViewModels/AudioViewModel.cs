using System.Collections.Generic;
using System.Collections.ObjectModel;
using Screna.Audio;
using ManagedBass;

namespace Captura
{
    public class AudioViewModel : ViewModelBase
    {
        public AudioViewModel()
        {
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
             
        public void RefreshAudioSources()
        {
            AvailableRecordingSources.Clear();
            AvailableLoopbackSources.Clear();

            AvailableRecordingSources.Add(new KeyValuePair<int?, string>(null, "[No Sound]"));
            AvailableLoopbackSources.Add(new KeyValuePair<int?, string>(null, "[No Sound]"));
            
            DeviceInfo info;

            for (int i = 0; Bass.RecordGetDeviceInfo(i, out info); ++i)
            {
                if (info.IsLoopback)
                    AvailableLoopbackSources.Add(new KeyValuePair<int?, string>(i, info.Name));
                else AvailableRecordingSources.Add(new KeyValuePair<int?, string>(i, info.Name));
            }

            SelectedRecordingSource = SelectedLoopbackSource = null;
        }
        
        public IAudioProvider GetAudioSource()
        {
            if (SelectedRecordingSource == null && SelectedLoopbackSource == null)
                return null;

            return new MixedAudioProvider(SelectedRecordingSource, SelectedLoopbackSource);
        }

        public IAudioFileWriter GetAudioFileWriter(string FileName, Screna.Audio.WaveFormat Wf)
        {
            return new AudioFileWriter(FileName, Wf);
        }
    }
}