using Captura.Audio;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Captura.Models
{
    public class AudioSourceViewModel : NotifyPropertyChanged, IRefreshable
    {
        readonly IAudioSource _audioSource;

        readonly ObservableCollection<IAudioItem> _microphones = new ObservableCollection<IAudioItem>(),
            _speakers = new ObservableCollection<IAudioItem>();

        public ReadOnlyObservableCollection<IAudioItem> AvailableMicrophones { get; }

        public ReadOnlyObservableCollection<IAudioItem> AvailableSpeakers { get; }

        public AudioSourceViewModel(IAudioSource AudioSource)
        {
            _audioSource = AudioSource;

            AvailableMicrophones = new ReadOnlyObservableCollection<IAudioItem>(_microphones);
            AvailableSpeakers = new ReadOnlyObservableCollection<IAudioItem>(_speakers);

            Refresh();

            RefreshCommand = new DelegateCommand(Refresh);
        }

        void RefreshMics()
        {
            var lastMicName = SelectedMicrophone?.Name;

            _microphones.Clear();

            _microphones.Add(_audioSource.DefaultMicrophone);

            foreach (var mic in _audioSource.Microphones)
            {
                _microphones.Add(mic);
            }

            var matchMic = _microphones.FirstOrDefault(M => M.Name == lastMicName);

            SelectedMicrophone = matchMic;
        }

        void RefreshSpeakers()
        {
            var lastSpeakerName = SelectedSpeaker?.Name;

            _speakers.Clear();

            _speakers.Add(_audioSource.DefaultSpeaker);

            foreach (var speaker in _audioSource.Speakers)
            {
                _speakers.Add(speaker);
            }

            var matchSpeaker = _speakers.FirstOrDefault(M => M.Name == lastSpeakerName);

            SelectedSpeaker = matchSpeaker;
        }

        public void Refresh()
        {
            RefreshMics();

            RefreshSpeakers();
        }

        public ICommand RefreshCommand { get; }

        public string Name => _audioSource.Name;

        IAudioItem _selectedMicrophone, _selectedSpeaker;

        public IAudioItem SelectedMicrophone
        {
            get => _selectedMicrophone;
            set => Set(ref _selectedMicrophone, value ?? AvailableMicrophones.FirstOrDefault());
        }

        public IAudioItem SelectedSpeaker
        {
            get => _selectedSpeaker;
            set => Set(ref _selectedSpeaker, value ?? AvailableSpeakers.FirstOrDefault());
        }
    }
}