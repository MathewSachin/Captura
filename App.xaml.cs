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

        public static bool IsNAudioPresent { get; private set; }

        string Dir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

        void Application_Startup(object sender, StartupEventArgs e)
        {
            string LamePath = Path.Combine(Dir, string.Format("lameenc{0}.dll", Environment.Is64BitProcess ? "64" : "32"));

            if (!File.Exists(LamePath)) IsLamePresent = false;
            else
            {
                Mp3AudioEncoderLame.SetLameDllLocation(LamePath);
                IsLamePresent = true;
            }

            IsNAudioPresent = File.Exists(Path.Combine(Dir, "NAudio.dll"));
        }

        void Application_Exit(object sender, ExitEventArgs e) { Settings.Default.Save(); }
    }
}