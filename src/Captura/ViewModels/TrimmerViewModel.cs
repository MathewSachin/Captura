using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Captura
{
    public class TrimmerViewModel : NotifyPropertyChanged, IDisposable
    {
        MediaElement _player;
        readonly DispatcherTimer _timer;

        public bool IsDragging { get; set; }

        public void AssignPlayer(MediaElement Player)
        {
            _player = Player;

            _player.MediaOpened += (S, E) =>
            {
                From = TimeSpan.Zero;

                if (_player.NaturalDuration.HasTimeSpan)
                {
                    To = End = TimeSpan.FromSeconds((int)_player.NaturalDuration.TimeSpan.TotalSeconds);
                }
                else To = End = TimeSpan.Zero;

                PlayCommand.RaiseCanExecuteChanged(true);
            };

            _timer.Start();
        }

        public TrimmerViewModel()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
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
                PlayCommand.RaiseCanExecuteChanged(false);

                Open(ofd.FileName);
            }
        }

        public void Open(string Path)
        {
            PlayCommand.RaiseCanExecuteChanged(false);

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
            set
            {
                _isPlaying = value;
                
                OnPropertyChanged();
            }
        }

        public TimeSpan PlaybackPosition
        {
            get => TimeSpan.FromSeconds((int) (_player?.Position.TotalSeconds ?? 0));
            set => _player.Position = value;
        }

        public void Dispose()
        {
            _player.Close();
            _player.Source = null;
        }
    }
}