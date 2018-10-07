using System.Media;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AudioPlayer : IAudioPlayer
    {
        public void PlayNormal()
        {
            SystemSounds.Beep.Play();
        }

        public void PlayShot()
        {
            SystemSounds.Beep.Play();
        }

        public void PlayError()
        {
            SystemSounds.Beep.Play();
        }
    }
}