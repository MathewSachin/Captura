using Captura.ViewModels;
using System;
using System.Diagnostics;
using System.Windows.Media;

namespace Captura
{
    public class AudioEditorViewModel : ViewModelBase
    {
        TimeSpan _begin, _end, _current, _duration;
        string _fileName;

        MediaPlayer _player;

        public TimeSpan Begin
        {
            get { return _begin; }
            set
            {
                _begin = value;

                OnPropertyChanged();
            }
        }

        public TimeSpan End
        {
            get { return _end; }
            set
            {
                _end = value;

                OnPropertyChanged();
            }
        }

        public TimeSpan Current
        {
            get { return _current; }
            set
            {
                _current = value;

                if (_current > End || _current < Begin)
                    _current = Begin;

                _player.Position = _current;

                OnPropertyChanged();
            }
        }
        
        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;

                OnPropertyChanged();
            }
        }

        public void Load(string FileName)
        {
            _fileName = FileName;
            _player = new MediaPlayer();
            
            _player.Open(new Uri(FileName));

            Begin = Current = TimeSpan.Zero;
            End = Duration = _player.NaturalDuration.TimeSpan;            
        }

        public void Save()
        {
            Process.Start(FFMpegService.FFMpegExePath, $"-y -ss {_begin.TotalSeconds} -i {_fileName} -to {_end.TotalSeconds} -acodec copy {_fileName}");
        }

        public void Reset()
        {
            _player.Stop();

            _player.Position = _begin = TimeSpan.Zero;

            _end = _duration;
        }
    }

    public partial class AudioEditor
    {
        public AudioEditor()
        {
            InitializeComponent();
        }        
    }
}
