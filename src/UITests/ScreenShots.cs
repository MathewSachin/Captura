using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using Captura.Properties;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Diagnostics;
using TestStack.White.UIItems.WindowItems;
using System.Threading;
using System.IO;
using System.Linq;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems;

namespace UITests
{
    [TestClass]
    public class ScreenShots
    {
        static Application App;
        static Window MainWindow;
        static TabPage MainTab;

        [ClassInitialize]
        public static void Init(TestContext Context)
        {
            App = Application.Launch("captura.ui.exe");

            MainWindow = App.GetWindow("Captura");

            MainTab = MainWindow.Get<TabPage>(SearchCriteria.ByText(Resources.Main));
        }

        [ClassCleanup]
        public static void Clean()
        {
            MainWindow.Close();
        }

        static void Shot(string FileName, IntPtr hWnd)
        {
            Thread.Sleep(500);

            var startInfo = new ProcessStartInfo
            {
                FileName = "captura",
                Arguments = $"shot --source win:{hWnd} -f {FileName}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();

            if (process.ExitCode != 0)
                Assert.Fail($"Error occured when taking ScreenShot, hWnd: {hWnd}, FileName: {FileName}, ExitCode: {process.ExitCode}");
        }
        
        static void VideoSourceKind(string Kind)
        {
            MainTab.Select();

            // TODO: Explicit Index does not seem good
            var videoSourceKind = MainTab.Get<ComboBox>(SearchCriteria.Indexed(0));
            
            videoSourceKind.Select(Kind);
        }

        /// <summary>
        /// Take ScreenShot of all Tabs.
        /// </summary>
        [TestMethod]
        public void ScreenShotTabs()
        {
            Directory.CreateDirectory("Tabs");

            VideoSourceKind(Resources.Window);

            foreach (var tabName in new[] { Resources.Main, Resources.Configure, Resources.Recent, Resources.Hotkeys, Resources.About })
            {
                var tab = MainWindow.Get<TabPage>(SearchCriteria.ByText(tabName));

                tab.Select();

                Shot($"Tabs/{tabName}.png", App.Process.MainWindowHandle);
            }
        }

        [TestMethod]
        public void ScreenShotWebCamView()
        {
            MainTab.Select();

            // TODO: Explicit Index does not seem good
            var webCamView = MainTab.Get<CheckBox>(SearchCriteria.Indexed(3));

            webCamView.Toggle();

            Thread.Sleep(500);

            var handle = Screna.Window.EnumerateVisible().FirstOrDefault(win => win.Title == Resources.WebCamView)?.Handle ?? IntPtr.Zero;
            
            Shot("WebCam.png", handle);

            webCamView.Toggle();
        }

        [TestMethod]
        public void ScreenShotRegionSelector()
        {
            VideoSourceKind(Resources.Region);
            
            Thread.Sleep(500);

            var handle = Screna.Window.EnumerateVisible().FirstOrDefault(win => win.Title == Resources.RegionSelector)?.Handle ?? IntPtr.Zero;

            Shot("RegionSelector.png", handle);

            VideoSourceKind(Resources.Window);
        }
    }
}
