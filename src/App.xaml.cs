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

        static void InitBass() => MixedAudioProvider.Init();

        void Application_Startup(object sender, StartupEventArgs e)
        {
            if (AudioViewModel.BassExists())
                InitBass();

            MainViewModel = FindResource(nameof(MainViewModel)) as MainViewModel;
            
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);
        }

        void Application_Exit(object sender, ExitEventArgs e) => Settings.Default.Save();
    }
}