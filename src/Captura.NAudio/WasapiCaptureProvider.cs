using NAudio.CoreAudioApi;

namespace Captura.Audio
{
    class WasapiCaptureProvider : NAudioProvider
    {
        public WasapiCaptureProvider(MMDevice Device)
            : base(new WasapiCapture(Device)) { }
    }
}