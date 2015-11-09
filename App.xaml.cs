using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Captura.Properties;
using SharpAvi.Codecs;

namespace Captura
{
    public partial class App : Application
    {
        public static bool IsLamePresent { get; private set; }

        void Application_Startup(object sender, StartupEventArgs e)
        {
            string LamePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                string.Format("lameenc{0}.dll", Environment.Is64BitProcess ? "64" : "32"));

            if (!File.Exists(LamePath)) IsLamePresent = false;
            else
            {
                Mp3AudioEncoderLame.SetLameDllLocation(LamePath);
                IsLamePresent = true;
            }
        }

        void Application_Exit(object sender, ExitEventArgs e) { Settings.Default.Save(); }
    }
}