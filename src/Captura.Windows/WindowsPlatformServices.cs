using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Captura.Native;
using Captura.Video;
using Captura.Windows.DesktopDuplication;
using SharpDX.DXGI;

namespace Captura.Windows
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class WindowsPlatformServices : IPlatformServices
    {
        readonly IPreviewWindow _previewWindow;

        public WindowsPlatformServices(IPreviewWindow PreviewWindow)
        {
            _previewWindow = PreviewWindow;
        }

        public IEnumerable<IScreen> EnumerateScreens()
        {
            return ScreenWrapper.Enumerate();
        }

        public IEnumerable<IWindow> EnumerateWindows()
        {
            return Window.EnumerateVisible();
        }

        public IEnumerable<IWindow> EnumerateAllWindows()
        {
            return Window
                .Enumerate()
                .Where(M => M.IsVisible)
                .SelectMany(GetAllChildren);
        }

        IEnumerable<Window> GetAllChildren(Window Window)
        {
            var children = Window
                .EnumerateChildren()
                .Where(M => M.IsVisible);

            foreach (var child in children)
            {
                foreach (var grandchild in GetAllChildren(child))
                {
                    yield return grandchild;
                }
            }

            yield return Window;
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
            return new RegionProvider(Region, _previewWindow, IncludeCursor, LocationFunction);
        }

        public IImageProvider GetWindowProvider(IWindow Window, bool IncludeCursor)
        {
            return new WindowProvider(Window, _previewWindow, IncludeCursor);
        }

        public IImageProvider GetScreenProvider(IScreen Screen, bool IncludeCursor, bool StepsMode)
        {
            if (!WindowsModule.ShouldUseGdi && !StepsMode)
            {
                var output = FindOutput(Screen);

                if (output != null)
                {
                    return new DeskDuplImageProvider(output, IncludeCursor, _previewWindow);
                }
            }

            return GetRegionProvider(Screen.Rectangle, IncludeCursor);
        }

        static Output1 FindOutput(IScreen Screen)
        {
            var outputs = new Factory1()
                .Adapters1
                .SelectMany(M => M.Outputs);

            var match = outputs.FirstOrDefault(M =>
            {
                var r1 = M.Description.DesktopBounds;
                var r2 = Screen.Rectangle;

                return r1.Left == r2.Left
                       && r1.Right == r2.Right
                       && r1.Top == r2.Top
                       && r1.Bottom == r2.Bottom;
            });

            return match?.QueryInterface<Output1>();
        }

        public IImageProvider GetAllScreensProvider(bool IncludeCursor, bool StepsMode)
        {
            if (!WindowsModule.ShouldUseGdi && !StepsMode)
            {
                return new DeskDuplFullScreenImageProvider(IncludeCursor, _previewWindow, this);
            }

            return GetRegionProvider(DesktopRectangle, IncludeCursor);
        }
    }
}