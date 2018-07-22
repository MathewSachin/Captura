using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    static class MfManager
    {
        const int MfSdkVersion = 0x2;
        const int MfApiVersion = 0x0070;

        public static void Startup()
        {
            MediaFactory.Startup(MfSdkVersion << 16 | MfApiVersion, 0);
        }

        public static void Shutdown()
        {
            MediaFactory.Shutdown();
        }
    }
}