using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Captura.Audio
{
    class NAudioDefaultItem : NAudioItem
    {
        public override MMDevice Device
        {
            get
            {
                if (IsLoopback)
                {
                    return WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice();
                }

                return WasapiCapture.GetDefaultCaptureDevice();
            }
        }

        NAudioDefaultItem(bool IsLoopback) : base("Default", IsLoopback) { }

        public static NAudioItem DefaultMicrophone = new NAudioDefaultItem(false);

        public static NAudioItem DefaultSpeaker = new NAudioDefaultItem(true);
    }
}