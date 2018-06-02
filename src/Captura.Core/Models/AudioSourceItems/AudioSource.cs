using Screna.Audio;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        public virtual void Dispose() { }

        public void Refresh()
        {
            RecordingSources.Clear();
            LoopbackSources.Clear();

            OnRefresh();
        }

        public bool AudioAvailable => AvailableRecordingSources.Count(M => M.Active) + AvailableLoopbackSources.Count(M => M.Active) > 0;

        protected abstract void OnRefresh();

        public abstract IAudioProvider GetAudioProvider();

        public static event Action AudioSourceActiveChanged;

        public static void RaiseAudioSourceActiveChanged()
        {
            AudioSourceActiveChanged?.Invoke();
        }
    }
}