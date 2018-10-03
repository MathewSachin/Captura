namespace Captura.Models
{
    public interface IAudioPlayer
    {
        void PlayBegin();

        void PlayEnd();

        void PlayPause();

        void PlayShot();

        void PlayError();
    }
}