using Captura.Audio;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Captura.Models
{
    public abstract class AudioSource : NotifyPropertyChanged, IDisposable, IRefreshable
    {
        protected readonly ObservableCollection<IAudioItem> RecordingSources = new ObservableCollection<IAudioItem>();

        public ReadOnlyObservableCollection<IAudioItem> AvailableRecordingSources { get; }

        protected AudioSource()
        {
            AvailableRecordingSources = new ReadOnlyObservableCollection<IAudioItem>(RecordingSources);
        }

        public virtual void Dispose() { }

        public void Refresh()
        {
            // Retain previously active sources
            var lastMicNames = RecordingSources
                .Where(M => M.Active)
                .Where(M => !M.IsLoopback)
                .Select(M => M.Name)
                .ToArray();

            var lastSpeakerNames = RecordingSources
                .Where(M => M.Active)
                .Where(M => M.IsLoopback)
                .Select(M => M.Name)
                .ToArray();

            RecordingSources.Clear();

            OnRefresh();

            foreach (var source in RecordingSources.Where(M => !M.IsLoopback))
            {
                source.Active = lastMicNames.Contains(source.Name);
            }

            foreach (var source in RecordingSources.Where(M => M.IsLoopback))
            {
                source.Active = lastSpeakerNames.Contains(source.Name);
            }
        }

        protected abstract void OnRefresh();

        public abstract IAudioProvider GetMixedAudioProvider();

        public abstract IAudioProvider[] GetMultipleAudioProviders();

        public abstract string Name { get; }

        public virtual bool CanChangeSourcesDuringRecording => false;
    }
}