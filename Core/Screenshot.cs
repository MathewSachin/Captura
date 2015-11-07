// Adopted from AeroShot
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ManagedWin32.Api;

namespace Captura
{
    class Screenshot
    {
        const int GWL_STYLE = -16;
        const long WS_SIZEBOX = 0x00040000L;
        const uint SWP_SHOWWINDOW = 0x0040;

        public bool CaptureMouse, ClipboardNotDisk, DoResize;
        public string FileName;
        public ImageFormat ImageFormat;
        public int ResizeX, ResizeY;
        public IntPtr WindowHandle;

        public void CaptureWindow(IntPtr hWnd, bool ToClipboard, bool IncludeCursor, string FileName, ImageFormat ImageFormat)
        {
            WindowHandle = hWnd;
            ClipboardNotDisk = ToClipboard;
            DoResize = false;
            ResizeX = ResizeY = 0;
            CaptureMouse = IncludeCursor;

            this.FileName = FileName;
            this.ImageFormat = ImageFormat;

            IntPtr start = User32.FindWindow("Button", "Start");
            IntPtr taskbar = User32.FindWindow("Shell_TrayWnd", null);

            try
            {
                // Hide the taskbar, just incase it gets in the way
                if (WindowHandle != start && WindowHandle != taskbar)
                {
                    User32.ShowWindow(start, 0);
                    User32.ShowWindow(taskbar, 0);
                    Application.DoEvents();
                }
                if (User32.IsIconic(WindowHandle))
                {
                    User32.ShowWindow(WindowHandle, (ShowWindowFlags)1);
                    Thread.Sleep(300); // Wait for window to be restored
                }
                else
                {
                    User32.ShowWindow(WindowHandle, (ShowWindowFlags)5);
                    Thread.Sleep(100);
                }
                User32.SetForegroundWindow(WindowHandle);

                var r = new RECT();
                if (DoResize)
                {
                    SmartResizeWindow(out r);
                    Thread.Sleep(100);
                }

                Bitmap s = CaptureCompositeScreenshot();

                // Show the taskbar again
                if (WindowHandle != start && WindowHandle != taskbar)
                {
                    User32.ShowWindow(start, (ShowWindowFlags)1);
                    User32.ShowWindow(taskbar, (ShowWindowFlags)1);
                }

                if (s == null)
                    MessageBox.Show("The screenshot taken was blank, it will not be saved.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                else
                {
                    if (ClipboardNotDisk) s.WriteToClipboard(true);
                    else s.Save(FileName, ImageFormat);
                    s.Dispose();
                }

                if (DoResize)
                {
                    if ((User32.GetWindowLong(WindowHandle, GWL_STYLE) & WS_SIZEBOX) == WS_SIZEBOX)
                    {
                        User32.SetWindowPos(WindowHandle,
                                                (IntPtr)0, r.Left, r.Top,
                                                r.Right - r.Left,
                                                r.Bottom - r.Top,
                                                SetWindowPositionFlags.ShowWindow);
                    }
                }
            }
            finally
            {
                if (WindowHandle != start && WindowHandle != taskbar)
                {
                    User32.ShowWindow(start, (ShowWindowFlags)1);
                    User32.ShowWindow(taskbar, (ShowWindowFlags)1);
                }
            }
        }

        void SmartResizeWindow(out RECT oldWindowSize)
        {
            oldWindowSize = new RECT();
            if ((User32.GetWindowLong(WindowHandle, GWL_STYLE) & WS_SIZEBOX) != WS_SIZEBOX)
                return;

            var r = new RECT();
            User32.GetWindowRect(WindowHandle, ref r);
            oldWindowSize = r;

            Bitmap f = CaptureCompositeScreenshot();
            if (f != null)
            {
                User32.SetWindowPos(WindowHandle, (IntPtr)0, r.Left,
                                        r.Top,
                                        ResizeX -
                                        (f.Width - (r.Right - r.Left)),
                                        ResizeY -
                                        (f.Height - (r.Bottom - r.Top)),
                                        SetWindowPositionFlags.ShowWindow);
                f.Dispose();
            }
            else User32.SetWindowPos(WindowHandle, (IntPtr)0, r.Left, r.Top, ResizeX, ResizeY, SetWindowPositionFlags.ShowWindow);
        }

        unsafe Bitmap CaptureCompositeScreenshot()
        {
            Color tmpColour = Color.White;

            var backdrop = new Form
            {
                AllowTransparency = true,
                BackColor = tmpColour,
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                Opacity = 0
            };

            // Generate a rectangle with the size of all monitors combined
            Rectangle totalSize = Rectangle.Empty;
            foreach (Screen s in Screen.AllScreens)
                totalSize = Rectangle.Union(totalSize, s.Bounds);

            var rct = new RECT();

            if (DWMApi.DwmGetWindowAttribute(WindowHandle, DwmWindowAttribute.ExtendedFrameBounds, ref rct, sizeof(RECT)) != 0)
                // DwmGetWindowAttribute() failed, usually means Aero is disabled so we fall back to GetWindowRect()
                User32.GetWindowRect(WindowHandle, ref rct);
            else
            {
                // DwmGetWindowAttribute() succeeded
                // Add a 100px margin for window shadows. Excess transparency is trimmed out later
                rct = new RECT(rct.Left - 100, rct.Top - 100, rct.Right + 100, rct.Bottom + 100);
            }

            // These next 4 checks handle if the window is outside of the visible screen
            if (rct.Left < totalSize.Left)
                rct.Left = totalSize.Left;
            if (rct.Top < totalSize.Top)
                rct.Top = totalSize.Top;
            if (rct.Right > totalSize.Right)
                rct.Right = totalSize.Right;
            if (rct.Bottom > totalSize.Bottom)
                rct.Bottom = totalSize.Bottom;

            User32.ShowWindow(backdrop.Handle, (ShowWindowFlags)4);
            User32.SetWindowPos(backdrop.Handle, WindowHandle, rct.Left,
                                    rct.Top, rct.Right - rct.Left,
                                    rct.Bottom - rct.Top, SetWindowPositionFlags.SWP_NOACTIVATE);
            backdrop.Opacity = 1;
            Application.DoEvents();

            // Capture screenshot with white background
            Bitmap whiteShot = CaptureScreenRegion(new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top));

            backdrop.BackColor = Color.Black;
            Application.DoEvents();

            // Capture screenshot with black background
            Bitmap blackShot = CaptureScreenRegion(new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top));

            backdrop.Dispose();

            Bitmap transparentImage = DifferentiateAlpha(whiteShot, blackShot);
            if (CaptureMouse) DrawCursorToBitmap(transparentImage, new Point(rct.Left, rct.Top));
            transparentImage = CropEmptyEdges(transparentImage, Color.FromArgb(0, 0, 0, 0));

            whiteShot.Dispose();
            blackShot.Dispose();

            // Returns a bitmap with transparency, calculated by differentiating the white and black screenshots
            return transparentImage;
        }

        static void DrawCursorToBitmap(Bitmap windowImage, Point offsetLocation)
        {
            var ci = new CursorInfo();
            ci.cbSize = Marshal.SizeOf(ci);
            if (User32.GetCursorInfo(out ci))
            {
                if (ci.flags == 1)
                {
                    IntPtr hicon = User32.CopyIcon(ci.hCursor);
                    IconInfo icInfo;
                    if (User32.GetIconInfo(hicon, out icInfo))
                    {
                        var loc = new Point(ci.ptScreenPos.X - offsetLocation.X - icInfo.xHotspot,
                                ci.ptScreenPos.Y - offsetLocation.Y - icInfo.yHotspot);
                        Icon ic = Icon.FromHandle(hicon);
                        Bitmap bmp = ic.ToBitmap();

                        Graphics g = Graphics.FromImage(windowImage);
                        g.DrawImage(bmp, new Rectangle(loc, bmp.Size));
                        g.Dispose();
                        User32.DestroyIcon(hicon);
                        bmp.Dispose();
                    }
                }
            }
        }

        static Bitmap CaptureScreenRegion(Rectangle crop)
        {
            Rectangle totalSize = Rectangle.Empty;

            foreach (Screen s in Screen.AllScreens)
                totalSize = Rectangle.Union(totalSize, s.Bounds);

            IntPtr hSrc = Gdi32.CreateDC("DISPLAY", null, null, 0);
            IntPtr hDest = Gdi32.CreateCompatibleDC(hSrc);
            IntPtr hBmp = Gdi32.CreateCompatibleBitmap(hSrc, crop.Right - crop.Left, crop.Bottom - crop.Top);
            IntPtr hOldBmp = Gdi32.SelectObject(hDest, hBmp);
            Gdi32.BitBlt(hDest, 0, 0, crop.Right - crop.Left,
                              crop.Bottom - crop.Top, hSrc, crop.Left, crop.Top,
                              CopyPixelOperation.SourceCopy |
                              CopyPixelOperation.CaptureBlt);
            Bitmap bmp = Image.FromHbitmap(hBmp);
            Gdi32.SelectObject(hDest, hOldBmp);
            Gdi32.DeleteObject(hBmp);
            Gdi32.DeleteDC(hDest);
            Gdi32.DeleteDC(hSrc);

            return bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);
        }

        static unsafe Bitmap CropEmptyEdges(Bitmap b1, Color trimColour)
        {
            if (b1 == null)
                return null;

            int sizeX = b1.Width;
            int sizeY = b1.Height;
            var b = new UnsafeBitmap(b1);
            b.LockImage();

            int left = -1;
            int top = -1;
            int right = -1;
            int bottom = -1;

            PixelData* pixel;

            for (int x = 0, y = 0; ; )
            {
                pixel = b.GetPixel(x, y);
                if (left == -1)
                {
                    if ((trimColour.A == 0 && pixel->Alpha != 0) ||
                        (trimColour.R != pixel->Red &
                         trimColour.G != pixel->Green &
                         trimColour.B != pixel->Blue))
                    {
                        left = x;
                        x = 0;
                        y = 0;
                        continue;
                    }
                    if (y == sizeY - 1)
                    {
                        x++;
                        y = 0;
                    }
                    else y++;

                    continue;
                }
                if (top == -1)
                {
                    if ((trimColour.A == 0 && pixel->Alpha != 0) ||
                        (trimColour.R != pixel->Red &
                         trimColour.G != pixel->Green &
                         trimColour.B != pixel->Blue))
                    {
                        top = y;
                        x = sizeX - 1;
                        y = 0;
                        continue;
                    }
                    if (x == sizeX - 1)
                    {
                        y++;
                        x = 0;
                    }
                    else
                        x++;

                    continue;
                }
                if (right == -1)
                {
                    if ((trimColour.A == 0 && pixel->Alpha != 0) ||
                        (trimColour.R != pixel->Red &
                         trimColour.G != pixel->Green &
                         trimColour.B != pixel->Blue))
                    {
                        right = x + 1;
                        x = 0;
                        y = sizeY - 1;
                        continue;
                    }
                    if (y == sizeY - 1)
                    {
                        x--;
                        y = 0;
                    }
                    else
                        y++;

                    continue;
                }
                if (bottom == -1)
                {
                    if ((trimColour.A == 0 && pixel->Alpha != 0) ||
                        (trimColour.R != pixel->Red &
                         trimColour.G != pixel->Green &
                         trimColour.B != pixel->Blue))
                    {
                        bottom = y + 1;
                        break;
                    }
                    if (x == sizeX - 1)
                    {
                        y--;
                        x = 0;
                    }
                    else
                        x++;

                    continue;
                }
            }
            b.UnlockImage();
            if (left >= right || top >= bottom)
                return null;

            Bitmap final =
                b1.Clone(new Rectangle(left, top, right - left, bottom - top),
                         b1.PixelFormat);
            b1.Dispose();
            return final;
        }

        static unsafe Bitmap DifferentiateAlpha(Bitmap whiteBitmap, Bitmap blackBitmap)
        {
            if (whiteBitmap == null || blackBitmap == null ||
                whiteBitmap.Width != blackBitmap.Width ||
                whiteBitmap.Height != blackBitmap.Height)
                return null;
            int sizeX = whiteBitmap.Width;
            int sizeY = whiteBitmap.Height;
            var final = new Bitmap(sizeX, sizeY, PixelFormat.Format32bppArgb);
            var a = new UnsafeBitmap(whiteBitmap);
            var b = new UnsafeBitmap(blackBitmap);
            var f = new UnsafeBitmap(final);
            a.LockImage();
            b.LockImage();
            f.LockImage();

            bool empty = true;

            for (int x = 0, y = 0; x < sizeX && y < sizeY; )
            {
                PixelData* pixelA = a.GetPixel(x, y);
                PixelData* pixelB = b.GetPixel(x, y);
                PixelData* pixelF = f.GetPixel(x, y);

                pixelF->Alpha =
                    ToByte((pixelB->Red - pixelA->Red + 255 + pixelB->Green -
                            pixelA->Green + 255 + pixelB->Blue - pixelA->Blue +
                            255) / 3);
                if (pixelF->Alpha > 0)
                {
                    // Following math creates an image optimized to be displayed on a black background
                    pixelF->Red = ToByte(255 * pixelB->Red / pixelF->Alpha);
                    pixelF->Green = ToByte(255 * pixelB->Green / pixelF->Alpha);
                    pixelF->Blue = ToByte(255 * pixelB->Blue / pixelF->Alpha);

                    // Following math creates an image optimized to be displayed on a white background
                    /*pixelF->Red =
                        ToByte(255*(pixelA->Red + pixelF->Alpha - 255)/
                               pixelF->Alpha);
                    pixelF->Green =
                        ToByte(255*(pixelA->Green + pixelF->Alpha - 255)/
                               pixelF->Alpha);
                    pixelF->Blue =
                        ToByte(255*(pixelA->Blue + pixelF->Alpha - 255)/
                               pixelF->Alpha);*/
                }
                if (empty && pixelF->Alpha > 0)
                    empty = false;

                if (x == sizeX - 1)
                {
                    y++;
                    x = 0;
                    continue;
                }
                x++;
            }

            a.UnlockImage();
            b.UnlockImage();
            f.UnlockImage();
            return empty ? null : final;
        }

        static byte ToByte(int i) { return (byte)(i > 255 ? 255 : (i < 0 ? 0 : i)); }
    }
}