using System;
using System.Drawing.Imaging;

namespace Captura
{
    struct ScreenshotTask
    {
        public bool CaptureMouse, ClipboardNotDisk, DoResize;
        public string FileName;
        public ImageFormat ImageFormat;
        public int ResizeX, ResizeY;
        public IntPtr WindowHandle;

        public ScreenshotTask(MainWindow MainWindow, string FileName, ImageFormat ImageFormat)
        {
            WindowHandle = MainWindow.SelectedWindow;
            ClipboardNotDisk = MainWindow.SaveToClipboard.IsChecked.Value;
            DoResize = MainWindow.DoResize.IsChecked.Value;
            ResizeX = (int)MainWindow.ResizeWidth.Value;
            ResizeY = (int)MainWindow.ResizeHeight.Value;
            CaptureMouse = MainWindow.IncludeCursor.IsChecked.Value;
            
            this.FileName = FileName;
            this.ImageFormat = ImageFormat;
        }
    }
}