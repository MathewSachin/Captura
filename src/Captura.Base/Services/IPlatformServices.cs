using System;
using System.Collections.Generic;
using System.Drawing;
using Captura.Video;

namespace Captura
{
    public interface IPlatformServices
    {
        IEnumerable<IScreen> EnumerateScreens();

        IEnumerable<IWindow> EnumerateWindows();

        IEnumerable<IWindow> EnumerateAllWindows();

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

        IImageProvider GetWindowProvider(IWindow Window, bool IncludeCursor);

        IImageProvider GetScreenProvider(IScreen Screen, bool IncludeCursor, bool StepsMode);

        IImageProvider GetAllScreensProvider(bool IncludeCursor, bool StepsMode);
    }
}