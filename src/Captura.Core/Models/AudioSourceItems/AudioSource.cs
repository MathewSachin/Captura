using Screna.Audio;
using System;
using System.Collections.ObjectModel;
using Captura.Properties;

namespace Captura.Models
{
    // Users need to call Init and Dispose methods
    public abstract class AudioSource : NotifyPropertyChanged, IDisposable
    {
        class NoSoundItem : NotifyPropertyChanged, IAudioItem
        {
            public static NoSoundItem Instance { get; } = new NoSoundItem();

            NoSoundItem()
            {
                TranslationSource.Instance.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(Name));
            }

            public string Name => Resources.NoAudio;

            public override string ToString() => Name;
        }

        public ObservableCollection<IAudioItem> AvailableRecordingSources { get; } = new ObservableCollection<IAudioItem>();
        public ObservableCollection<IAudioItem> AvailableLoopbackSources { get; } = new ObservableCollection<IAudioItem>();

        IAudioItem _recordingSource, _loopbackSource;

        public virtual void Init() { }

        public virtual void Dispose() { }

        public virtual IAudioItem SelectedRecordingSource
        {
            get => _recordingSource;
            set
            {
                _recordingSource = value ?? NoSoundItem.Instance;

                OnPropertyChanged();
            }
        }

        public virtual IAudioItem SelectedLoopbackSource
        {
            get => _loopbackSource;
            set
            {
                _loopbackSource = value ?? NoSoundItem.Instance;

                OnPropertyChanged();
            }
        }

        public void Refresh()
        {
            AvailableRecordingSources.Clear();
            AvailableLoopbackSources.Clear();

            AvailableRecordingSources.Add(NoSoundItem.Instance);
            AvailableLoopbackSources.Add(NoSoundItem.Instance);

            OnRefresh();

            SelectedRecordingSource = SelectedLoopbackSource = NoSoundItem.Instance;
        }

        public bool AudioAvailable => SelectedRecordingSource != NoSoundItem.Instance || SelectedLoopbackSource != NoSoundItem.Instance;

        protected abstract void OnRefresh();

        public abstract IAudioProvider GetAudioSource();
    }
}