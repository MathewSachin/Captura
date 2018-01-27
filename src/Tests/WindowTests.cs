using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Screna;

namespace Captura.Tests
{
    [TestClass]
    public class WindowTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ZeroPointerWindow()
        {
            var win = new Window(IntPtr.Zero);

            Assert.AreNotEqual(win.Handle, IntPtr.Zero);
        }

        [TestMethod]
        public void DesktopWindowNotZero()
        {
            Assert.AreNotEqual(Window.DesktopWindow, IntPtr.Zero);
        }
    }
}
