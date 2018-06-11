using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;

namespace Captura
{
    public class TrimmerViewModel : NotifyPropertyChanged, IDisposable
    {
        MediaElement _player;
        Window _window;
        readonly DispatcherTimer _timer;

        public bool IsDragging { get; set; }

        public void AssignPlayer(MediaElement Player, Window Window)
        {
            _player = Player;
            _window = Window;

            _player.MediaOpened += (S, E) =>
            {
                From = TimeSpan.Zero;

                if (_player.NaturalDuration.HasTimeSpan)
                {
                    To = End = TimeSpan.FromSeconds((int)_player.NaturalDuration.TimeSpan.TotalSeconds);
                }
                else To = End = TimeSpan.Zero;

                PlayCommand.RaiseCanExecuteChanged(true);
                TrimCommand.RaiseCanExecuteChanged(true);
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

            TrimCommand = new DelegateCommand(Trim, false);
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

                FileName = Path.GetFileNameWithoutExtension(value);

                OnPropertyChanged();
            }
        }

        public DelegateCommand OpenCommand { get; }

        public DelegateCommand PlayCommand { get; }

        public DelegateCommand TrimCommand { get; }

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
            PlayCommand.RaiseCanExecuteChanged(false);
            TrimCommand.RaiseCanExecuteChanged(false);

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

        async void Trim()
        {
            if (!FFMpegService.FFMpegExists)
            {
                ModernDialog.ShowMessage("FFMpeg not Found", "FFMpeg not Found", MessageBoxButton.OK, _window);

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

            if (sfd.ShowDialog().GetValueOrDefault())
            {
                var command = $"-i \"{FilePath}\" -ss {From} -to {To} -c copy \"{sfd.FileName}\"";

                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = FFMpegService.FFMpegExePath,
                        Arguments = command,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };

                var output = "";

                process.ErrorDataReceived += (Sender, Args) => output += "\n" + Args.Data;

                OpenCommand.RaiseCanExecuteChanged(false);
                PlayCommand.RaiseCanExecuteChanged(false);
                TrimCommand.RaiseCanExecuteChanged(false);
                
                process.Start();

                process.BeginErrorReadLine();
                
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode != 0)
                {
                    ModernDialog.ShowMessage($"FFMpeg Output:\n{output}", "An Error Occured", MessageBoxButton.OK, _window);
                }

                OpenCommand.RaiseCanExecuteChanged(true);
                PlayCommand.RaiseCanExecuteChanged(true);
                TrimCommand.RaiseCanExecuteChanged(true);
            }
        }
    }
}