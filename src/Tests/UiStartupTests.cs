using System;
using Xunit;

namespace Captura.Tests.Views
{
    [Collection(nameof(Tests))]
    public class UiStartupTests : IClassFixture<AppRunnerFixture>
    {
        readonly AppRunnerFixture _appRunner;

        public UiStartupTests(AppRunnerFixture appRunner)
        {
            _appRunner = appRunner;
        }

        [Fact]
        public void CapturaMainWindowStarts()
        {
            Assert.False(_appRunner.App.HasExited);
            Assert.NotEqual(IntPtr.Zero, _appRunner.MainWindowHandle);
        }
    }
}
