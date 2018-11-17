using NAudio.CoreAudioApi;

namespace Captura.NAudio
{
    public class WasapiCaptureProvider : NAudioProvider
    {
        public WasapiCaptureProvider(MMDevice Device)
            : base(new WasapiCapture(Device)) { }
    }
}