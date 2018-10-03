using System.Media;

namespace Captura.Models
{
    public class AudioPlayer : IAudioPlayer
    {
        public void PlayBegin()
        {
            SystemSounds.Beep.Play();
        }

        public void PlayEnd()
        {
            SystemSounds.Beep.Play();
        }

        public void PlayPause()
        {
            SystemSounds.Beep.Play();
        }

        public void PlayShot()
        {
            SystemSounds.Beep.Play();
        }

        public void PlayError()
        {
            SystemSounds.Asterisk.Play();
        }
    }
}