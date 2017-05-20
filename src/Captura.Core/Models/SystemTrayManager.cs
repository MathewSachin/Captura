using Captura.Models;
using System;
using System.Windows.Forms;

namespace Captura
{
    public static class SystemTrayManager
    {
        public static ISystemTray SystemTray { get; } = ServiceProvider.Get<ISystemTray>(ServiceName.SystemTray);
    }
}
