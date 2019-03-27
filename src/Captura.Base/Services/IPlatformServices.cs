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

        Rectangle DesktopRectangle { get; }

        bool DeleteFile(string FilePath);

        Point CursorPosition { get; }

        IBitmapImage CaptureTransparent(IWindow Window, bool IncludeCursor = false);

        IBitmapImage Capture(Rectangle Region, bool IncludeCursor = false);

        IImageProvider GetRegionProvider(Rectangle Region,
            bool IncludeCursor,
            Func<Point> LocationFunction = null);

        IImageProvider GetWindowProvider(IWindow Window, bool IncludeCursor, out Func<Point, Point> TransformerFunction);
    }
}