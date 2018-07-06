using Captura.Audio;

namespace Captura.Models
{
    /// <summary>
    /// Used when no audio sources are available.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NoAudioSource : AudioSource
    {
        public override IAudioProvider GetAudioProvider(int FrameRate) => null;

        protected override void OnRefresh() { }
    }
}