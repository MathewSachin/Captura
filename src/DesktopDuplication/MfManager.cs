using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    static class MfManager
    {
        public static void Startup()
        {
            MediaManager.Startup();
        }

        public static void Shutdown()
        {
            MediaFactory.Shutdown();
        }
    }
}