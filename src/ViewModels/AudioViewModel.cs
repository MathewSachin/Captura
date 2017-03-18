using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Captura
{
    public class AudioViewModel : ViewModelBase, IDisposable
    {
        public AudioSource AudioSource { get; }

        public AudioViewModel()
        {
            if (BassAudioSource.Available)
                AudioSource = new BassAudioSource();
            else AudioSource = NoAudioSource.Instance;

            AudioSource.Init();

            if (AudioSource != NoAudioSource.Instance && File.Exists("ffmpeg.exe"))
            {
                foreach (var item in FFMpegAudioWriterItem.Items)
                    AvailableAudioWriters.Add(item);

                SelectedAudioWriter = FFMpegAudioWriterItem.Mp3;
            }

            AudioSource.Refresh();
        }

        public void Dispose() => AudioSource.Dispose();
                
        public bool CanEncode { get; }
        
        public ObservableCollection<IAudioWriterItem> AvailableAudioWriters { get; } = new ObservableCollection<IAudioWriterItem>
        {
            WaveWriterItem.Instance
        };

        IAudioWriterItem _audioWriterItem = WaveWriterItem.Instance;

        public IAudioWriterItem SelectedAudioWriter
        {
            get { return _audioWriterItem; }
            set
            {
                _audioWriterItem = value;

                OnPropertyChanged();
            }
        }
    }
}