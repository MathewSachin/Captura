using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Captura
{
    public class AudioTrimmerViewModel : NotifyPropertyChanged
    {
        readonly MediaPlayer _player;
        readonly DispatcherTimer _timer;

        public AudioTrimmerViewModel()
        {
            _player = new MediaPlayer();

            _player.MediaOpened += (S, E) =>
            {
                From = TimeSpan.Zero;

                if (_player.NaturalDuration.HasTimeSpan)
                {
                    To = End = TimeSpan.FromSeconds((int) _player.NaturalDuration.TimeSpan.TotalSeconds);
                }
                else To = End = TimeSpan.Zero;
            };

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            _timer.Tick += (Sender, Args) =>
            {
                RaisePropertyChanged(nameof(PlaybackPosition));

                if (IsPlaying && _player.Position >= To)
                {
                    Stop();
                }
            };

            _timer.Start();

            OpenCommand = new DelegateCommand(Open);

            PlayCommand = new DelegateCommand(Play, false);
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

                OnPropertyChanged();
            }
        }

        void Stop()
        {
            if (!_isPlaying)
                return;

            _player.Stop();

            IsPlaying = false;
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
                
                OnPropertyChanged();
            }
        }

        public TimeSpan End
        {
            get => _end;
            set
            {
                _end = value;
                
                OnPropertyChanged();
            }
        }

        string _fileName;

        public string FileName
        {
            get => _fileName;
            private set
            {
                _fileName = value;
                
                OnPropertyChanged();
            }
        }

        string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;

                FileName = Path.GetFileName(value);

                OnPropertyChanged();
            }
        }

        public ICommand OpenCommand { get; }

        public DelegateCommand PlayCommand { get; }

        void Open()
        {
            var ofd = new OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true
            };

            if (ofd.ShowDialog().GetValueOrDefault())
            {
                _player.Open(new Uri(ofd.FileName));
                
                FilePath = ofd.FileName;

                PlayCommand.RaiseCanExecuteChanged(true);
            }
        }

        bool _isPlaying;

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                
                OnPropertyChanged();
            }
        }

        public TimeSpan PlaybackPosition => TimeSpan.FromSeconds((int) _player.Position.TotalSeconds);
    }
}