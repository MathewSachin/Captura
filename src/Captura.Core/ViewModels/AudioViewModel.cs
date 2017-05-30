using Captura.Models;
using System;
using System.Collections.ObjectModel;

namespace Captura.ViewModels
{
    public class AudioViewModel : ViewModelBase, IDisposable
    {
        public AudioSource AudioSource { get; }

        public AudioViewModel()
        {
            if (BassAudioSource.Available)
                AudioSource = new BassAudioSource();
            /*
            else if (NAudioSource.Available)
                AudioSource = new NAudioSource();
            */
            else AudioSource = NoAudioSource.Instance;

            AudioSource.Init();

            RefreshFFMpeg();
            
            AudioSource.Refresh();

            ServiceProvider.FFMpegPathChanged += RefreshFFMpeg;
        }

        public void RefreshFFMpeg()
        {
            if (ServiceProvider.FFMpegExists)
            {
                foreach (var item in FFMpegAudioWriterItem.Items)
                {
                    if (!AvailableAudioWriters.Contains(item))
                        AvailableAudioWriters.Add(item);
                }
            }
            else
            {
                foreach (var item in FFMpegAudioWriterItem.Items)
                {
                    if (AvailableAudioWriters.Contains(item))
                        AvailableAudioWriters.Remove(item);
                }
            }
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