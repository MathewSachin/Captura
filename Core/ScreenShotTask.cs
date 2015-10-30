using System.Drawing.Imaging;
using System;
using ManagedWin32;

namespace Captura
{
    struct ScreenshotTask
    {
        public bool CaptureMouse;
        public int CheckerboardSize;
        public bool ClipboardNotDisk;
        public string FileName;
        public ImageFormat ImageFormat;
        public bool DoResize;
        public int ResizeX;
        public int ResizeY;
        public IntPtr WindowHandle;
        public WindowHandler hWindow;

        public ScreenshotTask(MainWindow MainWindow, string FileName, ImageFormat ImageFormat)
        {
            WindowHandle = MainWindow.SelectedWindow;
            ClipboardNotDisk = MainWindow.SaveToClipboard.IsChecked.Value;
            DoResize = MainWindow.DoResize.IsChecked.Value;
            ResizeX = (int)MainWindow.ResizeWidth.Value;
            ResizeY = (int)MainWindow.ResizeHeight.Value;
            CheckerboardSize = 0;
            CaptureMouse = MainWindow.IncludeCursor.IsChecked.Value;
            hWindow = new WindowHandler(WindowHandle);

            this.FileName = FileName;
            this.ImageFormat = ImageFormat;
        }
    }
}