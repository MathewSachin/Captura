using System;
using System.IO;
using System.Reflection;
using System.Windows;
using SharpAvi.Codecs;

namespace Captura
{
    public partial class App : Application
    {
        void Application_Startup(object sender, StartupEventArgs e)
        {
            // Set LAME DLL path for MP3 encoder
            Mp3AudioEncoderLame.SetLameDllLocation(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                string.Format("lameenc{0}.dll", Environment.Is64BitProcess ? "64" : "32")));
        }
    }
}