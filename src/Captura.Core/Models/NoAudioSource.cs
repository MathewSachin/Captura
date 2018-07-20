using Captura.Audio;

namespace Captura.Models
{
    /// <summary>
    /// Used when no audio sources are available.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NoAudioSource : AudioSource
    {
        public override IAudioProvider GetMixedAudioProvider() => null;
        public override IAudioProvider[] GetMultipleAudioProviders() => new IAudioProvider[0];

        protected override void OnRefresh() { }
    }
}