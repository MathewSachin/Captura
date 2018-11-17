using NAudio.CoreAudioApi;
using NAudio.Wave;
using Wf = NAudio.Wave.WaveFormat;

namespace Captura.NAudio
{
    class WasapiLoopbackCaptureProvider : NAudioProvider
    {
        readonly IWavePlayer _wasapiOut;

        public WasapiLoopbackCaptureProvider(MMDevice Device)
            : base(new WasapiLoopbackCapture(Device))
        {
            _wasapiOut = new WasapiOut(Device, AudioClientShareMode.Shared, true, 200);

            _wasapiOut.Init(new SilenceProvider(Wf.CreateIeeeFloatWaveFormat(44100, 2)));
        }

        public override void Start()
        {
            _wasapiOut.Play();

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();

            _wasapiOut.Stop();
        }

        public override void Dispose()
        {
            base.Dispose();

            _wasapiOut.Dispose();
        }
    }
}