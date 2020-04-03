using SharpDX.MediaFoundation;

namespace Captura.Windows.MediaFoundation
{
    public static class MfManager
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