---
layout: page
title: Methods for taking ScreenShots
highlight: true
---

## GDI
Structures:

```csharp
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential)]
struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
```

Gdi32:

```csharp
using System;
using System.Runtime.InteropServices;

public class Gdi32
{
    public const int SrcCopy = 13369376;

    const string DllName = "gdi32.dll";

    [DllImport(DllName)]
    public static extern IntPtr DeleteDC(IntPtr hDc);

    [DllImport(DllName)]
    public static extern IntPtr DeleteObject(IntPtr hDc);

    [DllImport(DllName)]
    public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

    [DllImport(DllName)]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    [DllImport(DllName)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport (DllName)]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
}
```

```csharp
using System;
using System.Runtime.InteropServices;

public class User32
{
    const string DllName = "user32.dll";

    [DllImport(DllName)]
    public static extern IntPtr GetDesktopWindow();

    [DllImport(DllName)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    [DllImport(DllName)]
    public static extern IntPtr GetDC(IntPtr ptr);

    [DllImport(DllName)]
    public static extern IntPtr GetWindowDC(Int32 ptr);

    [DllImport(DllName)]
    public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
}
```

```csharp
using System;
using System.Drawing;

// WindowHandle can be Desktop Handle to capture entire screen.
Bitmap CaptureWindow(IntPtr WindowHandle)
{
    // Get the handle to the window device context.
    var hDC = User32.GetDC(WindowHandle);

    // Make a compatible device context in memory for device context.
    var hMemDC = Gdi32.CreateCompatibleDC(hDC);

    // If cannot get Rectangle, return null
    if (!User32.GetWindowRect(WindowHandle, out var rect));
        return null;

    // Create a compatible bitmap of the window size and using the window device context.
    var hBitmap = Gdi32.CreateCompatibleBitmap(hDC, rect.Width, rect.Height);

    // Select the compatible bitmap in the memeory device context and keep the refrence to the old bitmap.
    var hOld = Gdi32.SelectObject(hMemDC, hBitmap);

    // Copy the Bitmap to the memory device context.
    Gdi32.BitBlt(hMemDC, 0, 0, rect.Width, rect.Height, hDC, 0, 0, Gdi32.SrcCopy);

    // Select the old bitmap back to the memory device context.
    Gdi32.SelectObject(hMemDC, hOld);

    // Delete the memory device context.
    Gdi32.DeleteDC(hMemDC);

    // Release the screen device context.
    User32.ReleaseDC(WindowHandle, hDC);

    var bmp = Image.FromHbitmap(hBitmap); 
    
    // Release the memory to avoid memory leaks.
    Gdi32.DeleteObject(hBitmap);

    return bmp;
}
```

## System.Drawing.Graphics
.Net provides a simple method to shorten the above task: `System.Drawing.Graphics.CopyFromScreen`.

```csharp
using System;
using System.Runtime.InteropServices;

Bitmap CaptureWindow(IntPtr WindowHandle)
{
    // If cannot get Rectangle, return null
    if (!User32.GetWindowRect(WindowHandle, out var rect));
        return null;

    var bmp = new Bitmap(rect.Width, rect.Height);

    using (var g = Graphics.FromImage(bmp))
        g.CopyFromScreen(rect.X, rect.Y, 0, 0,
                         new Size(rect.Width, rect.Height),
                         CopyPixelOperation.SourceCopy);

    return bmp;
}
```