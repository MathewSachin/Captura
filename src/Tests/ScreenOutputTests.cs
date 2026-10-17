using System;
using System.Drawing;
using System.Linq;
using Captura.Video;
using Moq;
using SharpDX.DXGI;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class ScreenOutputTests
    {
        [Fact]
        public void ScreenWithDpiScaledBoundsStillCapturesItsWholeMonitor()
        {
            OutputDescription description;

            using (var factory = new Factory1())
            {
                var adapters = factory.Adapters1;
                var outputs = adapters.SelectMany(A => A.Outputs).ToArray();

                Assert.NotEmpty(outputs);

                description = outputs[0].Description;

                foreach (var comObject in outputs.Concat<IDisposable>(adapters))
                {
                    comObject.Dispose();
                }
            }

            var bounds = description.DesktopBounds;
            var width = bounds.Right - bounds.Left;
            var height = bounds.Bottom - bounds.Top;

            // WinForms bounds of a 100% monitor seen from a process at 125% system DPI
            var screen = new Mock<IScreen>();
            screen.Setup(M => M.DeviceName).Returns(description.DeviceName);
            screen.Setup(M => M.Rectangle).Returns(new Rectangle(bounds.Left, bounds.Top, (int)(width * 1.25), (int)(height * 1.25)));

            using var provider = ServiceProvider.Get<IPlatformServices>().GetScreenProvider(screen.Object, false, false);

            Assert.Equal(width, provider.Width);
            Assert.Equal(height, provider.Height);
        }
    }
}
