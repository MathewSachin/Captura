using Screna.Audio;
using System;
using System.Collections.ObjectModel;

namespace Captura.Models
{
    public abstract class AudioSource : NotifyPropertyChanged, IDisposable
    {
        protected readonly ObservableCollection<IAudioItem> RecordingSources = new ObservableCollection<IAudioItem>();
        protected readonly ObservableCollection<IAudioItem> LoopbackSources = new ObservableCollection<IAudioItem>();

        public ReadOnlyObservableCollection<IAudioItem> AvailableRecordingSources { get; }
        public ReadOnlyObservableCollection<IAudioItem> AvailableLoopbackSources { get; }

        protected AudioSource()
        {
            AvailableRecordingSources = new ReadOnlyObservableCollection<IAudioItem>(RecordingSources);
            AvailableLoopbackSources = new ReadOnlyObservableCollection<IAudioItem>(LoopbackSources);
        }

        IAudioItem _recordingSource = NoSoundItem.Instance,
            _loopbackSource = NoSoundItem.Instance;

        public virtual void Dispose() { }

        public virtual IAudioItem SelectedRecordingSource
        {
            get => _recordingSource;
            set
            {
                _recordingSource = value ?? NoSoundItem.Instance;

                OnPropertyChanged();

                RaisePropertyChanged(nameof(AudioAvailable));
            }
        }

        public virtual IAudioItem SelectedLoopbackSource
        {
            get => _loopbackSource;
            set
            {
                _loopbackSource = value ?? NoSoundItem.Instance;

                OnPropertyChanged();

                RaisePropertyChanged(nameof(AudioAvailable));
            }
        }

        public void Refresh()
        {
            RecordingSources.Clear();
            LoopbackSources.Clear();

            RecordingSources.Add(NoSoundItem.Instance);
            LoopbackSources.Add(NoSoundItem.Instance);

            OnRefresh();

            SelectedRecordingSource = SelectedLoopbackSource = NoSoundItem.Instance;
        }

        public bool AudioAvailable => SelectedRecordingSource != NoSoundItem.Instance || SelectedLoopbackSource != NoSoundItem.Instance;

        protected abstract void OnRefresh();

        public abstract IAudioProvider GetAudioProvider();
    }
}