using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Diagnostics;
using TestStack.White.UIItems.WindowItems;
using System.Threading;
using System.IO;

namespace Captura.Tests.Views
{
    [TestClass]
    public class ScreenShotTests
    {
        static Application _app;
        static Window _mainWindow;
        
        [ClassInitialize]
        public static void Init(TestContext Context)
        {
            _app = Application.Launch(new ProcessStartInfo(TestManager.GetUiPath(), "--no-persist"));

            _mainWindow = _app.GetWindow("Captura");
        }

        [ClassCleanup]
        public static void Clean()
        {
            _mainWindow.Close();

            _app.Close();
        }

        static void Shot(string FileName, IntPtr Window)
        {
            Thread.Sleep(500);

            var startInfo = new ProcessStartInfo
            {
                FileName = TestManager.GetCliPath(),
                Arguments = $"shot --source win:{Window} -f {FileName}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process?.WaitForExit();

            if (process == null || process.ExitCode != 0)
                Assert.Fail($"Error occurred when taking ScreenShot, hWnd: {Window}, FileName: {FileName}, ExitCode: {process?.ExitCode}");

            Assert.IsTrue(File.Exists(FileName), $"ScreenShot was not saved: {FileName}");
        }

        /// <summary>
        /// Take ScreenShot of all Tabs.
        /// </summary>
        [TestMethod]
        [Timeout(25000)]
        public void ScreenShotTabs()
        {
            Directory.CreateDirectory("Tabs");

            var loc = ServiceProvider.Get<LanguageManager>();

            var tabs = new[]
            {
                loc.Main,
                loc.Configure, loc.Hotkeys, "Overlays", "FFmpeg", loc.Proxy, loc.Extras,
                loc.Recent,
                loc.About
            };

            foreach (var tabName in tabs)
            {
                var tab = _mainWindow.Get<TabPage>(SearchCriteria.ByText(tabName));

                tab.Select();

                Shot($"Tabs/{tabName}.png", _app.Process.MainWindowHandle);
            }
        }
    }
}
