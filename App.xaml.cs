//using Captura.Properties;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public partial class App
    {
        public static bool IsLamePresent { get; private set; }

        readonly string _dir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

        void Application_Startup(object sender, StartupEventArgs e)
        {
            #region Settings
            var color = DWMApi.ColorizationColor;

            Current.Resources["AccentColor"] = color;
            Current.Resources["Accent"] = new SolidColorBrush(color);

            //OtherSettings.MinimizeOnStart = Settings.Default.MinimizeOnStart;

            //OtherSettings.IncludeCursor = Settings.Default.IncludeCursor;
            #endregion
            
            IsLamePresent = File.Exists(Path.Combine(_dir, $"lameenc{(Environment.Is64BitProcess ? "64" : "32")}.dll"));

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

        void Application_Exit(object sender, ExitEventArgs e)
        {
            //Settings.Default.MinimizeOnStart = OtherSettings.MinimizeOnStart;

            //Settings.Default.IncludeCursor = OtherSettings.IncludeCursor;

            //Settings.Default.Save(); 
        }
    }
}