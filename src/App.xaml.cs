using System;
using Captura.Properties;
using System.Windows;
using System.Threading;
using System.Globalization;
using Screna.Audio;

namespace Captura
{
    public partial class App
    {
        public static MainViewModel MainViewModel { get; private set; }

        void InitBass() => MixedAudioProvider.Init();

        void Application_Startup(object sender, StartupEventArgs e)
        {
            if (AudioViewModel.BassExists())
                InitBass();

            MainViewModel = FindResource(nameof(MainViewModel)) as MainViewModel;

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
            
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);
        }

        void Application_Exit(object sender, ExitEventArgs e) => Settings.Default.Save();
    }
}