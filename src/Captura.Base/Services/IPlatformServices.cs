using System;
using System.Collections.Generic;
using System.Drawing;
using Captura.Models;

namespace Captura
{
    public interface IPlatformServices
    {
        IEnumerable<IScreen> EnumerateScreens();

        IEnumerable<IWindow> EnumerateWindows();

        IWindow GetWindow(IntPtr Handle);

        IWindow DesktopWindow { get; }

        IWindow ForegroundWindow { get; }

        bool DeleteFile(string FilePath);

        Point CursorPosition { get; }
    }
}