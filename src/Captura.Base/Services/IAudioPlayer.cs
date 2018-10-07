namespace Captura.Models
{
    public interface IAudioPlayer
    {
        void PlayNormal();

        void PlayShot();

        void PlayError();
    }
}