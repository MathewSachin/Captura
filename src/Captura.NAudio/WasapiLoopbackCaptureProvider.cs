using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Captura.NAudio
{
    public class WasapiLoopbackCaptureProvider : NAudioProvider
    {
        public WasapiLoopbackCaptureProvider(MMDevice Device)
            : base(new WasapiLoopbackCapture(Device)) { }
    }
}