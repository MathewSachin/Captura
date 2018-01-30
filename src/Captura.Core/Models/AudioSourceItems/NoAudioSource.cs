using Screna.Audio;

namespace Captura.Models
{
    public class NoAudioSource : AudioSource
    {
        public override IAudioProvider GetAudioProvider() => null;

        protected override void OnRefresh() { }
    }
}