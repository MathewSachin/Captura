using Screna.Audio;

namespace Captura.Models
{
    public class NoAudioSource : AudioSource
    {
        public static NoAudioSource Instance { get; } = new NoAudioSource();

        NoAudioSource() { }

        public override IAudioProvider GetAudioProvider() => null;

        protected override void OnRefresh() { }
    }
}