using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using Captura.Properties;
using ManagedWin32;
using SharpAvi.Codecs;

namespace Captura
{
    public partial class App : Application
    {
        public static readonly int DesktopHeight, DesktopWidth;

        public static readonly IntPtr Desktop = WindowHandler.DesktopWindow.Handle;

        static App()
        {
            System.Windows.Media.Matrix toDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
                toDevice = source.CompositionTarget.TransformToDevice;

            DesktopHeight = (int)Math.Round(SystemParameters.PrimaryScreenHeight * toDevice.M22);
            DesktopWidth = (int)Math.Round(SystemParameters.PrimaryScreenWidth * toDevice.M11);
        }

        void Application_Startup(object sender, StartupEventArgs e)
        {
            // Set LAME DLL path for MP3 encoder
            Mp3AudioEncoderLame.SetLameDllLocation(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                string.Format("lameenc{0}.dll", Environment.Is64BitProcess ? "64" : "32")));
        }

        void Application_Exit(object sender, ExitEventArgs e) { Settings.Default.Save(); }
    }
}