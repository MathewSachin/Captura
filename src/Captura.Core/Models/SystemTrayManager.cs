using Captura.Models;

namespace Captura
{
    public static class SystemTrayManager
    {
        public static ISystemTray SystemTray { get; } = ServiceProvider.Get<ISystemTray>(ServiceName.SystemTray);
    }
}
