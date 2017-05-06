using Captura.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Captura.ViewModels
{
    public class AudioViewModel : ViewModelBase, IDisposable
    {
        public AudioSource AudioSource { get; }

        public AudioViewModel()
        {
            if (BassAudioSource.Available)
                AudioSource = new BassAudioSource();
            else if (NAudioSource.Available)
                AudioSource = new NAudioSource();
            else AudioSource = NoAudioSource.Instance;

            AudioSource.Init();

            if (AudioSource != NoAudioSource.Instance && File.Exists("ffmpeg.exe"))
            {
                foreach (var item in FFMpegAudioWriterItem.Items)
                {
                    if (item.Extension == ".mp3")
                        SelectedAudioWriter = item;

                    AvailableAudioWriters.Add(item);
                }
            }

            AudioSource.Refresh();
        }

        public void Dispose() => AudioSource.Dispose();
        
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