using System;
using System.IO;
using System.Windows.Media;

namespace Captura.Audio
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AudioPlayer : IAudioPlayer
    {
        readonly SoundSettings _settings;
        readonly MediaPlayer _mediaPlayer;

        public AudioPlayer(SoundSettings Settings)
        {
            _settings = Settings;
            _mediaPlayer = new MediaPlayer();
        }

        void PlaySound(string Path)
        {
            if (!File.Exists(Path))
                return;

            _mediaPlayer.Open(new Uri(Path));
            _mediaPlayer.Play();
        }

        public void Play(SoundKind SoundKind)
        {
            if (_settings.Items.TryGetValue(SoundKind, out var value))
                PlaySound(value);
        }
    }
}