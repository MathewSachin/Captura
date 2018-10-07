using System;
using System.IO;
using System.Windows.Media;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AudioPlayer : IAudioPlayer
    {
        readonly SoundSettings _settings;
        MediaPlayer _mediaPlayer;

        public AudioPlayer(SoundSettings Settings)
        {
            _settings = Settings;
        }

        void PlaySound(string Path)
        {
            if (!File.Exists(Path))
                return;

            if (_mediaPlayer == null)
            {
                _mediaPlayer = new MediaPlayer();
            }

            _mediaPlayer.Open(new Uri(Path));
            _mediaPlayer.Play();
        }

        public void PlayNormal()
        {
            PlaySound(_settings.Normal);
        }

        public void PlayShot()
        {
            PlaySound(_settings.Shot);
        }

        public void PlayError()
        {
            PlaySound(_settings.Error);
        }
    }
}