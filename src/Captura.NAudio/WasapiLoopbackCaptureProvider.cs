using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Captura.Audio
{
    class WasapiLoopbackCaptureProvider : NAudioProvider
    {
        readonly IWavePlayer _wasapiOut;

        public WasapiLoopbackCaptureProvider(MMDevice Device)
            : base(new WasapiLoopbackCapture(Device))
        {
            _wasapiOut = new WasapiOut(Device, AudioClientShareMode.Shared, true, 50);
            
            // Mix Format should be used in Shared mode
            using var audioClient = Device.AudioClient;
            _wasapiOut.Init(new SilenceProvider(audioClient.MixFormat));
        }

        public override void Start()
        {
            _wasapiOut.Play();

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();

            _wasapiOut.Pause();
        }

        public override void Dispose()
        {
            base.Dispose();

            _wasapiOut.Dispose();
        }
    }
}