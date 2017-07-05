using Screna.Native;
using System;
using System.Drawing;

namespace Screna
{
    /// <summary>
    /// Captures the specified window which can change dynamically. 
    /// The captured image is of the size of the whole desktop to accomodate any change in the Window.
    /// </summary>
    public class WindowProvider : ImageProviderBase
    {
        /// <summary>
        /// A <see cref="Rectangle"/> representing the entire Desktop.
        /// </summary>
        public static Rectangle DesktopRectangle { get; }

        static WindowProvider()
        {
            DesktopRectangle = System.Windows.Forms.SystemInformation.VirtualScreen;
        }

        readonly Func<Window> _windowFunction;
        readonly Color _backgroundColor;

        /// <summary>
        /// Creates a new instance of <see cref="WindowProvider"/>.
        /// </summary>
        /// <param name="Window">The Window to Capture.</param>
        /// <param name="BackgroundColor"><see cref="Color"/> to fill blank background.</param>
        public WindowProvider(Window Window = null, Color BackgroundColor = default(Color))
            : this(() => Window ?? Window.DesktopWindow, BackgroundColor) { }

        /// <summary>
        /// Creates a new instance of <see cref="WindowProvider"/>.
        /// </summary>
        /// <param name="WindowFunction">A Function returning the Window to Capture.</param>
        /// <param name="BackgroundColor"><see cref="Color"/> to fill blank background.</param>
        /// <exception cref="ArgumentNullException"><paramref name="WindowFunction"/> is null.</exception>
        public WindowProvider(Func<Window> WindowFunction, Color BackgroundColor = default(Color))
            : base(DesktopRectangle)
        {
            _windowFunction = WindowFunction ?? throw new ArgumentNullException(nameof(WindowFunction));
            _backgroundColor = BackgroundColor;
        }

        /// <summary>
        /// Capture Image.
        /// </summary>
        protected override void OnCapture(Graphics g)
        {
            var windowHandle = _windowFunction().Handle;

            var rect = DesktopRectangle;

            if (windowHandle != Window.DesktopWindow.Handle
                && windowHandle != IntPtr.Zero)
            {
                if (User32.GetWindowRect(windowHandle, out var r))
                    rect = r.ToRectangle();
            }
            
            if (_backgroundColor != Color.Transparent)
                g.FillRectangle(new SolidBrush(_backgroundColor), DesktopRectangle);

            g.CopyFromScreen(rect.Location, 
                             rect.Location,
                             rect.Size,
                             CopyPixelOperation.SourceCopy);
        }
    }
}
