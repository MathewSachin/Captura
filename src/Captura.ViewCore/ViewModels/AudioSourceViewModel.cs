using Captura.Audio;
using System.Collections.ObjectModel;
using System.Linq;

namespace Captura.Models
{
    public class AudioSourceViewModel : IRefreshable
    {
        readonly IAudioSource _audioSource;

        readonly ObservableCollection<IIsActive<IAudioItem>> RecordingSources = new ObservableCollection<IIsActive<IAudioItem>>();

        public ReadOnlyObservableCollection<IIsActive<IAudioItem>> AvailableRecordingSources { get; }

        public AudioSourceViewModel(IAudioSource AudioSource)
        {
            _audioSource = AudioSource;

            AvailableRecordingSources = new ReadOnlyObservableCollection<IIsActive<IAudioItem>>(RecordingSources);

            Refresh();
        }

        public void Refresh()
        {
            // Retain previously active sources
            var lastMicNames = RecordingSources
                .Where(M => M.IsActive)
                .Select(M => M.Item)
                .Where(M => !M.IsLoopback)
                .Select(M => M.Name)
                .ToArray();

            var lastSpeakerNames = RecordingSources
                .Where(M => M.IsActive)
                .Select(M => M.Item)
                .Where(M => M.IsLoopback)
                .Select(M => M.Name)
                .ToArray();

            RecordingSources.Clear();

            var sources = _audioSource.GetSources();

            foreach (var source in sources.Select(M => M.ToIsActive()))
            {
                RecordingSources.Add(source);

                source.IsActive = source.Item.IsLoopback
                    ? lastSpeakerNames.Contains(source.Item.Name)
                    : lastMicNames.Contains(source.Item.Name);
            }
        }

        public IAudioProvider GetMixedAudioProvider()
        {
            return _audioSource.GetMixedAudioProvider(RecordingSources);
        }

        public IAudioProvider[] GetMultipleAudioProviders()
        {
            return RecordingSources
                .Where(M => M.IsActive)
                .Select(M => _audioSource.GetAudioProvider(M.Item))
                .ToArray();
        }

        public string Name => _audioSource.Name;

        public bool CanChangeSourcesDuringRecording { get; }
    }
}