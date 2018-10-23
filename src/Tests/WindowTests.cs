using System;
using Screna;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class WindowTests
    {
        [Fact]
        public void ZeroPointerWindow()
        {
            Assert.Throws<ArgumentException>(() => new Window(IntPtr.Zero));
        }

        [Fact]
        public void DesktopWindowNotZero()
        {
            Assert.NotEqual(IntPtr.Zero, Window.DesktopWindow.Handle);
        }
    }
}
