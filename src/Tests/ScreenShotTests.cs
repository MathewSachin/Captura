using System;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Xunit;

namespace Captura.Tests.Views
{
    [Collection(nameof(Tests))]
    public class ScreenShotTests : IClassFixture<AppRunnerFixture>
    {
        readonly AppRunnerFixture _appRunner;

        public ScreenShotTests(AppRunnerFixture AppRunner)
        {
            _appRunner = AppRunner;
        }

        static void Shot(string FileName, IntPtr Window)
        {
            Thread.Sleep(500);

            var startInfo = new ProcessStartInfo
            {
                FileName = TestManagerFixture.GetCliPath(),
                Arguments = $"shot --source win:{Window} -f {FileName}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process?.WaitForExit();

            Assert.False(process == null || process.ExitCode != 0,
                $"Error occurred when taking ScreenShot, hWnd: {Window}, FileName: {FileName}, ExitCode: {process?.ExitCode}");

            Assert.True(File.Exists(FileName), $"ScreenShot was not saved: {FileName}");
        }

        /// <summary>
        /// Take ScreenShot of all Tabs.
        /// </summary>
        [Fact]
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
                var tab = _appRunner.MainWindow.Get<TabPage>(SearchCriteria.ByText(tabName));

                tab.Select();

                Shot($"Tabs/{tabName}.png", _appRunner.App.Process.MainWindowHandle);
            }
        }
    }
}
