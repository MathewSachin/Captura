using Captura.Properties;
using System.Windows;
using System.Threading;
using System.Globalization;

namespace Captura
{
    public partial class App
    {
        void Application_Startup(object sender, StartupEventArgs e)
        {
            // Localization
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);
        }

        // Save Settings
        void Application_Exit(object sender, ExitEventArgs e) => Settings.Default.Save();
    }
}