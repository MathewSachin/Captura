using System;
using System.Collections.Generic;
using Captura.Models;
using Screna;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WindowsPlatformServices : IPlatformServices
    {
        public IEnumerable<IScreen> EnumerateScreens()
        {
            return ScreenWrapper.Enumerate();
        }

        public IEnumerable<IWindow> EnumerateWindows()
        {
            return Window.EnumerateVisible();
        }

        public IWindow GetWindow(IntPtr Handle)
        {
            return new Window(Handle);
        }

        public IWindow DesktopWindow => Window.DesktopWindow;
        public IWindow ForegroundWindow => Window.ForegroundWindow;

        public bool DeleteFile(string FilePath)
        {
            return Shell32.FileOperation(FilePath, FileOperationType.Delete, 0) == 0;
        }
    }
}