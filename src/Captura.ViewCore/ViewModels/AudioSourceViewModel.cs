using System;
using Captura.Audio;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.Models
{
    public class AudioSourceViewModel : NotifyPropertyChanged
    {
        readonly IAudioSource _audioSource;

        readonly ObservableCollection<IAudioItem> _microphones = new ObservableCollection<IAudioItem>(),
            _speakers = new ObservableCollection<IAudioItem>();

        public ReadOnlyObservableCollection<IAudioItem> AvailableMicrophones { get; }

        public ReadOnlyObservableCollection<IAudioItem> AvailableSpeakers { get; }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        readonly IObservable<Unit> _refreshObservable;

        public AudioSourceViewModel(IAudioSource AudioSource)
        {
            _audioSource = AudioSource;

            AvailableMicrophones = new ReadOnlyObservableCollection<IAudioItem>(_microphones);
            AvailableSpeakers = new ReadOnlyObservableCollection<IAudioItem>(_speakers);

            Refresh();

            RefreshCommand = new DelegateCommand(Refresh);

            SelectedMicPeakLevel = Observable.Interval(TimeSpan.FromMilliseconds(50))
                .Where(M => ListeningPeakLevel)
                .ObserveOnUIDispatcher()
                .Select(M => SelectedMicrophone?.PeakLevel ?? 0)
                .ToReadOnlyReactivePropertySlim();

            SelectedSpeakerPeakLevel = Observable.Interval(TimeSpan.FromMilliseconds(50))
                .Where(M => ListeningPeakLevel)
                .ObserveOnUIDispatcher()
                .Select(M => SelectedSpeaker?.PeakLevel ?? 0)
                .ToReadOnlyReactivePropertySlim();

            _refreshObservable = Observable.FromEvent(M => AudioSource.DevicesUpdated += M,
                M => AudioSource.DevicesUpdated -= M)
                .Throttle(TimeSpan.FromSeconds(0.5));

            _refreshObservable
                .ObserveOnUIDispatcher()
                .Subscribe(M => Refresh());
        }

        void RefreshMics()
        {
            var lastMicName = SelectedMicrophone?.Name;

            foreach (var microphone in _microphones)
            {
                microphone.Dispose();
            }

            _microphones.Clear();

            if (_audioSource.DefaultMicrophone is { } defaultMicrophone)
            {
                _microphones.Add(defaultMicrophone);
            }

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

            foreach (var speaker in _speakers)
            {
                speaker.Dispose();
            }

            _speakers.Clear();
            
            if (_audioSource.DefaultSpeaker is { } defaultSpeaker)
            {
                _speakers.Add(defaultSpeaker);
            }

            foreach (var speaker in _audioSource.Speakers)
            {
                _speakers.Add(speaker);
            }

            var matchSpeaker = _speakers.FirstOrDefault(M => M.Name == lastSpeakerName);

            SelectedSpeaker = matchSpeaker;
        }

        void Refresh()
        {
            RefreshMics();

            RefreshSpeakers();
        }

        public ICommand RefreshCommand { get; }

        bool _listenPeakLvl;

        public bool ListeningPeakLevel
        {
            get => _listenPeakLvl;
            set
            {
                if (_listenPeakLvl && !value)
                {
                    _listenPeakLvl = false;

                    SelectedMicrophone?.StopListeningForPeakLevel();
                    SelectedSpeaker?.StopListeningForPeakLevel();
                }
                else if (!_listenPeakLvl && value)
                {
                    _listenPeakLvl = true;

                    SelectedMicrophone?.StartListeningForPeakLevel();
                    SelectedSpeaker?.StartListeningForPeakLevel();
                }
            }
        }

        public string Name => _audioSource.Name;

        IAudioItem _selectedMicrophone, _selectedSpeaker;

        public IAudioItem SelectedMicrophone
        {
            get => _selectedMicrophone;
            set
            {
                var old = SelectedMicrophone;

                if (Set(ref _selectedMicrophone, value ?? AvailableMicrophones.FirstOrDefault()))
                {
                    old?.StopListeningForPeakLevel();

                    if (ListeningPeakLevel)
                    {
                        _selectedMicrophone?.StartListeningForPeakLevel();
                    }
                }
            }
        }

        public IAudioItem SelectedSpeaker
        {
            get => _selectedSpeaker;
            set
            {
                var old = SelectedSpeaker;

                if (Set(ref _selectedSpeaker, value ?? AvailableSpeakers.FirstOrDefault()))
                {
                    old?.StopListeningForPeakLevel();

                    if (ListeningPeakLevel)
                    {
                        _selectedSpeaker?.StartListeningForPeakLevel();
                    }
                }
            }
        }

        public IReadOnlyReactiveProperty<double> SelectedMicPeakLevel { get; }

        public IReadOnlyReactiveProperty<double> SelectedSpeakerPeakLevel { get; }
    }
}