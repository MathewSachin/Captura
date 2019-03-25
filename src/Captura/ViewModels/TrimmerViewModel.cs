using System;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Captura.FFmpeg;
using Microsoft.Win32;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura
{
    public class TrimmerViewModel : NotifyPropertyChanged, IDisposable
    {
        MediaElement _player;
        readonly DispatcherTimer _timer;

        public bool IsDragging { get; set; }

        readonly IReactiveProperty<bool> _isOpened = new ReactivePropertySlim<bool>();
        readonly IReactiveProperty<bool> _isTrimming = new ReactivePropertySlim<bool>();

        public void AssignPlayer(MediaElement Player)
        {
            _player = Player;

            _player.MediaOpened += (S, E) =>
            {
                From = TimeSpan.Zero;

                if (_player.NaturalDuration.HasTimeSpan)
                {
                    To = End = _player.NaturalDuration.TimeSpan;
                }
                else To = End = TimeSpan.Zero;

                PlaybackPosition = From;

                _isOpened.Value = true;
            };

            _timer.Start();
        }

        public TrimmerViewModel()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            _timer.Tick += (Sender, Args) =>
            {
                if (IsDragging)
                    return;

                RaisePropertyChanged(nameof(PlaybackPosition));

                if (IsPlaying && _player.Position > To || _player.Position < From)
                {
                    Stop();
                }
            };

            OpenCommand = _isTrimming
                .Select(M => !M)
                .ToReactiveCommand()
                .WithSubscribe(Open);

            PlayCommand = new[]
                {
                    _isOpened,
                    _isTrimming
                        .Select(M => !M)
                }
                .CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(Play);

            TrimCommand = new[]
                {
                    _isOpened,
                    _isTrimming
                        .Select(M => !M)
                }
                .CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(Trim);
        }

        TimeSpan _from, _to, _end;
        
        public TimeSpan From
        {
            get => _from;
            set
            {
                _from = value;

                if (IsPlaying && value + TimeSpan.FromSeconds(1) >= _player.Position)
                {
                    Stop();
                }

                if (!IsPlaying)
                {
                    PlaybackPosition = value;
                }

                OnPropertyChanged();
            }
        }

        void Stop()
        {
            if (!_isPlaying)
                return;

            _player.Stop();

            IsPlaying = false;

            PlaybackPosition = From;
        }

        void Play()
        {
            if (IsPlaying)
                Stop();
            else
            {
                _player.Position = From;

                _player.Play();

                IsPlaying = true;
            }
        }

        public TimeSpan To
        {
            get => _to;
            set
            {
                _to = value;

                if (IsPlaying && _player.Position + TimeSpan.FromSeconds(1) >= value)
                {
                    Stop();
                }

                if (!IsPlaying)
                {
                    PlaybackPosition = value;
                }

                OnPropertyChanged();
            }
        }

        public TimeSpan End
        {
            get => _end;
            set => Set(ref _end, value);
        }

        string _fileName;

        public string FileName
        {
            get => _fileName;
            private set => Set(ref _fileName, value);
        }

        string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;

                FileName = Path.GetFileNameWithoutExtension(value);

                OnPropertyChanged();
            }
        }

        public ICommand OpenCommand { get; }

        public ICommand PlayCommand { get; }

        public ICommand TrimCommand { get; }

        void Open()
        {
            var ofd = new OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true
            };

            if (ofd.ShowDialog().GetValueOrDefault())
            {
                Open(ofd.FileName);
            }
        }

        public void Open(string Path)
        {
            _isOpened.Value = false;

            _player.Source = new Uri(Path);

            var oldVol = _player.Volume;

            // Force Load
            _player.Play();
            _player.Stop();

            _player.Volume = oldVol;

            FilePath = Path;
        }

        bool _isPlaying;

        public bool IsPlaying
        {
            get => _isPlaying;
            set => Set(ref _isPlaying, value);
        }

        public TimeSpan PlaybackPosition
        {
            get => _player?.Position ?? TimeSpan.Zero;
            set => _player.Position = value;
        }

        public void Dispose()
        {
            _player.Close();
            _player.Source = null;
        }

        async void Trim()
        {
            if (!FFmpegService.FFmpegExists)
            {
                ServiceProvider.Get<IFFmpegViewsProvider>().ShowUnavailable();

                return;
            }

            var ext = Path.GetExtension(FilePath);

            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ext,
                Filter = $"*{ext}|*{ext}",
                FileName = Path.GetFileName(FilePath),
                InitialDirectory = Path.GetDirectoryName(FilePath),
                CheckPathExists = true
            };

            if (!sfd.ShowDialog().GetValueOrDefault())
                return;

            var hasAudio = _player.HasAudio;

            _player.Close();

            _isTrimming.Value = true;

            var trimmer = new FFmpegTrimmer();

            try
            {
                await trimmer.Run(FilePath,
                    From,
                    To,
                    sfd.FileName,
                    hasAudio);
            }
            finally
            {
                _isTrimming.Value = false;
            }

            MessageBox.Show("Done");
        }
    }
}