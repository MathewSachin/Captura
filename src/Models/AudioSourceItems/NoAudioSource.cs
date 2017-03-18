using Screna.Audio;

namespace Captura
{
    public class NoAudioSource : AudioSource
    {
        public static NoAudioSource Instance { get; } = new NoAudioSource();

        NoAudioSource() { }

        public override IAudioProvider GetAudioSource() => null;

        protected override void OnRefresh() { }
    }
}