using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Diagnostics;
using TestStack.White.UIItems.WindowItems;
using System.Threading;
using System.IO;
using Captura;

namespace UITests
{
    [TestClass]
    public class ScreenShots
    {
        static Application _app;
        static Window _mainWindow;
        static TabPage _mainTab;

        [ClassInitialize]
        public static void Init(TestContext Context)
        {
            _app = Application.Launch("captura.ui.exe");

            _mainWindow = _app.GetWindow("Captura");

            _mainTab = _mainWindow.Get<TabPage>(SearchCriteria.ByText(LanguageManager.Instance.Main));
        }

        [ClassCleanup]
        public static void Clean()
        {
            _mainWindow.Close();

            _app.Close();
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
            process?.WaitForExit();

            if (process == null || process.ExitCode != 0)
                Assert.Fail($"Error occured when taking ScreenShot, hWnd: {hWnd}, FileName: {FileName}, ExitCode: {process.ExitCode}");
        }

        /// <summary>
        /// Take ScreenShot of all Tabs.
        /// </summary>
        [TestMethod]
        public void ScreenShotTabs()
        {
            Directory.CreateDirectory("Tabs");

            var tabs = new[]
            {
                LanguageManager.Instance.Main,
                LanguageManager.Instance.Configure, LanguageManager.Instance.Hotkeys, "Overlays", "FFMpeg", LanguageManager.Instance.Proxy, LanguageManager.Instance.Extras,
                LanguageManager.Instance.Recent,
                LanguageManager.Instance.About
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
