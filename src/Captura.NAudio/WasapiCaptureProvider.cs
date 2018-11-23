using NAudio.CoreAudioApi;

namespace Captura.NAudio
{
    class WasapiCaptureProvider : NAudioProvider
    {
        public WasapiCaptureProvider(MMDevice Device)
            : base(new WasapiCapture(Device)) { }
    }
}