using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    public static class MfManager
    {
        public static void Startup()
        {
            MediaFactory.Startup(MediaFactory.Version, 0);
        }

        public static void Shutdown()
        {
            MediaFactory.Shutdown();
        }
    }
}