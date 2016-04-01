using Captura.Properties;
using Screna.Audio;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Captura
{
    public partial class App
    {
        public static bool IsLamePresent { get; private set; }

        string Dir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

        void Application_Startup(object sender, StartupEventArgs e)
        {
            #region Settings
            AppearanceSettings.AccentColor = Settings.Default.MUIThemeColor;

            OtherSettings.MinimizeOnStart = Settings.Default.MinimizeOnStart;

            OtherSettings.IncludeCursor = Settings.Default.IncludeCursor;
            #endregion
            
            var LamePath = Path.Combine(Dir, $"lameenc{(Environment.Is64BitProcess ? "64" : "32")}.dll");

            if (!File.Exists(LamePath)) IsLamePresent = false;
            else
            {
                Mp3EncoderLame.Load(LamePath);
                IsLamePresent = true;
            }

#if !DEBUG
            App.Current.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(args.Exception.Message);
                if (args.Exception.InnerException != null) MessageBox.Show(args.Exception.InnerException.Message);
                args.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var E = args.ExceptionObject as Exception;

                MessageBox.Show(E.Message);
                if (E.InnerException != null) MessageBox.Show(E.InnerException.Message);
                if (args.IsTerminating)
                {
                    MessageBox.Show("App will terminate");
                    Application.Current.Shutdown();
                }
            };
#endif
        }

        void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.MUIThemeColor = AppearanceSettings.AccentColor;

            Settings.Default.MinimizeOnStart = OtherSettings.MinimizeOnStart;

            Settings.Default.IncludeCursor = OtherSettings.IncludeCursor;

            Settings.Default.Save(); 
        }
    }
}