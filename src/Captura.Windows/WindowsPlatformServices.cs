using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Captura.Models;
using Screna;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class WindowsPlatformServices : IPlatformServices
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

        public Rectangle DesktopRectangle => SystemInformation.VirtualScreen;

        public bool DeleteFile(string FilePath)
        {
            return Shell32.FileOperation(FilePath, FileOperationType.Delete, 0) == 0;
        }

        public Point CursorPosition
        {
            get
            {
                var p = new Point();
                User32.GetCursorPos(ref p);
                return p;
            }
        }

        public IBitmapImage CaptureTransparent(IWindow Window, bool IncludeCursor = false)
        {
            return ScreenShot.CaptureTransparent(Window, IncludeCursor, this);
        }

        public IBitmapImage Capture(Rectangle Region, bool IncludeCursor = false)
        {
            return ScreenShot.Capture(Region, IncludeCursor);
        }

        public IImageProvider GetRegionProvider(Rectangle Region, bool IncludeCursor, Func<Point> LocationFunction = null)
        {
            return new RegionProvider(Region, IncludeCursor, LocationFunction);
        }

        public IImageProvider GetWindowProvider(IWindow Window, bool IncludeCursor)
        {
            return new WindowProvider(Window, IncludeCursor);
        }
    }
}