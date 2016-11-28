using System;
using Captura.Properties;
using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public partial class App
    {
        void Application_Startup(object sender, StartupEventArgs e)
        {
            #region Color
            var color = DWMApi.ColorizationColor;

            Current.Resources["AccentColor"] = color;
            Current.Resources["Accent"] = new SolidColorBrush(color);
            #endregion

#if !DEBUG
            Current.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(args.Exception.Message);

                if (args.Exception.InnerException != null)
                    MessageBox.Show(args.Exception.InnerException.Message);

                args.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var E = args.ExceptionObject as Exception;

                MessageBox.Show(E.Message);

                if (E.InnerException != null)
                    MessageBox.Show(E.InnerException.Message);

                if (!args.IsTerminating)
                    return;

                MessageBox.Show("App will terminate");
                Current.Shutdown();
            };
#endif
        }

        void Application_Exit(object sender, ExitEventArgs e) => Settings.Default.Save();
    }
}